using Microsoft.AspNetCore.Authorization;
using Pantry.Common.Authentication;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
///     Adds the <c>RequireAnyScope</c> method to the <see cref="AuthorizationPolicyBuilder" />.
/// </summary>
public static class AnyScopeAuthorizationRequirementExtensions
{
    /// <summary>
    ///     Adds a <see cref="AnyScopeAuthorizationRequirement" /> to the current instance.
    /// </summary>
    /// <param name="authorizationPolicyBuilder">
    ///     The <see cref="AuthorizationPolicyBuilder" /> the requirement must be added to.
    /// </param>
    /// <param name="validScopes">The valid scopes.</param>
    /// <returns>The <see cref="AuthorizationPolicyBuilder" /> so that additional calls can be chained.</returns>
    public static AuthorizationPolicyBuilder RequireAnyScope(
        this AuthorizationPolicyBuilder authorizationPolicyBuilder,
        IEnumerable<string> validScopes)
    {
        authorizationPolicyBuilder.AddRequirements(new AnyScopeAuthorizationRequirement(validScopes));
        return authorizationPolicyBuilder;
    }
}
