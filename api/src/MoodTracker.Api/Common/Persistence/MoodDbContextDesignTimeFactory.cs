namespace MoodTracker.Api.Common.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

internal sealed class MoodDbContextDesignTimeFactory : IDesignTimeDbContextFactory<MoodDbContext>
{
    private const string DesignTimeConnectionString = "Data Source=design-time.db";

    public MoodDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<MoodDbContext>()
            .UseSqlite(DesignTimeConnectionString)
            .Options;

        return new MoodDbContext(options);
    }
}
