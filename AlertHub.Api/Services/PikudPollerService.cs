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

        // Add User-Agent; Pikud HaOref sometimes blocks empty UAs
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

        var response = await client.GetAsync(_pikudApiUrl, cancellationToken);

        // 204 No Content is common when there are no alerts
        if (response.StatusCode == System.Net.HttpStatusCode.NoContent) return [];

        response.EnsureSuccessStatusCode();

        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);

        if (bytes == null || bytes.Length <= 5) return [];

        try
        {
            using var json = JsonDocument.Parse(bytes);

            if (!json.RootElement.TryGetProperty(_dataPropertyName, out var data))
                return [];

            return [.. data.EnumerateArray()
                       .Select(a => a.GetString() ?? string.Empty)
                       .Where(s => !string.IsNullOrWhiteSpace(s))];
        }
        catch (JsonException)
        {
            return [];
        }
    }
}
