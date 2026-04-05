namespace AlertHub.WebApi.Logging;

internal static partial class AlertsControllerLogMessagesAlertsPollerLogMessages
{
    [LoggerMessage(EventId = 100, Level = LogLevel.Debug,
        Message = "Message received on #{Channel}")]
    public static partial void MessageRecieved(this ILogger logger, string channel);

    [LoggerMessage(EventId = 101, Level = LogLevel.Debug,
        Message = "Client disconnected from #{Channel}")]
    public static partial void ClientDisconnected(this ILogger logger, string channel);
}