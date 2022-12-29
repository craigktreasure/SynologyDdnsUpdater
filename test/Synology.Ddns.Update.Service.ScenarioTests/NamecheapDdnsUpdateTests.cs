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

    [Fact]
    public async Task Update_DomainNameNotFound()
    {
        // Arrange
        using TestWebAppFactory factory = new(this.testOutputHelper,
            request => BuildResponseMessage(MockResponseConstants.DomainNameNotFound));
        HttpClient client = factory.CreateClient();
        Uri endpoint = BuildEndpoint();

        // Act
        HttpResponseMessage response = await client.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        string responseContent = await response.Content.ReadAsStringAsync();
        Assert.Equal(SynologyDdnsResponses.NoHost, responseContent);
    }

    [Fact]
    public async Task Update_InvalidIp()
    {
        // Arrange
        using TestWebAppFactory factory = new(this.testOutputHelper,
            request => BuildResponseMessage(MockResponseConstants.InvalidIp));
        HttpClient client = factory.CreateClient();
        Uri endpoint = BuildEndpoint();

        // Act
        HttpResponseMessage response = await client.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        string responseContent = await response.Content.ReadAsStringAsync();
        Assert.Equal("911 [Invalid IP]", responseContent);
    }

    [Fact]
    public async Task Update_PasswordsDoNotMatch()
    {
        // Arrange
        using TestWebAppFactory factory = new(this.testOutputHelper,
            request => BuildResponseMessage(MockResponseConstants.PasswordsDoNotMatch));
        HttpClient client = factory.CreateClient();
        Uri endpoint = BuildEndpoint();

        // Act
        HttpResponseMessage response = await client.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        string responseContent = await response.Content.ReadAsStringAsync();
        Assert.Equal(SynologyDdnsResponses.BadAuth, responseContent);
    }

    [Fact]
    public async Task Update_RecordNotFound()
    {
        // Arrange
        using TestWebAppFactory factory = new(this.testOutputHelper,
            request => BuildResponseMessage(MockResponseConstants.RecordNotFound));
        HttpClient client = factory.CreateClient();
        Uri endpoint = BuildEndpoint();

        // Act
        HttpResponseMessage response = await client.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        string responseContent = await response.Content.ReadAsStringAsync();
        Assert.Equal(SynologyDdnsResponses.NoHost, responseContent);
    }

    [Fact]
    public async Task Update_Success()
    {
        // Arrange
        using TestWebAppFactory factory = new(this.testOutputHelper, request => BuildResponseMessage());
        HttpClient client = factory.CreateClient();
        Uri endpoint = BuildEndpoint();

        // Act
        HttpResponseMessage response = await client.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        string responseContent = await response.Content.ReadAsStringAsync();
        Assert.Equal(SynologyDdnsResponses.Good, responseContent);
    }

    [Fact]
    public async Task Update_UnexpectedError()
    {
        // Arrange
        using TestWebAppFactory factory = new(this.testOutputHelper,
            request => BuildResponseMessage(MockResponseConstants.UnexpectedError));
        HttpClient client = factory.CreateClient();
        Uri endpoint = BuildEndpoint();

        // Act
        HttpResponseMessage response = await client.GetAsync(endpoint);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        string responseContent = await response.Content.ReadAsStringAsync();
        Assert.Equal("911 [Some unexpected error occurred;]", responseContent);
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
