using AlertHub.Api.Options;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;

namespace AlertHub.Api.Functions;

internal sealed class AlertsSSE
{
    private readonly ISubscriber _subscriber;
    private readonly RedisOptions _redisOptions;

    public AlertsSSE(IConnectionMultiplexer multiplexer, IOptions<RedisOptions> redisOptions)
    {
        _subscriber = multiplexer.GetSubscriber();
        _redisOptions = redisOptions.Value;
    }

    [Function("AlertsSSE")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req, 
        CancellationToken cancellationToken)
    {
        var response = req.CreateResponse();

        response.Headers.Add("Content-Type", "text/event-stream");
        response.Headers.Add("Cache-Control", "no-cache");

        var client = req.Body;

        await _subscriber.SubscribeAsync(_redisOptions.AlertsChannel, async (channel, message) =>
        {
            var data = $"data: {JsonSerializer.Serialize(message)}\n\n";
            var bytes = Encoding.UTF8.GetBytes(data);
            await response.Body.WriteAsync(bytes, cancellationToken);
            await response.Body.FlushAsync(cancellationToken);
        });

        // Keep the HTTP connection open until the client disconnects
        try
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            // Client connection closed
        }
        finally
        {
            // Stop listening to redis when connection drops
            await _subscriber.UnsubscribeAsync(_redisOptions.AlertsChannel);
        }

        return response;
    }
}