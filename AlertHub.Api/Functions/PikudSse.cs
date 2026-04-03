using AlertHub.Api.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace AlertHub.Api.Functions;

public class PikudSse
{
    private static readonly HashSet<string> RecentAlerts = new();
    private const int AlertCacheDurationMs = 5000; // dedupe TTL
    private static readonly Channel<string> AlertChannel = Channel.CreateUnbounded<string>();

    private readonly IPikudPollerService _pikudPollerService;
    private ILogger? _logger;

    public PikudSse(IPikudPollerService pikudPollerService)
    {
        _pikudPollerService = pikudPollerService;

        _ = Task.Run(PollAlertsLoop);
    }

    private async Task PollAlertsLoop()
    {
        while (true)
        {
            try
            {
                var alerts = await _pikudPollerService.GetAlertsAsJson(CancellationToken.None);

                foreach (var alert in alerts)
                {
                    var key = alert + DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                    lock (RecentAlerts)
                    {
                        if (RecentAlerts.Contains(key)) continue;
                        
                        RecentAlerts.Add(key);
                        AlertChannel.Writer.TryWrite(alert);
                        _ = Task.Delay(AlertCacheDurationMs).ContinueWith(_ =>
                        {
                            lock (RecentAlerts) { RecentAlerts.Remove(key); }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError("Error polling Pikud HaOref API: {Message}", ex.Message);
            }

            await Task.Delay(5000); 
        }
    }

    [Function("AlertsSSE")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req,
        FunctionContext context)
    {
        _logger = context.GetLogger("PikudPoller");
        var response = req.CreateResponse();

        response.Headers.Add("Content-Type", "text/event-stream");
        response.Headers.Add("Cache-Control", "no-cache");
        response.StatusCode = System.Net.HttpStatusCode.OK;

        var responseStream = response.Body;
        var reader = AlertChannel.Reader;

        while (true)
        {
            while (reader.TryRead(out var alert))
            {
                var data = $"data: {JsonSerializer.Serialize(alert)}\n\n";
                var bytes = Encoding.UTF8.GetBytes(data);
                await responseStream.WriteAsync(bytes);
                await responseStream.FlushAsync();
            }

            await Task.Delay(200);
        }
    }
}