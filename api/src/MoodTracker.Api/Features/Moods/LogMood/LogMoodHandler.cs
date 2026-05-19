namespace MoodTracker.Api.Features.Moods.LogMood;

using Microsoft.EntityFrameworkCore;
using MoodTracker.Api.Common.Abstractions;
using MoodTracker.Api.Common.Persistence;

internal sealed partial class LogMoodHandler(
    MoodDbContext db,
    TimeProvider clock,
    ILogger<LogMoodHandler> logger) : IRequestHandler<LogMoodRequest, LogMoodResponse>
{
    public async Task<LogMoodResponse> HandleAsync(LogMoodRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entry = new MoodEntry(
            id: Guid.CreateVersion7(),
            mood: request.Mood,
            note: request.Note?.Trim(),
            loggedAt: request.LoggedAt ?? clock.GetUtcNow());

        db.MoodEntries.Add(entry);
        await db.SaveChangesAsync(cancellationToken);

        LogEntryPersisted(logger, entry.Id, entry.Mood, entry.LoggedAt, entry.CreatedAt);

        return LogMoodResponse.From(entry);
    }

    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Information,
        Message = "Mood entry persisted: Id={EntryId} Mood={Mood} LoggedAt={LoggedAt} CreatedAt={CreatedAt}")]
    private static partial void LogEntryPersisted(
        ILogger logger,
        Guid entryId,
        Mood mood,
        DateTimeOffset loggedAt,
        DateTimeOffset createdAt);
}
