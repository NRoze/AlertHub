using StackExchange.Redis;
using System.Text.Json;

namespace AlertHub.WebApi.Extensions;

internal static class StringExtensions
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = false
    };

    extension(RedisValue value)
    {
        public string MinifyJson()
        {
            using var doc = JsonDocument.Parse(value.HasValue ? value.ToString() : string.Empty);

            return JsonSerializer.Serialize(doc.RootElement, _jsonOptions);
        }
    }
}
