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
            var alert = await _pikudPollerService
                .GetAlertsAsJson(cancellationToken).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(alert))
            {
                logger.EmptyPayload();
                return new SignalRMessageAction("ping") { Arguments = ["heartbeat"] };
            }

            if (logger.IsEnabled(LogLevel.Information))
                logger.LogPayload(alert);

            var dto = JsonSerializer.Deserialize<AlertMessageDto>(alert);

            _alertCache.TryAdd(dto!);

            return new SignalRMessageAction("newAlert")
            {
                Arguments = [dto!]
            };
        }
        catch (Exception ex)
        {
            logger.PollingError(ex.Message);
            throw;
        }
    }

    private ImmutableArray<AlertLocationDto>? MapAlertLocations(string alert)
    {
        var dto = JsonSerializer.Deserialize<AlertMessageDto>(alert);
        var timestamp = dto?.Timestamp > 0 ?
                            DateTimeOffset.FromUnixTimeMilliseconds(dto.Timestamp) :
                            _timeProvider.GetLocalNow();

        return dto?.Data?.Select((locationStr) =>
            AlertLocationDto.Create(
                locationStr.Trim(),
                dto.Title.Trim(),
                dto.Desc.Trim(),
                timestamp)).ToImmutableArray();
    }
}