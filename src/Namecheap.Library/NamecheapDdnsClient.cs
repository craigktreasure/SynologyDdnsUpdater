namespace Namecheap.Library;

using Namecheap.Library.Extensions;
using Namecheap.Library.Models;
using System.Globalization;
using System.Text;
using System.Web;

/// <summary>
/// A Namecheap DDNS client used to update the IP address for a given host and domain.
/// https://www.namecheap.com/support/knowledgebase/article.aspx/29/11/how-to-dynamically-update-the-hosts-ip-with-an-http-request/
/// </summary>
public class NamecheapDdnsClient : INamecheapDdnsClient
{
    private readonly HttpClient httpClient;

    private const string endpointFormat = "https://dynamicdns.park-your-domain.com/update?host={0}&domain={1}&password={2}&ip={3}";

    /// <summary>
    /// Initializes a new instance of the <see cref="NamecheapDdnsClient"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    public NamecheapDdnsClient(HttpClient httpClient)
    {
        this.httpClient = Argument.NotNull(httpClient);
    }

    /// <summary>
    /// Updates the host IP address.
    /// </summary>
    /// <param name="host">The host.</param>
    /// <param name="domainName">The domain name.</param>
    /// <param name="ddnsPassword">The DDNS password.</param>
    /// <param name="ipAddress">The IP address.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns><see cref="NamecheapDdnsUpdateResponse"/>.</returns>
    public async Task<NamecheapDdnsUpdateResponse> UpdateHostIpAddressAsync(
        string host,
        string domainName,
        string ddnsPassword,
        string ipAddress,
        CancellationToken cancellationToken = default)
    {
        Argument.NotNullOrWhiteSpace(host);
        Argument.NotNullOrWhiteSpace(domainName);
        Argument.NotNullOrWhiteSpace(ddnsPassword);
        Argument.NotNullOrWhiteSpace(ipAddress);

        Uri endpoint = FormatEndpointUrl(host, domainName, ddnsPassword, ipAddress);

        HttpResponseMessage response = await this.httpClient.GetAsync(endpoint, cancellationToken);

        response.EnsureSuccessStatusCode();

        // The server appears to respond with an application/json content type, which defaults to being decoded using
        // UTF-8. The XML it returns specifies UTF-16 making it difficult to just get a stream compatible with the
        // XML content. So, we grab a string and convert it to a UTF-16 stream.
        string contentString = await response.Content.ReadAsStringAsync(cancellationToken);
        using MemoryStream contentStream = new(Encoding.Unicode.GetBytes(contentString), writable: false);
        NamecheapDdnsUpdateResponse? namecheapResponse = contentStream.ReadFromXml<NamecheapDdnsUpdateResponse>()
            ?? throw new InvalidDataException("The response resulted in null.");

        return namecheapResponse;
    }

    private static Uri FormatEndpointUrl(string host, string domainName, string ddnsPassword, string ipAddress)
    {
        string endpoint = string.Format(
            CultureInfo.InvariantCulture,
            endpointFormat,
            HttpUtility.UrlEncode(host),
            HttpUtility.UrlEncode(domainName),
            HttpUtility.UrlEncode(ddnsPassword),
            HttpUtility.UrlEncode(ipAddress));
        return new Uri(endpoint);
    }
}
