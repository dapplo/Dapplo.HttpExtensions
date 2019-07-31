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

#region Usings

using System;
using System.ComponentModel.DataAnnotations;

#endregion

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