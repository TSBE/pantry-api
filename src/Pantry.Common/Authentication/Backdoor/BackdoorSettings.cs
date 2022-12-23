using System.Security.Claims;

namespace Pantry.Common.Authentication.Backdoor;

/// <summary>
///     The settings for the <see cref="BackdoorAuthenticationMiddleware" />.
/// </summary>
public class BackdoorSettings
{
    /// <summary>
    ///     Gets or sets the default user id to be used as as value for both the name and name identifier claims
    ///     (unless otherwise set by the claims provider delegate).
    /// </summary>
    public string DefaultUserId { get; set; } = "test-user";

    /// <summary>
    ///     Gets or sets an optional delegate that receives the user id and returns a collection of claims to be
    ///     added to the fake identity.
    /// </summary>
    public Func<string, IEnumerable<Claim>>? ClaimsProvider { get; set; }
}
