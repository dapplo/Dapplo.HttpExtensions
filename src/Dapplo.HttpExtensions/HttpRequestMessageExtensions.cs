// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dapplo.HttpExtensions.Factory;

namespace Dapplo.HttpExtensions;

/// <summary>
///     Extensions for the HttpRequestMessage class
/// </summary>
public static class HttpRequestMessageExtensions
{
    private static readonly LogSource Log = new LogSource();

    /// <summary>
    ///     Add default request header without validation
    /// </summary>
    /// <param name="httpRequestMessage">HttpRequestMessage</param>
    /// <param name="name">Header name</param>
    /// <param name="value">Header value</param>
    /// <returns>HttpRequestMessage for fluent usage</returns>
    public static HttpRequestMessage AddRequestHeader(this HttpRequestMessage httpRequestMessage, string name, string value)
    {
        httpRequestMessage.Headers.TryAddWithoutValidation(name, value);
        return httpRequestMessage;
    }

    /// <summary>
    ///     Send the supplied HttpRequestMessage, and get a response back
    /// </summary>
    /// <typeparam name="TResponse">The Type to read into</typeparam>
    /// <param name="httpRequestMessage">HttpRequestMessage</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>the deserialized object of type T or default(T)</returns>
    public static async Task<TResponse> SendAsync<TResponse>(this HttpRequestMessage httpRequestMessage,
        CancellationToken cancellationToken = default) where TResponse : class
    {
        using var httpClient = HttpClientFactory.Create(httpRequestMessage.RequestUri);
        return await httpRequestMessage.SendAsync<TResponse>(httpClient, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Send the supplied HttpRequestMessage, and get a response back
    /// </summary>
    /// <typeparam name="TResponse">The Type to read into</typeparam>
    /// <param name="httpRequestMessage">HttpRequestMessage</param>
    /// <param name="httpClient">HttpClient</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>the deserialized object of type T or default(T)</returns>
    public static async Task<TResponse> SendAsync<TResponse>(this HttpRequestMessage httpRequestMessage, HttpClient httpClient,
        CancellationToken cancellationToken = default)
        where TResponse : class
    {
        var httpBehaviour = HttpBehaviour.Current;
        Log.Verbose().WriteLine("Sending {0} HttpRequestMessage with Uri: {1}", httpRequestMessage.Method, httpRequestMessage.RequestUri);
        var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, httpBehaviour.HttpCompletionOption, cancellationToken).ConfigureAwait(false);
        try
        {
            return await httpResponseMessage.GetAsAsync<TResponse>(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            var resultType = typeof(TResponse);
            // Quick exit if the caller just wants the HttpResponseMessage
            if (resultType != typeof(HttpResponseMessage))
            {
                httpResponseMessage.Dispose();
            }
        }
    }

    /// <summary>
    ///     Send the supplied HttpRequestMessage, ignoring the response
    /// </summary>
    /// <param name="httpRequestMessage">HttpRequestMessage</param>
    /// <param name="httpClient">HttpClient</param>
    /// <param name="cancellationToken">CancellationToken</param>
    public static async Task SendAsync(this HttpRequestMessage httpRequestMessage, HttpClient httpClient,
        CancellationToken cancellationToken = default)
    {
        var httpBehaviour = HttpBehaviour.Current;
        Log.Verbose().WriteLine("Sending {0} HttpRequestMessage with Uri: {1}", httpRequestMessage.Method, httpRequestMessage.RequestUri);

        using var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseContentRead, cancellationToken)
            .ConfigureAwait(false);
        if (httpBehaviour.ThrowOnError)
        {
            httpResponseMessage.EnsureSuccessStatusCode();
        }
    }
}