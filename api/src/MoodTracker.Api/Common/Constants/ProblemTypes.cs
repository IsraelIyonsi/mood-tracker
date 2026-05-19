namespace MoodTracker.Api.Common.Constants;

public static class ProblemTypes
{
    private const string Base = "https://moodtracker.dev/problems";

    public const string ValidationError = $"{Base}/validation-error";
    public const string ResourceNotFound = $"{Base}/resource-not-found";
    public const string RateLimitExceeded = $"{Base}/rate-limit-exceeded";
    public const string InternalServerError = $"{Base}/internal-server-error";
}
