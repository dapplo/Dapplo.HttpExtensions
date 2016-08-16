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
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.Factory;
using Dapplo.Log.Facade;

#endregion

namespace Dapplo.HttpExtensions
{
	/// <summary>
	///     Uri extension which perform an action
	/// </summary>
	public static class UriActionExtensions
	{
		private static readonly LogSource Log = new LogSource();

		/// <summary>
		///     Method to Delete content
		/// </summary>
		/// <typeparam name="TResponse">
		///     the generic type to return the result into, use HttpContent or HttpResponseMessage to get
		///     those unprocessed
		/// </typeparam>
		/// <param name="uri">Uri to send the delete to</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>TResponse</returns>
		public static async Task<TResponse> DeleteAsync<TResponse>(this Uri uri, CancellationToken token = default(CancellationToken))
			where TResponse : class
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}

			using (var client = HttpClientFactory.Create(uri))
			{
				return await client.DeleteAsync<TResponse>(uri, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		///     Get the response as the specified type
		/// </summary>
		/// <typeparam name="TResponse">Type to deserialize into</typeparam>
		/// <param name="uri">An Uri to specify the download location</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>TResponse</returns>
		public static async Task<TResponse> GetAsAsync<TResponse>(this Uri uri, CancellationToken token = default(CancellationToken)) where TResponse : class
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}
			using (var client = HttpClientFactory.Create(uri))
			{
				return await client.GetAsAsync<TResponse>(uri, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		///     Retrieve only the content headers, by using the HTTP HEAD method
		/// </summary>
		/// <param name="uri">Uri to get HEAD for</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>HttpContentHeaders</returns>
		public static async Task<HttpContentHeaders> HeadAsync(this Uri uri, CancellationToken token = default(CancellationToken))
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}
			using (var httpClient = HttpClientFactory.Create(uri))
			{
				return await httpClient.HeadAsync(uri, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		///     Get LastModified for a URI
		/// </summary>
		/// <param name="uri">Uri</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>DateTime</returns>
		public static async Task<DateTimeOffset> LastModifiedAsync(this Uri uri, CancellationToken token = default(CancellationToken))
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}

			try
			{
				var headers = await uri.HeadAsync(token).ConfigureAwait(false);
				if (headers.LastModified.HasValue)
				{
					return headers.LastModified.Value;
				}
			}
			catch (Exception ex)
			{
				Log.Warn().WriteLine(ex, "Couldn't read last modified value.");
			}
			// Pretend it is old
			return DateTimeOffset.MinValue;
		}

		/// <summary>
		///     Method to Post content
		/// </summary>
		/// <typeparam name="TResponse">
		///     the generic type to return the result into, use HttpContent or HttpResponseMessage to get
		///     those unprocessed
		/// </typeparam>
		/// <param name="uri">Uri to post to</param>
		/// <param name="content">Content to post</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>TResponse</returns>
		public static async Task<TResponse> PostAsync<TResponse>(this Uri uri, object content, CancellationToken token = default(CancellationToken))
			where TResponse : class
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}

			using (var client = HttpClientFactory.Create(uri))
			{
				return await client.PostAsync<TResponse>(uri, content, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		///     Method to Post content, ignore response
		/// </summary>
		/// <param name="uri">Uri to post to</param>
		/// <param name="content">Content to post</param>
		/// <param name="token">CancellationToken</param>
		public static async Task PostAsync(this Uri uri, object content, CancellationToken token = default(CancellationToken))
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}

			using (var client = HttpClientFactory.Create(uri))
			{
				await client.PostAsync(uri, content, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		///     Method to Put content
		/// </summary>
		/// <typeparam name="TResponse">
		///     the generic type to return the result into, use HttpContent or HttpResponseMessage to get
		///     those unprocessed
		/// </typeparam>
		/// <param name="uri">Uri to put to</param>
		/// <param name="content">Content to put</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>TResponse</returns>
		public static async Task<TResponse> PutAsync<TResponse>(this Uri uri, object content, CancellationToken token = default(CancellationToken))
			where TResponse : class
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}

			using (var client = HttpClientFactory.Create(uri))
			{
				return await client.PutAsync<TResponse>(uri, content, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		///     Method to Put content
		/// </summary>
		/// <param name="uri">Uri to put to</param>
		/// <param name="content">Content to put</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>task</returns>
		public static async Task PutAsync(this Uri uri, object content, CancellationToken token = default(CancellationToken))
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}

			using (var client = HttpClientFactory.Create(uri))
			{
				await client.PutAsync(uri, content, token).ConfigureAwait(false);
			}
		}
	}
}