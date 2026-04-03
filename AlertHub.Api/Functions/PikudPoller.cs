using AlertHub.Api.Logging;
using AlertHub.Api.Options;
using AlertHub.Api.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

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
        [TimerTrigger("*/5 * * * * *")] TimerInfo timer, 
        FunctionContext context,
        CancellationToken cancellationToken)
    {
        var logger = context.GetLogger("PikudPoller");
        
        logger.LogScheduledExecution(timer);

        try
        {
            var alerts = await _pikudPollerService
                .GetAlertsAsJson(cancellationToken).ConfigureAwait(false);

            foreach (var alert in alerts)
            {
                if (await _alertCache.TryAddAsync(alert, cancellationToken))
                {
                    logger.NewAlert(alert);
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