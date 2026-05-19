namespace MoodTracker.Api.Features.Moods.LogMood;

public sealed record LogMoodResponse(
    Guid Id,
    Mood Mood,
    string? Note,
    DateTimeOffset LoggedAt,
    DateTimeOffset CreatedAt)
{
    public static LogMoodResponse From(MoodEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        return new LogMoodResponse(entry.Id, entry.Mood, entry.Note, entry.LoggedAt, entry.CreatedAt);
    }
}
