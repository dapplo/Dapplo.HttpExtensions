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
		/// Download a Json response
		/// </summary>
		/// <param name="uri">An Uri to specify the download location</param>
		/// <param name="throwErrorOnNonSuccess"></param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>dynamic created with SimpleJson</returns>
		public static async Task<dynamic> GetAsJsonAsync(this Uri uri, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			using (var reponse = await uri.GetAsync(token, httpSettings).ConfigureAwait(false))
			{
				return await reponse.GetAsJsonAsync(throwErrorOnNonSuccess, token).ConfigureAwait(false);
            }
		}

		/// <summary>
		/// Get a response as json
		/// </summary>
		/// <typeparam name="T">Type to deserialize into</typeparam>
		/// <param name="uri">An Uri to specify the download location</param>
		/// <param name="throwErrorOnNonSuccess"></param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>T created with SimpleJson</returns>
		public static async Task<T> GetAsJsonAsync<T>(this Uri uri, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			using (var reponse = await uri.GetAsync(token, httpSettings).ConfigureAwait(false))
			{
				return await reponse.GetAsJsonAsync<T>(throwErrorOnNonSuccess, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Get a response as json
		/// The response is parsed depending on the HttpStatusCode:
		///  TNormal is used when Ok, the TError in the other cases.
		/// </summary>
		/// <typeparam name="TNormal">Type to deserialize into if the response don't have an error</typeparam>
		/// <typeparam name="TError">Type to deserialize into if the response has an error</typeparam>
		/// <param name="uri">An Uri to specify the download location</param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>HttpResponse of TNormal and TError filled by SimpleJson</returns>
		public static async Task<HttpResponse<TNormal, TError>> GetAsJsonAsync<TNormal, TError>(this Uri uri, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			using (var reponse = await uri.GetAsync(token, httpSettings).ConfigureAwait(false))
			{
				return await reponse.GetAsJsonAsync<TNormal, TError>(token).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Method to post JSON
		/// </summary>
		/// <typeparam name="T">Type to post</typeparam>
		/// <param name="uri">Uri to post json to</param>
		/// <param name="jsonContent">T</param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>HttpResponseMessage</returns>
		public static async Task<HttpResponseMessage> PostJsonAsync<T>(this Uri uri, T jsonContent, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}
			using (var client = HttpClientFactory.CreateHttpClient(httpSettings, uri))
			{
				return await client.PostJsonAsync(uri, jsonContent, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Method to post with JSON, and get JSON
		/// </summary>
		/// <typeparam name="T1">Type to post</typeparam>
		/// <typeparam name="T2">Type to read from the response</typeparam>
		/// <param name="uri">Uri to post to</param>
		/// <param name="jsonContent">T1</param>
		/// <param name="throwErrorOnNonSuccess">true to throw an exception when an error occurse, else null is returned</param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>T2</returns>
		public static async Task<T2> PostJsonAsync<T1, T2>(this Uri uri, T1 jsonContent, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}
			using (var client = HttpClientFactory.CreateHttpClient(httpSettings, uri))
			{
				return await client.PostJsonAsync<T1, T2>(uri, jsonContent, throwErrorOnNonSuccess, token).ConfigureAwait(false);
			}
		}
	}
}
