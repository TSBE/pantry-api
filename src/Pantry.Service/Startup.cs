﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Opw.HttpExceptions;
using Opw.HttpExceptions.AspNetCore;
using Opw.HttpExceptions.AspNetCore.Mappers;
using Pantry.Common;
using Pantry.Common.Authentication;
using Pantry.Common.Diagnostics.HealthChecks;
using Pantry.Common.Hosting;
using Pantry.Common.Time;
using Pantry.Core.Persistence;
using Pantry.Features.EanSearchOrg;
using Pantry.Features.OpenFoodFacts;
using Pantry.Features.WebFeature;
using Pantry.Service.Infrastructure;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Pantry.Service;

internal class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddPantryHealthChecks()
            .AddDbContextCheck<AppDbContext>(tags: [HealthCheckConstants.Tags.ReadinessTag]);

        services.Configure<HealthCheckPublisherOptions>(
            options =>
            {
                options.Delay = TimeSpan.FromSeconds(2);
                options.Period = TimeSpan.FromSeconds(30);
                options.Predicate = check => check.Tags.Contains(HealthCheckConstants.Tags.ReadinessTag);
            });

        services.AddSilverback()
            .UseModel(); // Optional. Enables the CQRS-Part of Silverback.

        services.AddControllers(options =>
        {
            options.Filters.Add(new ProducesAttribute("application/json"));
            options.Filters.Add(new ConsumesAttribute("application/json"));
            options.Filters.Add(new ResponseCacheAttribute
            {
                NoStore = true,
                Location = ResponseCacheLocation.None
            });
        })
        .AddControllersAsServices()
        .AddJsonOptions(
        options =>
        {
            // Limit excepton message for security reasons.
            options.AllowInputFormatterExceptionMessages = false;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.Converters.Add(new CustomJsonConverterForType());
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        })
        .AddHttpExceptions(o =>
        {
            // This is the same as the default behavior; only include exception details in a development environment.
            o.IncludeExceptionDetails = context => context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment() || context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsIntegrationTest();

            // This is a simplified version of the default behavior; only map exceptions for 4xx and 5xx responses.
            o.IsExceptionResponse = context => (context.Response.StatusCode >= 400 && context.Response.StatusCode < 600);

            // Only log the when it has a status code of 500 or higher, or when it not is a HttpException.
            o.ShouldLogException = exception => (exception is HttpExceptionBase httpException && (int)httpException.StatusCode >= 500) || exception is not HttpExceptionBase;

            // default exception mapper for mapping to Problem Details
            o.ExceptionMapper<Exception, ProblemDetailsExceptionMapper<Exception>>();
        });

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerGenConfiguration>();
        services.AddApiVersioning();
        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
        services.AddSwaggerGen();

        // Configures the most standard JWT authentication
        services.AddJwtAuthentication(Configuration);

        services.AddTransient<IPrincipal>(provider =>
        provider.GetService<IHttpContextAccessor>()?.HttpContext?.User
        ?? new GenericPrincipal(new GenericIdentity("nocontext"), new string[1]));

        var connectionString = Configuration.GetConnectionString("AppDatabase");
        void DbContextOptions(IServiceProvider serviceProvider, DbContextOptionsBuilder builder) =>
            builder.UseNpgsql(connectionString, options => options.CommandTimeout(15))
            .UseSnakeCaseNamingConvention();

        services.AddPooledDbContextFactory<AppDbContext>(DbContextOptions);

        // This is just required for the HealthCheck, since it does not (yet?) work with the Factory.
        services.AddDbContextPool<AppDbContext>(DbContextOptions);

        // Add core features.
        services.AddWebFeature(Configuration);
        services.AddOpenFoodFacts(Configuration);
        services.AddEanSearchOrg(Configuration);
    }

    public void Configure(IApplicationBuilder app, IHostEnvironment env, ILogger<Startup> logger, IApiVersionDescriptionProvider provider)
    {
        // this is the first middleware component added to the pipeline
        app.UseHttpExceptions();

        if (env.IsIntegrationTest())
        {
            app.UseDateTimeContext();
        }
        else
        {
            // migrate any database changes on startup (includes initial db creation)
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
                context.Database.Migrate();
            }
        }

        // Required. Writes common Runtime and Environment Info to the log.
        logger.LogRuntimeAndEnvironmentInformation();

        // Required. Enable routing for the middleware.
        app.UseRouting();

        // Required. Basic error handling for middleware.
        if (env.IsDevelopment())
        {
            // Use only when Opw.HttpExceptions is hiding the exception
            // app.UseDeveloperExceptionPage();
        }

        // Required. Used for authentication.
        app.UseAuthentication();

        // Allow Swagger ui for anonymous.
        app.UseStaticFiles();
        app.UseSwagger(
            options =>
            {
                options.RouteTemplate = "/api/doc/{documentName}.json";
            });
        app.UseSwaggerUI(
            uiOptions =>
            {
                uiOptions.EnableTryItOutByDefault();
                uiOptions.RoutePrefix = "api/doc/ui";
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    uiOptions.SwaggerEndpoint(new Uri($"/api/doc/{description.GroupName}.json", UriKind.Relative).ToString(), description.GroupName);
                    uiOptions.InjectStylesheet("/swagger-ui/dark.css");
                }
            });

        if (env.IsDevelopment() || env.IsIntegrationTest())
        {
            // Enable for sensitive data logging.
            // IdentityModelEventSource.ShowPII = true;

            // Add the backdoor middleware in between the Authentication and Authorization ones
            app.UseBackdoorAuthentication(
                "auth0|backdoor1234567890",
                claimsProvider: userId => { return new List<Claim> { new Claim(CustomClaimTypes.HOUSEHOLDID, "1") }; });
        }

        app.UseAuthorization();

        app.UseEndpoints(
            endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapPantryHealthChecks(true, conventionBuilder => conventionBuilder.WithMetadata(new AllowAnonymousAttribute()));
            });
    }
}
