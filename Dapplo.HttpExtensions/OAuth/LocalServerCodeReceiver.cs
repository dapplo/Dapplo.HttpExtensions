/*
	Dapplo - building blocks for desktop applications
	Copyright (C) 2015-2016 Dapplo

	For more information see: http://dapplo.net/
	Dapplo repositories are hosted on GitHub: https://github.com/dapplo

	This file is part of Dapplo.HttpExtensions.

	Dapplo.HttpExtensions is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	Dapplo.HttpExtensions is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with Dapplo.HttpExtensions. If not, see <http://www.gnu.org/licenses/>.
 */

using Dapplo.HttpExtensions.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.Factory;
using Dapplo.HttpExtensions.Listener;
using Dapplo.HttpExtensions.Support;

namespace Dapplo.HttpExtensions.OAuth
{
	/// <summary>
	/// OAuth (2.0) verification code receiver that runs a local server on a free port
	/// and waits for a call with the authorization verification code.
	/// </summary>
	public class LocalServerCodeReceiver
	{
		private static readonly LogContext Log = new LogContext();

		/// <summary>
		/// HTML code to to return the _browser, default it will try to close the _browser / tab, this won't always work.
		/// You can use CloudServiceName where you want to show the CloudServiceName from your OAuth2 settings
		/// </summary>
		public string ClosePageResponse { get; set; } = @"<html>
<head><title>OAuth 2.0 Authentication CloudServiceName</title></head>
<body>
The authentication process received information from CloudServiceName. You can close this browser / tab if it is not closed itself...
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
		/// The OAuth code receiver
		/// </summary>
		/// <param name="oauth2Settings"></param>
		/// <param name="cancellationToken"></param>
		/// <returns>Dictionary with values</returns>
		public async Task<IDictionary<string, string>> ReceiveCodeAsync(OAuth2Settings oauth2Settings, CancellationToken cancellationToken = default(CancellationToken))
		{
			Uri redirectUri;
			if (oauth2Settings.RedirectUrl == null)
			{
				redirectUri = new int[] { 0 }.CreateLocalHostUri();
				// Needs to be escaped as this is replaced in the uri that is opened in the browser, and escaped before creating the Uri
				oauth2Settings.RedirectUrl = redirectUri.AbsoluteUri;
			}
			else
			{
				if (!oauth2Settings.RedirectUrl.StartsWith("http:"))
				{
					var message = $"The LocalServerCodeReceiver only works for http URLs, not for {0}, use a different AuthorizeMode.";
					Log.Error().Write(message);
					throw new ArgumentException(message, nameof(oauth2Settings.RedirectUrl));
				}
				redirectUri = new Uri(oauth2Settings.RedirectUrl);
			}

			var listenTask = redirectUri.ListenAsync(async httpListenerContext =>
			{
				// Process the request
				var httpListenerRequest = httpListenerContext.Request;
				Log.Debug().Write("Got request {0}", httpListenerRequest.Url);
				// we got the result, parse the Query and set it as a result
				var result = httpListenerRequest.Url.QueryToDictionary();

				try
				{
					var htmlContent = HttpContentFactory.Create(ClosePageResponse.Replace("CloudServiceName", oauth2Settings.CloudServiceName));
					htmlContent.SetContentType(MediaTypes.Html.EnumValueOf());
					await httpListenerContext.RespondAsync(htmlContent, null, cancellationToken);
				}
				catch (Exception ex)
				{
					Log.Error().Write(ex, "Couldn't write a response");
				}
				return result;
			}, cancellationToken);

			// while the listener is beging starter in the "background", here we prepare opening the browser

			var uriBuilder = new UriBuilder(oauth2Settings.AuthorizationUri)
			{
				Query = oauth2Settings.AuthorizationUri.QueryToKeyValuePairs()
					.Select(x => new KeyValuePair<string, string>(x.Key, x.Value.FormatWith(oauth2Settings)))
					.ToQueryString()
			};

			// Get the formatted FormattedAuthUrl
			var authorizationUrl = uriBuilder.Uri;
			Log.Debug().Write("Opening a browser with: {0}", authorizationUrl.AbsoluteUri);
			// Open the url in the default browser
			Process.Start(authorizationUrl.AbsoluteUri);

			// Return result of the listening
			return await listenTask;
		}
	}
}