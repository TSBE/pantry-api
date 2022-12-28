using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pantry.Features.OpenFoodFacts.Configuration;
using Pantry.Features.OpenFoodFacts.Converters;
using Refit;

namespace Pantry.Features.OpenFoodFacts;

/// <summary>
///     Registers the OpenFoodFacts.
/// </summary>
public static class Registrar
{
    public static void AddOpenFoodFacts(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<OpenFoodFactsConfiguration>().Bind(configuration.GetRequiredSection(OpenFoodFactsConfiguration.Name)).ValidateDataAnnotations();

        var featureConfiguration = configuration.GetRequiredSection(OpenFoodFactsConfiguration.Name).Get<OpenFoodFactsConfiguration>();

        var jsonOptions = new JsonSerializerOptions();
        jsonOptions.Converters.Add(new JsonStringEnumConverter());
        jsonOptions.Converters.Add(new UnixEpochDateTimeConverter());

        services.AddRefitClient<IOpenFoodFactsApiService>(
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
