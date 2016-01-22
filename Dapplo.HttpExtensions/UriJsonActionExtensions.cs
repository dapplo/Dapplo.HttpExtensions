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
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// Uri extension which perform an action where JSON is involved
	/// </summary>
	public static class UriJsonActionExtensions
	{
		/// <summary>
		/// Create a behaviour which makes sure that we add "application/json" to the accepting
		/// </summary>
		/// <param name="httpBehaviour"></param>
		/// <returns>HttpBehaviour which is a clone of the original or GlobalHttpBehaviour</returns>
		private static IHttpBehaviour CreateJsonAcceptingHttpBehaviour(IHttpBehaviour httpBehaviour = null)
		{
			httpBehaviour = httpBehaviour ?? HttpBehaviour.GlobalHttpBehaviour;
			httpBehaviour = httpBehaviour.Clone();

			// Store the OnCreateHttpClient, so we can wrap the functionality
			Action<HttpClient> previousOnCreateHttpClient = httpBehaviour.OnCreateHttpClient;

			httpBehaviour.OnCreateHttpClient = httpClient => {
				// Wrap existing OnCreateHttpClient (if any)
				previousOnCreateHttpClient?.Invoke(httpClient);
				httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypes.Json.EnumValueOf()));
			};
			return httpBehaviour;
		}

		/// <summary>
		/// Method to post JSON
		/// </summary>
		/// <typeparam name="TContent">Type to post</typeparam>
		/// <param name="uri">Uri to post json to</param>
		/// <param name="jsonContent">T</param>
		/// <param name="httpBehaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>HttpResponseMessage</returns>
		public static async Task<HttpResponseMessage> PostJsonAsync<TContent>(this Uri uri, TContent jsonContent, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken))
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}
			using (var client = HttpClientFactory.Create(httpBehaviour, uri))
			{
				return await client.PostJsonAsync(uri, jsonContent, httpBehaviour, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Method to post with JSON, and get JSON
		/// </summary>
		/// <typeparam name="TContent">Type to post</typeparam>
		/// <typeparam name="TResult">Type to read from the response</typeparam>
		/// <param name="uri">Uri to post to</param>
		/// <param name="jsonContent">T1</param>
		/// <param name="httpBehaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>TResult</returns>
		public static async Task<TResult> PostJsonAsync<TContent, TResult>(this Uri uri, TContent jsonContent, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken)) where TResult : class
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}
			httpBehaviour = CreateJsonAcceptingHttpBehaviour(httpBehaviour);
			using (var client = HttpClientFactory.Create(httpBehaviour, uri))
			{
				return await client.PostJsonAsync<TContent, TResult>(uri, jsonContent, httpBehaviour, token).ConfigureAwait(false);
			}
		}
	}
}
