using Dapplo.LogFacade;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Dapplo.HttpExtensions.Desktop
{
	/// <summary>
	/// The OAuthLoginForm is used to allow the user to authorize Greenshot with an "Oauth" application
	/// </summary>
	public sealed partial class OAuthLoginForm : Form
	{
		private static readonly LogSource Log = new LogSource();
		private readonly string _callbackUrl;
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetForegroundWindow(IntPtr hWnd);

		public IDictionary<string, string> CallbackParameters { get; set; }

		public bool IsOk => DialogResult == DialogResult.OK;

		public OAuthLoginForm(string browserTitle, Size size, Uri authorizationLink, string callbackUrl)
		{
			_callbackUrl = callbackUrl;
			InitializeComponent();
			ClientSize = size;
			Text = browserTitle;
			_addressTextBox.Text = authorizationLink.ToString();

			// The script errors are suppressed by using the ExtendedWebBrowser
			_browser.ScriptErrorsSuppressed = false;
			_browser.DocumentCompleted += Browser_DocumentCompleted;
			_browser.Navigated += Browser_Navigated;
			_browser.Navigating += Browser_Navigating;
			_browser.Navigate(authorizationLink);
			Load += OAuthLoginForm_Load;
		}

		/// <summary>
		/// Make sure the form is visible
		/// </summary>
		/// <param name="sender">sender object</param>
		/// <param name="e">EventArgs</param>
		private void OAuthLoginForm_Load(object sender, EventArgs e)
		{
			Visible = true;
			SetForegroundWindow(Handle);
		}

		private void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			Log.Verbose().WriteLine("document completed with url: {0}", e.Url);
			CheckUrl(e.Url);
		}

		private void Browser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
		{
			Log.Verbose().WriteLine("Navigating to url: {0}", e.Url);
			_addressTextBox.Text = e.Url.ToString();
			CheckUrl(e.Url);
		}

		private void Browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
		{
			Log.Verbose().WriteLine("Navigated to url: {0}", e.Url);
			CheckUrl(e.Url);
		}

		private void CheckUrl(Uri uri)
		{
			if (uri == null)
			{
				return;
			}
			if (uri.AbsoluteUri.Contains("error"))
			{
				DialogResult = DialogResult.Abort;
			}
			else if (uri.AbsoluteUri.StartsWith(_callbackUrl))
			{
				CallbackParameters = uri.QueryToDictionary();
				DialogResult = DialogResult.OK;
			}
		}
	}
}
