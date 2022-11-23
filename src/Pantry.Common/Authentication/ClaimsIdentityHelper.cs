using System.Security.Claims;
using System.Security.Principal;
using Opw.HttpExceptions;

namespace Pantry.Common.Authentication;

public static class ClaimsIdentityHelper
{
    public static string? GetAuth0Id(this IIdentity identity) =>
        ((ClaimsIdentity)identity).GetAuth0Id();

    public static string GetAuth0IdOrThrow(this IIdentity identity)
        => identity?.GetAuth0Id()
        ?? throw new ForbiddenException(nameof(identity));

    public static string? GetAuth0Id(this ClaimsIdentity identity)
    {
        return identity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}
