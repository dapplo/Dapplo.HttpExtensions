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

namespace Dapplo.HttpExtensions.OAuth
{
	/// <summary>
	/// Specify the autorize mode that is used to get the token from the cloud service.
	/// Some details are described here: https://developers.google.com/identity/protocols/OAuth2InstalledApp
	/// You can register your implementations with the OAuthHttpMessageHandler
	/// Currently only a LocalServer is in this project
	/// </summary>
	public enum AuthorizeModes
	{
		// Default value, this will give an exception, caller needs to specify another value
		Unknown,
		// Used with a redirect URL to http://localhost:port, this is supported out of the box
		LocalServer,
		// This mode should monitor for title changes, used with redirect_uri of:
		// urn:ietf:wg:oauth:2.0:oob & urn:ietf:wg:oauth:2.0:oob:auto
		// Dapplo.Windows has possibilities to monitor titles
		MonitorTitle,
		// Should ask the user to enter the PIN which is shown in the browser
		Pin,
		// Should open an embedded _browser and catch the redirect
		EmbeddedBrowser,
		// Custom mode 1
		Custom1,
		// Custom mode 2
		Custom2
	}
}
