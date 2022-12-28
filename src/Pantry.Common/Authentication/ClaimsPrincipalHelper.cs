using System.Data;
using System.Security.Claims;
using System.Security.Principal;
using Opw.HttpExceptions;

namespace Pantry.Common.Authentication;

public static class ClaimsPrincipalHelper
{
    public static string? GetClientId(this IPrincipal principal)
        => principal.GetClaim(CustomClaimTypes.CLIENTIDENTIFIER);

    public static string? GetTokenIdentifier(this IPrincipal principal)
        => principal.GetClaim(CustomClaimTypes.TOKENIDENTIFIER);

    public static string? GetIssuedAt(this IPrincipal principal)
        => principal.GetClaim(CustomClaimTypes.ISSUEDAT);

    public static string? GetExpireAt(this IPrincipal principal)
        => principal.GetClaim(CustomClaimTypes.EXPIREAT);

    public static string? GetIssuer(this IPrincipal principal)
        => principal.GetClaim(CustomClaimTypes.ISSUER);

    public static string[]? GetScopes(this IPrincipal principal)
        => principal.GetClaim(CustomClaimTypes.SCOPES)?.Split(new[] { ' ', ',' });

    public static string? GetAuth0Id(this IPrincipal principal)
        => principal.Identity?.GetAuth0Id();

    public static string GetAuth0IdOrThrow(this IPrincipal principal)
        => principal.Identity?.GetAuth0IdOrThrow()
        ?? throw new ForbiddenException(nameof(principal.Identity));

    public static long? GetHouseholdId(this IPrincipal principal)
    {
        var value = principal.GetClaim(CustomClaimTypes.HOUSEHOLDID);
        return long.TryParse(value, out var householdId) ? householdId : default;
    }

    public static long GetHouseholdIdOrThrow(this IPrincipal principal)
        => principal.GetHouseholdId()
        ?? throw new ForbiddenException(nameof(principal));

    public static string? GetClaim(this IPrincipal principal, string claimType) =>
         (principal.Identity as ClaimsIdentity)?.Claims
         .Where(e => e.Type == claimType)
         .Select(e => e.Value).FirstOrDefault() ?? default;

    public static bool HasScope(this IPrincipal principal, string scope)
    {
        var scopes = principal.GetScopes();
        return scopes?.Length > 0 && scopes.Any(scope.Contains);
    }
}
