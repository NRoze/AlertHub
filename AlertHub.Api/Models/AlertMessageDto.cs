using System.Text.Json.Serialization;

namespace AlertHub.Api.Models;
public sealed class AlertMessageDto
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = default!;
    [JsonPropertyName("cat")]
    public string Cat { get; init; } = "0";
    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;
    [JsonPropertyName("data")]
    public List<string> Data { get; init; } = [];
    [JsonPropertyName("desc")]
    public string Desc { get; init; } = string.Empty;
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }
    [JsonPropertyName("expiresAt")]
    public long ExpiresAt { get; set; }
}
