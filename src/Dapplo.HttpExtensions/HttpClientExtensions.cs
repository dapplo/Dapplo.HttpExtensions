﻿// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net.Http.Headers;
using Dapplo.HttpExtensions.Factory;

namespace Dapplo.HttpExtensions;

/// <summary>
///     Extensions for the HttpClient class
/// </summary>
public static class HttpClientExtensions
{
    private static readonly LogSource Log = new LogSource();

    /// <summary>
    ///     Add default request header without validation
    /// </summary>
    /// <param name="client">HttpClient</param>
    /// <param name="name">Header name</param>
    /// <param name="value">Header value</param>
    /// <returns>HttpClient for fluent usage</returns>
    public static HttpClient AddDefaultRequestHeader(this HttpClient client, string name, string value)
    {
        client.DefaultRequestHeaders.TryAddWithoutValidation(name, value);
        return client;
    }

    /// <summary>
    ///     Send a Delete request to the server
    /// </summary>
    /// <typeparam name="TResponse">
    ///     the generic type to return the result into, use HttpContent or HttpResponseMessage to get
    ///     those unprocessed
    /// </typeparam>
    /// <param name="httpClient">HttpClient</param>
    /// <param name="uri">Uri to send the delete request to</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>TResult</returns>
    public static async Task<TResponse> DeleteAsync<TResponse>(this HttpClient httpClient, Uri uri, CancellationToken cancellationToken = default)
        where TResponse : class
    {
        if (uri is null)
        {
            throw new ArgumentNullException(nameof(uri));
        }

        using var httpRequestMessage = HttpRequestMessageFactory.CreateDelete<TResponse>(uri);
        return await httpRequestMessage.SendAsync<TResponse>(httpClient, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Send a Delete request to the server
    /// </summary>
    /// <param name="httpClient">HttpClient</param>
    /// <param name="uri">Uri to send the delete request to</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>Task</returns>
    public static Task DeleteAsync(this HttpClient httpClient, Uri uri, CancellationToken cancellationToken = default)
    {
        return httpClient.DeleteAsync<HttpResponse>(uri, cancellationToken);
    }

    /// <summary>
    ///     Get the content from the specified uri via the HttpClient read into a Type object
    ///     Currently we support Json objects which are annotated with the DataContract/DataMember attributes
    ///     We might support other object, e.g MemoryStream, Bitmap etc soon
    /// </summary>
    /// <typeparam name="TResponse">The Type to read into</typeparam>
    /// <param name="client">HttpClient</param>
    /// <param name="uri">URI</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>the deserialized object of type T or default(T)</returns>
    public static async Task<TResponse> GetAsAsync<TResponse>(this HttpClient client, Uri uri, CancellationToken cancellationToken = default)
        where TResponse : class
    {
        if (uri is null)
        {
            throw new ArgumentNullException(nameof(uri));
        }
        var httpBehaviour = HttpBehaviour.Current;

        using var httpRequestMessage = HttpRequestMessageFactory.CreateGet<TResponse>(uri);
        var httpResponseMessage = await client.SendAsync(httpRequestMessage, httpBehaviour.HttpCompletionOption, cancellationToken).ConfigureAwait(false);
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
    ///     Retrieve only the content headers, by using the HTTP HEAD method
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="uri">Uri to get HEAD for</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>HttpContentHeaders</returns>
    public static async Task<HttpContentHeaders> HeadAsync(this HttpClient httpClient, Uri uri, CancellationToken cancellationToken = default)
    {
        if (uri is null)
        {
            throw new ArgumentNullException(nameof(uri));
        }
        Log.Verbose().WriteLine("Requesting headers for: {0}", uri);
        using var httpRequestMessage = HttpRequestMessageFactory.CreateHead(uri);
        using var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);
        await httpResponseMessage.HandleErrorAsync().ConfigureAwait(false);
        return httpResponseMessage.Content.Headers;
    }

    /// <summary>
    ///     Patch the content, and get the reponse
    /// </summary>
    /// <typeparam name="TResponse">
    ///     the generic type to return the result into, use HttpContent or HttpResponseMessage to get
    ///     those unprocessed
    /// </typeparam>
    /// <param name="httpClient">HttpClient</param>
    /// <param name="uri">Uri to patch request to</param>
    /// <param name="content">Content to patch</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>TResult</returns>
    public static async Task<TResponse> PatchAsync<TResponse>(this HttpClient httpClient, Uri uri, object content,
        CancellationToken cancellationToken = default)
        where TResponse : class
    {
        if (uri is null)
        {
            throw new ArgumentNullException(nameof(uri));
        }

        if (content is null)
        {
            Log.Warn().WriteLine("No content supplied, this is ok but unusual.");
        }

        using var httpRequestMessage = HttpRequestMessageFactory.CreatePatch<TResponse>(uri, content);
        return await httpRequestMessage.SendAsync<TResponse>(httpClient, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Patch the content, and don't expect (ignore) the response
    /// </summary>
    /// <param name="httpClient">HttpClient</param>
    /// <param name="uri">Uri to patch an empty request to</param>
    /// <param name="content">Content to patch</param>
    /// <param name="cancellationToken">CancellationToken</param>
    public static async Task PatchAsync(this HttpClient httpClient, Uri uri, object content, CancellationToken cancellationToken = default)
    {
        if (uri is null)
        {
            throw new ArgumentNullException(nameof(uri));
        }
        if (content is null)
        {
            Log.Warn().WriteLine("No content supplied, this is ok but unusual.");
        }

        using var httpRequestMessage = HttpRequestMessageFactory.CreatePatch(uri, content);
        await httpRequestMessage.SendAsync(httpClient, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Post the content, and get the reponse
    /// </summary>
    /// <typeparam name="TResponse">
    ///     the generic type to return the result into, use HttpContent or HttpResponseMessage to get
    ///     those unprocessed
    /// </typeparam>
    /// <param name="httpClient">HttpClient</param>
    /// <param name="uri">Uri to post request to</param>
    /// <param name="content">Content to post</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>TResult</returns>
    public static async Task<TResponse> PostAsync<TResponse>(this HttpClient httpClient, Uri uri, object content,
        CancellationToken cancellationToken = default)
        where TResponse : class
    {
        if (uri is null)
        {
            throw new ArgumentNullException(nameof(uri));
        }

        if (content is null)
        {
            Log.Warn().WriteLine("No content supplied, this is ok but unusual.");
        }

        using var httpRequestMessage = HttpRequestMessageFactory.CreatePost<TResponse>(uri, content);
        return await httpRequestMessage.SendAsync<TResponse>(httpClient, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Post the content, and don't expect (ignore) the response
    /// </summary>
    /// <param name="httpClient">HttpClient</param>
    /// <param name="uri">Uri to post an empty request to</param>
    /// <param name="content">Content to post</param>
    /// <param name="cancellationToken">CancellationToken</param>
    public static async Task PostAsync(this HttpClient httpClient, Uri uri, object content, CancellationToken cancellationToken = default)
    {
        if (uri is null)
        {
            throw new ArgumentNullException(nameof(uri));
        }
        if (content is null)
        {
            Log.Warn().WriteLine("No content supplied, this is ok but unusual.");
        }

        using var httpRequestMessage = HttpRequestMessageFactory.CreatePost(uri, content);
        await httpRequestMessage.SendAsync(httpClient, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Put the content, and get the reponse
    /// </summary>
    /// <typeparam name="TResponse">
    ///     the generic type to return the result into, use HttpContent or HttpResponseMessage to get
    ///     those unprocessed
    /// </typeparam>
    /// <param name="httpClient">HttpClient</param>
    /// <param name="uri">Uri to put the request to</param>
    /// <param name="content">Content to put</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>TResult</returns>
    public static async Task<TResponse> PutAsync<TResponse>(this HttpClient httpClient, Uri uri, object content,
        CancellationToken cancellationToken = default)
        where TResponse : class
    {
        if (uri is null)
        {
            throw new ArgumentNullException(nameof(uri));
        }
        if (content is null)
        {
            Log.Warn().WriteLine("No content supplied, this is ok but unusual.");
        }

        using var httpRequestMessage = HttpRequestMessageFactory.CreatePut<TResponse>(uri, content);
        return await httpRequestMessage.SendAsync<TResponse>(httpClient, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Put the content, ignore the reponse
    /// </summary>
    /// <param name="httpClient">HttpClient</param>
    /// <param name="uri">Uri to put the request to</param>
    /// <param name="content">Content to put</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>Task</returns>
    public static async Task PutAsync(this HttpClient httpClient, Uri uri, object content, CancellationToken cancellationToken = default)
    {
        if (uri is null)
        {
            throw new ArgumentNullException(nameof(uri));
        }
        if (content is null)
        {
            Log.Warn().WriteLine("No content supplied, this is ok but unusual.");
        }

        using var httpRequestMessage = HttpRequestMessageFactory.CreatePut(uri, content);
        await httpRequestMessage.SendAsync(httpClient, cancellationToken).ConfigureAwait(false);
    }
}