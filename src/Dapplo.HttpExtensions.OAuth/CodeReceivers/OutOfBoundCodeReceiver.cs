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

#if NET45 || NET46

#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.Extensions;
using Dapplo.Log;
using Dapplo.Windows.Desktop;

#endregion

namespace Dapplo.HttpExtensions.OAuth.CodeReceivers
{
    /// <summary>
    ///     OAuth (2.0) verification code receiver that depending on the Mode:
    ///     OutOfBound: shows a simple dialog and waits for the answer
    ///     OutOfBoundAuto: monitors title changes
    ///
    /// This implementation is for OutOfBoundAuto, but due to issues with the title, expects a prefix || and suffix ||
    /// Alternatively, when this is not available, it just tries to parse the title as a query.
    /// </summary>
    internal class OutOfBoundCodeReceiver : IOAuthCodeReceiver
    {
        private static readonly LogSource Log = new LogSource();
        private static readonly Regex QueryPartOfTitleRegEx = new Regex(@".*\|\|(?<query>.*)\|\|.*", RegexOptions.IgnoreCase);

        /// <summary>
        ///     The OAuth code receiver
        /// </summary>
        /// <param name="authorizeMode">which of the AuthorizeModes was used to call the method</param>
        /// <param name="codeReceiverSettings"></param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>Dictionary with values</returns>
        public async Task<IDictionary<string, string>> ReceiveCodeAsync(AuthorizeModes authorizeMode, ICodeReceiverSettings codeReceiverSettings,
            CancellationToken cancellationToken = default)
        {
            // Force OOB Uri, if nothing is set
            if (string.IsNullOrEmpty(codeReceiverSettings.RedirectUrl))
            {
                switch (authorizeMode)
                {
                    case AuthorizeModes.OutOfBound:
                        codeReceiverSettings.RedirectUrl = "urn:ietf:wg:oauth:2.0:oob";
                        break;
                    case AuthorizeModes.OutOfBoundAuto:
                        codeReceiverSettings.RedirectUrl = "urn:ietf:wg:oauth:2.0:oob:auto";
                        break;
                    default:
                        throw new NotSupportedException($"Only {AuthorizeModes.OutOfBound} and {AuthorizeModes.OutOfBoundAuto} are supported modes for this receiver");
                }
            }

            var uriBuilder = new UriBuilder(codeReceiverSettings.AuthorizationUri)
            {
                Query = codeReceiverSettings.AuthorizationUri.QueryToKeyValuePairs()
                    .Select(x => new KeyValuePair<string, string>(x.Key, x.Value.FormatWith(codeReceiverSettings)))
                    .ToQueryString()
            };

            // Get the formatted FormattedAuthUrl
            var authorizationUrl = uriBuilder.Uri;
            Log.Debug().WriteLine("Opening a browser with: {0}", authorizationUrl.AbsoluteUri);
            // Open the url in the default browser
            Process.Start(authorizationUrl.AbsoluteUri);

            Log.Debug().WriteLine("Waiting until a window gets a title with the state {0}", codeReceiverSettings.State);
            // Wait until a window get's a title which contains the state object
            var title = await WinEventHook.WindowTileChangeObservable()
                .Select(info => InteropWindowFactory.CreateFor(info.Handle).Fill())
                .Where(interopWindow => !string.IsNullOrEmpty(interopWindow?.Caption))
                .Where(interopWindow => interopWindow.Caption.Contains(codeReceiverSettings.State))
                .Select(interopWindow => interopWindow.Caption)
                .Take(1).ToTask(cancellationToken);


            Log.Debug().WriteLine("Got title {0}", title);
            if (string.IsNullOrEmpty(title))
            {
                return new Dictionary<string, string>();
            }

            var match = QueryPartOfTitleRegEx.Match(title);
            if (!match.Success)
            {
                return UriParseExtensions.QueryStringToDictionary(title);
            }

            var queryParameters = match.Groups["query"]?.Value;
            if (string.IsNullOrEmpty(queryParameters))
            {
                return new Dictionary<string, string>();
            }
            Log.Debug().WriteLine("Query parameters: {0}", queryParameters);
            // Return result of the listening
            return UriParseExtensions.QueryStringToDictionary(queryParameters);
        }
    }
}

#endif