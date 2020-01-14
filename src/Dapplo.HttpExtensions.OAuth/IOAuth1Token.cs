// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;

namespace Dapplo.HttpExtensions.OAuth
{
    /// <summary>
    ///     The credentials which should be stored.
    ///     This can be used to extend your Dapplo.Config.IIniSection extending interface.
    /// </summary>
    public interface IOAuth1Token
    {
        /// <summary>
        ///     Token for accessing OAuth services
        /// </summary>
        [Display(Description = "Contains the OAuth token (encrypted)")]
        string OAuthToken { get; set; }

        /// <summary>
        ///     OAuth token secret
        /// </summary>
        [Display(Description = "OAuth token secret (encrypted)")]
        string OAuthTokenSecret { get; set; }

        /// <summary>
        ///     OAuth token verifier
        /// </summary>
        [Display(Description = "OAuth token verifier (encrypted)")]
        string OAuthTokenVerifier { get; set; }
    }
}