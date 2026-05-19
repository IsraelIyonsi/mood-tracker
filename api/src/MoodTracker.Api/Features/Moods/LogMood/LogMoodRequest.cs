namespace MoodTracker.Api.Features.Moods.LogMood;

public sealed record LogMoodRequest(
    Mood Mood,
    string? Note,
    DateTimeOffset? LoggedAt
);
