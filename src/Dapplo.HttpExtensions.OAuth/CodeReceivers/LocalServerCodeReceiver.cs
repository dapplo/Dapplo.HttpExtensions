// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace Dapplo.HttpExtensions.OAuth.CodeReceivers;

/// <summary>
///     OAuth (2.0) verification code receiver that runs a local server on a free port
///     and waits for a call with the authorization verification code.
/// </summary>
internal class LocalhostCodeReceiver : IOAuthCodeReceiver
{
#pragma warning disable IDE0090 // Use 'new(...)'
    private static readonly LogSource Log = new LogSource();
#pragma warning restore IDE0090 // Use 'new(...)'

    /// <summary>
    ///     HTML code to to return the _browser, default it will try to close the _browser / tab, this won't always work.
    ///     You can use CloudServiceName where you want to show the CloudServiceName from your OAuth2 settings
    /// </summary>
    public string ClosePageResponse { get; set; } = @"<html>
<head><title>OAuth Authentication CloudServiceName</title></head>
<body>
The authentication process received information from CloudServiceName. You can close this browser window / tab if it is not closed automatically...
<script type='text/javascript'>
	window.setTimeout(function() {
		window.open('', '_self', ''); 
		window.close(); 
	}, 1000);
	if (window.opener) {
		window.opener.checkToken();
	}
</script>
</body>
</html>";

    /// <summary>
    ///     The OAuth code receiver
    /// </summary>
    /// <param name="authorizeMode">AuthorizeModes tells you which mode was used to call this</param>
    /// <param name="codeReceiverSettings"></param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>Dictionary with values</returns>
    public async Task<IDictionary<string, string>> ReceiveCodeAsync(AuthorizeModes authorizeMode, ICodeReceiverSettings codeReceiverSettings,
        CancellationToken cancellationToken = default)
    {
        Uri redirectUri;
        if (codeReceiverSettings.RedirectUrl is null)
        {
            redirectUri = new[] {0}.CreateLocalHostUri();
            // TODO: This will create a problem that with the next "authorize" call it will try to use the same Url, while it might not work
            // But not setting it, will create a problem in the replacement
            codeReceiverSettings.RedirectUrl = redirectUri.AbsoluteUri;
        }
        else
        {
            if (!codeReceiverSettings.RedirectUrl.StartsWith("http:"))
            {
                var message = "The LocalServerCodeReceiver only works for http URLs, use a different AuthorizeMode.";
                Log.Error().WriteLine(message);
                throw new ArgumentException(message, nameof(codeReceiverSettings.RedirectUrl));
            }
            redirectUri = new Uri(codeReceiverSettings.RedirectUrl);
        }

        var listenTask = redirectUri.ListenAsync(async httpListenerContext =>
        {
            // Process the request
            var httpListenerRequest = httpListenerContext.Request;
            Log.Debug().WriteLine("Got request {0}", httpListenerRequest.Url);
            // we got the result, parse the Query and set it as a result
            var result = httpListenerRequest.Url.QueryToDictionary();

            try
            {
                var htmlContent = HttpContentFactory.Create(ClosePageResponse.Replace("CloudServiceName", codeReceiverSettings.CloudServiceName));
                htmlContent.SetContentType(MediaTypes.Html.EnumValueOf());
                await httpListenerContext.RespondAsync(htmlContent, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error().WriteLine(ex, "Couldn't write a response");
            }
            return result;
        }, cancellationToken);

        // while the listener is beging starter in the "background", here we prepare opening the browser
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
            UseShellExecute = true
        };
        Process.Start(processStartInfo);

        // Return result of the listening
        return await listenTask.ConfigureAwait(false);
    }
}