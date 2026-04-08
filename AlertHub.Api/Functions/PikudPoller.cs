using AlertHub.Api.Logging;
using AlertHub.Api.Services;
using Microsoft.Azure.Functions.Worker;

namespace AlertHub.Api.Functions;

internal sealed class PikudPoller
{
    private readonly IPikudPollerService _pikudPollerService;

    public PikudPoller(IPikudPollerService pikudPollerService)
    {
        _pikudPollerService = pikudPollerService;
    }

    [Function("PikudPoller")]
    [SignalROutput(HubName = "alertsHub")]
    public async Task<SignalRMessageAction?> Run(
        [TimerTrigger("%POLL_INTERVAL_CRON%")] TimerInfo timer,
        FunctionContext context,
        CancellationToken cancellationToken)
    {
        var logger = context.GetLogger("PikudPoller");
        
        try
        {
            var alerts = await _pikudPollerService
                .GetAlertsAsJson(cancellationToken).ConfigureAwait(false);
            
            if (alerts?.Any() != true) return null;

            return new SignalRMessageAction("newAlert")
            {
                Arguments = [alerts]
            };
        }
        catch (Exception ex)
        {
            logger.PollingError(ex.Message);
            throw;
        }
    }
}