namespace MoodTracker.Api.Tests.Persistence;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using MoodTracker.Api.Common.Persistence;
using MoodTracker.Api.Features.Moods;

public class AuditInterceptorTests : IAsyncLifetime
{
    private static readonly DateTimeOffset Now =
        new(2026, 5, 19, 12, 0, 0, TimeSpan.Zero);

    private SqliteConnection _connection = default!;
    private MoodDbContext _db = default!;
    private FakeTimeProvider _clock = default!;

    public async ValueTask InitializeAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        await _connection.OpenAsync();

        _clock = new FakeTimeProvider(Now);

        var options = new DbContextOptionsBuilder<MoodDbContext>()
            .UseSqlite(_connection)
            .AddInterceptors(new AuditInterceptor(_clock))
            .Options;

        _db = new MoodDbContext(options);
        await _db.Database.EnsureCreatedAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _db.DisposeAsync();
        await _connection.DisposeAsync();
    }

    [Fact]
    public async Task AddedEntity_GetsCreatedAt_FromTimeProvider()
    {
        var entry = new MoodEntry(Guid.CreateVersion7(), Mood.Happy, "test", Now);
        _db.MoodEntries.Add(entry);

        await _db.SaveChangesAsync(TestContext.Current.CancellationToken);

        entry.CreatedAt.ShouldBe(Now);
    }

    [Fact]
    public async Task AddedEntity_LeavesUpdatedAtNull()
    {
        var entry = new MoodEntry(Guid.CreateVersion7(), Mood.Happy, "test", Now);
        _db.MoodEntries.Add(entry);

        await _db.SaveChangesAsync(TestContext.Current.CancellationToken);

        entry.UpdatedAt.ShouldBeNull();
    }

    [Fact]
    public async Task ModifiedEntity_GetsUpdatedAt()
    {
        var entry = new MoodEntry(Guid.CreateVersion7(), Mood.Happy, "test", Now);
        _db.MoodEntries.Add(entry);
        await _db.SaveChangesAsync(TestContext.Current.CancellationToken);

        _clock.Advance(TimeSpan.FromMinutes(5));
        _db.Entry(entry).State = EntityState.Modified;
        await _db.SaveChangesAsync(TestContext.Current.CancellationToken);

        entry.UpdatedAt.ShouldBe(_clock.GetUtcNow());
    }

    [Fact]
    public async Task ModifiedEntity_DoesNotRewriteCreatedAt()
    {
        var entry = new MoodEntry(Guid.CreateVersion7(), Mood.Happy, "test", Now);
        _db.MoodEntries.Add(entry);
        await _db.SaveChangesAsync(TestContext.Current.CancellationToken);
        var originalCreated = entry.CreatedAt;

        _clock.Advance(TimeSpan.FromHours(2));
        _db.Entry(entry).State = EntityState.Modified;
        await _db.SaveChangesAsync(TestContext.Current.CancellationToken);

        entry.CreatedAt.ShouldBe(originalCreated);
    }

    [Fact]
    public async Task MultipleAdds_InSingleSaveChanges_GetSameCreatedAt()
    {
        var first  = new MoodEntry(Guid.CreateVersion7(), Mood.Happy, "a", Now);
        var second = new MoodEntry(Guid.CreateVersion7(), Mood.Sad,   "b", Now);
        _db.MoodEntries.AddRange(first, second);

        await _db.SaveChangesAsync(TestContext.Current.CancellationToken);

        first.CreatedAt.ShouldBe(Now);
        second.CreatedAt.ShouldBe(Now);
    }
}
