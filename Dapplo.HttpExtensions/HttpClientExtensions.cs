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

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// Extensions for the HttpClient class
	/// </summary>
	public static class HttpClientExtensions
	{
		/// <summary>
		/// Set Basic Authentication for the current client
		/// </summary>
		/// <param name="client"></param>
		/// <param name="user">username</param>
		/// <param name="password">password</param>
		/// <returns>HttpClient for fluent usage</returns>
		public static HttpClient SetBasicAuthorization(this HttpClient client, string user, string password)
		{
			string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", user, password)));
			return client.SetAuthorization("Basic", credentials);
		}

		/// <summary>
		/// Set Bearer "Authentication" for the current client
		/// </summary>
		/// <param name="client"></param>
		/// <param name="bearer">Bearer for the authorization</param>
		/// <returns>HttpClient for fluent usage</returns>
		public static HttpClient SetBearer(this HttpClient client, string bearer)
		{
			return client.SetAuthorization("Bearer", bearer);
		}

		/// <summary>
		/// Set Authorization for the current client
		/// </summary>
		/// <param name="client"></param>
		/// <param name="scheme">scheme</param>
		/// <param name="authorization">value</param>
		/// <returns>HttpClient for fluent usage</returns>
		public static HttpClient SetAuthorization(this HttpClient client, string scheme, string authorization)
		{
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, authorization);
			return client;
		}

		/// <summary>
		/// Add default request header without validation
		/// </summary>
		/// <param name="client"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns>HttpClient for fluent usage</returns>
		public static HttpClient AddDefaultRequestHeader(this HttpClient client, string name, string value)
		{
			client.DefaultRequestHeaders.TryAddWithoutValidation(name, value);
			return client;
		}

		/// <summary>
		/// Method to post without content
		/// </summary>
		/// <param name="client">HttpClient</param>
		/// <param name="uri"></param>
		/// <param name="token"></param>
		/// <returns>HttpResponseMessage</returns>
		public static async Task<HttpResponseMessage> PostAsync(this HttpClient client, Uri uri, CancellationToken token = default(CancellationToken))
		{
			using (var request = new HttpRequestMessage(HttpMethod.Post, uri))
			{
				return await client.SendAsync(request, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Method to post with JSON
		/// </summary>
		/// <param name="client">HttpClient</param>
		/// <param name="uri"></param>
		/// <param name="postData"></param>
		/// <param name="token"></param>
		/// <returns>HttpResponseMessage</returns>
		public static async Task<HttpResponseMessage> PostJsonAsync<T>(this HttpClient client, Uri uri, T postData, CancellationToken token = default(CancellationToken))
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
		/// <param name="uri"></param>
		/// <param name="postData">T1</param>
		/// <param name="throwErrorOnNonSuccess"></param>
		/// <param name="token"></param>
		/// <returns>T2</returns>
		public static async Task<T2> PostJsonAsync<T1, T2>(this HttpClient client, Uri uri, T1 postData, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken))
		{
			using (var content = new StringContent(SimpleJson.SerializeObject(postData), Encoding.UTF8, "application/json"))
			{
				var response = await client.PostAsync(uri, content, token).ConfigureAwait(false);
				return await response.GetAsJsonAsync<T2>(throwErrorOnNonSuccess, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Get the content as a MemoryStream
		/// </summary>
		/// <param name="client">HttpClient</param>
		/// <param name="uri">Uri</param>
		/// <param name="throwErrorOnNonSuccess">bool</param>
		/// <param name="token"></param>
		/// <returns>MemoryStream</returns>
		public static async Task<MemoryStream> GetAsMemoryStreamAsync(this HttpClient client, Uri uri, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken))
		{
			using (var response = await client.GetAsync(uri, token))
			{
				return await response.GetAsMemoryStreamAsync(throwErrorOnNonSuccess, token);
            }
		}

		/// <summary>
		/// Get the content as JSON
		/// </summary>
		/// <param name="client">HttpClient</param>
		/// <param name="uri">Uri</param>
		/// <param name="throwErrorOnNonSuccess">bool</param>
		/// <param name="token"></param>
		/// <returns>dynamic (JSON)</returns>
		public static async Task<dynamic> GetAsJsonAsync(this HttpClient client, Uri uri, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken))
		{
			using (var response = await client.GetAsync(uri, token))
			{
				return await response.GetAsJsonAsync(throwErrorOnNonSuccess, token);
			}
		}

		/// <summary>
		/// Get the content as JSON
		/// </summary>
		/// <param name="client">HttpClient</param>
		/// <param name="uri">Uri</param>
		/// <param name="throwErrorOnNonSuccess">bool</param>
		/// <param name="token"></param>
		/// <typeparam name="T">Type to use in the JSON parsing</typeparam>
		/// <returns>dynamic (json)</returns>
		public static async Task<T> GetAsJsonAsync<T>(this HttpClient client, Uri uri, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken))
		{
			using (var response = await client.GetAsync(uri, token))
			{
				return await response.GetAsJsonAsync<T>(throwErrorOnNonSuccess, token);
			}
		}
	}
}
