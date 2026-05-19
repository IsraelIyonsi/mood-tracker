namespace MoodTracker.Api.Features.Moods.GetRecentMoods;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using MoodTracker.Api.Common.Abstractions;
using MoodTracker.Api.Common.Constants;

internal static class GetRecentMoodsEndpoint
{
    public static IEndpointRouteBuilder MapGetRecentMoods(this IEndpointRouteBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.MapGet(ApiRoutes.Moods, Handle)
            .WithName(EndpointNames.GetRecentMoods)
            .Produces<GetRecentMoodsResponse>(StatusCodes.Status200OK);

        return app;
    }

    private static async Task<IResult> Handle(
        IRequestHandler<GetRecentMoodsQuery, GetRecentMoodsResponse> handler,
        IOptions<MoodOptions> options,
        CancellationToken cancellationToken,
        int? take = null)
    {
        var resolved = take ?? options.Value.RecentDefaultTake;
        var response = await handler.HandleAsync(new GetRecentMoodsQuery(resolved), cancellationToken);
        return Results.Ok(response);
    }
}
