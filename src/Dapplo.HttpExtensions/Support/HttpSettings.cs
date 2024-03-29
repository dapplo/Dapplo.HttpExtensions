﻿// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
#if NETFRAMEWORK
using System.Net.Cache;
using System.Security.Principal;
#endif

namespace Dapplo.HttpExtensions.Support
{
    /// <summary>
    ///     This class contains the default settings for the proxy / httpclient
    ///     These can be modified, are on a global "application" scale.
    ///     Most have their normal defaults, which would also normally be used, some have special settings
    ///     The default values and the property descriptions are in the IHttpSettings (which can be used by Dapplo.Config)
    /// </summary>
    public class HttpSettings : IHttpSettings
    {
        private const int Kb = 1024;
        private const int Mb = Kb * 1024;
        private const long Gb = Mb * 1024;

        private string _userAgent;

        /// <inheritdoc />
        public bool UseCookies { get; set; } = true;

        /// <inheritdoc />
        public bool UseDefaultCredentials { get; set; } = true;

        /// <inheritdoc />
        public ICredentials Credentials { get; set; }

        /// <inheritdoc />
        public ClientCertificateOption ClientCertificateOptions { get; set; }

        /// <inheritdoc />
        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(60);

        /// <inheritdoc />
        public bool AllowAutoRedirect { get; set; } = true;

        /// <inheritdoc />
        public DecompressionMethods DefaultDecompressionMethods { get; set; } = DecompressionMethods.GZip | DecompressionMethods.Deflate;

        /// <inheritdoc />
        public bool PreAuthenticate { get; set; } = true;

        /// <inheritdoc />
        public int MaxAutomaticRedirections { get; set; } = 50;

        /// <inheritdoc />
        public long MaxResponseContentBufferSize { get; set; } = 2 * Gb - 1;

        /// <inheritdoc />
        public string DefaultUserAgent
        {
            get
            {
                if (_userAgent is null)
                {
                    var clientAssembly = typeof(HttpSettings).GetTypeInfo().Assembly;
                    var userAgentBuilder = new StringBuilder();

                    var clientAssemblyName = clientAssembly.GetName();
                    userAgentBuilder.Append($"{clientAssemblyName.Name}/{clientAssemblyName.Version} ");
                    _userAgent = userAgentBuilder.ToString().Trim();
                }
                return _userAgent;
            }
            set { _userAgent = value; }
        }

        /// <inheritdoc />
        public X509CertificateCollection ClientCertificates { get; set; } = new X509CertificateCollection();

        /// <inheritdoc />
        public int MaxResponseHeadersLength { get; set; } = 256;

        /// <inheritdoc />
        public long MaxRequestContentBufferSize { get; set; } = 2 * Gb - 1;

        /// <inheritdoc />
        public bool UseProxy { get; set; } = true;

        /// <inheritdoc />
        public bool UseDefaultProxy { get; set; } = true;

        /// <inheritdoc />
        public bool UseDefaultCredentialsForProxy { get; set; } = true;

        /// <inheritdoc />
        public AuthenticationLevel AuthenticationLevel { get; set; } = AuthenticationLevel.MutualAuthRequested;

        /// <inheritdoc />
        public bool IgnoreSslCertificateErrors { get; set; } = false;

#if NETSTANDARD1_3 || NETSTANDARD2_0 || NETCOREAPP3_1 || NET6_0
        /// <inheritdoc />
        public int MaxConnectionsPerServer { get; set; } = int.MaxValue;
#endif

#if NETFRAMEWORK || NETSTANDARD2_0 || NETCOREAPP3_1 || NET6_0

        /// <inheritdoc />
        public Uri ProxyUri { get; set; }
    
        /// <inheritdoc />
        public bool ProxyBypassOnLocal { get; set; } = true;

        /// <inheritdoc />
        public string[] ProxyBypassList { get; set; }

        /// <inheritdoc />
        public ICredentials ProxyCredentials { get; set; }
#endif

#if NETFRAMEWORK

        /// <inheritdoc />
        public int ReadWriteTimeout { get; set; } = 300000;

        /// <inheritdoc />
        public TokenImpersonationLevel ImpersonationLevel { get; set; } = TokenImpersonationLevel.Delegation;


        /// <inheritdoc />
        public TimeSpan ContinueTimeout { get; set; } = TimeSpan.FromMilliseconds(350);

        /// <inheritdoc />
        public bool AllowPipelining { get; set; } = true;

        /// <inheritdoc />
        public RequestCacheLevel RequestCacheLevel { get; set; } = RequestCacheLevel.Default;


#endif

        /// <inheritdoc />
        public bool Expect100Continue { get; set; } = false;

        /// <summary>
        ///     Return a memberwise clone of the HttpSettings.
        ///     This is needed by the HttpBehaviour to prevent that a modification of a copy is changing the global settings!
        /// </summary>
        /// <returns></returns>
        public IHttpSettings ShallowClone()
        {
            return (IHttpSettings) MemberwiseClone();
        }
    }
}