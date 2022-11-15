using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;

namespace Pantry.Tests.Common;

/// <summary>
/// Provides various extensions for the <see cref="HttpClient"/>.
/// </summary>
public static class HttpClientExtensions
{
    /// <summary>
    /// Executes a Post request with the given object serialized as json.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="requestUri">The request uri.</param>
    /// <param name="obj">The object.</param>
    /// <param name="headers">Headers for the request.</param>
    /// <typeparam name="T">The type of the used object.</typeparam>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public static Task<HttpResponseMessage> PostAsJsonWithHeadersAsync<T>(this HttpClient client, string requestUri, T obj, ReadOnlyCollection<(string Key, string Value)> headers)
        => PostAsJsonWithHeadersAsync(client, new Uri(requestUri), obj, headers);

    /// <summary>
    /// Executes a Post request with the given object serialized as json.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="requestUri">The request uri.</param>
    /// <param name="obj">The object.</param>
    /// <param name="headers">Headers for the request.</param>
    /// <typeparam name="T">The type of the used object.</typeparam>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public static Task<HttpResponseMessage> PostAsJsonWithHeadersAsync<T>(this HttpClient client, Uri requestUri, T obj, ReadOnlyCollection<(string Key, string Value)> headers)
        => DoRequestWithJsonAndHeadersAsync(client, HttpMethod.Post, requestUri, obj, headers);

    /// <summary>
    /// Executes a Patch request with the given object serialized as json.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="requestUri">The request uri.</param>
    /// <param name="obj">The object.</param>
    /// <param name="headers">Headers for the request.</param>
    /// <typeparam name="T">The type of the used object.</typeparam>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public static Task<HttpResponseMessage> PatchAsJsonWithHeadersAsync<T>(this HttpClient client, string requestUri, T obj, ReadOnlyCollection<(string Key, string Value)> headers)
        => PatchAsJsonWithHeadersAsync(client, new Uri(requestUri), obj, headers);

    /// <summary>
    /// Executes a Patch request with the given object serialized as json.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="requestUri">The request uri.</param>
    /// <param name="obj">The object.</param>
    /// <param name="headers">Headers for the request.</param>
    /// <typeparam name="T">The type of the used object.</typeparam>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public static Task<HttpResponseMessage> PatchAsJsonWithHeadersAsync<T>(this HttpClient client, Uri requestUri, T obj, ReadOnlyCollection<(string Key, string Value)> headers)
        => DoRequestWithJsonAndHeadersAsync(client, HttpMethod.Patch, requestUri, obj, headers);

    /// <summary>
    /// Executes a Put request with the given object serialized as json.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="requestUri">The request uri.</param>
    /// <param name="obj">The object.</param>
    /// <param name="headers">Headers for the request.</param>
    /// <typeparam name="T">The type of the used object.</typeparam>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public static Task<HttpResponseMessage> PutAsJsonWithHeadersAsync<T>(this HttpClient client, string requestUri, T obj, ReadOnlyCollection<(string Key, string Value)> headers)
        => PutAsJsonWithHeadersAsync(client, new Uri(requestUri), obj, headers);

    /// <summary>
    /// Executes a Put request with the given object serialized as json.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="requestUri">The request uri.</param>
    /// <param name="obj">The object.</param>
    /// <param name="headers">Headers for the request.</param>
    /// <typeparam name="T">The type of the used object.</typeparam>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public static Task<HttpResponseMessage> PutAsJsonWithHeadersAsync<T>(this HttpClient client, Uri requestUri, T obj, ReadOnlyCollection<(string Key, string Value)> headers)
        => DoRequestWithJsonAndHeadersAsync(client, HttpMethod.Put, requestUri, obj, headers);

    /// <summary>
    /// Executes a Get request with the given headers.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="requestUri">The request uri.</param>
    /// <param name="headers">Headers for the request.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public static Task<HttpResponseMessage> GetWithHeadersAsync(this HttpClient client, string requestUri, ReadOnlyCollection<(string Key, string Value)> headers)
        => GetWithHeadersAsync(client, new Uri(requestUri), headers);

    /// <summary>
    /// Executes a Get request with the given headers.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="requestUri">The request uri.</param>
    /// <param name="headers">Headers for the request.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public static Task<HttpResponseMessage> GetWithHeadersAsync(this HttpClient client, Uri requestUri, ReadOnlyCollection<(string Key, string Value)> headers)
        => DoRequestWithHeadersAsync(client, HttpMethod.Get, requestUri, headers);

    /// <summary>
    /// Execute a Delete Request with the given headers.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="requestUri">The request uri.</param>
    /// <param name="headers">Headers for the request.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public static Task<HttpResponseMessage> DeleteWithHeadersAsync(this HttpClient client, string requestUri, ReadOnlyCollection<(string Key, string Value)> headers)
        => DeleteWithHeadersAsync(client, new Uri(requestUri), headers);

    /// <summary>
    /// Execute a Delete Request with the given headers.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="requestUri">The request uri.</param>
    /// <param name="headers">Headers for the request.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
    public static Task<HttpResponseMessage> DeleteWithHeadersAsync(this HttpClient client, Uri requestUri, ReadOnlyCollection<(string Key, string Value)> headers)
        => DoRequestWithHeadersAsync(client, HttpMethod.Delete, requestUri, headers);

    private static async Task<HttpResponseMessage> DoRequestWithHeadersAsync(this HttpClient client, HttpMethod method, Uri requestUri, ReadOnlyCollection<(string Key, string Value)> headers)
    {
        using var request = new HttpRequestMessage(method, requestUri);
        foreach ((string key, string value) in headers)
        {
            request.Headers.Add(key, value);
        }

        return await client.SendAsync(request);
    }

    private static async Task<HttpResponseMessage> DoRequestWithJsonAndHeadersAsync<T>(this HttpClient client, HttpMethod httpMethod, Uri requestUri, T obj, ReadOnlyCollection<(string Key, string Value)> headers)
    {
        string serialized = JsonSerializer.Serialize(obj);

        using var request = new HttpRequestMessage(httpMethod, requestUri)
        {
            Content = new StringContent(serialized, Encoding.UTF8, "application/json")
        };
        foreach ((string key, string value) in headers)
        {
            request.Headers.Add(key, value);
        }

        return await client.SendAsync(request);
    }
}
