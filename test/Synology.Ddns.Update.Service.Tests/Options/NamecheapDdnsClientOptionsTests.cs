namespace Synology.Ddns.Update.Service.Tests.Options;

using Microsoft.Extensions.Configuration;
using Synology.Ddns.Update.Service.Options;

public class NamecheapDdnsClientOptionsTests
{
    [Fact]
    public void FromConfiguration()
    {
        // Arrange
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
            {
                [$"{nameof(NamecheapDdnsClientOptions)}:{nameof(NamecheapDdnsClientOptions.MockClient)}"] = "true",
            })
            .Build();

        // Act
        NamecheapDdnsClientOptions options = NamecheapDdnsClientOptions.FromConfiguration(configuration);

        // Assert
        Assert.True(options.MockClient);
    }

    [Fact]
    public void FromConfiguration_EmptyConfiguration()
    {
        // Arrange
        IConfiguration configuration = new ConfigurationBuilder().Build();

        // Act
        NamecheapDdnsClientOptions options = NamecheapDdnsClientOptions.FromConfiguration(configuration);

        // Assert
        Assert.False(options.MockClient);
    }
}
