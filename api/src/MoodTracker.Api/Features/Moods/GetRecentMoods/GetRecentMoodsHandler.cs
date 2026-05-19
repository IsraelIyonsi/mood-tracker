namespace MoodTracker.Api.Features.Moods.GetRecentMoods;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MoodTracker.Api.Common.Abstractions;
using MoodTracker.Api.Common.Persistence;

internal sealed class GetRecentMoodsHandler(
    MoodDbContext db,
    IOptions<MoodOptions> options) : IRequestHandler<GetRecentMoodsQuery, GetRecentMoodsResponse>
{
    public async Task<GetRecentMoodsResponse> HandleAsync(GetRecentMoodsQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var moodOptions = options.Value;
        var take = Math.Clamp(query.Take, 1, moodOptions.RecentMaxTake);

        var entries = await db.MoodEntries
            .AsNoTracking()
            .OrderByDescending(entry => entry.LoggedAt)
            .Take(take)
            .ToListAsync(cancellationToken);

        return GetRecentMoodsResponse.From(entries);
    }
}
