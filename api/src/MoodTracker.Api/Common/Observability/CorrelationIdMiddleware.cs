namespace MoodTracker.Api.Common.Observability;

using Microsoft.AspNetCore.Http;
using MoodTracker.Api.Common.Constants;
using Serilog.Context;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    public const string LogPropertyName = "CorrelationId";

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var correlationId = context.Request.Headers[HttpHeaders.CorrelationId].FirstOrDefault()
                            ?? Guid.NewGuid().ToString();

        context.Response.Headers[HttpHeaders.CorrelationId] = correlationId;

        using (LogContext.PushProperty(LogPropertyName, correlationId))
        {
            await next(context);
        }
    }
}
