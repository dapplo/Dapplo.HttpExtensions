//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2015-2016 Dapplo
// 
//  For more information see: http://dapplo.net/
//  Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
//  This file is part of Dapplo.HttpExtensions
// 
//  Dapplo.HttpExtensions is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  Dapplo.HttpExtensions is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have a copy of the GNU Lesser General Public License
//  along with Dapplo.HttpExtensions. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

#region using

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.Factory;
using Dapplo.Log.Facade;

#endregion

namespace Dapplo.HttpExtensions
{
	/// <summary>
	///     Extensions for the HttpClient class
	/// </summary>
	public static class HttpClientExtensions
	{
		private static readonly LogSource Log = new LogSource();

		/// <summary>
		///     Add default request header without validation
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
		///     Send a Delete request to the server
		/// </summary>
		/// <typeparam name="TResponse">
		///     the generic type to return the result into, use HttpContent or HttpResponseMessage to get
		///     those unprocessed
		/// </typeparam>
		/// <param name="httpClient">HttpClient</param>
		/// <param name="uri">Uri to send the delete request to</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>TResult</returns>
		public static async Task<TResponse> DeleteAsync<TResponse>(this HttpClient httpClient, Uri uri, CancellationToken token = default(CancellationToken))
			where TResponse : class
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri), "No uri supplied");
			}

			using (var httpRequestMessage = HttpRequestMessageFactory.CreateDelete<TResponse>(uri))
			{
				return await httpRequestMessage.SendAsync<TResponse>(httpClient, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		///     Get the content from the specified uri via the HttpClient read into a Type object
		///     Currently we support Json objects which are annotated with the DataContract/DataMember attributes
		///     We might support other object, e.g MemoryStream, Bitmap etc soon
		/// </summary>
		/// <typeparam name="TResponse">The Type to read into</typeparam>
		/// <param name="client">HttpClient</param>
		/// <param name="uri">URI</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>the deserialized object of type T or default(T)</returns>
		public static async Task<TResponse> GetAsAsync<TResponse>(this HttpClient client, Uri uri, CancellationToken token = default(CancellationToken)) where TResponse : class
		{
			var httpBehaviour = HttpBehaviour.Current;

			using (var httpRequestMessage = HttpRequestMessageFactory.CreateGet<TResponse>(uri))
			using (var httpResponseMessage = await client.SendAsync(httpRequestMessage, httpBehaviour.HttpCompletionOption, token).ConfigureAwait(false))
			{
				return await httpResponseMessage.GetAsAsync<TResponse>(token).ConfigureAwait(false);
			}
		}

		/// <summary>
		///     Retrieve only the content headers, by using the HTTP HEAD method
		/// </summary>
		/// <param name="httpClient"></param>
		/// <param name="uri">Uri to get HEAD for</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>HttpContentHeaders</returns>
		public static async Task<HttpContentHeaders> HeadAsync(this HttpClient httpClient, Uri uri, CancellationToken token = default(CancellationToken))
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}
			Log.Verbose().WriteLine("Requesting headers for: {0}", uri);
			using (var httpRequestMessage = HttpRequestMessageFactory.CreateHead(uri))
			using (var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false))
			{
				await httpResponseMessage.HandleErrorAsync().ConfigureAwait(false);
				return httpResponseMessage.Content.Headers;
			}
		}

		/// <summary>
		///     Post the content, and get the reponse
		/// </summary>
		/// <typeparam name="TResponse">
		///     the generic type to return the result into, use HttpContent or HttpResponseMessage to get
		///     those unprocessed
		/// </typeparam>
		/// <param name="httpClient">HttpClient</param>
		/// <param name="uri">Uri to post request to</param>
		/// <param name="content">Content to post</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>TResult</returns>
		public static async Task<TResponse> PostAsync<TResponse>(this HttpClient httpClient, Uri uri, object content, CancellationToken token = default(CancellationToken))
			where TResponse : class
		{
			if (content == null)
			{
				Log.Warn().WriteLine("No content supplied, this is ok but unusual.");
			}

			using (var httpRequestMessage = HttpRequestMessageFactory.CreatePost<TResponse>(uri, content))
			{
				return await httpRequestMessage.SendAsync<TResponse>(httpClient, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		///     Post the content, and don't expect (ignore) the response
		/// </summary>
		/// <param name="httpClient">HttpClient</param>
		/// <param name="uri">Uri to post an empty request to</param>
		/// <param name="content">Content to post</param>
		/// <param name="token">CancellationToken</param>
		public static async Task PostAsync(this HttpClient httpClient, Uri uri, object content, CancellationToken token = default(CancellationToken))
		{
			if (content == null)
			{
				Log.Warn().WriteLine("No content supplied, this is ok but unusual.");
			}

			using (var httpRequestMessage = HttpRequestMessageFactory.CreatePost(uri, content))
			{
				await httpRequestMessage.SendAsync(httpClient, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		///     Put the content, and get the reponse
		/// </summary>
		/// <typeparam name="TResponse">
		///     the generic type to return the result into, use HttpContent or HttpResponseMessage to get
		///     those unprocessed
		/// </typeparam>
		/// <param name="httpClient">HttpClient</param>
		/// <param name="uri">Uri to put the request to</param>
		/// <param name="content">Content to put</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>TResult</returns>
		public static async Task<TResponse> PutAsync<TResponse>(this HttpClient httpClient, Uri uri, object content, CancellationToken token = default(CancellationToken))
			where TResponse : class
		{
			if (content == null)
			{
				Log.Warn().WriteLine("No content supplied, this is ok but unusual.");
			}

			using (var httpRequestMessage = HttpRequestMessageFactory.CreatePut<TResponse>(uri, content))
			{
				return await httpRequestMessage.SendAsync<TResponse>(httpClient, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		///     Set Authorization for the current client
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
		///     Set Basic Authentication for the current client
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
		///     Use the UserInfo from the Uri to set the basic authorization information
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
			var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(uri.UserInfo));
			return client.SetAuthorization("Basic", credentials);
		}

		/// <summary>
		///     Set Bearer "Authentication" for the current client
		/// </summary>
		/// <param name="client">HttpClient</param>
		/// <param name="bearer">Bearer for the authorization</param>
		/// <returns>HttpClient for fluent usage</returns>
		public static HttpClient SetBearer(this HttpClient client, string bearer)
		{
			return client.SetAuthorization("Bearer", bearer);
		}
	}
}