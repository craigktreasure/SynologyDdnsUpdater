namespace Synology.Ddns.Update.Service.Tests.Extensions;

using global::Namecheap.Library;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Synology.Ddns.Update.Service.Extensions;
using Synology.Ddns.Update.Service.Options;
using System.Collections.Generic;
using System.Threading.RateLimiting;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public async Task AddCustomRateLimiter()
    {
        // Arrange
        ServiceCollection services = new();
        services.AddLogging();
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
            {
                [$"{nameof(GlobalRateLimiterOptions)}:{nameof(GlobalRateLimiterOptions.PermitLimit)}"] = "1",
                [$"{nameof(GlobalRateLimiterOptions)}:{nameof(GlobalRateLimiterOptions.QueueLimit)}"] = "1",
            })
            .Build();

        // Act
        services.AddCustomRateLimiter(configuration);

        // Assert
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        RateLimiterOptions options = serviceProvider.GetRequiredService<IOptions<RateLimiterOptions>>().Value;
        Assert.NotNull(options);
        Assert.NotNull(options.OnRejected);
        Assert.NotNull(options.GlobalLimiter);

        // Arrange
        HttpContext httpContext = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider(),
        };

        // Act
        RateLimitLease lease = await options.GlobalLimiter.AcquireAsync(httpContext);

        // Arrange
        OnRejectedContext context = new()
        {
            HttpContext = httpContext,
            Lease = new RateLimitLeaseDecorator(lease, (MetadataName.RetryAfter.Name, TimeSpan.FromSeconds(25))),
        };

        // Act
        await options.OnRejected(context, default);
    }

    [Fact]
    public void AddNamecheapDdnsClient_NamecheapDdnsClientOptions_MockClient_False()
    {
        // Arrange
        ServiceCollection services = new();
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
            {
                [$"{nameof(NamecheapDdnsClientOptions)}:{nameof(NamecheapDdnsClientOptions.MockClient)}"] = "false",
            })
            .Build();

        // Act
        services.AddNamecheapDdnsClient(configuration);

        // Assert
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        Assert.IsType<NamecheapDdnsClient>(serviceProvider.GetRequiredService<INamecheapDdnsClient>());
    }

    [Fact]
    public void AddNamecheapDdnsClient_NamecheapDdnsClientOptions_MockClient_True()
    {
        // Arrange
        ServiceCollection services = new();
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
            {
                [$"{nameof(NamecheapDdnsClientOptions)}:{nameof(NamecheapDdnsClientOptions.MockClient)}"] = "true",
            })
            .Build();

        // Act
        services.AddNamecheapDdnsClient(configuration);

        // Assert
        ServiceProvider serviceProvider = services.BuildServiceProvider();
        Assert.IsType<MockNamecheapDdnsClient>(serviceProvider.GetRequiredService<INamecheapDdnsClient>());
    }

    private sealed class RateLimitLeaseDecorator : RateLimitLease
    {
        public override bool IsAcquired => this.lease.IsAcquired;

        public override IEnumerable<string> MetadataNames
            => this.lease.MetadataNames.Concat(this.fallbackMetadata.Keys).Distinct();

        private readonly Dictionary<string, object?> fallbackMetadata;

        private readonly RateLimitLease lease;

        public RateLimitLeaseDecorator(RateLimitLease lease, params (string, object?)[] fallbackMetadata)
            : this(lease, fallbackMetadata.Select(m => new KeyValuePair<string, object?>(m.Item1, m.Item2)).ToArray())
        {
        }

        public RateLimitLeaseDecorator(RateLimitLease lease, params KeyValuePair<string, object?>[] fallbackMetadata)
        {
            this.lease = lease;
            this.fallbackMetadata = new Dictionary<string, object?>(fallbackMetadata, StringComparer.OrdinalIgnoreCase);
        }

        public override bool TryGetMetadata(string metadataName, out object? metadata)
        {
            bool result = this.lease.TryGetMetadata(metadataName, out metadata);

            if (!result)
            {
                result = this.fallbackMetadata.TryGetValue(metadataName, out metadata);
            }

            return result;
        }
    }
}
