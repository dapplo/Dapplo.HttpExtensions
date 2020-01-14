// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace Dapplo.HttpExtensions.OAuth
{
    /// <summary>
    ///     The credentials which should be stored.
    ///     This can be used to extend your Dapplo.Config.IIniSection extending interface.
    /// </summary>
    public interface IOAuth2Token
    {
        /// <summary>
        ///     Bearer token for accessing OAuth 2 services
        /// </summary>
        [Display(Description = "Contains the OAuth 2 access token (encrypted)")]
        string OAuth2AccessToken { get; set; }

        /// <summary>
        ///     Expire time for the AccessToken, this time (-HttpExtensionsGlobals.OAuth2ExpireOffset) is check to know if a new
        ///     AccessToken needs to be generated with the RefreshToken
        /// </summary>
        [Display(Description = "When does the OAuth 2 AccessToken expire")]
        DateTimeOffset OAuth2AccessTokenExpires { get; set; }

        /// <summary>
        ///     Token used to get a new Access Token
        /// </summary>
        [Display(Description = "Contains the OAuth 2 refresh token (encrypted)")]
        string OAuth2RefreshToken { get; set; }
    }
}