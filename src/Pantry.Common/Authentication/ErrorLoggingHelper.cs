using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Pantry.Common.Authentication;

internal class ErrorLoggingHelper
{
    private ILogger<ErrorLoggingHelper>? _logger;

    public void LogAuthenticationException(AuthenticationFailedContext context)
    {
        _logger ??= context.HttpContext.RequestServices.GetRequiredService<ILogger<ErrorLoggingHelper>>();
        _logger.LogWarning(context.Exception, "Jwt authentication failed.");
    }
}
