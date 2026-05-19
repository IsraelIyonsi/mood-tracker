namespace MoodTracker.Api.Features.Moods;

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MoodTracker.Api.Common.Abstractions;
using MoodTracker.Api.Features.Moods.GetRecentMoods;
using MoodTracker.Api.Features.Moods.LogMood;

internal static class MoodsModule
{
    public static IServiceCollection AddMoodsModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddScoped<IRequestHandler<LogMoodRequest, LogMoodResponse>, LogMoodHandler>();
        services.AddScoped<IRequestHandler<GetRecentMoodsQuery, GetRecentMoodsResponse>, GetRecentMoodsHandler>();
        return services;
    }

    public static IEndpointRouteBuilder MapMoodsModule(this IEndpointRouteBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        app.MapLogMood();
        app.MapGetRecentMoods();
        return app;
    }
}
