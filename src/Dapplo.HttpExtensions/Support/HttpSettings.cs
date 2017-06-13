//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2015-2017 Dapplo
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

using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
#if NET45 ||NET46
using System.Net.Cache;
using System.Security.Principal;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

#endif

#endregion

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
                if (_userAgent == null)
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

#if NET45 || NET46

        /// <inheritdoc />
        public X509CertificateCollection ClientCertificates { get; set; } = new X509CertificateCollection();

        /// <inheritdoc />
        public int ReadWriteTimeout { get; set; } = 300000;

        /// <inheritdoc />
        public TokenImpersonationLevel ImpersonationLevel { get; set; } = TokenImpersonationLevel.Delegation;

        /// <inheritdoc />
        public int MaxResponseHeadersLength { get; set; } = 256;

        /// <inheritdoc />
        public long MaxRequestContentBufferSize { get; set; } = 2 * Gb - 1;

        /// <inheritdoc />
        public TimeSpan ContinueTimeout { get; set; } = TimeSpan.FromMilliseconds(350);

        /// <inheritdoc />
        public bool AllowPipelining { get; set; } = true;

        /// <inheritdoc />
        public AuthenticationLevel AuthenticationLevel { get; set; } = AuthenticationLevel.MutualAuthRequested;

        /// <inheritdoc />
        public bool UseProxy { get; set; } = true;

        /// <inheritdoc />
        public bool UseDefaultProxy { get; set; } = true;

        /// <inheritdoc />
        public bool UseDefaultCredentialsForProxy { get; set; } = true;

        /// <inheritdoc />
        public Uri ProxyUri { get; set; }

        /// <inheritdoc />
        public ICredentials ProxyCredentials { get; set; }

        /// <inheritdoc />
        public bool ProxyBypassOnLocal { get; set; } = true;

        /// <inheritdoc />
        public string[] ProxyBypassList { get; set; }

        /// <inheritdoc />
        public RequestCacheLevel RequestCacheLevel { get; set; } = RequestCacheLevel.Default;

        /// <inheritdoc />
        public bool IgnoreSslCertificateErrors { get; set; } = false;

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