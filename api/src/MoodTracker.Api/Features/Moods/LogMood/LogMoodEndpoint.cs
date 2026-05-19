namespace MoodTracker.Api.Features.Moods.LogMood;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Routing;
using MoodTracker.Api.Common.Abstractions;
using MoodTracker.Api.Common.Constants;
using MoodTracker.Api.Common.Errors;

internal static class LogMoodEndpoint
{
    public static IEndpointRouteBuilder MapLogMood(this IEndpointRouteBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.MapPost(ApiRoutes.Moods, Handle)
            .WithName(EndpointNames.LogMood)
            .AddEndpointFilter<ValidationFilter<LogMoodRequest>>()
            .RequireRateLimiting(RateLimitPolicies.LogMood)
            .Produces<LogMoodResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        return app;
    }

    private static async Task<IResult> Handle(
        LogMoodRequest request,
        IRequestHandler<LogMoodRequest, LogMoodResponse> handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(request, cancellationToken);
        return Results.Created($"{ApiRoutes.Moods}/{response.Id}", response);
    }
}
