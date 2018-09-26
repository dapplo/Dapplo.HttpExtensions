//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2018 Dapplo
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

#region Usings

using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.Factory;
using Dapplo.Log;

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
        ///     the generic type to return the result into, use HttpContent or HttpResponseMessage to get those unprocessed
        /// </typeparam>
        /// <param name="uri">Uri to send the delete to</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>TResponse</returns>
        public static async Task<TResponse> DeleteAsync<TResponse>(this Uri uri, CancellationToken cancellationToken = default)
            where TResponse : class
        {
            if (uri is null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            using (var client = HttpClientFactory.Create(uri))
            {
                return await client.DeleteAsync<TResponse>(uri, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        ///     Method to Delete content
        /// </summary>
        /// <param name="uri">Uri to send the delete to</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>Task</returns>
        public static async Task DeleteAsync(this Uri uri, CancellationToken cancellationToken = default)
        {
            if (uri is null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            using (var client = HttpClientFactory.Create(uri))
            {
                await client.DeleteAsync(uri, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        ///     Get the response as the specified type
        /// </summary>
        /// <typeparam name="TResponse">Type to deserialize into</typeparam>
        /// <param name="uri">An Uri to specify the download location</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>TResponse</returns>
        public static async Task<TResponse> GetAsAsync<TResponse>(this Uri uri, CancellationToken cancellationToken = default) where TResponse : class
        {
            if (uri is null)
            {
                throw new ArgumentNullException(nameof(uri));
            }
            using (var client = HttpClientFactory.Create(uri))
            {
                return await client.GetAsAsync<TResponse>(uri, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        ///     Retrieve only the content headers, by using the HTTP HEAD method
        /// </summary>
        /// <param name="uri">Uri to get HEAD for</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>HttpContentHeaders</returns>
        public static async Task<HttpContentHeaders> HeadAsync(this Uri uri, CancellationToken cancellationToken = default)
        {
            if (uri is null)
            {
                throw new ArgumentNullException(nameof(uri));
            }
            using (var httpClient = HttpClientFactory.Create(uri))
            {
                return await httpClient.HeadAsync(uri, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        ///     Get LastModified for a URI
        /// </summary>
        /// <param name="uri">Uri</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>DateTime</returns>
        public static async Task<DateTimeOffset> LastModifiedAsync(this Uri uri, CancellationToken cancellationToken = default)
        {
            if (uri is null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            try
            {
                var headers = await uri.HeadAsync(cancellationToken).ConfigureAwait(false);
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
        ///     Method to Patch content
        /// </summary>
        /// <typeparam name="TResponse">
        ///     the generic type to return the result into, use HttpContent or HttpResponseMessage to get
        ///     those unprocessed
        /// </typeparam>
        /// <param name="uri">Uri to patch to</param>
        /// <param name="content">Content to patch</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>TResponse</returns>
        public static async Task<TResponse> PatchAsync<TResponse>(this Uri uri, object content, CancellationToken cancellationToken = default)
            where TResponse : class
        {
            if (uri is null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            using (var client = HttpClientFactory.Create(uri))
            {
                return await client.PatchAsync<TResponse>(uri, content, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        ///     Method to Patch content, ignore response
        /// </summary>
        /// <param name="uri">Uri to patch to</param>
        /// <param name="content">Content to patch</param>
        /// <param name="cancellationToken">CancellationToken</param>
        public static async Task PatchAsync(this Uri uri, object content, CancellationToken cancellationToken = default)
        {
            if (uri is null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            using (var client = HttpClientFactory.Create(uri))
            {
                await client.PatchAsync(uri, content, cancellationToken).ConfigureAwait(false);
            }
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
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>TResponse</returns>
        public static async Task<TResponse> PostAsync<TResponse>(this Uri uri, object content, CancellationToken cancellationToken = default)
            where TResponse : class
        {
            if (uri is null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            using (var client = HttpClientFactory.Create(uri))
            {
                return await client.PostAsync<TResponse>(uri, content, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        ///     Method to Post content, ignore response
        /// </summary>
        /// <param name="uri">Uri to post to</param>
        /// <param name="content">Content to post</param>
        /// <param name="cancellationToken">CancellationToken</param>
        public static async Task PostAsync(this Uri uri, object content, CancellationToken cancellationToken = default)
        {
            if (uri is null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            using (var client = HttpClientFactory.Create(uri))
            {
                await client.PostAsync(uri, content, cancellationToken).ConfigureAwait(false);
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
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>TResponse</returns>
        public static async Task<TResponse> PutAsync<TResponse>(this Uri uri, object content, CancellationToken cancellationToken = default)
            where TResponse : class
        {
            if (uri is null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            using (var client = HttpClientFactory.Create(uri))
            {
                return await client.PutAsync<TResponse>(uri, content, cancellationToken).ConfigureAwait(false);
            }
        }

        /// <summary>
        ///     Method to Put content
        /// </summary>
        /// <param name="uri">Uri to put to</param>
        /// <param name="content">Content to put</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>task</returns>
        public static async Task PutAsync(this Uri uri, object content, CancellationToken cancellationToken = default)
        {
            if (uri is null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            using (var client = HttpClientFactory.Create(uri))
            {
                await client.PutAsync(uri, content, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}