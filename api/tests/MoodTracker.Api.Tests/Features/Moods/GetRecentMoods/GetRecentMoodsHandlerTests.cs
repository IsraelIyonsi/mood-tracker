namespace MoodTracker.Api.Tests.Features.Moods.GetRecentMoods;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using MoodTracker.Api.Common.Persistence;
using MoodTracker.Api.Features.Moods;
using MoodTracker.Api.Features.Moods.GetRecentMoods;
using MoodTracker.Api.Tests.Builders;

public class GetRecentMoodsHandlerTests : IAsyncLifetime
{
    private static readonly DateTimeOffset Now =
        new(2026, 5, 19, 12, 0, 0, TimeSpan.Zero);

    private static readonly MoodOptions DefaultMoodOptions = new()
    {
        NoteMaxLength = 280,
        RecentDefaultTake = 7,
        RecentMaxTake = 30,
        ClockSkewTolerance = TimeSpan.FromSeconds(30),
        MaxBackdateWindow = TimeSpan.FromDays(30),
    };

    private SqliteConnection _connection = default!;
    private MoodDbContext _db = default!;
    private GetRecentMoodsHandler _handler = default!;

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        await _connection.OpenAsync();

        var clock = new FakeTimeProvider(Now);
        var options = new DbContextOptionsBuilder<MoodDbContext>()
            .UseSqlite(_connection)
            .AddInterceptors(new AuditInterceptor(clock))
            .Options;

        _db = new MoodDbContext(options);
        await _db.Database.EnsureCreatedAsync();

        _handler = new GetRecentMoodsHandler(_db, Options.Create(DefaultMoodOptions));
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
        await _connection.DisposeAsync();
    }

    [Fact]
    public async Task Handle_ReturnsEmpty_WhenNoEntries()
    {
        var response = await _handler.HandleAsync(new GetRecentMoodsQuery(7), CancellationToken.None);

        response.Entries.ShouldBeEmpty();
        response.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Handle_OrdersByLoggedAtDescending()
    {
        await SeedAsync(
            A.MoodEntry().WithMood(Mood.Happy).WithLoggedAt(Now.AddDays(-3)),
            A.MoodEntry().WithMood(Mood.Sad).WithLoggedAt(Now),
            A.MoodEntry().WithMood(Mood.Neutral).WithLoggedAt(Now.AddDays(-1)));

        var response = await _handler.HandleAsync(new GetRecentMoodsQuery(7), CancellationToken.None);

        response.Entries[0].Mood.ShouldBe(Mood.Sad);
        response.Entries[1].Mood.ShouldBe(Mood.Neutral);
        response.Entries[2].Mood.ShouldBe(Mood.Happy);
    }

    [Fact]
    public async Task Handle_TakesRequestedCount()
    {
        await SeedAsync(Enumerable.Range(0, 10)
            .Select(offset => A.MoodEntry().WithLoggedAt(Now.AddHours(-offset)))
            .ToArray());

        var response = await _handler.HandleAsync(new GetRecentMoodsQuery(5), CancellationToken.None);

        response.Count.ShouldBe(5);
        response.Entries.Count.ShouldBe(5);
    }

    [Fact]
    public async Task Handle_ClampsTakeToMax()
    {
        await SeedAsync(Enumerable.Range(0, 50)
            .Select(offset => A.MoodEntry().WithLoggedAt(Now.AddHours(-offset)))
            .ToArray());

        var response = await _handler.HandleAsync(new GetRecentMoodsQuery(1000), CancellationToken.None);

        response.Count.ShouldBe(DefaultMoodOptions.RecentMaxTake);
    }

    [Fact]
    public async Task Handle_ClampsTakeToMinOne_WhenZeroOrNegative()
    {
        await SeedAsync(A.MoodEntry());

        var response = await _handler.HandleAsync(new GetRecentMoodsQuery(0), CancellationToken.None);

        response.Count.ShouldBe(1);
    }

    private async Task SeedAsync(params MoodEntryBuilder[] builders)
    {
        foreach (var builder in builders)
        {
            _db.MoodEntries.Add(builder.Build());
        }
        await _db.SaveChangesAsync();
    }
}
