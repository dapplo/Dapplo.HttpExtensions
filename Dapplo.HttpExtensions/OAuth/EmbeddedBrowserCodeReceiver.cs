//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2017 Dapplo
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
#region using

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using Dapplo.HttpExtensions.Desktop;
using Dapplo.HttpExtensions.Extensions;
using Dapplo.Log;

#endregion

namespace Dapplo.HttpExtensions.OAuth
{
	/// <summary>
	///     This will start an embedded browser to wait for the code
	/// </summary>
	public class EmbeddedBrowserCodeReceiver : IOAuthCodeReceiver
	{
		private static readonly LogSource Log = new LogSource();

		/// <summary>
		/// Receive the code from an OAuth server
		/// </summary>
		/// <param name="authorizeMode">AuthorizeModes</param>
		/// <param name="codeReceiverSettings">ICodeReceiverSettings</param>
		/// <param name="cancellationToken">CancellationToken</param>
		/// <returns>IDictionary with information</returns>
		public Task<IDictionary<string, string>> ReceiveCodeAsync(AuthorizeModes authorizeMode, ICodeReceiverSettings codeReceiverSettings,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			if (codeReceiverSettings.RedirectUrl == null)
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
			return Dispatcher.CurrentDispatcher.Invoke(async () => await Task.Run(() =>
			{
					var oAuthLoginForm = new OAuthLoginForm(codeReceiverSettings.CloudServiceName, new Size(codeReceiverSettings.EmbeddedBrowserWidth, codeReceiverSettings.EmbeddedBrowserHeight), uriBuilder.Uri, codeReceiverSettings.RedirectUrl);

					if (oAuthLoginForm.ShowDialog() == DialogResult.OK)
					{
						return oAuthLoginForm.CallbackParameters;
					}
					return null;
			}, cancellationToken));
		}
	}
}
#endif