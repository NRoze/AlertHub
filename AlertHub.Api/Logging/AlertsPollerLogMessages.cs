using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AlertHub.Api.Logging;

internal static partial class AlertsPollerLogMessages
{
    [LoggerMessage(EventId = 100, Level = LogLevel.Information, 
        Message = "New Alert recieved: {Alert}")]
    public static partial void NewAlert(this ILogger logger, string alert);

    [LoggerMessage(EventId = 101, Level = LogLevel.Error, 
        Message = "Error polling Pikud HaOref API: {Message}")]
    public static partial void PollingError(this ILogger logger, string message);

    [LoggerMessage(EventId = 102, Level = LogLevel.Information, 
        Message = "Scheduled execution of Alerts Poller function with last: {Last} | next: {Next}")]
    public static partial void LogScheduledExecution(this ILogger logger, DateTime? last, DateTime? next);
    public static void LogScheduledExecution(this ILogger logger, TimerInfo timer)
        => LogScheduledExecution(logger, timer.ScheduleStatus?.Last, timer.ScheduleStatus?.Next);
}
