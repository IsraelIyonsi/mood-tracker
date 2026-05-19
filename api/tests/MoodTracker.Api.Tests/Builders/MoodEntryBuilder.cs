namespace MoodTracker.Api.Tests.Builders;

using MoodTracker.Api.Features.Moods;

internal sealed class MoodEntryBuilder
{
    private Guid _id = Guid.CreateVersion7();
    private Mood _mood = Mood.Happy;
    private string? _note;
    private DateTimeOffset _loggedAt = DateTimeOffset.UtcNow;

    public MoodEntryBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public MoodEntryBuilder WithMood(Mood mood)
    {
        _mood = mood;
        return this;
    }

    public MoodEntryBuilder WithNote(string? note)
    {
        _note = note;
        return this;
    }

    public MoodEntryBuilder WithLoggedAt(DateTimeOffset loggedAt)
    {
        _loggedAt = loggedAt;
        return this;
    }

    public MoodEntry Build() => new(_id, _mood, _note, _loggedAt);

    public static implicit operator MoodEntry(MoodEntryBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.Build();
    }
}
