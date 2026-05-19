namespace MoodTracker.Api.Features.Moods.GetRecentMoods;

public sealed record GetRecentMoodsResponse(
    IReadOnlyList<MoodEntryView> Entries,
    int Count)
{
    public static GetRecentMoodsResponse From(IReadOnlyList<MoodEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);
        var views = entries.Select(MoodEntryView.From).ToArray();
        return new GetRecentMoodsResponse(views, views.Length);
    }
}
