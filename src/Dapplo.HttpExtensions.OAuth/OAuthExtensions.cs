//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2019 Dapplo
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

using System;

namespace Dapplo.HttpExtensions.OAuth
{
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
}
