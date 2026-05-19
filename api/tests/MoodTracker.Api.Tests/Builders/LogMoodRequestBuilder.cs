namespace MoodTracker.Api.Tests.Builders;

using MoodTracker.Api.Features.Moods;
using MoodTracker.Api.Features.Moods.LogMood;

internal sealed class LogMoodRequestBuilder
{
    private Mood _mood = Mood.Happy;
    private string? _note;
    private DateTimeOffset? _loggedAt;

    public LogMoodRequestBuilder WithMood(Mood mood)
    {
        _mood = mood;
        return this;
    }

    public LogMoodRequestBuilder WithNote(string? note)
    {
        _note = note;
        return this;
    }

    public LogMoodRequestBuilder WithLoggedAt(DateTimeOffset? loggedAt)
    {
        _loggedAt = loggedAt;
        return this;
    }

    public LogMoodRequestBuilder WithNoteOfLength(int length)
    {
        _note = new string('x', length);
        return this;
    }

    public LogMoodRequest Build() => new(_mood, _note, _loggedAt);

    public static implicit operator LogMoodRequest(LogMoodRequestBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.Build();
    }
}
