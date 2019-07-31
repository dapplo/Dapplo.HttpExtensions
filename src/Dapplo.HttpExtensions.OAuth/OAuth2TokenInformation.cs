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

#endregion

namespace Dapplo.HttpExtensions.OAuth
{
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
}