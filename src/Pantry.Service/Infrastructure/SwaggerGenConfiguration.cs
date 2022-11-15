using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Pantry.Service.Infrastructure;

public class SwaggerGenConfiguration : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public SwaggerGenConfiguration(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                new OpenApiInfo { Title = $"Pantry API {description.ApiVersion}", Version = description.ApiVersion.ToString() });
        }

        foreach (var xmlFile in Directory.GetFiles(AppContext.BaseDirectory, "*.xml"))
        {
            options.IncludeXmlComments(xmlFile);
        }
    }
}
