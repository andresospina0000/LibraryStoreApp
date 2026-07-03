using System.Net;
using System.Text.Json;
using Library.Application.Common;

namespace Library.API.Middleware;

/// <summary>
/// Catches unhandled exceptions, maps known <see cref="AppException"/> types to
/// their HTTP status codes and returns a standardized <see cref="Result"/> payload.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (AppException ex)
        {
            _logger.LogWarning(ex, "Handled application exception: {Message}", ex.Message);
            await WriteAsync(context, ex.Message, ex.ErrorType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteAsync(context, "An unexpected error occurred.", ErrorType.ServerError);
        }
    }

    private static async Task WriteAsync(HttpContext context, string message, ErrorType type)
    {
        var status = ErrorTypeMapper.ToStatusCode(type);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        var payload = Result.Failure(message, type);
        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        await context.Response.WriteAsync(json);
    }
}

/// <summary>Central mapping of <see cref="ErrorType"/> to HTTP status codes.</summary>
public static class ErrorTypeMapper
{
    public static HttpStatusCode ToStatusCode(ErrorType type) => type switch
    {
        ErrorType.Validation => HttpStatusCode.BadRequest,       // 400
        ErrorType.Unauthorized => HttpStatusCode.Unauthorized,   // 401
        ErrorType.Forbidden => HttpStatusCode.Forbidden,         // 403
        ErrorType.NotFound => HttpStatusCode.NotFound,           // 404
        ErrorType.Conflict => HttpStatusCode.Conflict,           // 409
        ErrorType.NotCreated => HttpStatusCode.UnprocessableEntity, // 422
        _ => HttpStatusCode.InternalServerError                  // 500
    };
}
