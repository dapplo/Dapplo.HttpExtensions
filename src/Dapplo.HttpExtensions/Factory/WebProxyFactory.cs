// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

            var proxyToUse = httpSettings.UseDefaultProxy ?
#if NET10_0 || NET8_0
                HttpClient.DefaultProxy
#else
                WebRequest.GetSystemWebProxy()
#endif
                : new WebProxy(httpSettings.ProxyUri, httpSettings.ProxyBypassOnLocal, httpSettings.ProxyBypassList);
            if (httpSettings.UseDefaultCredentialsForProxy)
            {
                if (proxyToUse is WebProxy webProxy)
                {
                    // Read note here: https://msdn.microsoft.com/en-us/library/system.net.webproxy.credentials.aspx
                    webProxy.UseDefaultCredentials = true;
                }
                else
                {
#if NET10_0 || NET8_0
                    if (!httpSettings.UseDefaultProxy)
                    {
                        proxyToUse.Credentials = CredentialCache.DefaultCredentials;
                    }
#else
                    proxyToUse.Credentials = CredentialCache.DefaultCredentials;
#endif
                }
            }
            else
            {
                if (proxyToUse is WebProxy webProxy)
                {
                    // Read note here: https://msdn.microsoft.com/en-us/library/system.net.webproxy.credentials.aspx
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