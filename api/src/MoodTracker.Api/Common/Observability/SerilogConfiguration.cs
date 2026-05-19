namespace MoodTracker.Api.Common.Observability;

using Serilog;

public static class SerilogConfiguration
{
    private const string ConsoleTemplate =
        "[{Timestamp:HH:mm:ss} {Level:u3}] {CorrelationId} {SourceContext} {Message:lj}{NewLine}{Exception}";

    public static void Configure(WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Host.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: ConsoleTemplate);
        });
    }
}
