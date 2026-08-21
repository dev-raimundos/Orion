using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Api.Common;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is AppException appException)
        {
            var statusCode = MapStatusCode(appException.ErrorType);

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = appException.ErrorType.ToString(),
                Detail = appException.Message,
                Extensions = { ["errorCode"] = appException.ErrorType.ToString() }
            };

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }

        logger.LogError(exception, "Erro não tratado");

        var unexpectedProblem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = ErrorType.Unexpected.ToString(),
            Detail = "Ocorreu um erro inesperado. Tente novamente mais tarde.",
            Extensions = { ["errorCode"] = ErrorType.Unexpected.ToString() }
        };

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(unexpectedProblem, cancellationToken);
        return true;
    }

    private static int MapStatusCode(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError
    };
}