// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;
using System.Net.Http;
using System.Net.Security;
using Dapplo.Log;

#if NETFRAMEWORK
using System.Net.Cache;
#endif

namespace Dapplo.HttpExtensions.Factory
{
    /// <summary>
    ///     Creating a HttpMessageHandler is not very straightforward, that is why the logic is capsulated in the
    ///     HttpMessageHandlerFactory.
    /// </summary>
    public static class HttpMessageHandlerFactory
    {
        private static readonly LogSource Log = new();

        /// <summary>
        ///     This creates an advanced HttpMessageHandler, used in Apps
        /// </summary>
        /// <returns>HttpMessageHandler (HttpClientHandler)</returns>
        private static HttpMessageHandler CreateHandler()
        {
#if NETFRAMEWORK
            var httpClientHandler = new WebRequestHandler();
#else
            var httpClientHandler = new HttpClientHandler();
#endif
            var httpBehaviour = HttpBehaviour.Current;
            var httpSettings = httpBehaviour.HttpSettings ?? HttpExtensionsGlobals.HttpSettings;

#if NETFRAMEWORK
            httpClientHandler.AllowPipelining = httpSettings.AllowPipelining;
            httpClientHandler.AuthenticationLevel = httpSettings.AuthenticationLevel;
            httpClientHandler.ContinueTimeout = httpSettings.ContinueTimeout;
            httpClientHandler.ImpersonationLevel = httpSettings.ImpersonationLevel;
            httpClientHandler.ReadWriteTimeout = httpSettings.ReadWriteTimeout;
            httpClientHandler.CachePolicy = new RequestCachePolicy(httpSettings.RequestCacheLevel);
#else
            httpClientHandler.MaxConnectionsPerServer = httpSettings.MaxConnectionsPerServer;
#endif

            httpClientHandler.AutomaticDecompression = httpSettings.DefaultDecompressionMethods;
            httpClientHandler.AllowAutoRedirect = httpSettings.AllowAutoRedirect;
            httpClientHandler.AutomaticDecompression = httpSettings.DefaultDecompressionMethods;
            httpClientHandler.ClientCertificateOptions = httpSettings.ClientCertificateOptions;
            // Add certificates, if any
            if (httpSettings.ClientCertificates?.Count > 0)
            {
                httpClientHandler.ClientCertificates.AddRange(httpSettings.ClientCertificates);
            }
            httpClientHandler.Credentials = httpSettings.UseDefaultCredentials ? CredentialCache.DefaultCredentials : httpSettings.Credentials;
            httpClientHandler.MaxAutomaticRedirections = httpSettings.MaxAutomaticRedirections;
            httpClientHandler.MaxRequestContentBufferSize = httpSettings.MaxRequestContentBufferSize;
            httpClientHandler.MaxResponseHeadersLength = httpSettings.MaxResponseHeadersLength;
            httpClientHandler.UseCookies = httpSettings.UseCookies;
            httpClientHandler.CookieContainer = httpSettings.UseCookies ? httpBehaviour.CookieContainer : null;
            httpClientHandler.UseDefaultCredentials = httpSettings.UseDefaultCredentials;
            httpClientHandler.PreAuthenticate = httpSettings.PreAuthenticate;

#if !NETSTANDARD1_3
            httpClientHandler.Proxy = httpSettings.UseProxy ? WebProxyFactory.Create() : null;
#endif
            httpClientHandler.UseProxy = httpSettings.UseProxy;


            // Add logic to ignore the certificate
            if (httpSettings.IgnoreSslCertificateErrors)
            {
#if !NETFRAMEWORK
                httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
#else
                httpClientHandler.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
#endif
                {
                    if (sslPolicyErrors != SslPolicyErrors.None)
                    {
                        Log.Warn().WriteLine("Ssl policy error {0}", sslPolicyErrors);
                    }
                    return true;
                };
            }
            return httpClientHandler;
        }

            /// <summary>
            ///     This creates a HttpMessageHandler
            ///     Should be the preferred method to use to create a HttpMessageHandler
            /// </summary>
            /// <returns>HttpMessageHandler (WebRequestHandler)</returns>
            public static HttpMessageHandler Create()
        {
            var httpBehaviour = HttpBehaviour.Current;
            var baseMessageHandler = CreateHandler();
            if (httpBehaviour.OnHttpMessageHandlerCreated != null)
            {
                return httpBehaviour.OnHttpMessageHandlerCreated.Invoke(baseMessageHandler);
            }
            return baseMessageHandler;
        }
    }
}