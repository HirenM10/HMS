using Clinical.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Clinical.API.Common;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception for request {Path}", httpContext.Request.Path);

        var problemDetails = exception switch
        {
            ValidationException validationException => new ProblemDetails
            {
                Title = "Validation failure",
                Status = StatusCodes.Status400BadRequest,
                Detail = validationException.Message,
                Extensions = { ["errors"] = validationException.Errors }
            },
            NotFoundException => new ProblemDetails
            {
                Title = "Resource not found",
                Status = StatusCodes.Status404NotFound,
                Detail = exception.Message
            },
            InvalidOperationException => new ProblemDetails
            {
                Title = "Invalid operation",
                Status = StatusCodes.Status409Conflict,
                Detail = exception.Message
            },
            _ => new ProblemDetails
            {
                Title = "Server error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred."
            }
        };

        problemDetails.Instance = httpContext.Request.Path;
        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
