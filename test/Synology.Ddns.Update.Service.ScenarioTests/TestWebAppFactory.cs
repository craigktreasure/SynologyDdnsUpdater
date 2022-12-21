namespace Synology.Ddns.Update.Service.Tests;

using global::Namecheap.Library;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Test.Library;
using Xunit.Abstractions;
using SendAction = Func<HttpRequestMessage, HttpResponseMessage>;

internal sealed class TestWebAppFactory : WebApplicationFactory<Program>
{
    private readonly ITestOutputHelper testOutputHelper;

    private readonly SendAction namecheapDdnsSendAction;

    public TestWebAppFactory(ITestOutputHelper testOutputHelper, SendAction namecheapDdnsSendAction)
    {
        this.testOutputHelper = testOutputHelper;
        this.namecheapDdnsSendAction = namecheapDdnsSendAction;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("https_port", "5001");
        builder.UseSetting("NamecheapDdnsClientOptions:MockClient", "false");
        builder.UseSetting("Logging:LogLevel:Microsoft.AspNetCore", "Information");

        base.ConfigureWebHost(builder);

        builder.ConfigureServices(services =>
        {
            services
                .AddHttpClient<INamecheapDdnsClient, NamecheapDdnsClient>(configure =>
                {
                    configure.Timeout = Timeout.InfiniteTimeSpan;
                })
                .ConfigurePrimaryHttpMessageHandler(() => new MockHttpMessageHandler(this.namecheapDdnsSendAction));

            services.AddLogging(configure =>
            {
                configure.AddXunit(this.testOutputHelper);
            });
        });
    }
}
