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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Dapplo.Log;
using Dapplo.Windows.Dpi.Forms;

#endregion

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

        /// <summary>
        ///     Make sure the form is visible
        /// </summary>
        /// <param name="sender">sender object</param>
        /// <param name="e">EventArgs</param>
        private void OAuthLoginForm_Load(object sender, EventArgs e)
        {
            Visible = true;
            SetForegroundWindow(Handle);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}

#endif