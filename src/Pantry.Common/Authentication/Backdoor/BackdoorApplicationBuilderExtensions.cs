using System.Security.Claims;
using Pantry.Common.Authentication.Backdoor;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
///     Adds the <c>UseBackdoorAuthentication</c> extension method to the <see cref="IApplicationBuilder" />.
/// </summary>
public static class BackdoorApplicationBuilderExtensions
{
    /// <summary>
    ///     <para>
    ///         Enables the authentication backdoor. This middleware should be registered after the actual authentication (
    ///         <c>UseAuthentication</c>) and the authorization (<c>UseAuthorization</c>) middleware.
    ///     </para>
    ///     <para>
    ///         Intended for usage in test environments only.
    ///     </para>
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder" /> to add the middleware to.</param>
    /// <param name="defaultUserId">
    ///     The default user id to be used as value for both the name and name identifier claims (unless
    ///     otherwise set by the claims provider delegate).
    /// </param>
    /// <param name="scopes">
    ///     The scopes to be added to the fake token. If null or empty a default scope will be issued.
    /// </param>
    /// <param name="claimsProvider">
    ///     A delegate that receives the user id and returns a collection of additional claims to be added to
    ///     the identity.
    /// </param>
    /// <returns>The <see cref="IApplicationBuilder" /> so that additional calls can be chained.</returns>
    public static IApplicationBuilder UseBackdoorAuthentication(
        this IApplicationBuilder builder,
        string defaultUserId,
        string? scopes = null,
        Func<string, IEnumerable<Claim>>? claimsProvider = null)
    {
        var settings = new BackdoorSettings
        {
            DefaultUserId = defaultUserId,
            Scopes = scopes,
            ClaimsProvider = claimsProvider
        };

        return UseBackdoorAuthentication(builder, settings);
    }

    /// <summary>
    ///     <para>
    ///         Enables the authentication backdoor. This middleware should be registered after the actual authentication (
    ///         <c>UseAuthentication</c>) and the authorization (<c>UseAuthorization</c>) middleware.
    ///     </para>
    ///     <para>
    ///         Intended for usage in test environments only.
    ///     </para>
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder" /> to add the middleware to.</param>
    /// <param name="settings">The settings such as the default user id.</param>
    /// <returns>The <see cref="IApplicationBuilder" /> so that additional calls can be chained.</returns>
    public static IApplicationBuilder UseBackdoorAuthentication(
        this IApplicationBuilder builder,
        BackdoorSettings? settings = null)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.UseMiddleware<BackdoorAuthenticationMiddleware>(settings ?? new BackdoorSettings());

        return builder;
    }
}
