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
using System.Collections.Generic;
using System.IO;
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
		/// <param name="throwErrorOnNonSuccess">true to throw an exception when an error is returned, else DateTimeOffset.MinValue is returned</param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>DateTime</returns>
		public static async Task<DateTimeOffset> LastModifiedAsync(this Uri uri, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}

			try
			{
				var headers = await uri.HeadAsync(throwErrorOnNonSuccess, token, httpSettings).ConfigureAwait(false);
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
		/// <param name="throwErrorOnNonSuccess">true to throw an exception when an error is returned, else the headers are returned</param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>HttpContentHeaders</returns>
		public static async Task<HttpContentHeaders> HeadAsync(this Uri uri, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}

			using (var client = HttpClientFactory.CreateHttpClient(httpSettings, uri))
			using (var request = new HttpRequestMessage(HttpMethod.Head, uri))
			{
				var responseMessage = await client.SendAsync(request, token).ConfigureAwait(false);
				await responseMessage.HandleErrorAsync(throwErrorOnNonSuccess, token);
				return responseMessage.Content.Headers;
			}
		}

		/// <summary>
		/// Method to Post without content
		/// </summary>
		/// <param name="uri">Uri to post to</param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>HttpResponseMessage</returns>
		public static async Task<HttpResponseMessage> PostAsync(this Uri uri, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}

			using (var client = HttpClientFactory.CreateHttpClient(httpSettings, uri))
			{
				return await client.PostAsync(uri, token);
			}
		}

		/// <summary>
		/// Method to Post content
		/// </summary>
		/// <param name="uri">Uri to post to</param>
		/// <param name="content">HttpContent to post</param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>HttpResponseMessage</returns>
		public static async Task<HttpResponseMessage> PostAsync(this Uri uri, HttpContent content, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}

			using (var client = HttpClientFactory.CreateHttpClient(httpSettings, uri))
			{
				return await client.PostAsync(uri, content, token);
			}
		}

		/// <summary>
		/// Simple extension to post Form-URLEncoded Content
		/// </summary>
		/// <param name="uri">Uri to post to</param>
		/// <param name="formContent">Dictionary with the values</param>
		/// <param name="token">Cancellationtoken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>HttpResponseMessage</returns>
		public static async Task<HttpResponseMessage> PostFormUrlEncodedAsync(this Uri uri, IDictionary<string, string> formContent, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}
			if (formContent == null)
			{
				throw new ArgumentNullException(nameof(formContent));
			}
			using (var content = new FormUrlEncodedContent(formContent))
			using (var client = HttpClientFactory.CreateHttpClient(httpSettings, uri))
			{
				return await client.PostAsync(uri, content, token);
			}
		}

		/// <summary>
		/// Download a uri response as string
		/// </summary>
		/// <param name="uri">An Uri to specify the download location</param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>HttpResponseMessage</returns>
		public static async Task<HttpResponseMessage> GetAsync(this Uri uri, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}
			using (var client = HttpClientFactory.CreateHttpClient(httpSettings, uri))
			{
				return await client.GetAsync(uri, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Download a uri response as string
		/// </summary>
		/// <param name="uri">An Uri to specify the download location</param>
		/// <param name="throwErrorOnNonSuccess">true to throw an exception when an error occurse, else null is returned</param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>string with the content</returns>
		public static async Task<string> GetAsStringAsync(this Uri uri, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}
			using (var client = HttpClientFactory.CreateHttpClient(httpSettings, uri))
			using (var response = await client.GetAsync(uri, HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(false))
			{
				return await response.GetAsStringAsync(throwErrorOnNonSuccess, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Get the content as a MemoryStream
		/// </summary>
		/// <param name="uri">Uri</param>
		/// <param name="throwErrorOnNonSuccess">true to throw an exception when an error occurse, else null is returned</param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>MemoryStream</returns>
		public static async Task<MemoryStream> GetAsMemoryStreamAsync(this Uri uri, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}
			using (var client = HttpClientFactory.CreateHttpClient(httpSettings, uri))
			{
				return await client.GetAsMemoryStreamAsync(uri, throwErrorOnNonSuccess, token);
			}
		}
	}
}
