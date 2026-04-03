using AlertHub.Api.Extensions;
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
        var response = await _httpClientFactory.CreateClient()
            .GetAsync(_pikudApiUrl, cancellationToken);

        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

        if (stream.IsEmpty(emptyLength: 5)) return [];

        var json = await JsonSerializer.DeserializeAsync<JsonElement>(stream, cancellationToken: cancellationToken);

        if (!json.TryGetProperty(_dataPropertyName, out var data)) return [];

        return [.. data.EnumerateArray().Select(a => a.GetString() ?? default!)];

    }
}
