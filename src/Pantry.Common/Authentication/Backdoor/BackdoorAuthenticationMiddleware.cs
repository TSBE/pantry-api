using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Pantry.Common.Authentication.Backdoor;

/// <summary>
///     Used in test environments only, it simulates the authentication creating a fake principal.
/// </summary>
public class BackdoorAuthenticationMiddleware
{
    private const string UserIdHeaderName = "x-user-id";

    private const string ScopesHeaderName = "x-scopes";

    private readonly ILogger<BackdoorAuthenticationMiddleware> _logger;

    private readonly RequestDelegate _next;

    private readonly string _scopes;

    private readonly BackdoorSettings _settings;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BackdoorAuthenticationMiddleware" /> class.
    /// </summary>
    /// <param name="next">
    ///     The next <see cref="RequestDelegate" />.
    /// </param>
    /// <param name="settings">
    ///     The <see cref="BackdoorSettings" />.
    /// </param>
    /// <param name="logger">
    ///     The <see cref="ILogger{TCategoryName}" />.
    /// </param>
    public BackdoorAuthenticationMiddleware(
        RequestDelegate next,
        BackdoorSettings settings,
        ILogger<BackdoorAuthenticationMiddleware> logger)
    {
        _next = next;
        _settings = settings;
        _logger = logger;

        _scopes = string.IsNullOrEmpty(settings.Scopes) ? DefaultScope : settings.Scopes;
    }

    /// <summary>
    ///     Gets the name of the default scope.
    /// </summary>
    public static string DefaultScope { get; } =
        $"backdoor-scope-{Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture)}";

    /// <summary>
    ///     Called to execute the middleware.
    /// </summary>
    /// <param name="context">
    ///     The <see cref="HttpContext" /> of the current request.
    /// </param>
    /// <returns>
    ///     A <see cref="Task" /> representing the asynchronous operation.
    /// </returns>
    public async Task Invoke(HttpContext context)
    {
        if (context.User.Identity is not { IsAuthenticated: true })
        {
            ClaimsPrincipal? userPrincipal = GetBackdoorUserPrincipal(context);

            if (userPrincipal != null)
            {
                context.User = userPrincipal;

                _logger.LogWarning(
                    "User {UserId} authenticated via backdoor.",
                    context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }
        }

        await _next(context);
    }

    private ClaimsPrincipal? GetBackdoorUserPrincipal(HttpContext context)
    {
        string? userId = context.Request.Headers[UserIdHeaderName].FirstOrDefault();

        if (string.IsNullOrEmpty(userId))
        {
            userId = _settings.DefaultUserId;
        }
        else if (userId == "anonymous")
        {
            return null;
        }

        List<Claim> claims = _settings.ClaimsProvider?.Invoke(userId).ToList() ?? new List<Claim>(3);

        if (claims.All(claim => claim.Type != ClaimTypes.NameIdentifier))
        {
            claims.Add(new Claim(ClaimTypes.Name, userId));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
        }

        string scopes = context.Request.Headers[ScopesHeaderName].FirstOrDefault() ?? _scopes;

        claims.Add(new Claim("scope", scopes));

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "backdoor"));
    }
}
