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
using System.Net.Http;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// Creating a HttpMessageHandler is not very straightforward, that is why the logic is capsulated in the HttpMessageHandlerFactory.
	/// </summary>
	public static class HttpMessageHandlerFactory
	{
		/// <summary>
		/// Apply settings on the HttpClientHandler
		/// </summary>
		/// <param name="httpClientHandler"></param>
		public static void SetDefaults(HttpClientHandler httpClientHandler)
		{
			httpClientHandler.AllowAutoRedirect = HttpSettings.Instance.AllowAutoRedirect;
			httpClientHandler.AutomaticDecompression = HttpSettings.Instance.DefaultDecompressionMethods;
			httpClientHandler.CookieContainer = HttpSettings.Instance.UseCookies ? new CookieContainer() : null;
			httpClientHandler.Credentials = HttpSettings.Instance.UseDefaultCredentials ? CredentialCache.DefaultCredentials : null;
			httpClientHandler.MaxAutomaticRedirections = HttpSettings.Instance.MaxAutomaticRedirections;
			httpClientHandler.MaxRequestContentBufferSize = HttpSettings.Instance.MaxRequestContentBufferSize;
			httpClientHandler.UseCookies = HttpSettings.Instance.UseCookies;
			httpClientHandler.UseDefaultCredentials = HttpSettings.Instance.UseDefaultCredentials;
			httpClientHandler.Proxy = HttpSettings.Instance.UseProxy ? ProxyFactory.CreateProxy() : null;
			httpClientHandler.UseProxy = HttpSettings.Instance.UseProxy;
			httpClientHandler.PreAuthenticate = HttpSettings.Instance.PreAuthenticate;
        }

		/// <summary>
		/// Apply settings on the WebRequestHandler, this also calls the SetDefaults for the underlying HttpClientHandler
		/// </summary>
		/// <param name="webRequestHandler"></param>
		public static void SetDefaults(WebRequestHandler webRequestHandler)
		{
			SetDefaults(webRequestHandler as HttpClientHandler);
			webRequestHandler.AllowPipelining = HttpSettings.Instance.AllowPipelining;
            webRequestHandler.ReadWriteTimeout = HttpSettings.Instance.ReadWriteTimeout;
			webRequestHandler.AuthenticationLevel = HttpSettings.Instance.AuthenticationLevel;
			webRequestHandler.ContinueTimeout = HttpSettings.Instance.ContinueTimeout;
			webRequestHandler.ImpersonationLevel = HttpSettings.Instance.ImpersonationLevel;
			webRequestHandler.MaxResponseHeadersLength = HttpSettings.Instance.MaxResponseHeadersLength;
        }

		/// <summary>
		/// This creates an HttpClientHandler, normally one should use CreateWebRequestHandler
		/// </summary>
		/// <returns>HttpMessageHandler (HttpClientHandler)</returns>
		public static HttpMessageHandler CreateHttpClientHandler()
		{
			var handler = new HttpClientHandler();
			SetDefaults(handler);
			return handler;
        }

		/// <summary>
		/// This creates an advanced HttpMessageHandler, used in desktop applications
		/// Should be preferred
		/// </summary>
		/// <returns>HttpMessageHandler (WebRequestHandler)</returns>
		public static HttpMessageHandler CreateWebRequestHandler()
		{
			var handler = new WebRequestHandler();
			SetDefaults(handler);
			return handler;
		}
	}
}
