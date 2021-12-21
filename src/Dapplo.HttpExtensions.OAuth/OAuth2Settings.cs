// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.HttpExtensions.OAuth;

/// <summary>
///     Settings for the OAuth 2 protocol
/// </summary>
public class OAuth2Settings : BaseOAuthSettings
{
    /// <summary>
    ///     This contains the code returned from the authorization, but only shortly after it was received.
    ///     It will be cleared as soon as it was used.
    /// </summary>
    internal string Code { get; set; }

    /// <summary>
    ///     Return true if the access token is expired.
    ///     Important "side-effect": if true is returned the AccessToken will be set to null!
    /// </summary>
    public bool IsAccessTokenExpired
    {
        get
        {
            var expired = Token.IsAccessTokenExpired();
                
            // Make sure the token is not usable
            if (expired)
            {
                Token.OAuth2AccessToken = null;
            }
            return expired;
        }
    }

    /// <summary>
    ///     The actualy token information, placed in an interface for usage with the Dapplo.Config project
    ///     the OAuth2Token, a default implementation is assigned when the settings are created.
    ///     When using a Dapplo.Config IIniSection for your settings, this can/should be overwritten with your interface, and
    ///     make it extend IOAuth2Token
    /// </summary>
    public IOAuth2Token Token { get; set; } = new OAuth2TokenInformation();
}