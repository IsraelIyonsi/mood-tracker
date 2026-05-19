namespace MoodTracker.Api.Common.Errors;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using MoodTracker.Api.Common.Constants;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(exception);

        var correlationId = httpContext.Request.Headers[HttpHeaders.CorrelationId].FirstOrDefault();

        logger.LogError(
            exception,
            "Unhandled exception while processing {Method} {Path}. CorrelationId={CorrelationId}",
            httpContext.Request.Method,
            httpContext.Request.Path,
            correlationId);

        var problem = exception.ToProblemDetails(httpContext, correlationId);
        httpContext.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = MediaTypes.ProblemJson;

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
