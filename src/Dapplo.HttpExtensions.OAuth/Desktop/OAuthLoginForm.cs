// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Dapplo.Log;
using Dapplo.Windows.Dpi.Forms;
using Dapplo.Windows.EmbeddedBrowser;
using Dapplo.Windows.User32;

namespace Dapplo.HttpExtensions.OAuth.Desktop
{
    /// <summary>
    ///     The OAuthLoginForm is used to allow the user to authorize Greenshot with an "Oauth" application
    /// </summary>
    public sealed partial class OAuthLoginForm : DpiAwareForm
    {
        private static readonly LogSource Log = new LogSource();
        private readonly string _callbackUrl;

        /// <summary>
        ///     Constructor for an OAuth login form
        /// </summary>
        /// <param name="browserTitle">title of the form</param>
        /// <param name="size">size of the form</param>
        /// <param name="authorizationLink">Uri for the link</param>
        /// <param name="callbackUrl">Uri for the callback</param>
        public OAuthLoginForm(string browserTitle, Size size, Uri authorizationLink, string callbackUrl)
        {
            // Make sure we use the most recent version of IE
            InternetExplorerVersion.ChangeEmbeddedVersion();

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
        ///     the parameters which were supplied in the uri-callback from the server are stored here
        /// </summary>
        public IDictionary<string, string> CallbackParameters { get; set; }

        /// <summary>
        ///     Check if the dialog result was an ok
        /// </summary>
        public bool IsOk => DialogResult == DialogResult.OK;

        private void Browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Log.Verbose().WriteLine("document completed with url: {0}", e.Url);
            CheckUrl(e.Url);
        }

        private void Browser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            Log.Verbose().WriteLine("Navigated to url: {0}", e.Url);
            CheckUrl(e.Url);
        }

        private void Browser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            Log.Verbose().WriteLine("Navigating to url: {0}", e.Url);
            _addressTextBox.Text = e.Url.ToString();
            CheckUrl(e.Url);
        }

        private void CheckUrl(Uri uri)
        {
            if (uri is null)
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

        /// <summary>
        ///     Make sure the form is visible
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">EventArgs</param>
        private void OAuthLoginForm_Load(object sender, EventArgs e)
        {
            Visible = true;
            User32Api.SetForegroundWindow(Handle);
        }
    }
}
