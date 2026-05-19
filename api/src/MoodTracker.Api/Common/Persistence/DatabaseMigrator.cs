namespace MoodTracker.Api.Common.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MoodTracker.Api.Common.Configuration;

internal sealed partial class DatabaseMigrator(
    IServiceProvider services,
    IOptions<DatabaseOptions> options,
    ILogger<DatabaseMigrator> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!options.Value.ApplyMigrationsOnStartup)
        {
            LogMigrationsSkipped(logger);
            return;
        }

        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<MoodDbContext>();

        LogApplyingMigrations(logger);
        await db.Database.MigrateAsync(cancellationToken);
        LogMigrationsApplied(logger);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    [LoggerMessage(EventId = 1001, Level = LogLevel.Information, Message = "Applying database migrations")]
    private static partial void LogApplyingMigrations(ILogger logger);

    [LoggerMessage(EventId = 1002, Level = LogLevel.Information, Message = "Database migrations applied")]
    private static partial void LogMigrationsApplied(ILogger logger);

    [LoggerMessage(EventId = 1003, Level = LogLevel.Warning, Message = "Database migrations skipped (ApplyMigrationsOnStartup=false)")]
    private static partial void LogMigrationsSkipped(ILogger logger);
}
