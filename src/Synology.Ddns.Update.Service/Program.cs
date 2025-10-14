namespace Synology.Ddns.Update.Service;

using System.Diagnostics.CodeAnalysis;

using Synology.Ddns.Update.Service.Extensions;

internal sealed class Program
{
    [SuppressMessage("Design", "CA1031:Do not catch general exception types")]
    [ExcludeFromCodeCoverage]
    public static int Main(string[] args)
    {
        try
        {
            Run(args);

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return ex.HResult;
        }
    }

    private static void Run(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.AddOpenTelemetry();
        builder.Services.AddCustomRateLimiter(builder.Configuration);
        builder.Services.AddOpenApi();
        builder.Services.AddNamecheapDdnsClient(builder.Configuration);

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "Synology DDNS Update Service API");
            });
        }

        app.UseHttpsRedirection();
        app.UseRateLimiter();
        app.MapGet("/", () => $"Synology DDNS Update Service {ThisAssembly.AssemblyInformationalVersion} is running");
        app.MapEndpoints();
        app.Run();
    }
}
