using AlertHub.Api.Options;
using AlertHub.WebApi.Logging;
using AlertHub.WebApi.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Threading.Channels;

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
        Response.Headers.Append("Cache-Control", "no-cache, no-transform");
        Response.Headers.Append("Connection", "keep-alive");
        Response.Headers.Append("X-Accel-Buffering", "no");

        await Response.Body.FlushAsync(cancellationToken);

        // 2. Create a local buffer (Channel) to bridge Redis to the main thread
        var buffer = Channel.CreateUnbounded<string>();
        var subscriber = _redis.GetSubscriber();

        // 3. Subscribe and move data into the local buffer
        await subscriber.SubscribeAsync(_redisOptions.AlertsChannel, (channel, message) =>
        {
            if (message.HasValue)
            {
                buffer.Writer.TryWrite(message.MinifyJson());
            }
        });

        try
        {
            // 4. Survival Loop: Keep the connection alive with heartbeats
            while (!cancellationToken.IsCancellationRequested)
            {
                // Wait for a message OR a 15-second timeout
                using var heartbeatCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                heartbeatCts.CancelAfter(TimeSpan.FromSeconds(15));

                try
                {
                    // Wait for data from Redis (via our buffer)
                    if (await buffer.Reader.WaitToReadAsync(heartbeatCts.Token))
                    {
                        while (buffer.Reader.TryRead(out var data))
                        {
                            await Response.WriteAsync($"data: {data}\n\n", cancellationToken);
                            await Response.Body.FlushAsync(cancellationToken);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // 5. If 15s pass with no Redis data, send a heartbeat to keep F1 proxy open
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await Response.WriteAsync(": heartbeat\n\n", cancellationToken);
                        await Response.Body.FlushAsync(cancellationToken);
                    }
                }
            }
        }
        finally
        {
            await subscriber.UnsubscribeAsync(_redisOptions.AlertsChannel);
        }
    }
    //[HttpGet("sse")]
    //public async Task GetSSE(CancellationToken cancellationToken)
    //{
    //    Response.Headers.Append("Content-Type", "text/event-stream");
    //    Response.Headers.Append("Cache-Control", "no-cache");
    //    Response.Headers.Append("Connection", "keep-alive");
    //    Response.Headers.Append("X-Accel-Buffering", "no");

    //    await Response.Body.FlushAsync(cancellationToken);

    //    var subscriber = _redis.GetSubscriber();

    //    await subscriber.SubscribeAsync(_redisOptions.AlertsChannel, async (channel, message) =>
    //    {
    //        if (!cancellationToken.IsCancellationRequested)
    //        {
    //            await HandleMessage(channel, message, cancellationToken);
    //        }
    //    });

    //    try
    //    {
    //        await Task.Delay(Timeout.Infinite, cancellationToken);
    //    }
    //    catch (OperationCanceledException)
    //    { }
    //    finally
    //    {
    //        await subscriber.UnsubscribeAsync(_redisOptions.AlertsChannel);
    //    }
    //}

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
