using AlertHub.Api.Options;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AlertHub.Api.Services;

internal sealed class PikudPollerService : IPikudPollerService
{
    private const string _dataPropertyName = "data";
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _pikudApiUrl;

    public PikudPollerService(
        IHttpClientFactory httpClientFactory,
        IOptions<PikudPollerOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _pikudApiUrl = options.Value.PikudApiUrl;
    }

    public async Task<IReadOnlyList<string>> GetAlertsAsJson(CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Referrer = new Uri("https://www.oref.org.il/");
        client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
        
        var response = await client.GetAsync(_pikudApiUrl, cancellationToken);
        response.EnsureSuccessStatusCode();

        var rawJson = await response.Content.ReadAsStringAsync(cancellationToken);
        rawJson = rawJson.Trim('\uFEFF', ' ');

        if (string.IsNullOrWhiteSpace(rawJson)) return [];

        using var json = JsonDocument.Parse(rawJson);

        if (!json.RootElement.TryGetProperty(_dataPropertyName, out var data))
            return [];

        return [.. data.EnumerateArray().Select(a => a.GetString() ?? string.Empty)];
    }
}
