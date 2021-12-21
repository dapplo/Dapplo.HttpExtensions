// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.HttpExtensions.OAuth;

/// <summary>
///     A default implementation for the IOAuthToken, nothing fancy
///     For more information, see the IOAuthToken interface
/// </summary>
public class OAuth1Token : IOAuth1Token
{
    /// <inheritdoc />
    public string OAuthTokenSecret { get; set; }

    /// <inheritdoc />
    public string OAuthToken { get; set; }

    /// <inheritdoc />
    public string OAuthTokenVerifier { get; set; }
}