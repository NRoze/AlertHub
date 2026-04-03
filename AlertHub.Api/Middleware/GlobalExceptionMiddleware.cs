using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Azure.Functions.Worker;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AlertHub.Api.Middleware;

internal sealed class GlobalExceptionMiddleware : IFunctionsWorkerMiddleware
{
    public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
    {
        try
        {
            await next(context);
        }
        catch
        {
            var req = await context.GetHttpRequestDataAsync();
            var response = context.GetHttpResponseData();

            if (response == null && req != null)
            {
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            if (response != null)
            {
                var problemDetails = new ProblemDetails
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "An unexpected error occurred.",
                    Detail = "Internal Server Error",
                    Instance = context.FunctionDefinition.Name
                };

                response.Headers.Add("Content-Type", "application/problem+json");
                var json = JsonSerializer.Serialize(problemDetails);
                await response.WriteStringAsync(json);

                context.GetInvocationResult().Value = response;
            }
        }
    }
}
