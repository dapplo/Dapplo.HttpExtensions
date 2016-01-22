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
using System.Threading;
using System.Threading.Tasks;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// Extensions for the HttpClient class which handle Json
	/// These are explicitly for JSON, but could be ignored if you use ReadAsSync which does the same
	/// </summary>
	public static class HttpClientJsonExtensions
	{
		/// <summary>
		/// Method to post with JSON
		/// </summary>
		/// <param name="client">HttpClient</param>
		/// <param name="uri">Uri to post the json to</param>
		/// <param name="postData">data to post</param>
		/// <param name="httpBehaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>HttpResponseMessage</returns>
		public static async Task<HttpResponseMessage> PostJsonAsync<TContent>(this HttpClient client, Uri uri, TContent postData, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken))
		{
			httpBehaviour = httpBehaviour ?? HttpBehaviour.GlobalHttpBehaviour;

			var jsonStringContent = httpBehaviour.JsonSerializer.SerializeJson(postData);
			using (var content = new StringContent(jsonStringContent, httpBehaviour.DefaultEncoding, MediaTypes.Json.EnumValueOf()))
			{
				return await client.PostAsync(uri, content, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Method to post with JSON, and get JSON
		/// </summary>
		/// <typeparam name="T1">Type to post</typeparam>
		/// <typeparam name="T2">Type to read from the response</typeparam>
		/// <param name="client">HttpClient</param>
		/// <param name="uri">Uri to post the json to</param>
		/// <param name="postData">data to post</param>
		/// <param name="httpBehaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>T2</returns>
		public static async Task<T2> PostJsonAsync<T1, T2>(this HttpClient client, Uri uri, T1 postData, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken)) where T2 : class
		{
			httpBehaviour = httpBehaviour ?? HttpBehaviour.GlobalHttpBehaviour;

			var jsonStringContent = httpBehaviour.JsonSerializer.SerializeJson(postData);

			using (var content = new StringContent(jsonStringContent, httpBehaviour.DefaultEncoding, MediaTypes.Json.EnumValueOf()))
			{
				var response = await client.PostAsync(uri, content, token).ConfigureAwait(false);
				return await response.ReadAsAsync<T2>(httpBehaviour, token).ConfigureAwait(false);
			}
		}
	}
}
