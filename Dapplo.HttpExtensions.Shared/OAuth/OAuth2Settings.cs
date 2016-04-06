//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2015-2016 Dapplo
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

#region using

using System;

#endregion

namespace Dapplo.HttpExtensions.OAuth
{
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
				var expired = false;
				if (!string.IsNullOrEmpty(Token.OAuth2AccessToken) && Token.OAuth2AccessTokenExpires != default(DateTimeOffset))
				{
					expired = DateTimeOffset.Now.AddSeconds(HttpExtensionsGlobals.OAuth2ExpireOffset) > Token.OAuth2AccessTokenExpires;
				}
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
}