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

#if NET461

#region Usings

using System.Net;

#endregion

namespace Dapplo.HttpExtensions.Factory
{
    /// <summary>
    ///     Creating a proxy is not very straightforward, that is why the logic is capsulated in the ProxyFactory.
    /// </summary>
    public static class WebProxyFactory
    {
        /// <summary>
        ///     Create a IWebProxy Object which can be used to access the Internet
        ///     This method will create a proxy according to the properties in the Settings class
        /// </summary>
        /// <returns>IWebProxy filled with all the proxy details or null if none is set/wanted</returns>
        public static IWebProxy Create()
        {
            var httpBehaviour = HttpBehaviour.Current;
            var httpSettings = httpBehaviour.HttpSettings ?? HttpExtensionsGlobals.HttpSettings;

            // This is already checked in the HttpClientFactory, but should be checked if this call is used elsewhere.
            if (!httpSettings.UseProxy)
            {
                return null;
            }
            var proxyToUse = httpSettings.UseDefaultProxy
                ? WebRequest.GetSystemWebProxy()
                : new WebProxy(httpSettings.ProxyUri, httpSettings.ProxyBypassOnLocal, httpSettings.ProxyBypassList);
            if (httpSettings.UseDefaultCredentialsForProxy)
            {
                if (proxyToUse is WebProxy)
                {
                    // Read note here: https://msdn.microsoft.com/en-us/library/system.net.webproxy.credentials.aspx
                    var webProxy = proxyToUse as WebProxy;
                    webProxy.UseDefaultCredentials = true;
                }
                else
                {
                    proxyToUse.Credentials = CredentialCache.DefaultCredentials;
                }
            }
            else
            {
                if (proxyToUse is WebProxy)
                {
                    // Read note here: https://msdn.microsoft.com/en-us/library/system.net.webproxy.credentials.aspx
                    var webProxy = proxyToUse as WebProxy;
                    webProxy.UseDefaultCredentials = false;
                    webProxy.Credentials = httpSettings.ProxyCredentials;
                }
                else
                {
                    proxyToUse.Credentials = httpSettings.ProxyCredentials;
                }
            }
            return proxyToUse;
        }
    }
}

#endif