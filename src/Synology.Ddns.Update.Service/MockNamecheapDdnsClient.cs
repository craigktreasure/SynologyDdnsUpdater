namespace Synology.Ddns.Update.Service;

using global::Namecheap.Library;
using global::Namecheap.Library.Models;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Performance", "CA1812: Avoid uninstantiated internal classes", Justification = "Created at runtime by DI.")]
internal sealed class MockNamecheapDdnsClient : INamecheapDdnsClient
{
    public Task<NamecheapDdnsUpdateResponse> UpdateHostIpAddressAsync(
        string host,
        string domainName,
        string ddnsPassword,
        string ipAddress,
        CancellationToken cancellationToken = default)
        => Task.FromResult(new NamecheapDdnsUpdateResponse()
        {
            Command = "SETDNSHOST",
            Done = true,
            ErrorCount = 0,
            Errors = new(),
            IPAddress = ipAddress,
            Language = "eng",
            ResponseCount = 0,
            Responses = Array.Empty<NamecheapDdnsUpdateOperationResponse>()
        });
}
