namespace MoodTracker.Api.Features.Moods;

public sealed class MoodOptions
{
    public const string SectionName = "Mood";

    public required int NoteMaxLength { get; init; }
    public required int RecentDefaultTake { get; init; }
    public required int RecentMaxTake { get; init; }
    public required TimeSpan ClockSkewTolerance { get; init; }
    public required TimeSpan MaxBackdateWindow { get; init; }
}
