namespace Synology.Ddns.Update.Service.Monitoring;

using System.Diagnostics;
using System.Diagnostics.Metrics;

internal static class Telemetry
{
    public static readonly ActivitySource ActivitySource = new(TelemetryConstants.ServiceName);

    public static readonly Meter Meter = new(TelemetryConstants.ServiceName, TelemetryConstants.ServiceVersion);
}
