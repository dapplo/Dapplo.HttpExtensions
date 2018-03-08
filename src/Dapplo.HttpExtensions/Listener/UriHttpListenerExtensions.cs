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

using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.Log;

#endregion

namespace Dapplo.HttpExtensions.Listener
{
    /// <summary>
    ///     Async helpers for the HttpListener
    /// </summary>
    public static class UriHttpListenerExtensions
    {
        private static readonly LogSource Log = new LogSource();

        /// <summary>
        ///     This method starts a HttpListener to make it possible to listen async for a SINGLE request.
        /// </summary>
        /// <param name="listenUri">
        ///     The Uri to listen to, use CreateFreeLocalHostUri (and add segments) if you don't have a
        ///     specific reason
        /// </param>
        /// <param name="httpListenerContextHandler">A function which gets a HttpListenerContext and returns a value</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>The value from the httpListenerContextHandler</returns>
        public static Task<T> ListenAsync<T>(this Uri listenUri, Func<HttpListenerContext, Task<T>> httpListenerContextHandler,
            CancellationToken cancellationToken = default)
        {
            var listenUriString = listenUri.AbsoluteUri.EndsWith("/") ? listenUri.AbsoluteUri : listenUri.AbsoluteUri + "/";
            Log.Debug().WriteLine("Start listening on {0}", listenUriString);
            var taskCompletionSource = new TaskCompletionSource<T>();

            // ReSharper disable once UnusedVariable
            var listenTask = Task.Factory.StartNew(async () =>
            {
                using (var httpListener = new HttpListener())
                {
                    try
                    {
                        // Add the URI to listen to, this SHOULD be localhost to prevent a lot of problemens with rights
                        httpListener.Prefixes.Add(listenUriString);
                        // Start listening
                        httpListener.Start();
                        Log.Debug().WriteLine("Started listening on {0}", listenUriString);

                        // Make the listener stop if the token is cancelled.
                        // This registratrion is disposed before the httpListener is disposed:
                        // ReSharper disable once AccessToDisposedClosure
                        var cancellationTokenRegistration = cancellationToken.Register(() => { httpListener.Stop(); });

                        // Get the context
                        var httpListenerContext = await httpListener.GetContextAsync().ConfigureAwait(false);

                        // Call the httpListenerContextHandler with the context we got for the result
                        var result = await httpListenerContextHandler(httpListenerContext).ConfigureAwait(false);

                        // Dispose the registration, so the stop isn't called on a disposed httpListener
                        cancellationTokenRegistration.Dispose();

                        // Set the result to the TaskCompletionSource, so the await on the task finishes
                        taskCompletionSource.TrySetResult(result);
                    }
                    catch (Exception ex)
                    {
                        Log.Error().WriteLine(ex, "Error while wait for or processing a request");

                        // Check if cancel was requested, is so set the taskCompletionSource as cancelled
                        if (cancellationToken.IsCancellationRequested)
                        {
                            taskCompletionSource.TrySetCanceled();
                        }
                        else
                        {
                            // Not cancelled, so we use the exception
                            taskCompletionSource.TrySetException(ex);
                        }
                        throw;
                    }
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).ConfigureAwait(false);

            // Return the taskCompletionSource.Task so the caller can await on it
            return taskCompletionSource.Task;
        }
    }
}

#endif