using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pantry.Features.WebFeature.Authentication;
using Pantry.Features.WebFeature.Commands;
using Pantry.Features.WebFeature.Configuration;
using Pantry.Features.WebFeature.Queries;
using Pantry.Features.WebFeature.V1.Controllers;
using Silverback.Messaging.Configuration;

namespace Pantry.Features.WebFeature;

/// <summary>
///     Registers the WebFeature.
///     - Offer a REST-API for the entity.
///     - Use Queries and Commands to provide the entity CRUD functionality.
///     - Use Entity Framework to persist the entity.
/// </summary>
public static class Registrar
{
    public static void AddWebFeature(this IServiceCollection services, IConfiguration configuration)
    {
        // Controllers
        services.AddScoped<DeviceController>();

        // Other requirements
        services.AddFluentValidationAutoValidation(_ =>
        {
            ValidatorOptions.Global.LanguageManager.Enabled = false;
        }).AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssembly(typeof(Registrar).Assembly, ServiceLifetime.Scoped);
        services.Configure<WebFeatureConfiguration>(configuration.GetRequiredSection(WebFeatureConfiguration.Name));
        services.AddTransient<IClaimsTransformation, HouseholdClaimsTransformation>();

        ISilverbackBuilder silverbackBuilder = services.ConfigureSilverback();

        // CommandHandlers
        silverbackBuilder
            .AddScopedSubscriber<CreateAccountCommandHandler>()
            .AddScopedSubscriber<DeleteAccountCommandHandler>()

            .AddScopedSubscriber<CreateDeviceCommandHandler>()
            .AddScopedSubscriber<UpdateDeviceCommandHandler>()
            .AddScopedSubscriber<DeleteDeviceCommandHandler>()

            .AddScopedSubscriber<CreateHouseholdCommandHandler>()

            .AddScopedSubscriber<CreateStorageLocationCommandHandler>()
            .AddScopedSubscriber<UpdateStorageLocationCommandHandler>()
            .AddScopedSubscriber<DeleteStorageLocationCommandHandler>()

            .AddScopedSubscriber<CreateArticleCommandHandler>()
            .AddScopedSubscriber<UpdateArticleCommandHandler>()
            .AddScopedSubscriber<DeleteArticleCommandHandler>()

            .AddScopedSubscriber<CreateInvitationCommandHandler>()
            .AddScopedSubscriber<AcceptInvitationCommandHandler>()
            .AddScopedSubscriber<DeclineInvitationCommandHandler>()
            ;

        // QueryHandlers
        silverbackBuilder
            .AddScopedSubscriber<AccountQueryHandler>()

            .AddScopedSubscriber<DeviceByIdQueryHandler>()
            .AddScopedSubscriber<DeviceListQueryHandler>()

            .AddScopedSubscriber<HouseholdQueryHandler>()

            .AddScopedSubscriber<StorageLocationByIdQueryHandler>()
            .AddScopedSubscriber<StorageLocationListQueryHandler>()

            .AddScopedSubscriber<ArticleByIdQueryHandler>()
            .AddScopedSubscriber<ArticleListQueryHandler>()

            .AddScopedSubscriber<InvitationListQueryHandler>()

            .AddScopedSubscriber<MetadataByGtinQueryHandler>()
            ;
    }
}
