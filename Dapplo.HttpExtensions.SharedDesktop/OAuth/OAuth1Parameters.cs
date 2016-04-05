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

using System.Runtime.Serialization;

#endregion

namespace Dapplo.HttpExtensions.OAuth
{
	public enum OAuth1Parameters
	{
		[EnumMember(Value = "oauth_consumer_key")] ConsumerKey,
		[EnumMember(Value = "oauth_callback")] Callback,
		[EnumMember(Value = "oauth_version")] Version,
		[EnumMember(Value = "oauth_signature_method")] SignatureMethod,
		[EnumMember(Value = "oauth_timestamp")] Timestamp,
		[EnumMember(Value = "oauth_nonce")] Nonce,
		[EnumMember(Value = "oauth_token")] Token,
		[EnumMember(Value = "oauth_verifier")] Verifier,
		[EnumMember(Value = "oauth_token_secret")] TokenSecret,
		[EnumMember(Value = "oauth_signature")] Signature
	}
}