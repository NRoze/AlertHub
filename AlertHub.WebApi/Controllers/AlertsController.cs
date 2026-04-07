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
        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");
        Response.Headers.Append("X-Accel-Buffering", "no");

        // IMPORTANT: Azure App Service needs an immediate flush to "open" the pipe
        // and avoid the 504 Gateway Timeout.
        await Response.WriteAsync(": connected\n\n", cancellationToken);
        await Response.Body.FlushAsync(cancellationToken);

        var subscriber = _redis.GetSubscriber();

        await subscriber.SubscribeAsync(_redisOptions.AlertsChannel, async (channel, message) =>
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                await HandleMessage(channel, message, cancellationToken);
            }
        });

        try
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
        catch (OperationCanceledException)
        { }
        finally
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
