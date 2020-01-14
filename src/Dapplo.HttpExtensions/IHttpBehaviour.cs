// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Dapplo.HttpExtensions
{
    /// <summary>
    ///     The IHttpBehaviour is used to control the behaviour of all operations in the HttpExtensions library.
    /// </summary>
    public interface IHttpBehaviour
    {
        /// <summary>
        ///     Configuration for different parts of the library, or your own implementations, which can be set on a thread/request
        ///     base.
        ///     For missing configurations the default is taken.
        /// </summary>
        IDictionary<string, IHttpRequestConfiguration> RequestConfigurations { get; }

        /// <summary>
        ///     The default encoding which is used wherever an encoding is specified.
        ///     The default is set to Encoding.UTF8
        /// </summary>
        Encoding DefaultEncoding { get; }

        /// <summary>
        ///     Action which is called to notify of download progress.
        ///     Only used when using non-string content like Bitmaps or MemoryStreams.
        ///     Also the UseProgressStream needs to be true for this download progress
        /// </summary>
        Action<float> DownloadProgress { get; }

        /// <summary>
        ///     This can be used to change the behaviour of Http operation, default is to read the complete response.
        /// </summary>
        HttpCompletionOption HttpCompletionOption { get; }

        /// <summary>
        ///     This is the list of IHttpContentConverters which is used when converting from/to HttpContent
        /// </summary>
        IList<IHttpContentConverter> HttpContentConverters { get; }

        /// <summary>
        ///     Pass your HttpSettings here, which will be used to create the HttpClient
        ///     If not specified, the HttpSettings.GlobalSettings will be used
        /// </summary>
        IHttpSettings HttpSettings { get; }

        /// <summary>
        ///     This is used to de- serialize Json, can be overwritten by your own implementation.
        ///     By default, also when empty, the SimpleJsonSerializer is used.
        /// </summary>
        IJsonSerializer JsonSerializer { get; }

        /// <summary>
        ///     An action which can modify the HttpClient which is generated in the HttpClientFactory.
        ///     Use cases for this, might be adding a header or other settings for specific cases
        /// </summary>
        Action<HttpClient> OnHttpClientCreated { get; }

        /// <summary>
        ///     An Func which can modify the HttpContent right before it's used to start the request.
        ///     This can be used to add a specific header, e.g. set a filename etc, or return a completely different HttpContent
        ///     type
        /// </summary>
        Func<HttpContent, HttpContent> OnHttpContentCreated { get; }

        /// <summary>
        ///     An Func which can modify or wrap the HttpMessageHandler which is generated in the HttpMessageHandlerFactory.
        ///     Use cases for this, might be if you have very specify settings which can't be set via the IHttpSettings
        ///     Or you want to add additional behaviour (extend DelegatingHandler!!) like the OAuthDelegatingHandler
        /// </summary>
        Func<HttpMessageHandler, HttpMessageHandler> OnHttpMessageHandlerCreated { get; }

        /// <summary>
        ///     An Func which can modify the HttpRequestMessage right before it's used to start the request.
        ///     This can be used to add a specific header, which should not be for all requests.
        ///     As the called func has access to HttpRequestMessage with the content, uri and method this is quite useful, it can
        ///     return a completely different HttpRequestMessage
        /// </summary>
        Func<HttpRequestMessage, HttpRequestMessage> OnHttpRequestMessageCreated { get; }

        /// <summary>
        ///     Specify the buffer for reading operations
        /// </summary>
        int ReadBufferSize { get; }

        /// <summary>
        ///     If a request gets a response which has a HTTP status code which is an error, it would normally THROW an exception.
        ///     Sometimes you would still want the response, settings this to false would allow this.
        ///     This can be ignored for all HttpResponse returning methods.
        /// </summary>
        bool ThrowOnError { get; }

        /// <summary>
        ///     Action which is called to notify of upload progress, be sure to handle the UI thread issues yourself.
        ///     Only used when using non-string content like Bitmaps or MemoryStreams.
        ///     Also the UseProgressStream needs to be true for this upload progress
        /// </summary>
        Action<float> UploadProgress { get; }

        /// <summary>
        ///     Whenever a post is made to upload memory stream or bitmaps, this value is used to decide:
        ///     true: ProgressStream is used, instead of Stream
        /// </summary>
        bool UseProgressStream { get; }

        /// <summary>
        ///     Check if the response has the expected content-type, when servers are used that are not following specifications
        ///     this should be set to false
        /// </summary>
        bool ValidateResponseContentType { get; }

        /// <summary>
        ///     This cookie container will be assessed when creating the HttpMessageHandler and HttpSettings.UseCookies is true
        /// </summary>
        CookieContainer CookieContainer { get; }

        /// <summary>
        ///     Set this IHttpBehaviour on the CallContext
        /// </summary>
        void MakeCurrent();

        /// <summary>
        ///     Make a memberwise clone of the object, this is "shallow".
        /// </summary>
        /// <returns>"Shallow" Cloned instance of IChangeableHttpBehaviour</returns>
        IChangeableHttpBehaviour ShallowClone();
    }
}