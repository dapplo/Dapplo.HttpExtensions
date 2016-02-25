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

using Dapplo.HttpExtensions.Support;
using System.ComponentModel;

namespace Dapplo.HttpExtensions.OAuth
{
	/// <summary>
	/// The credentials which should be stored.
	/// This can be used to extend your Dapplo.Config.IIniSection extending interface.
	/// </summary>
	public interface IOAuth1Token
	{
		/// <summary>
		/// Token for accessing OAuth services
		/// </summary>
		[Description("Contains the OAuth token (encrypted)"), TypeConverter(typeof(DelegatingStringEncryptionTypeConverter))]
		string OAuthToken
		{
			get;
			set;
		}

		/// <summary>
		/// OAuth token secret
		/// </summary>
		[Description("OAuth token secret (encrypted)"), TypeConverter(typeof(DelegatingStringEncryptionTypeConverter))]
		string OAuthTokenSecret
		{
			get;
			set;
		}

		/// <summary>
		/// OAuth token verifier
		/// </summary>
		[Description("OAuth token verifier (encrypted)"), TypeConverter(typeof(DelegatingStringEncryptionTypeConverter))]
		string OAuthTokenVerifier
		{
			get;
			set;
		}
	}
}
