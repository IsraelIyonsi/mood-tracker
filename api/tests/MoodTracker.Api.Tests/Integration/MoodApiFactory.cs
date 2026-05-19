namespace MoodTracker.Api.Tests.Integration;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoodTracker.Api.Common.Persistence;

public sealed class MoodApiFactory(SqliteConnection connection) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:ApplyMigrationsOnStartup"] = "false",
            });
        });

        builder.ConfigureServices(services =>
        {
            var existing = services.Single(descriptor =>
                descriptor.ServiceType == typeof(DbContextOptions<MoodDbContext>));
            services.Remove(existing);

            services.AddDbContext<MoodDbContext>((serviceProvider, options) =>
            {
                var interceptor = serviceProvider.GetRequiredService<AuditInterceptor>();
                options.UseSqlite(connection).AddInterceptors(interceptor);
            });
        });
    }
}
