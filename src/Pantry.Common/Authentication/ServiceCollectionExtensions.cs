using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Pantry.Common.Authentication;
using Pantry.Common.Authentication.Backdoor;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     Adds the <c>AddJwtAuthentication</c> method to the <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    private static readonly ErrorLoggingHelper ErrorLoggingHelper = new();

    /// <summary>
    ///     Registers the services necessary to enable the JWT based authentication (and authorization).
    /// </summary>
    /// <param name="services">
    ///     The <see cref="IServiceCollection" /> to add services to.
    /// </param>
    /// <param name="configuration">
    ///     The entire <see cref="IConfiguration" />.
    /// </param>
    /// <param name="configSectionKey">
    ///     The key of the configuration sections containing the <see cref="JwtTokenSettings" />.
    /// </param>
    /// <param name="configureAuthorizationAction">
    ///     An action delegate to configure the provided <see cref="AuthorizationOptions" />.
    /// </param>
    /// <returns>
    ///     The <see cref="IServiceCollection" /> so that additional calls can be chained.
    /// </returns>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        string configSectionKey = "JwtToken",
        Action<AuthorizationOptions>? configureAuthorizationAction = null) =>
        services.AddJwtAuthentication(
            configuration,
            new[] { configSectionKey },
            configureAuthorizationAction);

    /// <summary>
    ///     Registers the services necessary to enable the JWT based authentication (and authorization).
    /// </summary>
    /// <param name="services">
    ///     The <see cref="IServiceCollection" /> to add services to.
    /// </param>
    /// <param name="configuration">
    ///     The entire <see cref="IConfiguration" />.
    /// </param>
    /// <param name="configSectionsKeys">
    ///     The keys of the configuration sections containing the <see cref="JwtTokenSettings" />.
    /// </param>
    /// <param name="configureAuthorizationAction">
    ///     An action delegate to configure the provided <see cref="AuthorizationOptions" />.
    /// </param>
    /// <returns>
    ///     The <see cref="IServiceCollection" /> so that additional calls can be chained.
    /// </returns>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        IEnumerable<string>? configSectionsKeys,
        Action<AuthorizationOptions>? configureAuthorizationAction = null) =>
        services.AddJwtAuthentication(
            configSectionsKeys.Select(key => configuration.GetRequiredSection(key).Get<JwtTokenSettings>()),
            configureAuthorizationAction);

    /// <summary>
    ///     Registers the services necessary to enable the JWT based authentication (and authorization).
    /// </summary>
    /// <param name="services">
    ///     The <see cref="IServiceCollection" /> to add services to.
    /// </param>
    /// <param name="tokensSettings">
    ///     The multiple <see cref="JwtTokenSettings" /> to be used to validate the token.
    /// </param>
    /// <param name="configureAuthorizationAction">
    ///     An action delegate to configure the provided <see cref="AuthorizationOptions" />.
    /// </param>
    /// <returns>
    ///     The <see cref="IServiceCollection" /> so that additional calls can be chained.
    /// </returns>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IEnumerable<JwtTokenSettings> tokensSettings,
        Action<AuthorizationOptions>? configureAuthorizationAction = null)
    {
        List<JwtTokenSettings> settingsList = tokensSettings.ToList();

        List<string?> issuers = settingsList
            .Select(settings => settings.Issuer)
            .Where(issuer => !string.IsNullOrEmpty(issuer))
            .ToList();

        List<string?> audiences = settingsList
            .Select(settings => settings.Audience)
            .Where(audience => !string.IsNullOrEmpty(audience))
            .ToList();

        List<string> scopes = settingsList
            .Where(settings => settings.Scopes != null)
            .SelectMany(
                settings =>
                    settings.Scopes?
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(scope => scope.Trim()) ?? Array.Empty<string>())
            .Where(scope => !string.IsNullOrEmpty(scope))
            .ToList();

        return services.AddJwtAuthentication(
            issuers!,
            audiences!,
            scopes,
            CertificatesHelper.GetCertificates(settingsList),
            configureAuthorizationAction);
    }

    /// <summary>
    ///     Registers the services necessary to enable the JWT based authentication (and authorization).
    /// </summary>
    /// <param name="services">
    ///     The <see cref="IServiceCollection" /> to add services to.
    /// </param>
    /// <param name="validIssuers">
    ///     The valid issuers that will be used to check against the token's issuer.
    /// </param>
    /// <param name="validAudiences">
    ///     The valid audiences that will be used to check against the token's audience.
    ///     The audience validation will be disabled when null or an empty enumerable is supplied.
    /// </param>
    /// <param name="validScopes">
    ///     The valid scopes that will be used to check against the token's scopes. The scope validation will be
    ///     disabled when null or empty but it is <b>strongly</b> recommended to enable it.
    /// </param>
    /// <param name="signingKeyCertificates">
    ///     The certificates to be used to check the token's signature. If multiple ones are provided the <c>kid</c>
    ///     header is used to determine which key is to be used (the key identifier can either be the certificate's
    ///     subject or its thumbprint). As last resort all certificates will be tried.
    /// </param>
    /// <param name="configureAuthorizationAction">
    ///     An action delegate to configure the provided <see cref="AuthorizationOptions" />.
    /// </param>
    /// <returns>The <see cref="IServiceCollection" /> so that additional calls can be chained.</returns>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IEnumerable<string> validIssuers,
        IEnumerable<string>? validAudiences,
        IEnumerable<string>? validScopes,
        IEnumerable<X509Certificate2> signingKeyCertificates,
        Action<AuthorizationOptions>? configureAuthorizationAction = null) =>
        services
            .AddAuthorization(
                options =>
                {
                    SetDefaultPolicies(options, validScopes?.ToList());
                    configureAuthorizationAction?.Invoke(options);
                })
            .AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme =
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
            .AddJwtBearer(
                options =>
                    ConfigureJwtBearer(
                        options,
                        validIssuers.ToList(),
                        validAudiences?.ToList() ?? new List<string>(),
                        signingKeyCertificates))
            .Services;

    private static void SetDefaultPolicies(AuthorizationOptions options, ICollection<string>? validScopes)
    {
        AuthorizationPolicyBuilder policyBuilder = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser();

        if (validScopes != null && validScopes.Count > 0)
        {
            // Allow backdoor authentication with default scope (randomly generated)
            validScopes.Add(BackdoorAuthenticationMiddleware.DefaultScope);
            policyBuilder.RequireAnyScope(validScopes);
        }

        options.DefaultPolicy = options.FallbackPolicy = policyBuilder.Build();
    }

    private static void ConfigureJwtBearer(
        JwtBearerOptions options,
        IReadOnlyCollection<string> validIssuers,
        IReadOnlyCollection<string> validAudiences,
        IEnumerable<X509Certificate2> signingKeyCertificates)
    {
        //options.Authority = "https://pantry-mobile.eu.auth0.com";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidIssuers = validIssuers,
            ValidateAudience = validAudiences.Any(),
            ValidAudiences = validAudiences,
            IssuerSigningKeys = GetSigningKeys(signingKeyCertificates)
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                ErrorLoggingHelper.LogAuthenticationException(context);
                return Task.CompletedTask;
            }
        };
    }

    private static IEnumerable<SecurityKey> GetSigningKeys(IEnumerable<X509Certificate2> signingKeyCertificates)
    {
        foreach (X509Certificate2 certificate in signingKeyCertificates)
        {
            yield return new X509SecurityKey(certificate, certificate.Thumbprint);
            yield return new X509SecurityKey(certificate, certificate.Subject);
        }
    }
}
