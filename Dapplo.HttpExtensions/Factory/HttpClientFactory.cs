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

using System;
using System.Net.Http;

namespace Dapplo.HttpExtensions.Factory
{
	/// <summary>
	/// Creating a HttpClient is not very straightforward, that is why the logic is capsulated in the HttpClientFactory.
	/// </summary>
	public static class HttpClientFactory
	{
		/// <summary>
		/// Create a HttpClient which is modified by the settings specified in the IHttpSettings of the HttpBehaviour.
		/// If nothing is passed, the GlobalSettings are used
		/// </summary>
		/// <param name="httpBehaviour">HttpBehaviour instance or null if the global settings need to be used</param>
		/// <param name="uriForConfiguration">If a Uri is supplied, this is used to configure the HttpClient. Currently the Uri.UserInfo is used to set the basic authorization.</param>
		/// <returns>HttpClient</returns>
		public static HttpClient Create(IHttpBehaviour httpBehaviour = null, Uri uriForConfiguration = null)
		{
			httpBehaviour = httpBehaviour ?? new HttpBehaviour();
			var httpSettings = httpBehaviour.HttpSettings ?? HttpExtensionsGlobals.HttpSettings;

			var client = new HttpClient(HttpMessageHandlerFactory.Create(httpBehaviour))
			{
				Timeout = httpSettings.RequestTimeout,
				MaxResponseContentBufferSize = httpSettings.MaxResponseContentBufferSize
			};
			if (!string.IsNullOrEmpty(httpSettings.DefaultUserAgent))
			{
				client.DefaultRequestHeaders.UserAgent.TryParseAdd(httpSettings.DefaultUserAgent);
			}

			// If the uri has username/password, use this to set Basic Authorization
			client.SetBasicAuthorization(uriForConfiguration);

			// Allow the passed OnCreateHttpClient action to modify the HttpClient
			httpBehaviour.OnHttpClientCreated?.Invoke(client);
			return client;
		}
	}
}
