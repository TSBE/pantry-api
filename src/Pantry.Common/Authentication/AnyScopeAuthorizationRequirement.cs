using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Pantry.Common.Authentication;

/// <summary>
///     Implements an <see cref="IAuthorizationHandler" /> and <see cref="IAuthorizationRequirement" />
///     which requires any of the specified scopes.
/// </summary>
public class AnyScopeAuthorizationRequirement
    : AuthorizationHandler<AnyScopeAuthorizationRequirement>, IAuthorizationRequirement
{
    private static readonly char[] ScopesSeparators = { ' ', ',' };

    /// <summary>
    ///     Initializes a new instance of the <see cref="AnyScopeAuthorizationRequirement" /> class.
    /// </summary>
    /// <param name="validScopes">The valid scopes.</param>
    public AnyScopeAuthorizationRequirement(IEnumerable<string> validScopes)
    {
        List<string> requiredScopesList = validScopes.ToList();

        if (validScopes == null || !requiredScopesList.Any())
        {
            throw new ArgumentException(
                $"{nameof(validScopes)} must contain at least one value.",
                nameof(validScopes));
        }

        ValidScopes = requiredScopesList;
    }

    /// <summary>
    ///     Gets the collection of valid scopes.
    /// </summary>
    public IReadOnlyCollection<string> ValidScopes { get; }

    /// <inheritdoc />
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AnyScopeAuthorizationRequirement requirement)
    {
        Claim? scopeClaim = context.User.Claims.FirstOrDefault(
            claim => string.Equals(claim.Type, "scope", StringComparison.OrdinalIgnoreCase));

        if (scopeClaim != null)
        {
            string[] scopes = scopeClaim.Value.Split(ScopesSeparators, StringSplitOptions.RemoveEmptyEntries);
            if (requirement.ValidScopes.Any(validScope => scopes.Contains(validScope)))
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}
