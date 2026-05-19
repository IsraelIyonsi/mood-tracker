namespace MoodTracker.Api.Tests.Features.Moods.LogMood;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using MoodTracker.Api.Common.Persistence;
using MoodTracker.Api.Features.Moods;
using MoodTracker.Api.Features.Moods.LogMood;
using MoodTracker.Api.Tests.Builders;

public class LogMoodHandlerTests : IAsyncLifetime
{
    private static readonly DateTimeOffset Now =
        new(2026, 5, 19, 12, 0, 0, TimeSpan.Zero);

    private SqliteConnection _connection = default!;
    private MoodDbContext _db = default!;
    private FakeTimeProvider _clock = default!;
    private LogMoodHandler _handler = default!;

    public async Task InitializeAsync()
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

        _handler = new LogMoodHandler(_db, _clock, NullLogger<LogMoodHandler>.Instance);
    }

    public async Task DisposeAsync()
    {
        await _db.DisposeAsync();
        await _connection.DisposeAsync();
    }

    [Fact]
    public async Task Handle_PersistsRequest()
    {
        var request = A.LogMoodRequest().WithMood(Mood.Happy).WithNote("Great day!").Build();

        var response = await _handler.HandleAsync(request, CancellationToken.None);

        var persisted = await _db.MoodEntries.SingleAsync(CancellationToken.None);
        persisted.Id.ShouldBe(response.Id);
        persisted.Mood.ShouldBe(Mood.Happy);
        persisted.Note.ShouldBe("Great day!");
    }

    [Fact]
    public async Task Handle_DefaultsLoggedAtToClockNow_WhenNotProvided()
    {
        var request = A.LogMoodRequest().WithMood(Mood.Happy).WithLoggedAt(null).Build();

        var response = await _handler.HandleAsync(request, CancellationToken.None);

        response.LoggedAt.ShouldBe(Now);
    }

    [Fact]
    public async Task Handle_HonorsProvidedLoggedAt_WhenBackdated()
    {
        var backdated = Now.AddHours(-3);
        var request = A.LogMoodRequest().WithLoggedAt(backdated).Build();

        var response = await _handler.HandleAsync(request, CancellationToken.None);

        response.LoggedAt.ShouldBe(backdated);
    }

    [Fact]
    public async Task Handle_TrimsWhitespaceFromNote()
    {
        var request = A.LogMoodRequest().WithNote("  trimmed  ").Build();

        var response = await _handler.HandleAsync(request, CancellationToken.None);

        response.Note.ShouldBe("trimmed");
    }

    [Fact]
    public async Task Handle_PopulatesCreatedAtViaInterceptor()
    {
        var request = A.LogMoodRequest().Build();

        var response = await _handler.HandleAsync(request, CancellationToken.None);

        response.CreatedAt.ShouldBe(Now);
    }

    [Fact]
    public async Task Handle_GeneratesSortableGuidV7()
    {
        var request = A.LogMoodRequest().Build();

        var response = await _handler.HandleAsync(request, CancellationToken.None);

        response.Id.Version.ShouldBe((byte)7);
    }

    [Fact]
    public async Task Handle_RespectsCancellationToken()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var request = A.LogMoodRequest().Build();

        await Should.ThrowAsync<OperationCanceledException>(() =>
            _handler.HandleAsync(request, cts.Token));
    }
}
