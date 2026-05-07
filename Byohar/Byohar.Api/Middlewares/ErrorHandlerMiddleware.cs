using Byohar.Api.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using System.Text.Json;

namespace Byohar.Api.Middlewares;

public class ErrorHandlerMiddleware
{
    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    private readonly RequestDelegate _next;

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
    {
        var statusCode = GetStatusCode(exception);

        var response = new
        {
            Title = GetTitle(exception),
            Status = statusCode,
            Detail = exception.Message,
            Errors = GetErrors(exception)
        };

        httpContext.Response.ContentType = "application/json";

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static int GetStatusCode(Exception exception) =>
        exception switch
        {
            BadRequestException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            ValidationException => StatusCodes.Status422UnprocessableEntity,
            BadHttpRequestException => StatusCodes.Status413RequestEntityTooLarge,
            _ => StatusCodes.Status500InternalServerError
        };

    private static string GetTitle(Exception exception) =>
        exception switch
        {
            ApplicationException applicationException => applicationException.Message,
            ValidationException => "Validation Exception",
            _ => "Server Error"
        };

    private static IEnumerable<ValidationFailure> GetErrors(Exception exception)
    {
        IEnumerable<ValidationFailure> errors;

        if (exception is ValidationException validationException)
        {
            errors = validationException.Errors;
        }
        else
        {
            errors = new List<ValidationFailure>() { new ValidationFailure { ErrorMessage = "Internal Server Error" } };
        }

        return errors;
    }
}