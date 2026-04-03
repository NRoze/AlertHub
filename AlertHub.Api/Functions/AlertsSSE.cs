using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;

public class AlertsSSE
{
    private readonly ISubscriber _subscriber;

    public AlertsSSE()
    {
        _subscriber = RedisConnection.Connection.GetSubscriber();
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