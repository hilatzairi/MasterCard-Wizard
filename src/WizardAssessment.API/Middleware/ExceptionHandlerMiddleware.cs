using System.Net;
using System.Text.Json;

namespace WizardAssessment.API.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, $"Business rule violation: '{ex.Message}'");
            await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, $"Resource not found: '{ex.Message}'");
            await HandleExceptionAsync(context, ex, HttpStatusCode.NotFound);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, $"Invalid argument: '{ex.Message}'");
            await HandleExceptionAsync(context, ex, HttpStatusCode.BadRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unhandled exception occurred: '{ex.Message}'");
            await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            statusCode = (int)statusCode,
            message = statusCode == HttpStatusCode.InternalServerError
                ? "An error occurred while processing your request"
                : exception.Message,
            details = statusCode == HttpStatusCode.InternalServerError ? null : exception.Message
        };

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}

