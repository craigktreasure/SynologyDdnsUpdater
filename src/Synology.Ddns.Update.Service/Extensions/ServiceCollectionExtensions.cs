namespace Synology.Ddns.Update.Service.Extensions;

using System.Globalization;
using System.Net;
using System.Threading.RateLimiting;

using global::Namecheap.Library;

using Synology.Ddns.Update.Service.Monitoring;
using Synology.Ddns.Update.Service.Options;

internal static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the custom rate limiter policies.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddCustomRateLimiter(this IServiceCollection services, IConfiguration configuration)
    {
        GlobalRateLimiterOptions rateLimiterOptions = GlobalRateLimiterOptions.FromConfiguration(configuration);

        services.AddRateLimiter(limiterOptions =>
        {
            limiterOptions.OnRejected = (context, cancellationToken) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                }

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                    .CreateLogger("Microsoft.AspNetCore.RateLimitingMiddleware")
                    .OnRejected();

                return new ValueTask();
            };

            limiterOptions.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, IPAddress>(context =>
            {
                return RateLimitPartition.GetFixedWindowLimiter(GetRemoteIpAddress(context), _ =>
                    new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = rateLimiterOptions.PermitLimit,
                        Window = rateLimiterOptions.Window,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = rateLimiterOptions.QueueLimit,
                    });
            });
        });

        return services;
    }

    /// <summary>
    /// Adds the Namecheap DDNS client.
    /// </summary>
    /// <param name="services">The services.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddNamecheapDdnsClient(this IServiceCollection services, IConfiguration configuration)
    {
        NamecheapDdnsClientOptions namecheapClientOptions = NamecheapDdnsClientOptions.FromConfiguration(configuration);

        if (namecheapClientOptions.MockClient)
        {
            services.AddSingleton<INamecheapDdnsClient, MockNamecheapDdnsClient>();
        }
        else
        {
            services.AddHttpClient<INamecheapDdnsClient, NamecheapDdnsClient>();
        }

        return services;
    }

    private static IPAddress GetRemoteIpAddress(HttpContext httpContext)
        => httpContext.Connection.RemoteIpAddress ?? IPAddress.Loopback;
}
