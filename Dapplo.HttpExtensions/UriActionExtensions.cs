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
using System.Threading;
using System.Threading.Tasks;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// Uri extension which perform an action
	/// </summary>
	public static class UriActionExtensions
	{
		/// <summary>
		/// Get LastModified for a URI
		/// </summary>
		/// <param name="uri">Uri</param>
		/// <param name="httpBehaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>DateTime</returns>
		public static async Task<DateTimeOffset> LastModifiedAsync(this Uri uri, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken))
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}

			try
			{
				var headers = await uri.HeadAsync(httpBehaviour, token).ConfigureAwait(false);
				if (headers.LastModified.HasValue)
				{
					return headers.LastModified.Value;
				}
			}
			catch
			{
				// Ignore
			}
			// Pretend it is old
			return DateTimeOffset.MinValue;
		}

		/// <summary>
		/// Retrieve only the content headers, by using the HTTP HEAD method
		/// </summary>
		/// <param name="uri">Uri to get HEAD for</param>
		/// <param name="httpBehaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>HttpContentHeaders</returns>
		public static async Task<HttpContentHeaders> HeadAsync(this Uri uri, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken))
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}

			using (var client = HttpClientFactory.Create(httpBehaviour, uri))
			using (var httpRequestMessage = HttpRequestMessageFactory.CreateHead(uri, httpBehaviour))
			using (var httpResponseMessage = await client.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false))
			{
				await httpResponseMessage.HandleErrorAsync(httpBehaviour, token).ConfigureAwait(false);
				return httpResponseMessage.Content.Headers;
			}
		}

		/// <summary>
		/// Method to Post content
		/// </summary>
		/// <typeparam name="TResponse">the generic type to return the result into, use HttpContent or HttpResponseMessage to get those unprocessed</typeparam>
		/// <param name="uri">Uri to post to</param>
		/// <param name="content">HttpContent to post</param>
		/// <param name="httpBehaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>TResponse</returns>
		public static async Task<TResponse> PostAsync<TResponse, TContent>(this Uri uri, TContent content, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken)) where TResponse : class where TContent : class
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}
			httpBehaviour = httpBehaviour ?? new HttpBehaviour();

			using (var client = HttpClientFactory.Create(httpBehaviour, uri))
			{
				return await client.PostAsync<TResponse, TContent>(uri, content, httpBehaviour, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Method to post content
		/// </summary>
		/// <typeparam name="TResponse">the generic type to return the result into, use HttpContent or HttpResponseMessage to get those unprocessed</typeparam>
		/// <typeparam name="TContent">the generic type to for the content</typeparam>
		/// <typeparam name="TErrorResponse">what to return an error into, use HttpContent or HttpResponseMessage to get those unprocessed</typeparam>
		/// <param name="client">HttpClient</param>
		/// <param name="uri">Uri to post an empty request to</param>
		/// <param name="content">TContent with the content to post</param>
		/// <param name="httpBehaviour">IHttpBehaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>IHttpResponse with TResponse and TErrorResponse</returns>
		public static async Task<IHttpResponse<TResponse, TErrorResponse>> PostAsync<TResponse, TErrorResponse, TContent>(this Uri uri, TContent content, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken)) where TResponse : class where TErrorResponse : class where TContent : class
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}
			httpBehaviour = httpBehaviour ?? new HttpBehaviour();

			using (var client = HttpClientFactory.Create(httpBehaviour, uri))
			{
				return await client.PostAsync<TResponse, TErrorResponse, TContent>(uri, content, httpBehaviour, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Get the response as the specified type
		/// </summary>
		/// <typeparam name="TResult">Type to deserialize into</typeparam>
		/// <param name="uri">An Uri to specify the download location</param>
		/// <param name="httpBehaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>string with the content</returns>
		public static async Task<TResult> GetAsAsync<TResult>(this Uri uri, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken)) where TResult : class
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}
			using (var client = HttpClientFactory.Create(httpBehaviour, uri))
			{
				return await client.GetAsAsync<TResult>(uri, httpBehaviour, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Get the response as the specified type
		/// </summary>
		/// <typeparam name="TResponse">Type to deserialize into</typeparam>
		/// <typeparam name="TErrorResponse">Type to deserialize into when an error occured</typeparam>
		/// <param name="uri">An Uri to specify the download location</param>
		/// <param name="httpBehaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>HttpResponse with TResult and TError</returns>
		public static async Task<IHttpResponse<TResponse, TErrorResponse>> GetAsAsync<TResponse, TErrorResponse>(this Uri uri, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken)) where TResponse : class where TErrorResponse : class
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}
			using (var client = HttpClientFactory.Create(httpBehaviour, uri))
			{
				return await client.GetAsAsync<TResponse, TErrorResponse>(uri, httpBehaviour, token).ConfigureAwait(false);
			}
		}
	}
}
