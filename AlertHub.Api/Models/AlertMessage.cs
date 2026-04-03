using AlertHub.Api.Enums;

namespace AlertHub.Api.Models;

public sealed class AlertMessage
{
    public string Id { get; init; } = default!;
    public DateTimeOffset Timestamp { get; init; }

    public AlertType Type { get; init; }
    public AlertSeverity Severity { get; init; }

    public IReadOnlyList<string> Locations { get; init; } = [];
    public string Instructions { get; init; } = string.Empty;

    public bool IsActive { get; init; }

    // Keep raw for debugging / auditing
    public AlertMessageDto Raw { get; init; } = default!;
}
