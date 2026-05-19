namespace MoodTracker.Api.Common.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MoodTracker.Api.Features.Moods;

public sealed class MoodDbContext(DbContextOptions<MoodDbContext> options) : DbContext(options)
{
    public DbSet<MoodEntry> MoodEntries => Set<MoodEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MoodDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);
        configurationBuilder
            .Properties<DateTimeOffset>()
            .HaveConversion<DateTimeOffsetToBinaryConverter>();
    }
}
