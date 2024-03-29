// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.HttpExtensions;

/// <summary>
///     This interface extends the IHttpBehaviour but makes it possible to change the values.
///     A use-case would be to call Clone on the IHttpBehaviour and modify the settings, return/assign the new value to a
///     IHttpBehaviour
///     This would be needed to pass the IHttpBehaviour via a CallContext.
/// </summary>
public interface IChangeableHttpBehaviour : IHttpBehaviour
{
    /// <summary>
    ///     The default encoding which is used wherever an encoding is specified.
    ///     The default is set to Encoding.UTF8
    /// </summary>
    new Encoding DefaultEncoding { get; set; }

    /// <summary>
    ///     Action which is called to notify of download progress.
    ///     Only used when using non-string content like Bitmaps or MemoryStreams.
    ///     Also the UseProgressStream needs to be true for this download progress
    /// </summary>
    new Action<float> DownloadProgress { get; set; }

    /// <summary>
    ///     This can be used to change the behaviour of Http operation, default is to read the complete response.
    /// </summary>
    new HttpCompletionOption HttpCompletionOption { get; set; }

    /// <summary>
    ///     This is the list of IHttpContentConverters which is used when converting from/to HttpContent
    /// </summary>
    new IList<IHttpContentConverter> HttpContentConverters { get; set; }

    /// <summary>
    ///     Pass your HttpSettings here, which will be used to create the HttpClient
    ///     If not specified, the HttpSettings.GlobalSettings will be used
    /// </summary>
    new IHttpSettings HttpSettings { get; set; }

    /// <summary>
    ///     This is used to de- serialize Json, can be overwritten by your own implementation.
    ///     By default, also when empty, the SimpleJsonSerializer is used.
    /// </summary>
    new IJsonSerializer JsonSerializer { get; set; }

    /// <summary>
    ///     An action which can modify the HttpClient which is generated in the HttpClientFactory.
    ///     Use cases for this, might be adding a header or other settings for specific cases
    /// </summary>
    new Action<HttpClient> OnHttpClientCreated { get; set; }

    /// <summary>
    ///     An Func which can modify the HttpContent right before it's used to start the request.
    ///     This can be used to add a specific header, e.g. set a filename etc, or return a completely different HttpContent
    ///     type
    /// </summary>
    new Func<HttpContent, HttpContent> OnHttpContentCreated { get; set; }

    /// <summary>
    ///     An Func which can modify or wrap the HttpMessageHandler which is generated in the HttpMessageHandlerFactory.
    ///     Use cases for this, might be if you have very specify settings which can't be set via the IHttpSettings
    ///     Or you want to add additional behaviour (extend DelegatingHandler!!) like the OAuthDelegatingHandler
    /// </summary>
    new Func<HttpMessageHandler, HttpMessageHandler> OnHttpMessageHandlerCreated { get; set; }

    /// <summary>
    ///     An Func which can modify the HttpRequestMessage right before it's used to start the request.
    ///     This can be used to add a specific header, which should not be for all requests.
    ///     As the called func has access to HttpRequestMessage with the content, uri and method this is quite usefull, it can
    ///     return a completely different HttpRequestMessage
    /// </summary>
    new Func<HttpRequestMessage, HttpRequestMessage> OnHttpRequestMessageCreated { get; set; }

    /// <summary>
    ///     Specify the buffer for reading operations
    /// </summary>
    new int ReadBufferSize { get; set; }

    /// <summary>
    ///     If a request gets a response which has a HTTP status code which is an error, it would normally THROW an exception.
    ///     Sometimes you would still want the response, settings this to false would allow this.
    ///     This can be ignored for all HttpResponse returning methods.
    /// </summary>
    new bool ThrowOnError { get; set; }

    /// <summary>
    ///     Action which is called to notify of upload progress.
    ///     Only used when using non-string content like Bitmaps or MemoryStreams.
    ///     Also the UseProgressStream needs to be true for this upload progress
    /// </summary>
    new Action<float> UploadProgress { get; set; }

    /// <summary>
    ///     Whenever a post is made to upload memorystream or bitmaps, this value is used to decide:
    ///     true: ProgressStreamContent is used, instead of StreamContent
    /// </summary>
    new bool UseProgressStream { get; set; }

    /// <summary>
    ///     This cookie container will be assed when creating the HttpMessageHandler and UseCookies is true
    /// </summary>
    new CookieContainer CookieContainer { get; set; }

    /// <summary>
    ///     Check if the response has the expected content-type, when servers are used that are not following specifications
    ///     this should be set to false
    /// </summary>
    new bool ValidateResponseContentType { get; set; }

    /// <inheritdoc cref="IHttpBehaviour" />
    new IDictionary<string, IHttpRequestConfiguration> RequestConfigurations { get; set; }
}