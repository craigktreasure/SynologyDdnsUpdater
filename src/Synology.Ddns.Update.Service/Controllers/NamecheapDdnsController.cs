namespace Synology.Ddns.Update.Service.Controllers;

using global::Namecheap.Library;
using global::Namecheap.Library.Models;
using Microsoft.AspNetCore.Mvc;
using Synology.Ddns.Update.Service.Monitoring;
using Synology.Namecheap.Adapter.Library;
using System.Diagnostics;
using System.Diagnostics.Metrics;

/// <summary>
/// Represents an API controller that performs Namecheap DDNS updates.
/// Implements the <see cref="ControllerBase" />
/// </summary>
/// <seealso cref="ControllerBase" />
[ApiController]
[Route("namecheap/ddns")]
public class NamecheapDdnsController : ControllerBase
{
    private static readonly Counter<long> namecheapDdnsUpdateCounter = Telemetry.Meter.CreateCounter<long>("NamecheapDdnsUpdate");

    private readonly ILogger<NamecheapDdnsController> logger;

    private readonly INamecheapDdnsClient namecheapDdnsClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="NamecheapDdnsController"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="namecheapDdnsClient">The namecheap DDNS client.</param>
    public NamecheapDdnsController(ILogger<NamecheapDdnsController> logger, INamecheapDdnsClient namecheapDdnsClient)
    {
        this.logger = logger;
        this.namecheapDdnsClient = namecheapDdnsClient;
    }

    /// <summary>
    /// Updates the specified host.
    /// </summary>
    /// <param name="host">The host.</param>
    /// <param name="domainName">The name of the domain.</param>
    /// <param name="ddnsPassword">The DDNS password.</param>
    /// <param name="ipAddress">The ip address.</param>
    /// <returns><see cref="string"/>.</returns>
    [HttpGet("update", Name = "Update")]
    public async Task<string> Update(
        [FromQuery(Name = "host")] string host,
        [FromQuery(Name = "domain")] string domainName,
        [FromQuery(Name = "password")] string ddnsPassword,
        [FromQuery(Name = "ip")] string ipAddress)
    {
        using Activity? activity = Telemetry.ActivitySource.StartActivity("NamecheapDdnsUpdate");

        try
        {
            logger.DdnsUpdating(host, domainName);

            NamecheapDdnsUpdateResponse namecheapUpdateResponse = await this.namecheapDdnsClient.UpdateHostIpAddressAsync(host, domainName, ddnsPassword, ipAddress);

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
