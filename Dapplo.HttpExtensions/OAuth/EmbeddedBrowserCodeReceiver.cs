using Dapplo.HttpExtensions.Desktop;
using Dapplo.LogFacade;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dapplo.HttpExtensions.OAuth
{
	/// <summary>
	/// This will start an embedded browser to wait for the code
	/// </summary>
	public class EmbeddedBrowserCodeReceiver : IOAuthCodeReceiver
	{
		private static readonly LogSource Log = new LogSource();

		public async Task<IDictionary<string, string>> ReceiveCodeAsync(AuthorizeModes authorizeMode, ICodeReceiverSettings codeReceiverSettings, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (codeReceiverSettings.RedirectUrl == null)
			{
				throw new ArgumentNullException("The EmbeddedBrowserCodeReceiver needs a redirect url.", nameof(codeReceiverSettings.RedirectUrl));
			}
			var formattingObjects = new object[] { codeReceiverSettings }.Concat(codeReceiverSettings.AuthorizeFormattingParameters).ToArray();
			// while the listener is beging starter in the "background", here we prepare opening the browser
			var uriBuilder = new UriBuilder(codeReceiverSettings.AuthorizationUri)
			{
				Query = codeReceiverSettings.AuthorizationUri.QueryToKeyValuePairs()
					.Select(x => new KeyValuePair<string, string>(x.Key, x.Value.FormatWith(formattingObjects)))
					.ToQueryString()
			};

			return await Task.Factory.StartNew(() =>
			{
				var oAuthLoginForm = new OAuthLoginForm(codeReceiverSettings.CloudServiceName, new System.Drawing.Size(codeReceiverSettings.EmbeddedBrowserWidth, codeReceiverSettings.EmbeddedBrowserHeight), uriBuilder.Uri, codeReceiverSettings.RedirectUrl);

				oAuthLoginForm.ShowDialog();
				return oAuthLoginForm.CallbackParameters;
			}, cancellationToken, TaskCreationOptions.None, HttpExtensionsGlobals.UITaskScheduler).ConfigureAwait(false);
		}
	}
}
