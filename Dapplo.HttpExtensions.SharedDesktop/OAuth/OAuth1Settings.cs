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
using System.Net.Http;
using System.Security.Cryptography;

#endregion

namespace Dapplo.HttpExtensions.OAuth
{
	/// <summary>
	///     Settings for the OAuth protocol, if possible this should not be used and OAuth 2.0 is a better choice
	/// </summary>
	public class OAuth1Settings : BaseOAuthSettings
	{
		/// <summary>
		/// The HttpMethod which is used for getting the access token
		/// </summary>
		public HttpMethod AccessTokenMethod { get; set; } = HttpMethod.Get;

		/// <summary>
		///     The URL to get an access token
		/// </summary>
		public Uri AccessTokenUrl { get; set; }

		/// <summary>
		///     OAuth authorize token
		/// </summary>
		public string AuthorizeToken { get; internal set; }

		/// <summary>
		///     If this is set, the value of the verifier will be validated (not null)
		/// </summary>
		public bool CheckVerifier { get; set; } = true;


		/// <summary>
		///     OAuth request token
		/// </summary>
		public string RequestToken { get; internal set; }

		/// <summary>
		///     OAuth request token secret
		/// </summary>
		public string RequestTokenSecret { get; internal set; }

		/// <summary>
		///     The type of signature that is used, mostly this is HMacSha1
		/// </summary>
		public OAuth1SignatureTypes SignatureType { get; set; } = OAuth1SignatureTypes.HMacSha1;

		/// <summary>
		///     For OAuth1SignatureTypes.RsaSha1 set this
		/// </summary>
		public RSACryptoServiceProvider RsaSha1Provider { get; set; }

		/// <summary>
		///     The actualy token information, placed in an interface for usage with the Dapplo.Config project
		///     the OAuthToken, a default implementation is assigned when the settings are created.
		///     When using a Dapplo.Config IIniSection for your settings, this property can/should be overwritten with an instance
		///     of your interface by makeing it extend IOAuthToken
		/// </summary>
		public IOAuth1Token Token { get; set; } = new OAuth1Token();

		/// <summary>
		/// The HttpMethod which is used for getting the token
		/// </summary>
		public HttpMethod TokenMethod { get; set; } = HttpMethod.Post;
	}
}