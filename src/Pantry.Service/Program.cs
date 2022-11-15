using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pantry.Service;

public static class Program
{
    [ExcludeFromCodeCoverage(Justification = "Not testable.")]
    public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", Justification = "Necessary for the integration tests.")]
    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return new HostBuilder()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureAppConfiguration(
                (hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                        .AddEnvironmentVariables();
                    if (hostingContext.HostingEnvironment.IsDevelopment())
                    {
                        config.AddUserSecrets<Startup>();
                    }
                })
            .ConfigureLogging(
                (hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    if (hostingContext.HostingEnvironment.IsDevelopment())
                    {
                        logging.AddConsole();
                    }
                    else
                    {
                        logging.AddPantryConsole();
                    }

                    logging.Configure(
                        options =>
                        {
                            options.ActivityTrackingOptions = ActivityTrackingOptions.SpanId
                                                              | ActivityTrackingOptions.TraceId
                                                              | ActivityTrackingOptions.ParentId;
                        });
                })
            .UseDefaultServiceProvider(
                (context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment() || context.HostingEnvironment.IsIntegrationTest();
                    options.ValidateOnBuild = context.HostingEnvironment.IsDevelopment() || context.HostingEnvironment.IsIntegrationTest();
                })
            .ConfigureWebHostDefaults(
                webBuilder =>
                {
                    webBuilder
                        .UseKestrel()
                        .UseStartup<Startup>();
                });
    }
}
