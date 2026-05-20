using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MoodTracker.Api.Common.Configuration;
using MoodTracker.Api.Common.Constants;
using MoodTracker.Api.Common.Errors;
using MoodTracker.Api.Common.Observability;
using MoodTracker.Api.Common.Persistence;
using MoodTracker.Api.Features.Moods;

var builder = WebApplication.CreateBuilder(args);

// Bind to platform-injected PORT (Railway/Render/Fly inject this)
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
    builder.WebHost.UseUrls($"http://+:{port}");
}

SerilogConfiguration.Configure(builder);

builder.Services.AddOptions<DatabaseOptions>()
    .BindConfiguration(DatabaseOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<CorsOptions>()
    .BindConfiguration(CorsOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<RateLimitOptions>()
    .BindConfiguration(RateLimitOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<MoodTracker.Api.Features.Moods.MoodOptions>()
    .BindConfiguration(MoodTracker.Api.Features.Moods.MoodOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<AuditInterceptor>();

builder.Services.AddDbContext<MoodDbContext>((serviceProvider, options) =>
{
    var dbOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
    options
        .UseSqlite(dbOptions.ConnectionString)
        .AddInterceptors(serviceProvider.GetRequiredService<AuditInterceptor>());
});

builder.Services.AddHostedService<DatabaseMigrator>();

builder.Services.ConfigureHttpJsonOptions(jsonOptions =>
{
    jsonOptions.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    jsonOptions.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddValidatorsFromAssemblyContaining<Program>(includeInternalTypes: true);
builder.Services.AddMoodsModule();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<MoodDbContext>("database", tags: ["ready"]);

var corsConfig = builder.Configuration.GetSection(CorsOptions.SectionName).Get<CorsOptions>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsOptions.PolicyName, policy =>
    {
        if (corsConfig is { AllowedOrigins.Count: > 0 } cors)
        {
            policy.WithOrigins([.. cors.AllowedOrigins])
                .AllowAnyHeader()
                .AllowAnyMethod()
                .WithExposedHeaders(HttpHeaders.CorrelationId);
        }
    });
});

var rateLimitConfig = builder.Configuration.GetSection(RateLimitOptions.SectionName).Get<RateLimitOptions>();
builder.Services.AddRateLimiter(options =>
{
    if (rateLimitConfig is null)
    {
        return;
    }

    options.AddFixedWindowLimiter(RateLimitPolicies.LogMood, limiter =>
    {
        limiter.PermitLimit = rateLimitConfig.LogMood.PerMinute;
        limiter.Window = TimeSpan.FromMinutes(1);
        limiter.QueueLimit = rateLimitConfig.LogMood.QueueLimit;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseCors(CorsOptions.PolicyName);
app.UseRateLimiter();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapHealthChecks(ApiRoutes.HealthLive, new HealthCheckOptions { Predicate = _ => false });
app.MapHealthChecks(ApiRoutes.HealthReady, new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

app.MapOpenApi(ApiRoutes.OpenApiSpec);

app.MapMoodsModule();

app.MapFallbackToFile("index.html");

app.Run();

public partial class Program;
