namespace Namecheap.Library;

using System.Threading;
using System.Threading.Tasks;

using Namecheap.Library.Models;

/// <summary>
/// An interface representing a Namechear DDNS client.
/// </summary>
public interface INamecheapDdnsClient
{
    /// <summary>
    /// Updates the host IP address.
    /// </summary>
    /// <param name="host">The host.</param>
    /// <param name="domainName">The domain name.</param>
    /// <param name="ddnsPassword">The DDNS password.</param>
    /// <param name="ipAddress">The IP address.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns><see cref="NamecheapDdnsUpdateResponse"/>.</returns>
    Task<NamecheapDdnsUpdateResponse> UpdateHostIpAddressAsync(
        string host,
        string domainName,
        string ddnsPassword,
        string ipAddress,
        CancellationToken cancellationToken = default);
}
