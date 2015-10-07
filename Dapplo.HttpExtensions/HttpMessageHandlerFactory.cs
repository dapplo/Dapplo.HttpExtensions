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
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		public static void SetDefaults(HttpClientHandler httpClientHandler, IHttpSettings httpSettings = null)
		{
			var settings = httpSettings ?? HttpSettings.Instance;

			httpClientHandler.AllowAutoRedirect = httpSettings.AllowAutoRedirect;
			httpClientHandler.AutomaticDecompression = httpSettings.DefaultDecompressionMethods;
			httpClientHandler.CookieContainer = httpSettings.UseCookies ? new CookieContainer() : null;
			httpClientHandler.Credentials = httpSettings.UseDefaultCredentials ? CredentialCache.DefaultCredentials : null;
			httpClientHandler.MaxAutomaticRedirections = httpSettings.MaxAutomaticRedirections;
			httpClientHandler.MaxRequestContentBufferSize = httpSettings.MaxRequestContentBufferSize;
			httpClientHandler.UseCookies = httpSettings.UseCookies;
			httpClientHandler.UseDefaultCredentials = httpSettings.UseDefaultCredentials;
			httpClientHandler.Proxy = httpSettings.UseProxy ? ProxyFactory.CreateProxy(httpSettings) : null;
			httpClientHandler.UseProxy = httpSettings.UseProxy;
			httpClientHandler.PreAuthenticate = httpSettings.PreAuthenticate;
        }

		/// <summary>
		/// Apply settings on the WebRequestHandler, this also calls the SetDefaults for the underlying HttpClientHandler
		/// </summary>
		/// <param name="webRequestHandler"></param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		public static void SetDefaults(WebRequestHandler webRequestHandler, IHttpSettings httpSettings = null)
		{
			var settings = httpSettings ?? HttpSettings.Instance;

			SetDefaults(webRequestHandler as HttpClientHandler, settings);

			webRequestHandler.AllowPipelining = settings.AllowPipelining;
            webRequestHandler.ReadWriteTimeout = settings.ReadWriteTimeout;
			webRequestHandler.AuthenticationLevel = settings.AuthenticationLevel;
			webRequestHandler.ContinueTimeout = settings.ContinueTimeout;
			webRequestHandler.ImpersonationLevel = settings.ImpersonationLevel;
			webRequestHandler.MaxResponseHeadersLength = settings.MaxResponseHeadersLength;
        }

		/// <summary>
		/// This creates an HttpClientHandler, normally one should use CreateWebRequestHandler
		/// </summary>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>HttpMessageHandler (HttpClientHandler)</returns>
		public static HttpMessageHandler CreateHttpClientHandler(IHttpSettings httpSettings = null)
		{
			var settings = httpSettings ?? HttpSettings.Instance;
			var handler = new HttpClientHandler();
			SetDefaults(handler, settings);
			return handler;
        }

		/// <summary>
		/// This creates an advanced HttpMessageHandler, used in desktop applications
		/// Should be preferred
		/// </summary>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>HttpMessageHandler (WebRequestHandler)</returns>
		public static HttpMessageHandler CreateWebRequestHandler(IHttpSettings httpSettings = null)
		{
			var settings = httpSettings ?? HttpSettings.Instance;
			var handler = new WebRequestHandler();
			SetDefaults(handler, settings);
			return handler;
		}
	}
}
