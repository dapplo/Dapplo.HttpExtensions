/*
 * dapplo - building blocks for desktop applications
 * Copyright (C) 2015 Robin Krom
 * 
 * For more information see: http://dapplo.net/
 * dapplo repositories are hosted on GitHub: https://github.com/dapplo
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */

using System.Net;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// Creating a proxy is not very straightforward, that is why the logic is capsulated in the ProxyFactory.
	/// </summary>
	public static class ProxyFactory
	{
		/// <summary>
		/// Create a IWebProxy Object which can be used to access the Internet
		/// This method will create a proxy according to the properties in the Settings class
		/// </summary>
		/// <returns>IWebProxy filled with all the proxy details or null if none is set/wanted</returns>
		public static IWebProxy CreateProxy()
		{
			// This is already checked in the HttpClientFactory, but should be checked if this call is used elsewhere.
			if (!HttpSettings.Instance.UseProxy)
			{
				return null;
			}
			IWebProxy proxyToUse;
			if (HttpSettings.Instance.UseDefaultProxy)
			{
				proxyToUse = WebRequest.GetSystemWebProxy();
			}
			else
			{
				if (HttpSettings.Instance.ProxyBypassList != null)
				{
					proxyToUse = new WebProxy(HttpSettings.Instance.ProxyUri, HttpSettings.Instance.ProxyBypassOnLocal, HttpSettings.Instance.ProxyBypassList);
				}
				else
				{
					proxyToUse = new WebProxy(HttpSettings.Instance.ProxyUri, HttpSettings.Instance.ProxyBypassOnLocal);
				}
			}

			if (proxyToUse != null)
			{
				if (HttpSettings.Instance.UseDefaultCredentialsForProy)
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
						webProxy.Credentials = HttpSettings.Instance.ProxyCredentials;
					}
					else
					{
						proxyToUse.Credentials = HttpSettings.Instance.ProxyCredentials;
					}
				}
			}
			return proxyToUse;
		}
	}
}
