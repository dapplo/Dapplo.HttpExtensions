using Dapplo.HttpExtensions.Desktop;
using Dapplo.LogFacade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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

			return await Task.Factory.StartNew(() =>
			{
				var oAuthLoginForm = new OAuthLoginForm(codeReceiverSettings.CloudServiceName, new System.Drawing.Size(codeReceiverSettings.EmbeddedBrowserWidth, codeReceiverSettings.EmbeddedBrowserHeight), uriBuilder.Uri, codeReceiverSettings.RedirectUrl);

				if (oAuthLoginForm.ShowDialog() == DialogResult.OK)
				{
					return oAuthLoginForm.CallbackParameters;
				}
				return null;
			}, cancellationToken, TaskCreationOptions.None, HttpExtensionsGlobals.UiTaskScheduler).ConfigureAwait(false);
		}
	}
}
