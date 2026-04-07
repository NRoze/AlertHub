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
            var corsHeader = context.Response.Headers.AccessControlAllowOrigin;

            if (context.Response.HasStarted) return;

            context.Response.Clear();

            if (!string.IsNullOrEmpty(corsHeader))
            {
                context.Response.Headers.AccessControlAllowOrigin = corsHeader;
            }

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(new { 
                error = ex.Message, 
                type = ex.GetType().Name,
                traceId = context.TraceIdentifier 
            });
        }
    }
}