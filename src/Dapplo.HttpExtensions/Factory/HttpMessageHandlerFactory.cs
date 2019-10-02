//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2019 Dapplo
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

using System.Net;
using System.Net.Http;
using System.Net.Security;
using Dapplo.Log;

#if NET461
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
        private static readonly LogSource Log = new LogSource();

        /// <summary>
        ///     This creates an advanced HttpMessageHandler, used in Apps
        /// </summary>
        /// <returns>HttpMessageHandler (HttpClientHandler)</returns>
        private static HttpMessageHandler CreateHandler()
        {
#if NET461
            var httpClientHandler = new WebRequestHandler();
#else
            var httpClientHandler = new HttpClientHandler();
#endif
            var httpBehaviour = HttpBehaviour.Current;
            var httpSettings = httpBehaviour.HttpSettings ?? HttpExtensionsGlobals.HttpSettings;

#if NET461
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
#if !NET461
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