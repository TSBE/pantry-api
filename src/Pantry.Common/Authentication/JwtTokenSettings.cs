namespace Pantry.Common.Authentication;

/// <summary>
///     Usually used as model for the settings configuration section, contains the information used
///     to validate the JWT token.
/// </summary>
public class JwtTokenSettings
{
    /// <summary>
    ///     Gets or sets the valid issuer that will be used to check against the token's issuer.
    /// </summary>
    public string? Issuer { get; set; }

    /// <summary>
    ///     Gets or sets the valid audience that will be used to check against the token's audience.
    ///     Leave it blank (or null) to ignore the audience.
    /// </summary>
    public string? Audience { get; set; }

    /// <summary>
    ///     Gets or sets valid scopes that will be used to check against the token's scopes. Multiple scopes can be
    ///     provided as a comma-separated list. The scope validation will be disabled when null or empty but it is
    ///     <b>strongly</b> recommended to enable it.
    /// </summary>
    public string? Scopes { get; set; }

    /// <summary>
    ///     Gets or sets the base 64 encoded certificate (in Pfx format) to be used to check the JWT token signature.
    /// </summary>
    public string? Base64EncodedPfx { get; set; }

    /// <summary>
    ///     Gets or sets the optional certificate password.
    /// </summary>
    public string? PasswordPfx { get; set; }
}
