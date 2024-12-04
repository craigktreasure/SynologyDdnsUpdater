namespace Synology.Ddns.Update.Service.Endpoints;

using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;

using global::Namecheap.Library;
using global::Namecheap.Library.Models;

using Microsoft.AspNetCore.Mvc;

using Synology.Ddns.Update.Service.Monitoring;
using Synology.Namecheap.Adapter.Library;

internal class NamecheapDdns
{
    private static readonly Counter<long> namecheapDdnsUpdateCounter = Telemetry.Meter.CreateCounter<long>("NamecheapDdnsUpdate");

    /// <summary>
    /// Updates the specified host.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="namecheapDdnsClient">The Namecheap DDNS client.</param>
    /// <param name="host">The host.</param>
    /// <param name="domainName">The name of the domain.</param>
    /// <param name="ddnsPassword">The DDNS password.</param>
    /// <param name="ipAddress">The ip address.</param>
    /// <returns><see cref="string"/>.</returns>
    [HttpGet("update", Name = "Update")]
    [EndpointSummary("Updates the specified host.")]
    public static async Task<string> Update(
        [FromServices] ILogger<NamecheapDdns> logger,
        [FromServices] INamecheapDdnsClient namecheapDdnsClient,
        [Description("The host.")]
        [FromQuery(Name = "host")] string host,
        [Description("The name of the domain.")]
        [FromQuery(Name = "domain")] string domainName,
        [Description("The DDNS password.")]
        [FromQuery(Name = "password")] string ddnsPassword,
        [Description("The ip address.")]
        [FromQuery(Name = "ip")] string ipAddress)
    {
        using Activity? activity = Telemetry.ActivitySource.StartActivity("NamecheapDdnsUpdate");

        try
        {
            logger.DdnsUpdating(host, domainName);

            NamecheapDdnsUpdateResponse namecheapUpdateResponse = await namecheapDdnsClient.UpdateHostIpAddressAsync(host, domainName, ddnsPassword, ipAddress);

            string result = NamecheapResponseAdapter.GetSynologyResponse(namecheapUpdateResponse);

            namecheapDdnsUpdateCounter.Add(1, new KeyValuePair<string, object?>("success", namecheapUpdateResponse.Success));
            if (namecheapUpdateResponse.Success)
            {
                activity?.SetStatus(ActivityStatusCode.Ok, result);
                logger.DdnsUpdated(host, domainName);
            }
            else
            {
                activity?.SetStatus(ActivityStatusCode.Error, result);
                logger.DdnsUpdateFailed(host, domainName, result);
            }

            return result;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            logger.DdnsUpdateError(host, domainName, ex);
            throw;
        }
    }
}
