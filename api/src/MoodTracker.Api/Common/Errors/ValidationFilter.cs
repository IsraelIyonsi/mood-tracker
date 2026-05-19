namespace MoodTracker.Api.Common.Errors;

using FluentValidation;
using Microsoft.AspNetCore.Http;
using MoodTracker.Api.Common.Constants;

public sealed class ValidationFilter<TRequest> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);

        var request = context.Arguments.OfType<TRequest>().FirstOrDefault();
        if (request is null)
        {
            return Results.Problem(
                title: "Missing request body",
                statusCode: StatusCodes.Status400BadRequest,
                type: ProblemTypes.ValidationError);
        }

        var validator = context.HttpContext.RequestServices.GetService<IValidator<TRequest>>();
        if (validator is null)
        {
            return await next(context);
        }

        var result = await validator.ValidateAsync(request, context.HttpContext.RequestAborted);
        if (result.IsValid)
        {
            return await next(context);
        }

        var errors = result.Errors
            .GroupBy(error => error.PropertyName, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).ToArray(),
                StringComparer.Ordinal);

        return Results.ValidationProblem(
            errors: errors,
            type: ProblemTypes.ValidationError,
            statusCode: StatusCodes.Status422UnprocessableEntity);
    }
}
