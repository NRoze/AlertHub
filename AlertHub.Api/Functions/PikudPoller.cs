using AlertHub.Api.Logging;
using AlertHub.Api.Models;
using AlertHub.Api.Options;
using AlertHub.Api.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AlertHub.Api.Functions;

internal sealed class PikudPoller
{
    private readonly CacheOptions _options;
    private readonly IPikudPollerService _pikudPollerService;
    private readonly IAlertCache _alertCache;
    private readonly TimeProvider _timeProvider;

    public PikudPoller(
        IPikudPollerService pikudPollerService,
        IAlertCache alertCache,
        TimeProvider timeProvider,
        IOptions<CacheOptions> options)
    {
        _pikudPollerService = pikudPollerService;
        _alertCache = alertCache;
        _timeProvider = timeProvider;
        _options = options.Value;
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
            var alert = await _pikudPollerService
                .GetAlertsAsJson(cancellationToken).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(alert))
            {
                logger.EmptyPayload();
                return new SignalRMessageAction("ping") { Arguments = ["heartbeat"] };
            }

            if (logger.IsEnabled(LogLevel.Information))
                logger.LogPayload(alert);

            var dto = JsonSerializer.Deserialize<AlertMessageDto>(alert)!;

            dto.Timestamp = dto.Timestamp > 0 ? dto.Timestamp : DateTimeOffset.Now.ToUnixTimeMilliseconds();
            dto.ExpiresAt = dto.Timestamp + (long)_options.AlertExpiry.TotalMilliseconds;
            if (_alertCache.TryAdd(dto!))
            {
                return new SignalRMessageAction("newAlert")
                {
                    Arguments = [dto]
                };
            }

            return new SignalRMessageAction("ping") { Arguments = ["heartbeat"] };
        }
        catch (Exception ex)
        {
            logger.PollingError(ex.Message);
            throw;
        }
    }
}