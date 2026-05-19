namespace MoodTracker.Api.Common.Errors;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using MoodTracker.Api.Common.Constants;

internal sealed partial class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        ArgumentNullException.ThrowIfNull(exception);

        var correlationId = httpContext.Request.Headers[HttpHeaders.CorrelationId].FirstOrDefault();

        LogUnhandledException(
            logger,
            exception,
            httpContext.Request.Method,
            httpContext.Request.Path,
            correlationId);

        var problem = exception.ToProblemDetails(httpContext, correlationId);
        httpContext.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = MediaTypes.ProblemJson;

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }

    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Error,
        Message = "Unhandled exception while processing {Method} {Path}. CorrelationId={CorrelationId}")]
    private static partial void LogUnhandledException(
        ILogger logger,
        Exception exception,
        string method,
        string path,
        string? correlationId);
}
