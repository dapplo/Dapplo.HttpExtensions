/*
	Dapplo - building blocks for desktop applications
	Copyright (C) 2015-2016 Dapplo

	For more information see: http://dapplo.net/
	Dapplo repositories are hosted on GitHub: https://github.com/dapplo

	This file is part of Dapplo.HttpExtensions.

	Dapplo.HttpExtensions is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	Dapplo.HttpExtensions is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
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
		/// <param name="suppliedHttpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		public static void SetDefaults(HttpClientHandler httpClientHandler, IHttpSettings suppliedHttpSettings = null)
		{
			var httpSettings = suppliedHttpSettings ?? HttpSettings.Instance;

			httpClientHandler.AllowAutoRedirect = httpSettings.AllowAutoRedirect;
			httpClientHandler.AutomaticDecompression = httpSettings.DefaultDecompressionMethods;
			httpClientHandler.CookieContainer = httpSettings.UseCookies ? new CookieContainer() : null;
			httpClientHandler.Credentials = httpSettings.UseDefaultCredentials ? CredentialCache.DefaultCredentials : httpSettings.Credentials;
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
		/// <param name="suppliedHttpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		public static void SetDefaults(WebRequestHandler webRequestHandler, IHttpSettings suppliedHttpSettings = null)
		{
			var httpSettings = suppliedHttpSettings ?? HttpSettings.Instance;

			SetDefaults(webRequestHandler as HttpClientHandler, httpSettings);

			webRequestHandler.AllowPipelining = httpSettings.AllowPipelining;
            webRequestHandler.ReadWriteTimeout = httpSettings.ReadWriteTimeout;
			webRequestHandler.AuthenticationLevel = httpSettings.AuthenticationLevel;
			webRequestHandler.ContinueTimeout = httpSettings.ContinueTimeout;
			webRequestHandler.ImpersonationLevel = httpSettings.ImpersonationLevel;
			webRequestHandler.MaxResponseHeadersLength = httpSettings.MaxResponseHeadersLength;
		}

		/// <summary>
		/// This creates an HttpClientHandler, normally one should use CreateWebRequestHandler
		/// </summary>
		/// <param name="suppliedHttpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>HttpMessageHandler (HttpClientHandler)</returns>
		public static HttpMessageHandler CreateHttpClientHandler(IHttpSettings suppliedHttpSettings = null)
		{
			var httpSettings = suppliedHttpSettings ?? HttpSettings.Instance;
			var handler = new HttpClientHandler();
			SetDefaults(handler, httpSettings);
			return handler;
		}

		/// <summary>
		/// This creates an advanced HttpMessageHandler, used in desktop applications
		/// Should be preferred
		/// </summary>
		/// <param name="suppliedHttpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>HttpMessageHandler (WebRequestHandler)</returns>
		public static HttpMessageHandler CreateWebRequestHandler(IHttpSettings suppliedHttpSettings = null)
		{
			var httpSettings = suppliedHttpSettings ?? HttpSettings.Instance;
			var handler = new WebRequestHandler();
			SetDefaults(handler, httpSettings);
			return handler;
		}
	}
}
