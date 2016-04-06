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
	/// <summary>
	///     This enum is used internally for the mapping of the field names
	/// </summary>
	internal enum OAuth2Fields
	{
		[EnumMember(Value = "refresh_token")] RefreshToken,
		[EnumMember(Value = "code")] Code,
		[EnumMember(Value = "client_id")] ClientId,
		[EnumMember(Value = "client_secret")] ClientSecret,
		[EnumMember(Value = "grant_type")] GrantType,
		[EnumMember(Value = "redirect_uri")] RedirectUri,
		[EnumMember(Value = "error")] Error,
		[EnumMember(Value = "error_description")] ErrorDescription
	}
}