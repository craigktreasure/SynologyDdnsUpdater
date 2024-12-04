namespace Synology.Ddns.Update.Service.Extensions;

using Synology.Ddns.Update.Service.Endpoints;

internal static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// Registers all the route endpoints.
    /// </summary>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder" /> to add routes to.</param>
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("namecheap/ddns/update", NamecheapDdns.Update);

        return endpoints;
    }
}
