//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2015-2017 Dapplo
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

#if NET45 || NET46

#region Usings

using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.Factory;
using Dapplo.Log;

#endregion

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
        ///     It's actually a bit overkill, as it convers to HttpContent and writes this to a stream
        ///     But performance and memory usage are currently not our main concern for the HttpListener
        /// </summary>
        /// <typeparam name="TContent">Type of the content</typeparam>
        /// <param name="httpListenerContext">HttpListenerContext</param>
        /// <param name="content">TContent object</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>Task</returns>
        public static async Task RespondAsync<TContent>(this HttpListenerContext httpListenerContext, TContent content,
            CancellationToken cancellationToken = default)
            where TContent : class
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

            using (var response = httpListenerContext.Response)
            {
                if (httpContent == null)
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
                using (var stream = response.OutputStream)
                {
                    await httpContent.CopyToAsync(stream).ConfigureAwait(false);
                }
            }
        }
    }
}

#endif