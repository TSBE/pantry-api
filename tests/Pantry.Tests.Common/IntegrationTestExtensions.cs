using System;
using System.Globalization;
using System.Net.Http;
using Pantry.Common.Time;

namespace Pantry.Tests.Common;

/// <summary>
///     This class holds several extension methods to make it easy, to use the <see cref="DateTimeContextMiddleware" /> with
///     integration tests.
/// </summary>
public static class IntegrationTestExtensions
{
    /// <summary>
    ///     Adds the necessary header to the request to set the passed DateTime as <see cref="DateTimeContext" /> for this request.
    /// </summary>
    /// <param name="requestMessage">The <see cref="HttpRequestMessage" /> to add the header to.</param>
    /// <param name="dateTime">The <see cref="DateTime" /> to use as current point of time during the request.</param>
    public static void WithDateTimeContext(this HttpRequestMessage requestMessage, DateTime dateTime)
    {
        if (requestMessage == null)
        {
            throw new ArgumentNullException(nameof(requestMessage));
        }

        requestMessage.Headers.Add(DateTimeContextMiddleware.DateTimeContextHttpHeaderName, dateTime.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture));
    }

    /// <summary>
    ///     Adds the necessary header to the request to set the passed DateTime as <see cref="DateTimeContext" /> for this request.
    /// </summary>
    /// <param name="httpContent">The <see cref="HttpContent" /> to add the header to.</param>
    /// <param name="dateTime">The <see cref="DateTime" /> to use as current point of time during the request.</param>
    public static void WithDateTimeContext(this HttpContent httpContent, DateTime dateTime)
    {
        if (httpContent == null)
        {
            throw new ArgumentNullException(nameof(httpContent));
        }

        httpContent.Headers.Add(DateTimeContextMiddleware.DateTimeContextHttpHeaderName, dateTime.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture));
    }

    /// <summary>
    ///     Adds the necessary header to all requests to set the passed DateTime as <see cref="DateTimeContext" /> for this request.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient" /> to add the default header.</param>
    /// <param name="dateTime">The <see cref="DateTime" /> to use as current point of time during the request.</param>
    public static void StartDateTimeContext(this HttpClient client, DateTime dateTime)
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        client.DefaultRequestHeaders.Add(DateTimeContextMiddleware.DateTimeContextHttpHeaderName, dateTime.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture));
    }

    /// <summary>
    ///     Removes the default request header added from <see cref="StartDateTimeContext" />.
    /// </summary>
    /// <param name="client">The <see cref="HttpClient" /> to remove the default header from.</param>
    public static void EndDateTimeContext(this HttpClient client)
    {
        if (client == null)
        {
            throw new ArgumentNullException(nameof(client));
        }

        client.DefaultRequestHeaders.Remove(DateTimeContextMiddleware.DateTimeContextHttpHeaderName);
    }
}
