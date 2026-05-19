namespace MoodTracker.Api.Common.Configuration;

public sealed class CorsOptions
{
    public const string SectionName = "Cors";
    public const string PolicyName = "WebOrigin";

    public required IReadOnlyList<string> AllowedOrigins { get; init; }
}
