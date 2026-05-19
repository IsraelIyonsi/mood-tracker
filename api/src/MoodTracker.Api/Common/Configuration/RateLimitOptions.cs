namespace MoodTracker.Api.Common.Configuration;

public sealed class RateLimitOptions
{
    public const string SectionName = "RateLimit";

    public required LogMoodRateLimit LogMood { get; init; }
}

public sealed class LogMoodRateLimit
{
    public required int PerMinute { get; init; }
    public required int QueueLimit { get; init; }
}
