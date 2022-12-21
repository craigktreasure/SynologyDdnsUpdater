namespace Synology.Ddns.Update.Service.Tests.Options;

using Microsoft.Extensions.Configuration;
using Synology.Ddns.Update.Service.Options;

public class GlobalRateLimiterOptionsTests
{
    [Fact]
    public void FromConfiguration()
    {
        // Arrange
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
            {
                [$"{nameof(GlobalRateLimiterOptions)}:{nameof(GlobalRateLimiterOptions.PermitLimit)}"] = "100",
                [$"{nameof(GlobalRateLimiterOptions)}:{nameof(GlobalRateLimiterOptions.QueueLimit)}"] = "200",
                [$"{nameof(GlobalRateLimiterOptions)}:{nameof(GlobalRateLimiterOptions.Window)}"] = "00:02:03",
            })
            .Build();

        // Act
        GlobalRateLimiterOptions options = GlobalRateLimiterOptions.FromConfiguration(configuration);

        // Assert
        Assert.Equal(100, options.PermitLimit);
        Assert.Equal(200, options.QueueLimit);
        Assert.Equal(TimeSpan.FromSeconds(123), options.Window);
    }

    [Fact]
    public void FromConfiguration_EmptyConfiguration()
    {
        // Arrange
        IConfiguration configuration = new ConfigurationBuilder().Build();

        // Act
        GlobalRateLimiterOptions options = GlobalRateLimiterOptions.FromConfiguration(configuration);

        // Assert
        Assert.Equal(5, options.PermitLimit);
        Assert.Equal(5, options.QueueLimit);
        Assert.Equal(TimeSpan.FromSeconds(15), options.Window);
    }
}
