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
		/// <param name="client">HttpClient</param>
		/// <param name="user">username</param>
		/// <param name="password">password</param>
		/// <returns>HttpClient for fluent usage</returns>
		public static HttpClient SetBasicAuthorization(this HttpClient client, string user, string password)
		{
			string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", user, password)));
			return client.SetAuthorization("Basic", credentials);
		}

		/// <summary>
		/// Use the UserInfo from the Uri to set the basic authorization information
		/// </summary>
		/// <param name="client">HttpClient</param>
		/// <param name="uri">Uri with UserInfo</param>
		/// <returns>HttpClient for fluent usage</returns>
		public static HttpClient SetBasicAuthorization(this HttpClient client, Uri uri)
		{
			if (string.IsNullOrEmpty(uri?.UserInfo))
			{
				return client;
			}
			string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(uri.UserInfo));
			return client.SetAuthorization("Basic", credentials);
		}

		/// <summary>
		/// Set Bearer "Authentication" for the current client
		/// </summary>
		/// <param name="client">HttpClient</param>
		/// <param name="bearer">Bearer for the authorization</param>
		/// <returns>HttpClient for fluent usage</returns>
		public static HttpClient SetBearer(this HttpClient client, string bearer)
		{
			return client.SetAuthorization("Bearer", bearer);
		}

		/// <summary>
		/// Set Authorization for the current client
		/// </summary>
		/// <param name="client">HttpClient</param>
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
		/// <param name="client">HttpClient</param>
		/// <param name="name">Header name</param>
		/// <param name="value">Header value</param>
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
		/// <param name="uri">Uri to post an empty request to</param>
		/// <param name="token">CancellationToken</param>
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
		/// <param name="uri">Uri to post the json to</param>
		/// <param name="postData">data to post</param>
		/// <param name="token">CancellationToken</param>
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
		/// <param name="uri">Uri to post the json to</param>
		/// <param name="postData">data to post</param>
		/// <param name="throwErrorOnNonSuccess">true to throw an exception when an error occurse, else null is returned</param>
		/// <param name="token">CancellationToken</param>
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
		/// <param name="throwErrorOnNonSuccess">true to throw an exception when an error occurse, else null is returned</param>
		/// <param name="token">CancellationToken</param>
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
		/// <param name="throwErrorOnNonSuccess">true to throw an exception when an error occurse, else null is returned</param>
		/// <param name="token">CancellationToken</param>
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
		/// <param name="throwErrorOnNonSuccess">true to throw an exception when an error occurse, else null is returned</param>
		/// <param name="token">CancellationToken</param>
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
