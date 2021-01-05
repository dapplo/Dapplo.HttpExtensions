// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Dapplo.HttpExtensions.Extensions;
using Dapplo.HttpExtensions.Support;
using Dapplo.Log;

namespace Dapplo.HttpExtensions.Factory
{
    /// <summary>
    ///     Dapplo.HttpExtension uses the HttpRequestMessage to send the requests.
    ///     This makes it a lot more flexible to use Accept headers and other stuff
    ///     This is the factory for it.
    /// </summary>
    public static class HttpRequestMessageFactory
    {
        private static readonly LogSource Log = new LogSource();
        private static readonly HttpMethod PatchMethod = new HttpMethod("PATCH");
        /// <summary>
        ///     Create a HttpRequestMessage for the specified method
        /// </summary>
        /// <param name="method">Method to create the request message for</param>
        /// <param name="requestUri">the target uri for this message</param>
        /// <param name="resultType">Type</param>
        /// <param name="contentType">Type</param>
        /// <param name="content">content to convert to HttpContent</param>
        /// <returns>HttpRequestMessage</returns>
        public static HttpRequestMessage Create(HttpMethod method, Uri requestUri, Type resultType = null, Type contentType = null, object content = null)
        {
            Log.Verbose().WriteLine("Creating request for {0}", requestUri);
            var httpBehaviour = HttpBehaviour.Current;
            var configuration = httpBehaviour.GetConfig<HttpRequestMessageConfiguration>();
            contentType ??= content?.GetType();

            var httpRequestMessage = new HttpRequestMessage(method, requestUri)
            {
                Content = HttpContentFactory.Create(contentType, content),
                Version = configuration.HttpMessageVersion
            };
#if NET5_0
            // Set supplied Properties from the HttpRequestMessageConfiguration
            foreach (var key in configuration.Properties.Keys)
            {
                httpRequestMessage.Options.Set(new HttpRequestOptionsKey<object>(key), configuration.Properties[key]);
            }
#else
            // Set supplied Properties from the HttpRequestMessageConfiguration
            foreach (var key in configuration.Properties.Keys)
            {
                httpRequestMessage.Properties.Add(key, configuration.Properties[key]);
            }
#endif

            // if the type has a HttpAttribute with HttpPart.Request
            if (contentType?.GetTypeInfo().GetCustomAttribute<HttpRequestAttribute>() != null)
            {
                // And a property has a HttpAttribute with HttpPart.RequestHeaders
                var headersPropertyInfo = contentType.GetProperties().FirstOrDefault(t => t.GetCustomAttribute<HttpPartAttribute>()?.Part == HttpParts.RequestHeaders);
                if (headersPropertyInfo?.GetValue(content) is IDictionary<string, string> headersValue)
                {
                    foreach (var headerName in headersValue.Keys)
                    {
                        var headerValue = headersValue[headerName];
                        httpRequestMessage.Headers.TryAddWithoutValidation(headerName, headerValue);
                    }
                }
            }

            if (resultType != null && httpBehaviour.HttpContentConverters != null)
            {
                foreach (var httpContentConverter in httpBehaviour.HttpContentConverters)
                {
                    httpContentConverter.AddAcceptHeadersForType(resultType, httpRequestMessage);
                }
            }

            // Make sure the OnCreateHttpRequestMessage function is called
            return httpBehaviour.OnHttpRequestMessageCreated?.Invoke(httpRequestMessage) ?? httpRequestMessage;
        }

        /// <summary>
        ///     Create a HttpRequestMessage for the specified method
        /// </summary>
        /// <typeparam name="TResponse">The type for the response, this modifies the Accept headers</typeparam>
        /// <typeparam name="TContent">The type of the content (for put / post)</typeparam>
        /// <param name="method">Method to create the request message for</param>
        /// <param name="requestUri">the target uri for this message</param>
        /// <param name="content">HttpContent</param>
        /// <returns>HttpRequestMessage</returns>
        public static HttpRequestMessage Create<TResponse, TContent>(HttpMethod method, Uri requestUri, TContent content = default)
            where TResponse : class where TContent : class
        {
            return Create(method, requestUri, typeof(TResponse), typeof(TContent), content);
        }

        /// <summary>
        ///     Create a HttpRequestMessage for the specified method
        /// </summary>
        /// <typeparam name="TResponse">The type for the response, this modifies the Accept headers</typeparam>
        /// <param name="method">Method to create the request message for</param>
        /// <param name="requestUri">the target uri for this message</param>
        /// <returns>HttpRequestMessage</returns>
        public static HttpRequestMessage Create<TResponse>(HttpMethod method, Uri requestUri)
            where TResponse : class
        {
            return Create(method, requestUri, typeof(TResponse));
        }

        /// <summary>
        ///     Create a HttpRequestMessage for the DELETE method
        /// </summary>
        /// <param name="requestUri">the target uri for this message</param>
        /// <typeparam name="TResponse">The type for the response, this modifies the Accep headers</typeparam>
        /// <returns>HttpRequestMessage</returns>
        public static HttpRequestMessage CreateDelete<TResponse>(Uri requestUri)
            where TResponse : class
        {
            return Create<TResponse>(HttpMethod.Delete, requestUri);
        }

        /// <summary>
        ///     Create a HttpRequestMessage for the DELETE method
        /// </summary>
        /// <param name="requestUri">the target uri for this message</param>
        /// <returns>HttpRequestMessage</returns>
        public static HttpRequestMessage CreateDelete(Uri requestUri)
        {
            return Create(HttpMethod.Delete, requestUri);
        }

        /// <summary>
        ///     Create a HttpRequestMessage for the GET method
        /// </summary>
        /// <param name="requestUri">the target uri for this message</param>
        /// <typeparam name="TResponse">The type for the response, this modifies the Accept headers</typeparam>
        /// <returns>HttpRequestMessage</returns>
        public static HttpRequestMessage CreateGet<TResponse>(Uri requestUri)
            where TResponse : class
        {
            return Create<TResponse>(HttpMethod.Get, requestUri);
        }

        /// <summary>
        ///     Create a HttpRequestMessage for the HEAD method
        /// </summary>
        /// <param name="requestUri">the target uri for this message</param>
        /// <returns>HttpRequestMessage</returns>
        public static HttpRequestMessage CreateHead(Uri requestUri)
        {
            return Create(HttpMethod.Head, requestUri);
        }

        /// <summary>
        ///     Create a HttpRequestMessage for the PATCH method
        /// </summary>
        /// <typeparam name="TResponse">The type for the response, this modifies the Accept headers</typeparam>
        /// <param name="requestUri">the target uri for this message</param>
        /// <param name="content">HttpContent</param>
        /// <returns>HttpRequestMessage</returns>
        public static HttpRequestMessage CreatePatch<TResponse>(Uri requestUri, object content = null)
            where TResponse : class
        {
            return Create(PatchMethod, requestUri, typeof(TResponse), content?.GetType(), content);
        }

        /// <summary>
        ///     Create a HttpRequestMessage for the PATCH method
        /// </summary>
        /// <param name="requestUri">the target uri for this message</param>
        /// <param name="content">HttpContent</param>
        /// <returns>HttpRequestMessage</returns>
        public static HttpRequestMessage CreatePatch(Uri requestUri, object content = null)
        {
            return Create(PatchMethod, requestUri, null, content?.GetType(), content);
        }

        /// <summary>
        ///     Create a HttpRequestMessage for the POST method
        /// </summary>
        /// <typeparam name="TResponse">The type for the response, this modifies the Accept headers</typeparam>
        /// <param name="requestUri">the target uri for this message</param>
        /// <param name="content">HttpContent</param>
        /// <returns>HttpRequestMessage</returns>
        public static HttpRequestMessage CreatePost<TResponse>(Uri requestUri, object content = null)
            where TResponse : class
        {
            return Create(HttpMethod.Post, requestUri, typeof(TResponse), content?.GetType(), content);
        }

        /// <summary>
        ///     Create a HttpRequestMessage for the POST method
        /// </summary>
        /// <param name="requestUri">the target uri for this message</param>
        /// <param name="content">HttpContent</param>
        /// <returns>HttpRequestMessage</returns>
        public static HttpRequestMessage CreatePost(Uri requestUri, object content = null)
        {
            return Create(HttpMethod.Post, requestUri, null, content?.GetType(), content);
        }

        /// <summary>
        ///     Create a HttpRequestMessage for the PUT method
        /// </summary>
        /// <typeparam name="TResponse">The type for the response, this modifies the Accept headers</typeparam>
        /// <param name="requestUri">the target uri for this message</param>
        /// <param name="content">HttpContent</param>
        /// <returns>HttpRequestMessage</returns>
        public static HttpRequestMessage CreatePut<TResponse>(Uri requestUri, object content = null)
            where TResponse : class
        {
            return Create(HttpMethod.Put, requestUri, typeof(TResponse), content?.GetType(), content);
        }

        /// <summary>
        ///     Create a HttpRequestMessage for the PUT method
        /// </summary>
        /// <param name="requestUri">the target uri for this message</param>
        /// <param name="content">HttpContent</param>
        /// <returns>HttpRequestMessage</returns>
        public static HttpRequestMessage CreatePut(Uri requestUri, object content = null)
        {
            return Create(HttpMethod.Put, requestUri, null, content?.GetType(), content);
        }
    }
}