namespace MoodTracker.Api.Tests.Scenarios;

using MoodTracker.Api.Features.Moods.LogMood;

public sealed record ValidationScenario(
    string Description,
    LogMoodRequest Request,
    string? ExpectedErrorProperty = null)
{
    public override string ToString() => Description;
}
