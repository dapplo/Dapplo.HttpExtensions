// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.HttpExtensions.OAuth;

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