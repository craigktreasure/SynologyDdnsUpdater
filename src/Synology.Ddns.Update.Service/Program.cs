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

        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, ThisAssembly.AssemblyName + ".xml"));
        });
        builder.Services.AddNamecheapDdnsClient(builder.Configuration);

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseRateLimiter();

        app.MapControllers();

        app.Run();
    }
}
