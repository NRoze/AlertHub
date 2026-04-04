using AlertHub.Api.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace AlertHub.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertsController : ControllerBase
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<AlertsController> _logger;
    private readonly RedisOptions _redisOptions;

    public AlertsController(
        IConnectionMultiplexer multiplexer, 
        IOptions<RedisOptions> redisOptions,
        ILogger<AlertsController> logger)
    {
        _redis = multiplexer;
        _logger = logger;
        _redisOptions = redisOptions.Value;
    }

    [HttpGet("sse")]
    public async Task GetSSE(CancellationToken cancellationToken)
    {
        Response.Headers.Append("Content-Type", "text/event-stream; charset=utf-8");
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Access-Control-Allow-Origin", "*");

        var subscriber = _redis.GetSubscriber();
        var tcs = new TaskCompletionSource();

        await subscriber.SubscribeAsync(_redisOptions.AlertsChannel, async (channel, message) =>
        {
            await HandleMessage(channel, message, cancellationToken);
        });

        try
        {
            await tcs.Task.WaitAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            await subscriber.UnsubscribeAsync(_redisOptions.AlertsChannel);
        }
    }

    private async Task HandleMessage(
        RedisChannel channel, 
        RedisValue message, 
        CancellationToken cancellationToken)
    { 
            if (message.HasValue)
            {
                _logger.LogInformation("Message received on channel {Channel}: {Message}", channel, message);
                string jsonString = JsonSerializer.Serialize(message.ToString()); 
                var data = $"data: {jsonString}\n\n";

                try
                {
                    await Response.WriteAsync(data, cancellationToken);
                    await Response.Body.FlushAsync(cancellationToken);
                    _logger.LogInformation("Message sent to client: {Data}", data);
            }
            catch 
            {
                    _logger.LogInformation("Client disconnected while sending message: {Data}", data);
            }
        }
    }
}
