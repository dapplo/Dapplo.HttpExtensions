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

using Dapplo.HttpExtensions.Factory;
using System;
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
			var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}"));
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
		/// <param name="httpBehaviour">IHttpBehaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>HttpResponseMessage</returns>
		public static async Task<HttpResponseMessage> PostAsync(this HttpClient client, Uri uri, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken))
		{
			httpBehaviour = httpBehaviour ?? new HttpBehaviour();
			using (var httpRequestMessage = HttpRequestMessageFactory.Create(HttpMethod.Post, uri, null, null, httpBehaviour))
			{
				return await client.SendAsync(httpRequestMessage, httpBehaviour.HttpCompletionOption, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Method to post without content
		/// </summary>
		/// <typeparam name="TResult">the generic type to return the result into</typeparam>
		/// <typeparam name="TContent">the generic type to for the content</typeparam>
		/// <param name="client">HttpClient</param>
		/// <param name="uri">Uri to post an empty request to</param>
		/// <param name="content">TContent with the content to post</param>
		/// <param name="httpBehaviour">IHttpBehaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>TResult</returns>
		public static async Task<TResult> PostAsync<TResult, TContent>(this HttpClient client, Uri uri, TContent content, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken)) where TResult : class where TContent : class
		{
			httpBehaviour = httpBehaviour ?? new HttpBehaviour();
			using (var httpContent = HttpContentFactory.Create(content, httpBehaviour))
			{
				if (httpContent != null)
				{
					using (var httpRequestMessage = HttpRequestMessageFactory.Create<TResult>(HttpMethod.Post, uri, httpContent, httpBehaviour))
					using (var httpResponseMessage = await client.SendAsync(httpRequestMessage, httpBehaviour.HttpCompletionOption, token).ConfigureAwait(false))
					{
						return await httpResponseMessage.GetAsAsync<TResult>(httpBehaviour, token).ConfigureAwait(false);
					}
				}
			}

			// No content, send empty post
			using (var httpRequestMessage = HttpRequestMessageFactory.Create<TResult>(HttpMethod.Post, uri, null, httpBehaviour))
			using (var httpResponseMessage = await client.SendAsync(httpRequestMessage, httpBehaviour.HttpCompletionOption, token).ConfigureAwait(false))
			{
				return await httpResponseMessage.GetAsAsync<TResult>(httpBehaviour, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Method to post without content
		/// </summary>
		/// <typeparam name="TResponse">the generic type to return the result into</typeparam>
		/// <typeparam name="TContent">the generic type to for the content</typeparam>
		/// <typeparam name="TErrorResponse">what to return an error into</typeparam>
		/// <param name="client">HttpClient</param>
		/// <param name="uri">Uri to post an empty request to</param>
		/// <param name="content">TContent with the content to post</param>
		/// <param name="httpBehaviour">IHttpBehaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>TResult</returns>
		public static async Task<IHttpResponse<TResponse, TErrorResponse>> PostAsync<TResponse, TErrorResponse, TContent>(this HttpClient client, Uri uri, TContent content, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken)) where TResponse : class where TErrorResponse : class where TContent : class
		{
			httpBehaviour = httpBehaviour ?? new HttpBehaviour();
			using (var httpContent = HttpContentFactory.Create(content, httpBehaviour))
			{
				if (httpContent != null)
				{
					using (var httpRequestMessage = HttpRequestMessageFactory.Create<TResponse>(HttpMethod.Post, uri, httpContent, httpBehaviour))
					using (var httpResponseMessage = await client.SendAsync(httpRequestMessage, httpBehaviour.HttpCompletionOption, token).ConfigureAwait(false))
					{
						return await httpResponseMessage.GetAsAsync<TResponse, TErrorResponse>(httpBehaviour, token).ConfigureAwait(false);
					}
				}
			}

			// No content, send empty post
			using (var httpRequestMessage = HttpRequestMessageFactory.CreatePost(uri))
			using (var httpResponseMessage = await client.SendAsync(httpRequestMessage, httpBehaviour.HttpCompletionOption, token).ConfigureAwait(false))
			{
				return await httpResponseMessage.GetAsAsync<TResponse, TErrorResponse>(httpBehaviour, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Get the content from the specified uri via the HttpClient read into a Type object
		/// Currently we support Json objects which are annotated with the DataContract/DataMember attributes
		/// We might support other object, e.g MemoryStream, Bitmap etc soon
		/// </summary>
		/// <typeparam name="TResult">The Type to read into</typeparam>
		/// <param name="client">HttpClient</param>
		/// <param name="uri">URI</param>
		/// <param name="httpBehaviour">HttpBehaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>the deserialized object of type T or default(T)</returns>
		public static async Task<TResult> GetAsAsync<TResult>(this HttpClient client, Uri uri, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken)) where TResult : class
		{
			using (var httpResponseMessage = await client.GetAsync(uri, token).ConfigureAwait(false))
			{
				return await httpResponseMessage.GetAsAsync<TResult>(httpBehaviour, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Get the content from the specified uri via the HttpClient read into a Type object
		/// Currently we support Json objects which are annotated with the DataContract/DataMember attributes
		/// We might support other object, e.g MemoryStream, Bitmap etc soon
		/// </summary>
		/// <typeparam name="TResponse">The Type to read into</typeparam>
		/// <typeparam name="TErrorResponse">The Type to read into when an error occured</typeparam>
		/// <param name="client">HttpClient</param>
		/// <param name="uri">URI</param>
		/// <param name="httpBehaviour">HttpBehaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>HttpResponse with all result and error information</returns>
		public static async Task<IHttpResponse<TResponse, TErrorResponse>> GetAsAsync<TResponse, TErrorResponse>(this HttpClient client, Uri uri, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken)) where TResponse : class where TErrorResponse : class
		{
			using (var httpResponseMessage = await client.GetAsync(uri, token).ConfigureAwait(false))
			{
				return await httpResponseMessage.GetAsAsync<TResponse, TErrorResponse>(httpBehaviour, token).ConfigureAwait(false);
			}
		}
	}
}
