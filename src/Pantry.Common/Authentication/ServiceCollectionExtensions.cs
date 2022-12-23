using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Pantry.Common.Authentication;

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
    /// <returns>
    ///     The <see cref="IServiceCollection" /> so that additional calls can be chained.
    /// </returns>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        string configSectionKey = "JwtToken") =>
        services.AddJwtAuthentication(configuration.GetRequiredSection(configSectionKey).Get<JwtTokenSettings>() ?? new JwtTokenSettings());

    /// <summary>
    ///     Registers the services necessary to enable the JWT based authentication (and authorization).
    /// </summary>
    /// <param name="services">
    ///     The <see cref="IServiceCollection" /> to add services to.
    /// </param>
    /// <param name="tokensSettings">
    ///     The multiple <see cref="JwtTokenSettings" /> to be used to validate the token.
    /// </param>
    /// <returns>
    ///     The <see cref="IServiceCollection" /> so that additional calls can be chained.
    /// </returns>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        JwtTokenSettings tokensSettings)
    {
        return services.AddJwtAuthentication(tokensSettings, CertificatesHelper.GetCertificates(tokensSettings));
    }

    /// <summary>
    ///     Registers the services necessary to enable the JWT based authentication (and authorization).
    /// </summary>
    /// <param name="services">
    ///     The <see cref="IServiceCollection" /> to add services to.
    /// </param>
    /// <param name="tokensSettings">
    ///     The multiple <see cref="JwtTokenSettings" /> to be used to validate the token.
    /// </param>
    /// <param name="signingKeyCertificates">
    ///     The certificates to be used to check the token's signature. If multiple ones are provided the <c>kid</c>
    ///     header is used to determine which key is to be used (the key identifier can either be the certificate's
    ///     subject or its thumbprint). As last resort all certificates will be tried.
    /// </param>
    /// <returns>The <see cref="IServiceCollection" /> so that additional calls can be chained.</returns>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        JwtTokenSettings tokensSettings,
        IEnumerable<X509Certificate2> signingKeyCertificates) =>
        services
            .AddAuthorization(
                options =>
                {
                    AuthorizationPolicyBuilder policyBuilder = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser();

                    options.DefaultPolicy = options.FallbackPolicy = policyBuilder.Build();
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
                        tokensSettings.Issuer is not null ? new[] { tokensSettings.Issuer } : new List<string>(),
                        tokensSettings.Audience is not null ? new[] { tokensSettings.Audience } : new List<string>(),
                        signingKeyCertificates))
            .Services;

    private static void ConfigureJwtBearer(
        JwtBearerOptions options,
        IReadOnlyCollection<string> validIssuers,
        IReadOnlyCollection<string> validAudiences,
        IEnumerable<X509Certificate2> signingKeyCertificates)
    {
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
