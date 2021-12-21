// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.HttpExtensions.OAuth;

/// <summary>
///     A default implementation for the IOAuth2Token, nothing fancy
///     For more information, see the IOAuth2Token interface
/// </summary>
internal class OAuth2TokenInformation : IOAuth2Token
{
    public string OAuth2AccessToken { get; set; }

    public DateTimeOffset OAuth2AccessTokenExpires { get; set; }

    public string OAuth2RefreshToken { get; set; }
}