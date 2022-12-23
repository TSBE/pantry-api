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
    /// <param name="claimsProvider">
    ///     A delegate that receives the user id and returns a collection of additional claims to be added to
    ///     the identity.
    /// </param>
    /// <returns>The <see cref="IApplicationBuilder" /> so that additional calls can be chained.</returns>
    public static IApplicationBuilder UseBackdoorAuthentication(
        this IApplicationBuilder builder,
        string defaultUserId,
        Func<string, IEnumerable<Claim>>? claimsProvider = null)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        var settings = new BackdoorSettings
        {
            DefaultUserId = defaultUserId,
            ClaimsProvider = claimsProvider
        };

        builder.UseMiddleware<BackdoorAuthenticationMiddleware>(settings);

        return builder;
    }
}
