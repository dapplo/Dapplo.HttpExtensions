//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2018 Dapplo
// 
//  For more information see: http://dapplo.net/
//  Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
//  This file is part of Dapplo.HttpExtensions
// 
//  Dapplo.HttpExtensions is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  Dapplo.HttpExtensions is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have a copy of the GNU Lesser General Public License
//  along with Dapplo.HttpExtensions. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

#region Usings

using System.Net;
using System.Net.Http;
using System.Net.Security;
using Dapplo.Log;

#if !NETSTANDARD1_3
using System.Net.Cache;
#endif

#endregion

namespace Dapplo.HttpExtensions.Factory
{
    /// <summary>
    ///     Creating a HttpMessageHandler is not very straightforward, that is why the logic is capsulated in the
    ///     HttpMessageHandlerFactory.
    /// </summary>
    public static class HttpMessageHandlerFactory
    {
        private static readonly LogSource Log = new LogSource();
#if NET461

        /// <summary>
        ///     This creates an advanced HttpMessageHandler, used in desktop applications
        ///     Should be preferred
        /// </summary>
        /// <returns>HttpMessageHandler (WebRequestHandler)</returns>
        private static HttpMessageHandler CreateHandler()
        {
            var webRequestHandler = new WebRequestHandler();

            var httpBehaviour = HttpBehaviour.Current;
            var httpSettings = httpBehaviour.HttpSettings ?? HttpExtensionsGlobals.HttpSettings;

            webRequestHandler.AllowPipelining = httpSettings.AllowPipelining;
            webRequestHandler.AuthenticationLevel = httpSettings.AuthenticationLevel;
            webRequestHandler.CachePolicy = new RequestCachePolicy(httpSettings.RequestCacheLevel);
            webRequestHandler.ClientCertificateOptions = httpSettings.ClientCertificateOptions;
            // Add certificates, if any
            if (httpSettings.ClientCertificates?.Count > 0)
            {
                webRequestHandler.ClientCertificates.AddRange(httpSettings.ClientCertificates);
            }
            webRequestHandler.ContinueTimeout = httpSettings.ContinueTimeout;
            webRequestHandler.ImpersonationLevel = httpSettings.ImpersonationLevel;
            webRequestHandler.MaxResponseHeadersLength = httpSettings.MaxResponseHeadersLength;

            webRequestHandler.Proxy = httpSettings.UseProxy ? WebProxyFactory.Create() : null;
            webRequestHandler.ReadWriteTimeout = httpSettings.ReadWriteTimeout;

            // Add logic to ignore the certificate
            if (httpSettings.IgnoreSslCertificateErrors)
            {
                webRequestHandler.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                {
                    if (sslPolicyErrors != SslPolicyErrors.None)
                    {
                        Log.Warn().WriteLine("Ssl policy error {0}", sslPolicyErrors);
                    }
                    return true;
                };
            }
            return webRequestHandler;
        }
#else
        /// <summary>
        ///     This creates an advanced HttpMessageHandler, used in Apps
        /// </summary>
        /// <returns>HttpMessageHandler (HttpClientHandler)</returns>
        private static HttpMessageHandler CreateHandler()
        {
            var httpClientHandler = new HttpClientHandler();

            var httpBehaviour = HttpBehaviour.Current;
            var httpSettings = httpBehaviour.HttpSettings ?? HttpExtensionsGlobals.HttpSettings;

            httpClientHandler.AllowAutoRedirect = httpSettings.AllowAutoRedirect;
            httpClientHandler.AutomaticDecompression = httpSettings.DefaultDecompressionMethods;
            httpClientHandler.ClientCertificateOptions = httpSettings.ClientCertificateOptions;
            // Add certificates, if any
            if (httpSettings.ClientCertificates?.Count > 0)
            {
                httpClientHandler.ClientCertificates.AddRange(httpSettings.ClientCertificates);
            }
            httpClientHandler.CookieContainer = httpSettings.UseCookies ? httpBehaviour.CookieContainer : null;
            httpClientHandler.Credentials = httpSettings.UseDefaultCredentials ? CredentialCache.DefaultCredentials : httpSettings.Credentials;
            httpClientHandler.MaxAutomaticRedirections = httpSettings.MaxAutomaticRedirections;
            httpClientHandler.MaxConnectionsPerServer = httpSettings.MaxConnectionsPerServer;

            httpClientHandler.MaxRequestContentBufferSize = httpSettings.MaxRequestContentBufferSize;
            httpClientHandler.MaxResponseHeadersLength = httpSettings.MaxResponseHeadersLength;
#if !NETSTANDARD1_3
            httpClientHandler.Proxy = httpSettings.UseProxy ? WebProxyFactory.Create() : null;
#endif
            httpClientHandler.UseProxy = httpSettings.UseProxy;

            httpClientHandler.UseCookies = httpSettings.UseCookies;
            httpClientHandler.UseDefaultCredentials = httpSettings.UseDefaultCredentials;
            httpClientHandler.PreAuthenticate = httpSettings.PreAuthenticate;
            // Add logic to ignore the certificate
            if (httpSettings.IgnoreSslCertificateErrors)
            {
                httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
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
#endif

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