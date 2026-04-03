using AlertHub.Api.Infrastructure;
using AlertHub.Api.Logging;
using AlertHub.Api.Services;
using Microsoft.Azure.Functions.Worker;
using StackExchange.Redis;

namespace AlertHub.Api.Functions;

public class PikudPoller
{
    private readonly IPikudPollerService _pikudPollerService;
    private readonly IAlertCache _alertCache;
    private readonly ISubscriber _subscriber;

    public PikudPoller(IPikudPollerService pikudPollerService, IAlertCache alertCache)
    {
        _pikudPollerService = pikudPollerService;
        _alertCache = alertCache;
        _subscriber = RedisConnection.Connection.GetSubscriber();
    }

    [Function("PikudPoller")]
    public async Task Run(
        [TimerTrigger("*/5 * * * * *")] TimerInfo timer, 
        FunctionContext context,
        CancellationToken cancellationToken)
    {
        var logger = context.GetLogger("PikudPoller");

        try
        {
            var alerts = await _pikudPollerService
                .GetAlertsAsJson(cancellationToken).ConfigureAwait(false);

            foreach (var alert in alerts)
            {
                if (await _alertCache.TryAddAsync(alert))
                {
                    logger.NewAlert(alert);
                    await _subscriber.PublishAsync(RedisConnection.AlertsChannel, alert);
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