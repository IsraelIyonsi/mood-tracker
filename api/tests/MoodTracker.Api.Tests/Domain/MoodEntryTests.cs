namespace MoodTracker.Api.Tests.Domain;

using MoodTracker.Api.Common.Persistence;
using MoodTracker.Api.Features.Moods;

public class MoodEntryTests
{
    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var id = Guid.CreateVersion7();
        var loggedAt = new DateTimeOffset(2026, 5, 19, 12, 0, 0, TimeSpan.Zero);

        var entry = new MoodEntry(id, Mood.Happy, "feeling good", loggedAt);

        entry.Id.ShouldBe(id);
        entry.Mood.ShouldBe(Mood.Happy);
        entry.Note.ShouldBe("feeling good");
        entry.LoggedAt.ShouldBe(loggedAt);
    }

    [Fact]
    public void Constructor_AllowsNullNote()
    {
        var entry = new MoodEntry(Guid.CreateVersion7(), Mood.Neutral, null, DateTimeOffset.UtcNow);

        entry.Note.ShouldBeNull();
    }

    [Fact]
    public void Constructor_AllowsEmptyNote()
    {
        var entry = new MoodEntry(Guid.CreateVersion7(), Mood.Sad, string.Empty, DateTimeOffset.UtcNow);

        entry.Note.ShouldBe(string.Empty);
    }

    [Fact]
    public void Entity_ImplementsIAuditable()
    {
        typeof(IAuditable).IsAssignableFrom(typeof(MoodEntry)).ShouldBeTrue();
    }

    [Fact]
    public void UpdatedAt_IsNullOnCreation()
    {
        var entry = new MoodEntry(Guid.CreateVersion7(), Mood.Happy, null, DateTimeOffset.UtcNow);

        entry.UpdatedAt.ShouldBeNull();
    }
}
