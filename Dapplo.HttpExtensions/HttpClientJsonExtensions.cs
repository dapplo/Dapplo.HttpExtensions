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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// Extensions for the HttpClient class which handle Json
	/// </summary>
	public static class HttpClientJsonExtensions
	{
		/// <summary>
		/// Method to post with JSON
		/// </summary>
		/// <param name="client">HttpClient</param>
		/// <param name="uri">Uri to post the json to</param>
		/// <param name="postData">data to post</param>
		/// <param name="behaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>HttpResponseMessage</returns>
		public static async Task<HttpResponseMessage> PostJsonAsync<TContent>(this HttpClient client, Uri uri, TContent postData, HttpBehaviour behaviour = null, CancellationToken token = default(CancellationToken))
		{
			using (var content = new StringContent(SimpleJson.SerializeObject(postData), Encoding.UTF8, "application/json"))
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
		/// <param name="behaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>T2</returns>
		public static async Task<T2> PostJsonAsync<T1, T2>(this HttpClient client, Uri uri, T1 postData, HttpBehaviour behaviour = null, CancellationToken token = default(CancellationToken))
		{
			using (var content = new StringContent(SimpleJson.SerializeObject(postData), Encoding.UTF8, "application/json"))
			{
				var response = await client.PostAsync(uri, content, token).ConfigureAwait(false);
				return await response.GetAsJsonAsync<T2>(behaviour, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Get the content as JSON
		/// </summary>
		/// <param name="client">HttpClient</param>
		/// <param name="uri">Uri</param>
		/// <param name="behaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>dynamic (JSON)</returns>
		public static async Task<dynamic> GetAsJsonAsync(this HttpClient client, Uri uri, HttpBehaviour behaviour = null, CancellationToken token = default(CancellationToken))
		{
			using (var response = await client.GetAsync(uri, token))
			{
				return await response.GetAsJsonAsync(behaviour, token);
			}
		}

		/// <summary>
		/// Get the content as JSON
		/// </summary>
		/// <param name="client">HttpClient</param>
		/// <param name="uri">Uri</param>
		/// <param name="behaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <typeparam name="TResult">Type to use in the JSON parsing</typeparam>
		/// <returns>dynamic (json)</returns>
		public static async Task<TResult> GetAsJsonAsync<TResult>(this HttpClient client, Uri uri, HttpBehaviour behaviour = null, CancellationToken token = default(CancellationToken))
		{
			using (var response = await client.GetAsync(uri, token))
			{
				return await response.GetAsJsonAsync<TResult>(behaviour, token);
			}
		}
	}
}
