// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dapplo.HttpExtensions.Extensions;
using Dapplo.HttpExtensions.OAuth.Desktop;
using Dapplo.Log;

namespace Dapplo.HttpExtensions.OAuth.CodeReceivers
{
    /// <summary>
    ///     This will start an embedded browser to wait for the code
    /// </summary>
    public class EmbeddedBrowserCodeReceiver : IOAuthCodeReceiver
    {
        private static readonly LogSource Log = new LogSource();

        /// <summary>
        ///     Receive the code from an OAuth server
        /// </summary>
        /// <param name="authorizeMode">AuthorizeModes</param>
        /// <param name="codeReceiverSettings">ICodeReceiverSettings</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>IDictionary with information</returns>
        public Task<IDictionary<string, string>> ReceiveCodeAsync(AuthorizeModes authorizeMode, ICodeReceiverSettings codeReceiverSettings,
            CancellationToken cancellationToken = default)
        {
            if (codeReceiverSettings.RedirectUrl is null)
            {
                throw new ArgumentNullException(nameof(codeReceiverSettings.RedirectUrl), "The EmbeddedBrowserCodeReceiver needs a redirect url.");
            }
            // while the listener is beging starter in the "background", here we prepare opening the browser
            var uriBuilder = new UriBuilder(codeReceiverSettings.AuthorizationUri)
            {
                Query = codeReceiverSettings.AuthorizationUri.QueryToKeyValuePairs()
                    .Select(x => new KeyValuePair<string, string>(x.Key, x.Value.FormatWith(codeReceiverSettings)))
                    .ToQueryString()
            };
            Log.Verbose().WriteLine("Opening Uri {0}", uriBuilder.Uri.AbsoluteUri);

            // Needs to run on th UI thread.
            return Task.Factory.StartNew(() =>
            {
                var oAuthLoginForm = new OAuthLoginForm(codeReceiverSettings.CloudServiceName,
                    new Size(codeReceiverSettings.EmbeddedBrowserWidth, codeReceiverSettings.EmbeddedBrowserHeight),
                    uriBuilder.Uri, codeReceiverSettings.RedirectUrl);

                if (oAuthLoginForm.ShowDialog() == DialogResult.OK)
                {
                    return oAuthLoginForm.CallbackParameters;
                }

                return null;
            }, cancellationToken, TaskCreationOptions.None, codeReceiverSettings.UiTaskScheduler ?? TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}