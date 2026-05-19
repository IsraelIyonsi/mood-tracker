namespace MoodTracker.Api.Common.Configuration;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public required string ConnectionString { get; init; }
}
