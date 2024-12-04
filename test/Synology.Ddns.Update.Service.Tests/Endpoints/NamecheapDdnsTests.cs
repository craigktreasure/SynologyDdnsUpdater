namespace Synology.Ddns.Update.Service.Tests.Endpoints;

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

using global::Namecheap.Library;

using Microsoft.Extensions.Logging;

using Synology.Ddns.Update.Service.Endpoints;
using Synology.Namecheap.Adapter.Library;

using Test.Library;
using Test.Library.Namecheap;

using Xunit.Abstractions;

public class NamecheapDdnsTests
{
    private readonly ITestOutputHelper testOutputHelper;

    public NamecheapDdnsTests(ITestOutputHelper testOutputHelper) => this.testOutputHelper = testOutputHelper;

    [Theory]
    [InlineData(SynologyDdnsResponses.NoHost, MockResponseConstants.DomainNameNotFound)]
    [InlineData("911 [Invalid IP]", MockResponseConstants.InvalidIp)]
    [InlineData(SynologyDdnsResponses.BadAuth, MockResponseConstants.PasswordsDoNotMatch)]
    [InlineData(SynologyDdnsResponses.NoHost, MockResponseConstants.RecordNotFound)]
    [InlineData(SynologyDdnsResponses.Good, MockResponseConstants.Success)]
    [InlineData("911 [Some unexpected error occurred;]", MockResponseConstants.UnexpectedError)]
    public async Task Update(string expectedResponse, string clientResponse)
    {
        // Arrange
        ILogger<NamecheapDdns> logger = this.testOutputHelper.BuildLoggerFor<NamecheapDdns>();
        using NamecheapDdnsClient namecheapDdnsClient = BuildMockedClient(clientResponse);
        const string host = "@";
        const string domain = "mydomain.com";
        const string password = "secret";
        const string ip = "127.0.0.1";

        // Act
        string result = await NamecheapDdns.Update(logger, namecheapDdnsClient, host, domain, password, ip);

        // Assert
        Assert.Equal(expectedResponse, result);
    }

    [Fact]
    public async Task Update_WithClientException()
    {
        // Arrange
        ILogger<NamecheapDdns> logger = this.testOutputHelper.BuildLoggerFor<NamecheapDdns>();
        using NamecheapDdnsClient namecheapDdnsClient = BuildMockedClient(_ => throw new InvalidOperationException());
        const string host = "@";
        const string domain = "mydomain.com";
        const string password = "secret";
        const string ip = "127.0.0.1";

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() => NamecheapDdns.Update(logger, namecheapDdnsClient, host, domain, password, ip));
    }

    private static NamecheapDdnsClient BuildMockedClient(string responseContent = MockResponseConstants.Success)
        => BuildMockedClient(_ => BuildResponseMessage(responseContent));

    [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Dispose handled by NamecheapDdnsClient.")]
    private static NamecheapDdnsClient BuildMockedClient(Func<HttpRequestMessage, HttpResponseMessage> sendAction)
        => new(new HttpClient(new MockHttpMessageHandler(sendAction)));

    private static HttpResponseMessage BuildResponseMessage(string content)
    {
        HttpResponseMessage response = new(HttpStatusCode.OK)
        {
            Content = new ReadOnlyMemoryContent(Encoding.UTF8.GetBytes(content)),
        };
        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        return response;
    }

}
