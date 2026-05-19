namespace MoodTracker.Api.Features.Moods.GetRecentMoods;

public sealed record MoodEntryView(
    Guid Id,
    Mood Mood,
    string? Note,
    DateTimeOffset LoggedAt)
{
    public static MoodEntryView From(MoodEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        return new MoodEntryView(entry.Id, entry.Mood, entry.Note, entry.LoggedAt);
    }
}
