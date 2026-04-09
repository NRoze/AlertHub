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
    private const string _dataPropertyName = "data";
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

    public async Task<IReadOnlyList<string>> GetAlertsAsJson(CancellationToken cancellationToken)
    {
        _logger.LogInformation("[PikudPollerService] Starting");
        var client = _httpClientFactory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, _pikudApiUrl);

        // Browser-mimicking headers
        request.Headers.Add("Origin", "https://www.oref.org.il");
        request.Headers.Referrer = new Uri("https://www.oref.org.il/");
        request.Headers.Add("X-Requested-With", "XMLHttpRequest");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.AcceptLanguage.ParseAdd("he-IL,he;q=0.9,en-US;q=0.8,en;q=0.7");
        request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        request.Headers.Add("Cache-Control", "no-cache");
        request.Headers.ConnectionClose = false;
        
        if (!string.IsNullOrEmpty(_lastEtag))
        {
            if (EntityTagHeaderValue.TryParse(_lastEtag, out var etag))
            {
                request.Headers.IfNoneMatch.Add(etag);
            }
        }

        // Add User-Agent; Pikud HaOref sometimes blocks empty UAs
        //client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(5));
        
        var response = await client.SendAsync(request, cts.Token);
        //var response = await client.GetAsync(_pikudApiUrl, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotModified) return Array.Empty<string>();
        if (response.StatusCode == HttpStatusCode.NoContent) return Array.Empty<string>();

        if (response.Headers.ETag != null)
        {
            _lastEtag = response.Headers.ETag.ToString();
        }

        if (!response.IsSuccessStatusCode) return Array.Empty<string>();

        foreach (var (key, value) in response.Headers)
        {
            var values = string.Join(", ", value);
            _logger.LogInformation("[PikudPollerService] Header - {Key}: {Value}", key, values);
        }

        // 2. Log Content Headers (Type, Length, Encoding)
        foreach (var (key, value) in response.Content.Headers)
        {
            var values = string.Join(", ", value);
            _logger.LogInformation("[PikudPollerService] Content Header - {Key}: {Value}", key, values);
        }

        long? length = response.Content.Headers.ContentLength;

        if (length <= 5)
        {
            _logger.LogInformation("Confirmed empty payload via length check ({Length})", length);
            return [];
        }

        var bytes = await response.Content.ReadAsByteArrayAsync(cts.Token);
        //var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);

        _logger.LogInformation("[PikudPollerService] Read As Byte {bytes}", bytes);

        try
        {
            _logger.LogInformation("[PikudPollerService] Parsing {bytes}", bytes);
            using var json = JsonDocument.Parse(bytes);

            _logger.LogInformation("[PikudPollerService] Parsed {json}", json);
            if (json.RootElement.TryGetProperty(_dataPropertyName, out var data) &&
                data.ValueKind == JsonValueKind.Array)
            {
                return data.EnumerateArray()
                       .Select(a => a.GetString())
                       .Where(s => !string.IsNullOrWhiteSpace(s))
                       .ToList()!;
            }   
        }
        catch (JsonException ex)
        {
            _logger.LogError("[PikudPollerService] Error parsing {Message}", ex.Message);
        }

        return [];
    }
}


// ### explicitly parse out BOM and new line
//var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);

//    // Skip the BOM if present (first 3 bytes)
//    var span = new ReadOnlySpan<byte>(bytes);
//if (span.StartsWith([239, 187, 191]))
//{
//    span = span[3..];
//}

//// Trim whitespace from the start (skips those 13, 10 bytes)
//span = span.TrimStart((byte)' ').TrimStart((byte)'\r').TrimStart((byte)'\n');

//// If nothing is left, it's empty
//if (span.IsEmpty) return [];

//try
//{
//    // Pass the span directly - it's fast and handles the trimmed data
//    using var json = JsonDocument.Parse(span.ToArray());
//    // ... rest of your logic
//}
//catch (JsonException)
//{
//    return [];
//}