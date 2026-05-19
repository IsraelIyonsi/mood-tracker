namespace MoodTracker.Api.Tests.Builders;

internal static class A
{
    public static LogMoodRequestBuilder LogMoodRequest() => new();
    public static MoodEntryBuilder MoodEntry() => new();
}
