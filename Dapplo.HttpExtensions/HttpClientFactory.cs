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
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>HttpClient</returns>
		public static HttpClient CreateHttpClient(IHttpSettings httpSettings = null)
		{
			var settings = httpSettings ?? HttpSettings.Instance;

			var client = new HttpClient(HttpMessageHandlerFactory.CreateWebRequestHandler(settings));
			client.Timeout = settings.RequestTimeout;
			client.MaxResponseContentBufferSize = settings.MaxResponseContentBufferSize;
            return client;
		}
	}
}
