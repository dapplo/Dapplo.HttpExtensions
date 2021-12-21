// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.HttpExtensions.OAuth;

/// <summary>
/// Helper method for OAuth
/// </summary>
public static class OAuthExtensions
{
    /// <summary>
    /// Test if the IOAuth1Token has a token set 
    /// </summary>
    /// <param name="oAuth1Token">IOAuth1Token</param>
    /// <returns>true if the specified IOAuth1Token has token information</returns>
    public static bool HasToken(this IOAuth1Token oAuth1Token)
    {
        return !string.IsNullOrEmpty(oAuth1Token.OAuthToken) ||
               !string.IsNullOrEmpty(oAuth1Token.OAuthTokenSecret);
    }

    /// <summary>
    /// Reset the oauth token information
    /// </summary>
    /// <param name="oAuth1Token">IOAuth1Token</param>
    public static void ResetToken(this IOAuth1Token oAuth1Token)
    {
        oAuth1Token.OAuthToken = null;
        oAuth1Token.OAuthTokenSecret = null;
        oAuth1Token.OAuthTokenVerifier = null;
    }

    /// <summary>
    /// Test if the IOAuth2Token has a token set 
    /// </summary>
    /// <param name="oAuth2Token">IOAuth2Token</param>
    /// <returns>true if the specified IOAuth2Token has token information</returns>
    public static bool HasToken(this IOAuth2Token oAuth2Token)
    {
        return !string.IsNullOrEmpty(oAuth2Token.OAuth2AccessToken) ||
               !string.IsNullOrEmpty(oAuth2Token.OAuth2RefreshToken);
    }

    /// <summary>
    /// Reset the oauth token information
    /// </summary>
    /// <param name="oAuth2Token"></param>
    public static void ResetToken(this IOAuth2Token oAuth2Token)
    {
        oAuth2Token.OAuth2AccessToken = null;
        oAuth2Token.OAuth2RefreshToken = null;
        oAuth2Token.OAuth2AccessTokenExpires = default;
    }

    /// <summary>
    /// Test if the access token is expired
    /// </summary>
    /// <param name="oAuth2Token">IOAuth2Token</param>
    /// <returns>True if the token is expired</returns>
    public static bool IsAccessTokenExpired(this IOAuth2Token oAuth2Token)
    {
        if (string.IsNullOrEmpty(oAuth2Token.OAuth2AccessToken) || oAuth2Token.OAuth2AccessTokenExpires == default)
        {
            return false;
        }

        return DateTimeOffset.Now.AddSeconds(HttpExtensionsGlobals.OAuth2ExpireOffset) > oAuth2Token.OAuth2AccessTokenExpires;
    }

}