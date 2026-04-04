using AlertHub.Api.Logging;
using AlertHub.Api.Models;
using AlertHub.Api.Options;
using AlertHub.Api.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace AlertHub.Api.Functions;

internal sealed class PikudPoller
{
    private readonly IPikudPollerService _pikudPollerService;
    private readonly IAlertCache _alertCache;
    private readonly ISubscriber _subscriber;
    private readonly RedisOptions _redisOptions;

    public PikudPoller(
        IPikudPollerService pikudPollerService, 
        IAlertCache alertCache, 
        IConnectionMultiplexer multiplexer, 
        IOptions<RedisOptions> redisOptions)
    {
        _pikudPollerService = pikudPollerService;
        _alertCache = alertCache;
        _subscriber = multiplexer.GetSubscriber();
        _redisOptions = redisOptions.Value;
    }

    [Function("PikudPoller")]
    public async Task Run(
        [TimerTrigger("%POLL_INTERVAL_CRON%")] TimerInfo timer, 
        FunctionContext context,
        CancellationToken cancellationToken)
    {
        var logger = context.GetLogger("PikudPoller");
        
        //logger.LogScheduledExecution(timer);

        try
        {
            var alerts = await _pikudPollerService
                .GetAlertsAsJson(cancellationToken).ConfigureAwait(false);

            foreach (var alert in alerts)
            {
                var alertDto = JsonSerializer.Deserialize<AlertMessageDto>(alert)!;

                if (await _alertCache.TryAddAsync(alertDto.Id, cancellationToken))
                {
                    logger.NewAlert(alertDto);
                    await _subscriber.PublishAsync(_redisOptions.AlertsChannel, alert);
                }
            }
        }
        catch (Exception ex)
        {
            logger.PollingError(ex.Message);
            throw;
        }
    }
}