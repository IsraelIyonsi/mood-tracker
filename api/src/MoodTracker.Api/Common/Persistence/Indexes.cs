namespace MoodTracker.Api.Common.Persistence;

public static class Indexes
{
    public const string MoodEntriesLoggedAt = $"IX_{Tables.MoodEntries}_LoggedAt";
}
