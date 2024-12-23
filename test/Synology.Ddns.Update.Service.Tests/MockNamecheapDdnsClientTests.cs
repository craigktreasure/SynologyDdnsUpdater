namespace Synology.Ddns.Update.Service.Tests;

using global::Namecheap.Library.Models;

public class MockNamecheapDdnsClientTests
{
    [Fact]
    public async Task UpdateHostIpAddressAsync_Success()
    {
        // Arrange
        MockNamecheapDdnsClient client = new();

        // Act
        NamecheapDdnsUpdateResponse response = await client.UpdateHostIpAddressAsync(
            "@", "mydomain.com", "mypassword", "127.0.0.1", TestContext.Current.CancellationToken);

        // Assert
        Assert.True(response.Success);
    }
}
