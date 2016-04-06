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
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.Factory;

#endregion

namespace Dapplo.HttpExtensions.OAuth
{
	/// <summary>
	///     Due to the complexity of the Oauth requests, these extensions are supplied.
	///     For Oauth 2.0 there is no need to have special extensions
	/// </summary>
	public static class OAuth1UriActions
	{
		/// <summary>
		///     Make an OAuth GET, returns the response as the specified type
		/// </summary>
		/// <typeparam name="TResponse">Type to deserialize into</typeparam>
		/// <param name="uri">An Uri to specify the download location</param>
		/// <param name="cancellationToken">CancellationToken</param>
		/// <returns>TResponse</returns>
		public static async Task<TResponse> OAuth1GetAsAsync<TResponse>(this Uri uri, CancellationToken cancellationToken = default(CancellationToken)) where TResponse : class
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}

			using (var httpRequestMessage = HttpRequestMessageFactory.CreateGet<TResponse>(uri))
			{
				return await httpRequestMessage.SendAsync<TResponse>(cancellationToken).ConfigureAwait(false);
			}
		}

		/// <summary>
		///     Make an OAuth GET, returns the response as the specified type
		/// </summary>
		/// <typeparam name="TResponse">Type to deserialize into</typeparam>
		/// <typeparam name="T">Dictionary value type</typeparam>
		/// <param name="uri">An Uri to specify the download location</param>
		/// <param name="properties">Properties for the OAuth request</param>
		/// <param name="cancellationToken">CancellationToken</param>
		/// <returns>TResponse</returns>
		public static async Task<TResponse> OAuth1GetAsAsync<TResponse, T>(this Uri uri, IDictionary<string, T> properties, CancellationToken cancellationToken = default(CancellationToken))
			where TResponse : class
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}

			using (var httpRequestMessage = HttpRequestMessageFactory.CreateGet<TResponse>(uri))
			{
				if (properties != null)
				{
					foreach (var key in properties.Keys)
					{
						httpRequestMessage.Properties.Add(key, properties[key]);
					}
				}

				return await httpRequestMessage.SendAsync<TResponse>(cancellationToken).ConfigureAwait(false);
			}
		}

		/// <summary>
		///     Make an OAuth POST, returns the response as the specified type
		/// </summary>
		/// <typeparam name="TResponse">
		///     the generic type to return the result into, use HttpContent or HttpResponseMessage to get
		///     those unprocessed
		/// </typeparam>
		/// <param name="uri">Uri to post to</param>
		/// <param name="content">Content to post</param>
		/// <param name="properties">properties to post</param>
		/// <param name="cancellationToken">CancellationToken</param>
		/// <returns>TResponse</returns>
		public static async Task<TResponse> OAuth1PostAsync<TResponse>(this Uri uri, object content, IDictionary<string, object> properties = null,
			CancellationToken cancellationToken = default(CancellationToken))
			where TResponse : class
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}

			using (var httpRequestMessage = HttpRequestMessageFactory.CreatePost<TResponse>(uri, content))
			{
				if (properties != null)
				{
					foreach (var key in properties.Keys)
					{
						httpRequestMessage.Properties.Add(key, properties[key]);
					}
				}

				return await httpRequestMessage.SendAsync<TResponse>(cancellationToken).ConfigureAwait(false);
			}
		}

		/// <summary>
		///     Make an OAuth POST, returns the response as the specified type
		/// </summary>
		/// <typeparam name="TResponse">
		///     the generic type to return the result into, use HttpContent or HttpResponseMessage to get
		///     those unprocessed
		/// </typeparam>
		/// <param name="uri">Uri to post to</param>
		/// <param name="properties">properties to post</param>
		/// <param name="cancellationToken">CancellationToken</param>
		/// <returns>TResponse</returns>
		public static async Task<TResponse> OAuth1PostAsync<TResponse>(this Uri uri, IDictionary<string, object> properties = null, CancellationToken cancellationToken = default(CancellationToken))
			where TResponse : class
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}

			using (var httpRequestMessage = HttpRequestMessageFactory.Create<TResponse>(HttpMethod.Post, uri))
			{
				if (properties != null)
				{
					foreach (var key in properties.Keys)
					{
						httpRequestMessage.Properties.Add(key, properties[key]);
					}
				}

				return await httpRequestMessage.SendAsync<TResponse>(cancellationToken).ConfigureAwait(false);
			}
		}
	}
}