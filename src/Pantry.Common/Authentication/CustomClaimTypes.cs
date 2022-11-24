namespace Pantry.Common.Authentication;

public static class CustomClaimTypes
{
    /// <summary>
    /// client_id : client identifier.
    /// </summary>
    public const string CLIENTIDENTIFIER = "client_id";

    /// <summary>
    /// jti : string identifier for the token.
    /// </summary>
    public const string TOKENIDENTIFIER = "jti";

    /// <summary>
    /// iat : integer timestamp indicating when this token was issued.
    /// </summary>
    public const string ISSUEDAT = "iat";

    /// <summary>
    /// exp : integer timestamp indicating when this token will expire.
    /// </summary>
    public const string EXPIREAT = "exp";

    /// <summary>
    /// iss : string representing issuer of this token.
    /// </summary>
    public const string ISSUER = "iss";

    /// <summary>
    /// scope : space-separated list of scopes.
    /// </summary>
    public const string SCOPES = "scope";

    /// <summary>
    /// scope : space-separated list of scopes.
    /// </summary>
    public const string PERMISSIONS = "permission";

    /// <summary>
    /// household_id : string representing id of household.
    /// </summary>
    public const string HOUSEHOLDID = "household_id";
}
