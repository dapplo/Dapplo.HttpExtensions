//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2015-2016 Dapplo
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

#region using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.Log.Facade;
using Dapplo.Utils.Extensions;

#endregion

namespace Dapplo.HttpExtensions.OAuth
{
	/// <summary>
	///     OAuth (2.0) verification code receiver that depending on the Mode:
	///     OutOfBound: shows a simple dialog and waits for the answer
	///     OutOfBoundAuto: monitors title changes
	/// </summary>
	internal class OutOfBoundCodeReceiver : IOAuthCodeReceiver
	{
		private static readonly LogSource Log = new LogSource();

		/// <summary>
		///     The OAuth code receiver
		/// </summary>
		/// <param name="authorizeMode">which of the AuthorizeModes was used to call the method</param>
		/// <param name="codeReceiverSettings"></param>
		/// <param name="cancellationToken">CancellationToken</param>
		/// <returns>Dictionary with values</returns>
		public async Task<IDictionary<string, string>> ReceiveCodeAsync(AuthorizeModes authorizeMode, ICodeReceiverSettings codeReceiverSettings,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			// Force OOB Uri
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

			// while the listener is beging starter in the "background", here we prepare opening the browser
			var uriBuilder = new UriBuilder(codeReceiverSettings.AuthorizationUri)
			{
				Query = codeReceiverSettings.AuthorizationUri.QueryToKeyValuePairs()
					.Select(x => new KeyValuePair<string, string>(x.Key, x.Value.FormatWith(codeReceiverSettings)))
					.ToQueryString()
			};

			// Get the formatted FormattedAuthUrl
			var authorizationUrl = uriBuilder.Uri;
			Log.Info().WriteLine("Opening a browser with: {0}", authorizationUrl.AbsoluteUri);
			// Open the url in the default browser
			Process.Start(authorizationUrl.AbsoluteUri);

			// TODO: Get the response here, this should be done via the windows title

			await Task.Delay(10*1000, cancellationToken).ConfigureAwait(false);
			// Return result of the listening
			return null;
		}
	}
}