// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if NETFRAMEWORK || NETSTANDARD2_0 || NETCOREAPP3_1 || NET5_0 || NET6_0

using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.Factory;
using Dapplo.Log;

namespace Dapplo.HttpExtensions.Listener
{
    /// <summary>
    ///     Extensions for the HttpListener
    /// </summary>
    public static class HttpListenerContextExtensions
    {
        private static readonly LogSource Log = new LogSource();

        /// <summary>
        ///     This writes the supplied content to the response of the httpListenerContext
        ///     It's actually a bit overkill, as it converts to HttpContent and writes this to a stream
        ///     But performance and memory usage are currently not our main concern for the HttpListener
        /// </summary>
        /// <typeparam name="TContent">Type of the content</typeparam>
        /// <param name="httpListenerContext">HttpListenerContext</param>
        /// <param name="content">TContent object</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>Task</returns>
        public static async Task RespondAsync<TContent>(this HttpListenerContext httpListenerContext, TContent content, CancellationToken cancellationToken = default) where TContent : class
        {
            HttpContent httpContent;
            if (typeof(HttpContent).IsAssignableFrom(typeof(TContent)))
            {
                httpContent = content as HttpContent;
            }
            else
            {
                httpContent = HttpContentFactory.Create(content);
            }

            using var response = httpListenerContext.Response;
            if (httpContent is null)
            {
                Log.Error().WriteLine("Nothing to respond with...");
                response.StatusCode = (int) HttpStatusCode.InternalServerError;
                return;
            }
            // Write to response stream.
            response.ContentLength64 = httpContent.Headers?.ContentLength ?? 0;
            response.ContentType = httpContent.GetContentType();
            response.StatusCode = (int) HttpStatusCode.OK;
            Log.Debug().WriteLine("Responding with {0}", response.ContentType);
            using var stream = response.OutputStream;
            if (!cancellationToken.IsCancellationRequested)
            {
                await httpContent.CopyToAsync(stream).ConfigureAwait(false);
            }
        }
    }
}

#endif