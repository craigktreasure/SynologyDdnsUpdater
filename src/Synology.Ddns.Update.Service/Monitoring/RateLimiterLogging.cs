namespace Synology.Ddns.Update.Service.Monitoring;

internal static partial class RateLimiterLogging
{
    [LoggerMessage(
        EventName = nameof(OnRejected),
        Level = LogLevel.Warning,
        Message = "Request rejected due to rate limiting.")]
    public static partial void OnRejected(this ILogger logger);
}
