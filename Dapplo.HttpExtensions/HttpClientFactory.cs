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

using System;
using System.Net.Http;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// Creating a HttpClient is not very straightforward, that is why the logic is capsulated in the HttpClientFactory.
	/// </summary>
	public static class HttpClientFactory
	{
		/// <summary>
		/// Create a HttpClient with default, in the HttpClientExtensions configured, settings
		/// </summary>
		/// <param name="suppliedHttpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <param name="uriForConfiguration">If a Uri is supplied, this is used to configure the HttpClient. Currently the Uri.UserInfo is used to set the basic authorization.</param>
		/// <returns>HttpClient</returns>
		public static HttpClient CreateHttpClient(IHttpSettings suppliedHttpSettings = null, Uri uriForConfiguration = null)
		{
			var httpSettings = suppliedHttpSettings ?? HttpSettings.GlobalHttpSettings;

			var client = new HttpClient(HttpMessageHandlerFactory.CreateWebRequestHandler(httpSettings))
			{
				Timeout = httpSettings.RequestTimeout,
				MaxResponseContentBufferSize = httpSettings.MaxResponseContentBufferSize
			};
			if (!string.IsNullOrEmpty(httpSettings.DefaultUserAgent))
			{
				client.DefaultRequestHeaders.UserAgent.TryParseAdd(httpSettings.DefaultUserAgent);
			}
			client.SetBasicAuthorization(uriForConfiguration);
			return client;
		}
	}
}
