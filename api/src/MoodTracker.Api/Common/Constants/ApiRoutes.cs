namespace MoodTracker.Api.Common.Constants;

public static class ApiRoutes
{
    private const string Base = "/api/v1";

    public const string Moods = $"{Base}/moods";

    public const string Health = "/health";
    public const string HealthLive = $"{Health}/live";
    public const string HealthReady = $"{Health}/ready";

    public const string OpenApiSpec = "/openapi/v1.json";
}
