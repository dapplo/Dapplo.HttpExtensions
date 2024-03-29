﻿// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Dapplo.Windows.Desktop;

namespace Dapplo.HttpExtensions.OAuth.CodeReceivers;

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
#pragma warning disable IDE0090 // Use 'new(...)'
    private static readonly LogSource Log = new LogSource();
#pragma warning restore IDE0090 // Use 'new(...)'
    private static readonly Regex QueryPartOfTitleRegEx = new(@".*\|\|(?<query>.*)\|\|.*", RegexOptions.IgnoreCase);

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
            codeReceiverSettings.RedirectUrl = authorizeMode switch
            {
                AuthorizeModes.OutOfBound => "urn:ietf:wg:oauth:2.0:oob",
                AuthorizeModes.OutOfBoundAuto => "urn:ietf:wg:oauth:2.0:oob:auto",
                _ => throw new NotSupportedException(
                    $"Only {AuthorizeModes.OutOfBound} and {AuthorizeModes.OutOfBoundAuto} are supported modes for this receiver")
            };
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
        var processStartInfo = new ProcessStartInfo(authorizationUrl.AbsoluteUri)
        {
            CreateNoWindow = true,
            UseShellExecute = true
        };
        Process.Start(processStartInfo);

        Log.Debug().WriteLine("Waiting until a window gets a title with the state {0}", codeReceiverSettings.State);
        // Wait until a window get's a title which contains the state object
        var title = await WinEventHook.WindowTitleChangeObservable()
            .Select(info => InteropWindowFactory.CreateFor(info.Handle).Fill())
            .Where(interopWindow => !string.IsNullOrEmpty(interopWindow?.Caption))
            .Where(interopWindow => interopWindow.Caption.Contains(codeReceiverSettings.State))
            // Skip temporary titles, where the redirect URL os briefly seen
            .Where(interopWindow => interopWindow?.Caption.Contains(codeReceiverSettings.RedirectUrl) != true)
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