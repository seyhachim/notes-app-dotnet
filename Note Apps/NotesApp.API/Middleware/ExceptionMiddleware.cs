using System.Net;
using System.Text.Json;

namespace NotesApp.API.Middleware;

/// <summary>
/// Sits at the top of the middleware pipeline and catches every unhandled exception
/// from any controller, service, or repository in the app.
/// 
/// Without this, ASP.NET returns a 500 with a full stack trace —
/// which leaks internal implementation details to the client.
/// 
/// With this, every error returns a clean JSON shape with the right HTTP status code.
/// One place to handle all errors instead of try/catch in every controller.
/// </summary>
public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Pass the request down the pipeline normally
            await next(context);
        }
        catch (Exception ex)
        {
            // Log the full exception internally — we want the details for debugging
            // but we never send them to the client
            logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        // Map exception types to HTTP status codes
        // This is where all the throws in our services get translated to proper responses
        var (statusCode, message) = ex switch
        {
            // Service throws this when email/username already taken → 409 Conflict
            InvalidOperationException =>
                (HttpStatusCode.Conflict, ex.Message),

            // Service throws this when note not found or wrong user → 404 Not Found
            KeyNotFoundException =>
                (HttpStatusCode.NotFound, ex.Message),

            // Service throws this when password is wrong → 401 Unauthorized
            UnauthorizedAccessException =>
                (HttpStatusCode.Unauthorized, ex.Message),

            // Argument validation failures → 400 Bad Request
            ArgumentException =>
                (HttpStatusCode.BadRequest, ex.Message),

            // Anything else is a real server error → 500
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
            // Note: we never send the real error message for 500s —
            // stack traces and internal details stay on the server
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            status = (int)statusCode,
            message
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}