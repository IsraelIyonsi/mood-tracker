namespace MoodTracker.Api.Common.Persistence;

public interface IAuditable
{
    DateTimeOffset CreatedAt { get; }
    DateTimeOffset? UpdatedAt { get; }
}
