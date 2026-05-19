namespace MoodTracker.Api.Common.Persistence;

using Microsoft.EntityFrameworkCore;
using MoodTracker.Api.Features.Moods;

public sealed class MoodDbContext(DbContextOptions<MoodDbContext> options) : DbContext(options)
{
    public DbSet<MoodEntry> MoodEntries => Set<MoodEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MoodDbContext).Assembly);
    }
}
