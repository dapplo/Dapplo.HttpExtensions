/*
	Dapplo - building blocks for desktop applications
	Copyright (C) 2015-2016 Dapplo

	For more information see: http://dapplo.net/
	Dapplo repositories are hosted on GitHub: https://github.com/dapplo

	This file is part of Dapplo.HttpExtensions.

	Dapplo.HttpExtensions is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	Dapplo.HttpExtensions is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with Dapplo.HttpExtensions. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Runtime.Serialization;

namespace Dapplo.HttpExtensions.OAuth
{
	/// <summary>
	/// Container for the OAuth token / refresh response
	/// </summary>
	[DataContract]
	public class OAuth2TokenResponse
	{
		private readonly DateTimeOffset _responseTime = DateTimeOffset.Now;

		/// <summary>
		/// The error
		/// </summary>
		[DataMember(Name = "error")]
		public string Error { get; set; }

		/// <summary>
		/// Details to the error
		/// </summary>
		[DataMember(Name = "error_description")]
		public string ErrorDescription { get; set; }

		/// <summary>
		/// The access token
		/// </summary>
		[DataMember(Name = "access_token")]
		public string AccessToken { get; set; }

		/// <summary>
		/// DateTimeOffset.Now.AddSeconds(expiresIn)
		/// </summary>
		[DataMember(Name = "expires_in")]
		public long ExpiresInSeconds { get; set; }

		/// <summary>
		/// Returns the time that the token expires
		/// </summary>
		public DateTimeOffset Expires => _responseTime.AddSeconds(ExpiresInSeconds);

		/// <summary>
		/// Type for the token
		/// </summary>
		[DataMember(Name = "token_type")]
		public string TokenType { get; set; }

		/// <summary>
		/// Refresh token, used to get a new access token
		/// </summary>
		[DataMember(Name = "refresh_token")]
		public string RefreshToken { get; set; }

		/// <summary>
		/// Test if the error is an invalid grant
		/// </summary>
		public bool IsInvalidGrant => HasError && Error == "invalid_grant";

		/// <summary>
		/// Test if the response has an error
		/// </summary>
		public bool HasError => !string.IsNullOrEmpty(Error);
	}
}
