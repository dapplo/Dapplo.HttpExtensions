// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Net.Http;

namespace Dapplo.HttpExtensions
{
    /// <summary>
    ///     Extensions for the HttpBehaviour class
    /// </summary>
    public static class HttpBehaviourExtensions
    {
        /// <summary>
        ///     Chain the current OnHttpContentCreated
        /// </summary>
        /// <param name="changeableHttpBehaviour"></param>
        /// <param name="newOnHttpContentCreated"></param>
        /// <returns>IChangeableHttpBehaviour for fluent usage</returns>
        public static IChangeableHttpBehaviour ChainOnHttpContentCreated(this IChangeableHttpBehaviour changeableHttpBehaviour,
            Func<HttpContent, HttpContent> newOnHttpContentCreated)
        {
            var oldOnHttpContentCreated = changeableHttpBehaviour.OnHttpContentCreated;

            changeableHttpBehaviour.OnHttpContentCreated = httpContent =>
            {
                if (oldOnHttpContentCreated != null)
                {
                    httpContent = oldOnHttpContentCreated(httpContent);
                }
                return newOnHttpContentCreated(httpContent);
            };
            return changeableHttpBehaviour;
        }

        /// <summary>
        ///     Chain the current OnHttpMessageHandlerCreated
        /// </summary>
        /// <param name="changeableHttpBehaviour"></param>
        /// <param name="newOnHttpMessageHandlerCreated">function which accepts a HttpMessageHandler and also returns one</param>
        /// <returns>IChangeableHttpBehaviour for fluent usage</returns>
        public static IChangeableHttpBehaviour ChainOnHttpMessageHandlerCreated(this IChangeableHttpBehaviour changeableHttpBehaviour,
            Func<HttpMessageHandler, HttpMessageHandler> newOnHttpMessageHandlerCreated)
        {
            var oldOnHttpMessageHandlerCreated = changeableHttpBehaviour.OnHttpMessageHandlerCreated;

            changeableHttpBehaviour.OnHttpMessageHandlerCreated = httpMessageHandler =>
            {
                if (oldOnHttpMessageHandlerCreated != null)
                {
                    httpMessageHandler = oldOnHttpMessageHandlerCreated(httpMessageHandler);
                }
                return newOnHttpMessageHandlerCreated(httpMessageHandler);
            };
            return changeableHttpBehaviour;
        }

        /// <summary>
        ///     Chain the current OnHttpRequestMessageCreated function
        /// </summary>
        /// <param name="changeableHttpBehaviour">IChangeableHttpBehaviour</param>
        /// <param name="newOnHttpRequestMessageCreated">Function which accepts and returns HttpRequestMessage</param>
        /// <returns>IChangeableHttpBehaviour for fluent usage</returns>
        public static IChangeableHttpBehaviour ChainOnHttpRequestMessageCreated(this IChangeableHttpBehaviour changeableHttpBehaviour,
            Func<HttpRequestMessage, HttpRequestMessage> newOnHttpRequestMessageCreated)
        {
            var onHttpRequestMessageCreated = changeableHttpBehaviour.OnHttpRequestMessageCreated;

            changeableHttpBehaviour.OnHttpRequestMessageCreated = httpRequestMessage =>
            {
                if (onHttpRequestMessageCreated != null)
                {
                    httpRequestMessage = onHttpRequestMessageCreated(httpRequestMessage);
                }
                return newOnHttpRequestMessageCreated(httpRequestMessage);
            };
            return changeableHttpBehaviour;
        }
    }
}