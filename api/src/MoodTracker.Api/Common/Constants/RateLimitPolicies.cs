namespace MoodTracker.Api.Common.Constants;

public static class RateLimitPolicies
{
    public const string LogMood = nameof(LogMood);

    public const int LogMoodPerMinute = 30;
    public const int LogMoodQueueLimit = 0;
}
