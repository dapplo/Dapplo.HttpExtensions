﻿// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if NETFRAMEWORK || NETSTANDARD2_0 || NETCOREAPP3_1 || NET6_0

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
            var taskCompletionSource = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);

            // ReSharper disable once UnusedVariable
            var listenTask = Task.Factory.StartNew(async () =>
            {
                using var httpListener = new HttpListener();
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
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).ConfigureAwait(false);

            // Return the taskCompletionSource.Task so the caller can await on it
            return taskCompletionSource.Task;
        }
    }
}

#endif