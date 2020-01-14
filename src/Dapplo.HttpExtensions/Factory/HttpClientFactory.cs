// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Net.Http;

namespace Dapplo.HttpExtensions.Factory
{
    /// <summary>
    ///     Creating a HttpClient is not very straightforward, that is why the logic is capsulated in the HttpClientFactory.
    /// </summary>
    public static class HttpClientFactory
    {
        /// <summary>
        ///     Create a HttpClient which is modified by the settings specified in the IHttpSettings of the HttpBehaviour.
        ///     If nothing is passed, the GlobalSettings are used
        /// </summary>
        /// <param name="uriForConfiguration">
        ///     If a Uri is supplied, this is used to configure the HttpClient. Currently the
        ///     Uri.UserInfo is used to set the basic authorization.
        /// </param>
        /// <returns>HttpClient</returns>
        public static HttpClient Create(Uri uriForConfiguration = null)
        {
            var httpBehaviour = HttpBehaviour.Current;
            var httpSettings = httpBehaviour.HttpSettings ?? HttpExtensionsGlobals.HttpSettings;

            var httpClient = new HttpClient(HttpMessageHandlerFactory.Create())
            {
                Timeout = httpSettings.RequestTimeout,
                MaxResponseContentBufferSize = httpSettings.MaxResponseContentBufferSize
            };
            if (!string.IsNullOrEmpty(httpSettings.DefaultUserAgent))
            {
                httpClient.DefaultRequestHeaders.UserAgent.TryParseAdd(httpSettings.DefaultUserAgent);
            }
            // If the uri has username/password, use this to set Basic Authorization
            if (uriForConfiguration != null)
            {
                httpClient.SetBasicAuthorization(uriForConfiguration);
            }

            // Copy the expect continue value
            httpClient.DefaultRequestHeaders.ExpectContinue = httpSettings.Expect100Continue;

            // Allow the passed OnCreateHttpClient action to modify the HttpClient
            httpBehaviour.OnHttpClientCreated?.Invoke(httpClient);
            return httpClient;
        }
    }
}