using AlertHub.Api.Logging;
using AlertHub.Api.Models;
using AlertHub.Api.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Text.Json;

namespace AlertHub.Api.Functions;

internal sealed class PikudPoller
{
    private readonly IPikudPollerService _pikudPollerService;
    private readonly IAlertCache _alertCache; 
    private readonly TimeProvider _timeProvider;

    public PikudPoller(
        IPikudPollerService pikudPollerService, 
        IAlertCache alertCache, 
        TimeProvider timeProvider)
    {
        _pikudPollerService = pikudPollerService;
        _alertCache = alertCache;
        _timeProvider = timeProvider;
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

            if (alerts?.Any() != true)
            {
                logger.EmptyPayload();
                return new SignalRMessageAction("ping") { Arguments = ["heartbeat"] };
            }

            if (logger.IsEnabled(LogLevel.Information))
                logger.LogPayload(alerts);

            var dtos = MapAlertLocations(alerts); 

            _alertCache.TryAddRange(dtos);

            return new SignalRMessageAction("newAlert")
            {
                Arguments = [dtos]
            };
        }
        catch (Exception ex)
        {
            logger.PollingError(ex.Message);
            throw;
        }
    }

    private ImmutableArray<AlertLocationDto> MapAlertLocations(IReadOnlyList<string> alerts)
    {
        return alerts
            .Where(alert => !string.IsNullOrWhiteSpace(alert))
            .Select(alert => JsonSerializer.Deserialize<AlertMessageDto>(alert))
            .SelectMany<AlertMessageDto?, AlertLocationDto>(messageDto =>
            {
                if (messageDto is null) return [];

                var timestamp = messageDto.Timestamp > 0 ?
                            DateTimeOffset.FromUnixTimeMilliseconds(messageDto.Timestamp) : 
                            _timeProvider.GetLocalNow();

                return messageDto.Data.Select((locationStr) =>
                    AlertLocationDto.Create(
                        locationStr.Trim(),
                        messageDto.Title.Trim(),
                        messageDto.Desc.Trim(),
                        timestamp));
            }).ToImmutableArray();
    }
}