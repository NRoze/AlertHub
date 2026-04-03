using AlertHub.Api.Infrastructure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;

namespace AlertHub.Api.Functions;

public class AlertsSSE
{
    private readonly ISubscriber _subscriber;

    public AlertsSSE(IConnectionMultiplexer multiplexer)
    {
        _subscriber = multiplexer.GetSubscriber();
    }

    [Function("AlertsSSE")]
    public async Task<HttpResponseData> Run(HttpRequestData req)
    {
        var response = req.CreateResponse();

        response.Headers.Add("Content-Type", "text/event-stream");
        response.Headers.Add("Cache-Control", "no-cache");

        var client = req.Body;

        await _subscriber.SubscribeAsync(RedisConnection.AlertsChannel, async (channel, message) =>
        {
            var data = $"data: {JsonSerializer.Serialize(message)}\n\n";
            var bytes = Encoding.UTF8.GetBytes(data);
            await response.Body.WriteAsync(bytes);
            await response.Body.FlushAsync();
        });

        // Keep the HTTP connection open
        // Azure Functions will keep this function alive until the client disconnects
        return response;
    }
}