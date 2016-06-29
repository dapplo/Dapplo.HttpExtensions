//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2015-2016 Dapplo
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

#region using

using System.Net;
using System.Net.Http;

#endregion

namespace Dapplo.HttpExtensions.Factory
{
	/// <summary>
	///     Creating a HttpMessageHandler is not very straightforward, that is why the logic is capsulated in the
	///     HttpMessageHandlerFactory.
	/// </summary>
	public static partial class HttpMessageHandlerFactory
	{
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

		/// <summary>
		///     This creates an HttpClientHandler, normally one should use CreateWebRequestHandler
		///     But this might be needed for Apps
		/// </summary>
		/// <returns>HttpMessageHandler (HttpClientHandler)</returns>
		// ReSharper disable once UnusedMember.Local
		private static HttpMessageHandler CreateHttpClientHandler()
		{
			var httpClientHandler = new HttpClientHandler();
			SetDefaults(httpClientHandler);
			return httpClientHandler;
		}

		/// <summary>
		///     Apply settings on the HttpClientHandler
		/// </summary>
		/// <param name="httpClientHandler"></param>
		private static void SetDefaults(HttpClientHandler httpClientHandler)
		{
			var httpBehaviour = HttpBehaviour.Current;
			var httpSettings = httpBehaviour.HttpSettings ?? HttpExtensionsGlobals.HttpSettings;

			httpClientHandler.AllowAutoRedirect = httpSettings.AllowAutoRedirect;
			httpClientHandler.AutomaticDecompression = httpSettings.DefaultDecompressionMethods;
			httpClientHandler.CookieContainer = httpSettings.UseCookies ? httpBehaviour.CookieContainer : null;
			httpClientHandler.Credentials = httpSettings.UseDefaultCredentials ? CredentialCache.DefaultCredentials : httpSettings.Credentials;
			httpClientHandler.MaxAutomaticRedirections = httpSettings.MaxAutomaticRedirections;

#if !_PCL_
			httpClientHandler.MaxRequestContentBufferSize = httpSettings.MaxRequestContentBufferSize;

			if (!httpSettings.UseProxy)
			{
				httpClientHandler.Proxy = null;
			}
			httpClientHandler.UseProxy = httpSettings.UseProxy;
#endif

			httpClientHandler.UseCookies = httpSettings.UseCookies;
			httpClientHandler.UseDefaultCredentials = httpSettings.UseDefaultCredentials;
			httpClientHandler.PreAuthenticate = httpSettings.PreAuthenticate;
		}
	}
}