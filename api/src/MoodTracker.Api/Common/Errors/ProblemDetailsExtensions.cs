namespace MoodTracker.Api.Common.Errors;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoodTracker.Api.Common.Constants;

public static class ProblemDetailsExtensions
{
    public const string CorrelationExtensionKey = "correlationId";

    public static ProblemDetails ToProblemDetails(
        this Exception exception,
        HttpContext httpContext,
        string? correlationId)
    {
        ArgumentNullException.ThrowIfNull(exception);
        ArgumentNullException.ThrowIfNull(httpContext);

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred",
            Type = ProblemTypes.InternalServerError,
            Detail = exception.Message,
            Instance = httpContext.Request.Path,
        };

        if (correlationId is not null)
        {
            problem.Extensions[CorrelationExtensionKey] = correlationId;
        }

        return problem;
    }
}
