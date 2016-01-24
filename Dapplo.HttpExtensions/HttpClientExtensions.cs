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
using System.Linq;
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
			if (content != null)
			{
				httpBehaviour = httpBehaviour ?? new HttpBehaviour();
				var httpContentConverter = httpBehaviour.HttpContentConverters.OrderBy(x => x.Order).FirstOrDefault(x => x.CanConvertToHttpContent(content, httpBehaviour));
				if (httpContentConverter != null)
				{
					var httpContent = httpContentConverter.ConvertToHttpContent(content, httpBehaviour);
					var httpRequestMessage = HttpRequestMessageFactory.Create(HttpMethod.Post, uri, httpContent, httpBehaviour);
					var httpResponseMessage = await client.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);
					return await httpResponseMessage.GetAsAsync<TResult>(httpBehaviour, token).ConfigureAwait(false);
				}
			}

			// No content, send empty post
			using (var request = new HttpRequestMessage(HttpMethod.Post, uri))
			{
				var response = await client.SendAsync(request, token).ConfigureAwait(false);
				return await response.GetAsAsync<TResult>(httpBehaviour, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Method to post without content
		/// </summary>
		/// <typeparam name="TResult">the generic type to return the result into</typeparam>
		/// <typeparam name="TContent">the generic type to for the content</typeparam>
		/// <typeparam name="TError">what to return an error into</typeparam>
		/// <param name="client">HttpClient</param>
		/// <param name="uri">Uri to post an empty request to</param>
		/// <param name="content">TContent with the content to post</param>
		/// <param name="httpBehaviour">IHttpBehaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>TResult</returns>
		public static async Task<HttpResponse<TResult, TError>> PostAsync<TResult, TError, TContent>(this HttpClient client, Uri uri, TContent content, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken)) where TResult : class where TError : class where TContent : class
		{
			if (content != null)
			{
				httpBehaviour = httpBehaviour ?? new HttpBehaviour();
				var httpContentConverter = httpBehaviour.HttpContentConverters.OrderBy(x => x.Order).FirstOrDefault(x => x.CanConvertToHttpContent(content, httpBehaviour));
				if (httpContentConverter != null)
				{
					var httpContent = httpContentConverter.ConvertToHttpContent(content, httpBehaviour);
					var httpRequestMessage = HttpRequestMessageFactory.Create(HttpMethod.Post, uri, httpContent, httpBehaviour);
					var httpResponseMessage = await client.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);
					return await httpResponseMessage.GetAsAsync<TResult, TError>(httpBehaviour, token).ConfigureAwait(false);
				}
			}

			// No content, send empty post
			using (var request = new HttpRequestMessage(HttpMethod.Post, uri))
			{
				var response = await client.SendAsync(request, token).ConfigureAwait(false);
				return await response.GetAsAsync<TResult, TError>(httpBehaviour, token).ConfigureAwait(false);
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
			using (var response = await client.GetAsync(uri, token))
			{
				return await response.GetAsAsync<TResult>(httpBehaviour, token);
			}
		}

		/// <summary>
		/// Get the content from the specified uri via the HttpClient read into a Type object
		/// Currently we support Json objects which are annotated with the DataContract/DataMember attributes
		/// We might support other object, e.g MemoryStream, Bitmap etc soon
		/// </summary>
		/// <typeparam name="TResult">The Type to read into</typeparam>
		/// <typeparam name="TError">The Type to read into when an error occured</typeparam>
		/// <param name="client">HttpClient</param>
		/// <param name="uri">URI</param>
		/// <param name="httpBehaviour">HttpBehaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>HttpResponse with all result and error information</returns>
		public static async Task<HttpResponse<TResult, TError>> GetAsAsync<TResult, TError>(this HttpClient client, Uri uri, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken)) where TResult : class where TError : class
		{
			using (var response = await client.GetAsync(uri, token))
			{
				return await response.GetAsAsync<TResult, TError>(httpBehaviour, token);
			}
		}
	}
}
