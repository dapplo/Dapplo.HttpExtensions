//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2015-2017 Dapplo
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

namespace Dapplo.HttpExtensions.OAuth
{
    /// <summary>
    ///     Specify the autorize mode that is used to get the token from the cloud service.
    ///     Some details are described here: https://developers.google.com/identity/protocols/OAuth2InstalledApp
    ///     You can register your implementations with the OAuthHttpMessageHandler
    ///     Currently only a LocalServer is in this project
    /// </summary>
    public enum AuthorizeModes
    {
        /// <summary>
        ///     Default value, this will give an exception, caller needs to specify another value
        /// </summary>
        Unknown,

        /// <summary>
        ///     Used with tests
        /// </summary>
        TestPassThrough,

        /// <summary>
        ///     Used with a redirect URL to http://localhost:port, this is supported out of the box
        /// </summary>
        LocalhostServer,

        /// <summary>
        ///     This mode should show a popup where the user can paste the code, this is used with a redirect_uri of:
        ///     urn:ietf:wg:oauth:2.0:oob
        /// </summary>
        OutOfBound,

        /// <summary>
        ///     This mode should monitor for title changes, used with a redirect_uri of: urn:ietf:wg:oauth:2.0:oob:auto
        ///     Dapplo.Windows has possibilities to monitor titles, this could be used for an implementation
        /// </summary>
        OutOfBoundAuto,

        /// <summary>
        ///     Should ask the user to enter the PIN which is shown in the browser
        /// </summary>
        Pin,

        /// <summary>
        ///     Should open an embedded _browser and catch the redirect
        /// </summary>
        EmbeddedBrowser,

        /// <summary>
        ///     Custom mode 1
        /// </summary>
        Custom1,

        /// <summary>
        ///     Custom mode 2
        /// </summary>
        Custom2
    }
}