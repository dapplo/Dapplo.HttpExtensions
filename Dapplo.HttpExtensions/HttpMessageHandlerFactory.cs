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
	along with Dapplo.HttpExtensions. If not, see <http://www.gnu.org/licenses/>.
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
		/// <param name="httpBehaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		public static void SetDefaults(HttpClientHandler httpClientHandler, IHttpBehaviour httpBehaviour = null)
		{
			httpBehaviour = httpBehaviour ?? HttpBehaviour.GlobalHttpBehaviour;
			var httpSettings = httpBehaviour.HttpSettings ?? HttpSettings.GlobalHttpSettings;

			httpClientHandler.AllowAutoRedirect = httpSettings.AllowAutoRedirect;
			httpClientHandler.AutomaticDecompression = httpSettings.DefaultDecompressionMethods;
			httpClientHandler.CookieContainer = httpSettings.UseCookies ? new CookieContainer() : null;
			httpClientHandler.Credentials = httpSettings.UseDefaultCredentials ? CredentialCache.DefaultCredentials : httpSettings.Credentials;
			httpClientHandler.MaxAutomaticRedirections = httpSettings.MaxAutomaticRedirections;
			httpClientHandler.MaxRequestContentBufferSize = httpSettings.MaxRequestContentBufferSize;
			httpClientHandler.UseCookies = httpSettings.UseCookies;
			httpClientHandler.UseDefaultCredentials = httpSettings.UseDefaultCredentials;
			httpClientHandler.Proxy = httpSettings.UseProxy ? WebProxyFactory.Create(httpBehaviour) : null;
			httpClientHandler.UseProxy = httpSettings.UseProxy;
			httpClientHandler.PreAuthenticate = httpSettings.PreAuthenticate;
		}

		/// <summary>
		/// Apply settings on the WebRequestHandler, this also calls the SetDefaults for the underlying HttpClientHandler
		/// </summary>
		/// <param name="webRequestHandler">WebRequestHandler to set the defaults to</param>
		/// <param name="httpBehaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		public static void SetDefaults(WebRequestHandler webRequestHandler, IHttpBehaviour httpBehaviour = null)
		{
			httpBehaviour = httpBehaviour ?? HttpBehaviour.GlobalHttpBehaviour;
			SetDefaults(webRequestHandler as HttpClientHandler, httpBehaviour);

			var httpSettings = httpBehaviour.HttpSettings ?? HttpSettings.GlobalHttpSettings;

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
		/// <param name="httpBehaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <returns>HttpMessageHandler (HttpClientHandler)</returns>
		public static HttpMessageHandler CreateHttpClientHandler(IHttpBehaviour httpBehaviour = null)
		{
			httpBehaviour = httpBehaviour ?? HttpBehaviour.GlobalHttpBehaviour;
			var httpClientHandler = new HttpClientHandler();
			SetDefaults(httpClientHandler, httpBehaviour);
			httpBehaviour.OnCreateHttpMessageHandler?.Invoke(httpClientHandler);
			return httpClientHandler;
		}

		/// <summary>
		/// This creates an advanced HttpMessageHandler, used in desktop applications
		/// Should be preferred
		/// </summary>
		/// <param name="httpBehaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <returns>HttpMessageHandler (WebRequestHandler)</returns>
		public static HttpMessageHandler CreateWebRequestHandler(IHttpBehaviour httpBehaviour = null)
		{
			httpBehaviour = httpBehaviour ?? HttpBehaviour.GlobalHttpBehaviour;
			var webRequestHandler = new WebRequestHandler();
			SetDefaults(webRequestHandler, httpBehaviour);
			httpBehaviour.OnCreateHttpMessageHandler?.Invoke(webRequestHandler);
			return webRequestHandler;
		}

		/// <summary>
		/// This creates a HttpMessageHandler
		/// Should be the preferred method to use to create a HttpMessageHandler
		/// </summary>
		/// <param name="httpBehaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <returns>HttpMessageHandler (WebRequestHandler)</returns>
		public static HttpMessageHandler Create(IHttpBehaviour httpBehaviour = null)
		{
			return CreateWebRequestHandler(httpBehaviour);
		}
	}
}
