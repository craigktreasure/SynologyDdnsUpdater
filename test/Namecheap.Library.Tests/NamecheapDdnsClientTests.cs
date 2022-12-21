namespace Namecheap.Library.Tests;

using Namecheap.Library.Models;
using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Test.Library;
using static Test.Library.Namecheap.MockResponseConstants;

public class NamecheapDdnsClientTests
{
    [Fact]
    public void Constructor()
    {
        // Arrange
        using MockHttpMessageHandler messageHandler = new((request) =>
        {
            throw new InvalidOperationException();
        });
        using HttpClient httpClient = new(messageHandler);

        // Act
        _ = new NamecheapDdnsClient(httpClient);
        Assert.Throws<ArgumentNullException>(nameof(httpClient), () => new NamecheapDdnsClient(null!));
    }

    [Fact]
    public async Task UpdateHostIpAddress_UnicodeResponse()
    {
        // Arrange
        Uri? requestUri = null;
        using MockHttpMessageHandler messageHandler = new((request) =>
        {
            requestUri = request.RequestUri;
            HttpResponseMessage response = new(System.Net.HttpStatusCode.OK)
            {
                Content = new ReadOnlyMemoryContent(Encoding.Unicode.GetBytes(Success)),
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml", "utf-16");

            return response;
        });
        using HttpClient httpClient = new(messageHandler);
        NamecheapDdnsClient client = new(httpClient);

        // Act
        NamecheapDdnsUpdateResponse response = await client.UpdateHostIpAddressAsync(
            host: "@",
            domainName: "mydomain.com",
            ddnsPassword: "myDdnsPassword",
            ipAddress: "127.0.0.1");

        // Assert
        Assert.NotNull(requestUri);
        Assert.Equal("https://dynamicdns.park-your-domain.com/update?host=%40&domain=mydomain.com&password=myDdnsPassword&ip=127.0.0.1", requestUri.ToString());
        Assert.True(response.Success);
        Assert.Equal("127.0.0.1", response.IPAddress);
    }

    [Fact]
    public async Task UpdateHostIpAddress_Utf8Response()
    {
        // Arrange
        Uri? requestUri = null;
        using MockHttpMessageHandler messageHandler = new((request) =>
        {
            requestUri = request.RequestUri;
            HttpResponseMessage response = new(System.Net.HttpStatusCode.OK)
            {
                Content = new ReadOnlyMemoryContent(Encoding.UTF8.GetBytes(Success)),
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return response;
        });
        using HttpClient httpClient = new(messageHandler);
        NamecheapDdnsClient client = new(httpClient);

        // Act
        NamecheapDdnsUpdateResponse response = await client.UpdateHostIpAddressAsync(
            host: "@",
            domainName: "mydomain.com",
            ddnsPassword: "myDdnsPassword",
            ipAddress: "127.0.0.1");

        // Assert
        Assert.NotNull(requestUri);
        Assert.Equal("https://dynamicdns.park-your-domain.com/update?host=%40&domain=mydomain.com&password=myDdnsPassword&ip=127.0.0.1", requestUri.ToString());
        Assert.True(response.Success);
        Assert.Equal("127.0.0.1", response.IPAddress);
    }

    [Fact]
    public async Task UpdateHostIpAddress_PrameterInjection()
    {
        // Arrange
        Uri? requestUri = null;
        using MockHttpMessageHandler messageHandler = new((request) =>
        {
            requestUri = request.RequestUri;

            HttpResponseMessage response = new(System.Net.HttpStatusCode.OK)
            {
                Content = new ReadOnlyMemoryContent(Encoding.UTF8.GetBytes(DomainNameNotFound)),
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return response;
        });
        using HttpClient httpClient = new(messageHandler);
        NamecheapDdnsClient client = new(httpClient);

        // Act
        NamecheapDdnsUpdateResponse response = await client.UpdateHostIpAddressAsync(
            host: "@&attack1=malicious",
            domainName: "mydomain.com&attack2=malicious",
            ddnsPassword: "myDdnsPassword&attack3=malicious",
            ipAddress: "127.0.0.1&attack4=malicious");

        // Assert
        Assert.False(response.Success);
        Assert.NotNull(requestUri);
        NameValueCollection queries = HttpUtility.ParseQueryString(requestUri.Query);

        // ParseQueryString above decodes the parameter content
        Assert.Equal("@&attack1=malicious", queries["host"]);
        Assert.Equal("mydomain.com&attack2=malicious", queries["domain"]);
        Assert.Equal("myDdnsPassword&attack3=malicious", queries["password"]);
        Assert.Equal("127.0.0.1&attack4=malicious", queries["ip"]);

        Assert.Equal("https://dynamicdns.park-your-domain.com/update?host=%40%26attack1%3dmalicious&domain=mydomain.com%26attack2%3dmalicious&password=myDdnsPassword%26attack3%3dmalicious&ip=127.0.0.1%26attack4%3dmalicious", requestUri.ToString());
    }
}
