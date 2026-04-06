using System.Net;
using System.Text.Json;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
            {
                return;
            }

            var corsHeader = context.Response.Headers["Access-Control-Allow-Origin"];

            context.Response.Clear();
            if (!string.IsNullOrEmpty(corsHeader))
            {
                context.Response.Headers.Append("Access-Control-Allow-Origin", corsHeader);
            }

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/problem+json";

            var problemDetails = new
            {
                Status = context.Response.StatusCode,
                Title = "An unexpected error occurred.",
                Detail = ex.Message,
                Instance = context.TraceIdentifier
            };

            var json = JsonSerializer.Serialize(problemDetails);

            await context.Response.WriteAsync(json);
        }
    }
}