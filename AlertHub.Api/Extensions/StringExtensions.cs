using System.Text.Json;

namespace AlertHub.Api.Extensions;

internal static class StringExtensions
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = false
    };

    extension(string value)
    {
        public string MinifyJson()
        {
            using var doc = JsonDocument.Parse(value);

            return JsonSerializer.Serialize(doc.RootElement, _jsonOptions);
        }
    }
}
