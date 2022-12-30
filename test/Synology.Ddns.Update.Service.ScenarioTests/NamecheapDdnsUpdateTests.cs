namespace Synology.Ddns.Update.Service.ScenarioTests;

using System.Net;
using System.Net.Http.Headers;
using System.Text;

using Synology.Namecheap.Adapter.Library;

using Test.Library.Namecheap;

using Xunit.Abstractions;

public class NamecheapDdnsUpdateTests
{
    private readonly ITestOutputHelper testOutputHelper;

    public NamecheapDdnsUpdateTests(ITestOutputHelper testOutputHelper)
        => this.testOutputHelper = testOutputHelper;

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
        using TestWebAppFactory factory = new(this.testOutputHelper,
            request => BuildResponseMessage(clientResponse));
        using HttpClient client = factory.CreateClient();
        Uri endpoint = BuildEndpoint();

        // Act
        using HttpResponseMessage response = await client.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        string responseContent = await response.Content.ReadAsStringAsync();
        Assert.Equal(expectedResponse, responseContent);
    }

    [Fact]
    public async Task Update_WithClientException()
    {
        // Arrange
        using TestWebAppFactory factory = new(this.testOutputHelper,
            _ => throw new InvalidOperationException());
        using HttpClient client = factory.CreateClient();
        Uri endpoint = BuildEndpoint();

        // Act
        using HttpResponseMessage response = await client.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    private static Uri BuildEndpoint(string host = "@", string domain = "mydomain.com", string password = "myPassword", string ip = "127.0.0.1")
        => new($"/namecheap/ddns/update?host={host}&domain={domain}&password={password}&ip={ip}", UriKind.Relative);

    private static HttpResponseMessage BuildResponseMessage(string content = MockResponseConstants.Success)
    {
        HttpResponseMessage response = new(HttpStatusCode.OK)
        {
            Content = new ReadOnlyMemoryContent(Encoding.UTF8.GetBytes(content)),
        };
        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        return response;
    }
}
