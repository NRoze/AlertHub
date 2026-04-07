using AlertHub.Api.Options;
using AlertHub.WebApi.Logging;
using AlertHub.WebApi.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

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
        try
        {
            // Test if Redis is even alive before starting the stream
            var db = _redis.GetDatabase();
            await db.PingAsync();

            await Response.Body.FlushAsync(); // Send 200 OK to browser
        }
        catch (Exception ex)
        {
            // This will now be caught by your NEW Middleware 
            // and show the REAL error in the React console.
            throw new Exception($"Redis Connection Failed: {ex.Message}");
        }
        Response.Headers.Append("Content-Type", "text/event-stream; charset=utf-8");
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

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
                if (_logger.IsEnabled(LogLevel.Debug)) _logger.MessageRecieved(channel!);
                var data = $"data: {message.MinifyJson()}\n\n";

                try
                {
                    await Response.WriteAsync(data, cancellationToken);
                    await Response.Body.FlushAsync(cancellationToken);
            }
            catch 
            {
                if (_logger.IsEnabled(LogLevel.Debug)) _logger.ClientDisconnected(channel!);
            }
        }
    }
}
