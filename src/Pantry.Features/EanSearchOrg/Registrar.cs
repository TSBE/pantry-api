using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pantry.Features.EanSearchOrg.Configuration;
using Refit;

namespace Pantry.Features.EanSearchOrg;

/// <summary>
///     Registers the EanSearchOrg.
/// </summary>
public static class Registrar
{
    public static void AddEanSearchOrg(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<EanSearchOrgConfiguration>().Bind(configuration.GetRequiredSection(EanSearchOrgConfiguration.Name)).ValidateDataAnnotations();

        var featureConfiguration = configuration.GetRequiredSection(EanSearchOrgConfiguration.Name).Get<EanSearchOrgConfiguration>();

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        services.AddRefitClient<IEanSearchOrgApiService>(
             service => new RefitSettings()
             {
                 ContentSerializer = new SystemTextJsonContentSerializer(jsonOptions)
             })
            .ConfigureHttpClient(
            config =>
            {
                config.BaseAddress = featureConfiguration!.BaseUrl;
                config.Timeout = TimeSpan.FromMilliseconds(featureConfiguration!.TimeoutInMilliseconds);
            })
            .ConfigurePrimaryHttpMessageHandler(x => new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });
    }
}
