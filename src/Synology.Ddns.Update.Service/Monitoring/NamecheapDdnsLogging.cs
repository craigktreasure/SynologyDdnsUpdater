namespace Synology.Ddns.Update.Service.Monitoring;

using Synology.Ddns.Update.Service.Endpoints;

internal static partial class NamecheapDdnsLogging
{
    [LoggerMessage(
        EventName = nameof(DdnsUpdated),
        Level = LogLevel.Information,
        Message = "Updated IP address for {Host} {DomainName}")]
    public static partial void DdnsUpdated(
        this ILogger<NamecheapDdns> logger,
        string host,
        string domainName);

    [LoggerMessage(
        EventName = nameof(DdnsUpdateError),
        Level = LogLevel.Error,
        Message = "IP address update failed for {Host} {DomainName}.")]
    public static partial void DdnsUpdateError(
        this ILogger<NamecheapDdns> logger,
        string host,
        string domainName,
        Exception exception);

    [LoggerMessage(
        EventName = nameof(DdnsUpdateFailed),
        Level = LogLevel.Warning,
        Message = "IP address update failed for {Host} {DomainName} with {Result}.")]
    public static partial void DdnsUpdateFailed(
        this ILogger<NamecheapDdns> logger,
        string host,
        string domainName,
        string result);

    [LoggerMessage(
                    EventName = nameof(DdnsUpdating),
        Level = LogLevel.Information,
        Message = "Updating IP address for {Host} {DomainName}")]
    public static partial void DdnsUpdating(
        this ILogger<NamecheapDdns> logger,
        string host,
        string domainName);
}
