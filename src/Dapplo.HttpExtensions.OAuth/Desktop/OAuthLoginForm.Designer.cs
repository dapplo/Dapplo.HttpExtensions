/*
	Dapplo - building blocks for desktop applications
	Copyright (C) 2016-2018 Dapplo

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

using Dapplo.Windows.EmbeddedBrowser;

#if NET45 || NET46
namespace Dapplo.HttpExtensions.OAuth.Desktop
{
	sealed partial class OAuthLoginForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._addressTextBox = new System.Windows.Forms.TextBox();
			this._browser = new ExtendedWebBrowser();
			this.SuspendLayout();
			// 
			// _addressTextBox
			// 
			this._addressTextBox.Cursor = System.Windows.Forms.Cursors.Arrow;
			this._addressTextBox.Dock = System.Windows.Forms.DockStyle.Top;
			this._addressTextBox.Enabled = false;
			this._addressTextBox.Location = new System.Drawing.Point(0, 0);
			this._addressTextBox.Name = "addressTextBox";
			this._addressTextBox.Size = new System.Drawing.Size(595, 20);
			this._addressTextBox.TabIndex = 3;
			this._addressTextBox.TabStop = false;
			// 
			// _browser
			// 
			_browser.Dock = System.Windows.Forms.DockStyle.Fill;
			this._browser.Location = new System.Drawing.Point(0, 20);
			this._browser.MinimumSize = new System.Drawing.Size(100, 100);
			this._browser.Name = "browser";
			this._browser.Size = new System.Drawing.Size(595, 295);
			this._browser.TabIndex = 4;
			// 
			// OAuthLoginForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(595, 315);
			this.Controls.Add(this._browser);
			this.Controls.Add(this._addressTextBox);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OAuthLoginForm";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

#endregion

		private System.Windows.Forms.TextBox _addressTextBox;
		private ExtendedWebBrowser _browser;
	}
}
#endif