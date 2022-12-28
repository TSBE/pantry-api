using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Pantry.Common.Time;

/// <summary>
///     A middleware that reads a datetime value from a HTTP request header. This datetime value is then used to construct a
///     <see cref="DateTimeContext" /> that is used by the application. As a consequence whenever the application requests the
///     current datetime, the provided datetime value from the HTTP request is used.
///     <remarks>Make sure that this middleware is not used in the real production environment!</remarks>
/// </summary>
public static class DateTimeContextMiddleware
{
    /// <summary>
    ///     The name of the http header which can be used to instruct the middleware to set a DateTime for the duration of the request.
    /// </summary>
    public static readonly string DateTimeContextHttpHeaderName = $"X-{nameof(DateTimeContext)}";

    /// <summary>
    ///     Adds a middleware that reads a datetime value from a HTTP request header. This datetime value is then used to construct a
    ///     <see cref="DateTimeContext" /> that is used by the application. As a consequence whenever the application requests the
    ///     current datetime, the provided datetime value from the HTTP request is used.
    ///     <remarks>
    ///         To request the execution within a <see cref="DateTimeContext" />, set the corresponding http header (see
    ///         <see cref="DateTimeContextHttpHeaderName" />) in ISO 8601 format. If no timezone is specified, it is assumed that UTC is
    ///         used. Make sure that this middleware is not used in the real production environment!
    ///     </remarks>
    /// </summary>
    /// <param name="builder">The <see cref="IApplicationBuilder" /> to add the middleware to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IApplicationBuilder UseDateTimeContext(this IApplicationBuilder builder) => builder.Use(DateTimeContextMiddlewareImplementation);

    private static async Task DateTimeContextMiddlewareImplementation(HttpContext context, Func<Task> next)
    {
        DateTimeContext? dateTimeContext = null;

        if (context.Request.Headers.TryGetValue(DateTimeContextHttpHeaderName, out StringValues headers))
        {
            var header = headers[0]; // Only one value is expected.
            const DateTimeStyles dateTimeStyles = DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal;
            if (DateTime.TryParse(header, new DateTimeFormatInfo(), dateTimeStyles, out DateTime dateTime))
            {
                dateTimeContext = new DateTimeContext(dateTime);
            }
        }

        try
        {
            await next();
        }
        finally
        {
            dateTimeContext?.Dispose();
        }
    }
}
