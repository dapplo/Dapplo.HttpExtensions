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
	/// </summary>
	public enum AuthorizeModes
	{
		Unknown, // Will give an exception, caller needs to specify another value
		LocalServer, // Will specify a redirect URL to http://localhost:port/authorize, while having a HttpListener
		MonitorTitle, // Not implemented yet: Will monitor for title changes
		Pin, // Not implemented yet: Will ask the user to enter the shown PIN
		EmbeddedBrowser // Will open into an embedded _browser (OAuthLoginForm), and catch the redirect
	}
}
