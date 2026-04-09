using AlertHub.Api.Options;
using Azure.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AlertHub.Api.Services;

public sealed class PikudPollerService : IPikudPollerService
{
    private string? _lastEtag;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PikudPollerService> _logger;
    private readonly string _pikudApiUrl;

    public PikudPollerService(
        IHttpClientFactory httpClientFactory,
        IOptions<PikudPollerOptions> options,
        ILogger<PikudPollerService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _pikudApiUrl = options.Value.PikudApiUrl;
        _logger = logger;
    }
    public async Task<string> GetAlertsAsJson(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[PikudPollerService] Starting");
        var client = _httpClientFactory.CreateClient();
        using var request = CreateRequest();
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        cts.CancelAfter(TimeSpan.FromSeconds(5));
        
        var response = await client.SendAsync(request, cts.Token);

        if (response.StatusCode == HttpStatusCode.NotModified ||
            response.StatusCode == HttpStatusCode.NoContent ||
            response.Content.Headers.ContentLength <= 5 ||
            !response.IsSuccessStatusCode)
        {
            return string.Empty;
        }

        if (response.Headers.ETag != null)
        {
            _lastEtag = response.Headers.ETag.ToString();
        }

        var bytes = await response.Content.ReadAsByteArrayAsync(cts.Token);

        if (bytes.Length == 0) return string.Empty;
        
        try
        {
            using var ms = new MemoryStream(bytes);
            using var reader = new StreamReader(ms, System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            string rawJson = await reader.ReadToEndAsync(cancellationToken);

            return rawJson.Trim();

        }
        catch (JsonException ex)
        {
            _logger.LogError(
                "[PikudPollerService] JSON Error: {Message}",
                ex.Message);
        }

        return string.Empty;
    }

    private HttpRequestMessage CreateRequest()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, _pikudApiUrl);

        request.Headers.Add("Origin", "https://www.oref.org.il");
        request.Headers.Referrer = new Uri("https://www.oref.org.il/");
        request.Headers.Add("X-Requested-With", "XMLHttpRequest");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.AcceptLanguage.ParseAdd("he-IL,he;q=0.9,en-US;q=0.8,en;q=0.7");
        request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        request.Headers.Add("Cache-Control", "no-cache");
        request.Headers.ConnectionClose = false;

        if (!string.IsNullOrEmpty(_lastEtag) &&
            EntityTagHeaderValue.TryParse(_lastEtag, out var etag))
        {
            request.Headers.IfNoneMatch.Add(etag);
        }

        return request;
    }
}