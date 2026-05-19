namespace MoodTracker.Api.Features.Moods;

using MoodTracker.Api.Common.Persistence;

public sealed class MoodEntry : IAuditable
{
    public Guid Id { get; private init; }
    public Mood Mood { get; private init; }
    public string? Note { get; private init; }
    public DateTimeOffset LoggedAt { get; private init; }
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset? UpdatedAt { get; private init; }

    public MoodEntry(Guid id, Mood mood, string? note, DateTimeOffset loggedAt)
    {
        Id = id;
        Mood = mood;
        Note = note;
        LoggedAt = loggedAt;
    }

    private MoodEntry() { }
}
