namespace Synology.Ddns.Update.Service.Extensions;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Web;

using Azure.Monitor.OpenTelemetry.AspNetCore;

using Microsoft.Extensions.Primitives;

using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using Synology.Ddns.Update.Service.Monitoring;
using Synology.Ddns.Update.Service.Options;

[ExcludeFromCodeCoverage]
internal static class OpenTelemetryExtensions
{
    /// <summary>
    /// Adds and configures OpenTelemetry.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <returns><see cref="WebApplicationBuilder"/>.</returns>
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        MonitoringOptions monitoringOptions = MonitoringOptions.FromConfiguration(builder.Configuration);

        if (!monitoringOptions.OpenTelemetryEnabled)
        {
            return builder;
        }

        static void configureResource(ResourceBuilder builder)
        {
            builder
            .AddService(
                serviceName: TelemetryConstants.ServiceName,
                serviceVersion: TelemetryConstants.ServiceVersion,
                serviceInstanceId: Environment.MachineName);
        }

#pragma warning disable CS0618 // Type or member is obsolete
        OpenTelemetryBuilder openTelemetryBuilder = builder.Services
            .AddOpenTelemetry()
            .ConfigureResource(configureResource)
            .WithTracing(builder =>
            {
                builder
                    .SetSampler(new AlwaysOnSampler())
                    .AddConsoleExporter()
                    .AddSource(TelemetryConstants.ServiceName)
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        // The Microsoft.AspNetCore logging logs the url with query parameters and we don't want to log
                        // some specific ones. So, we painfully redact them here.
                        options.EnrichWithHttpRequest = RedactHttpRequest;
                    });
            })
            .WithMetrics(builder =>
            {
                builder
                    .AddConsoleExporter()
                    .AddMeter(Telemetry.Meter.Name)
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation();
            });
#pragma warning restore CS0618 // Type or member is obsolete

        if (monitoringOptions.AzureMonitorEnabled)
        {
            openTelemetryBuilder.UseAzureMonitor();
        }

        // Clear default logging providers used by WebApplication host.
        builder.Logging.ClearProviders();

        builder.Logging.AddOpenTelemetry(options =>
        {
            ResourceBuilder resourceBuilder = ResourceBuilder.CreateDefault();
            configureResource(resourceBuilder);
            options
                .SetResourceBuilder(resourceBuilder)
                .AddConsoleExporter();
        });

        return builder;
    }

    private static void RedactHttpRequest(Activity activity, HttpRequest request)
    {
        if (activity.Source.Name != "Microsoft.AspNetCore" || request.Query.Count == 0)
        {
            return;
        }

        bool hasIp = request.Query.TryGetValue("ip", out StringValues ipQueryValues);
        bool hasPassword = request.Query.TryGetValue("password", out StringValues passwordQueryValues);

        if (hasIp || hasPassword)
        {
            // The request.Host contains host:port.
            string[] hostParts = request.Host.Value.Split(":");

            if (hostParts.Length != 2)
            {
                throw new InvalidOperationException($"The host didn't contain the expected value: '{request.Host.Value}'.");
            }

            UriBuilder uriBuilder = new(request.Scheme, hostParts[0], int.Parse(hostParts[1], CultureInfo.InvariantCulture.NumberFormat))
            {
                Path = request.Path,
                Query = request.QueryString.Value,
            };
            string redactedUrl = uriBuilder.ToString();

            if (hasIp)
            {
                redactedUrl = RedactUrlValues(redactedUrl, ipQueryValues);
            }

            if (hasPassword)
            {
                redactedUrl = RedactUrlValues(redactedUrl, passwordQueryValues);
            }

            activity.SetTag("http.url", redactedUrl);
        }
    }

    private static string RedactUrlValue(string url, string? valueToRedact)
    {
        if (valueToRedact is not null)
        {
            return url.Replace(HttpUtility.UrlEncode(valueToRedact), "****", StringComparison.Ordinal);
        }

        return url;
    }

    private static string RedactUrlValues(string url, StringValues valuesToRedact)
    {
        foreach (string? valueToRedact in valuesToRedact)
        {
            url = RedactUrlValue(url, valueToRedact);
        }

        return url;
    }
}
