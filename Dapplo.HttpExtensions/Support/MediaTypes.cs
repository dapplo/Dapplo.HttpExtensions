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

using System.Runtime.Serialization;

namespace Dapplo.HttpExtensions.Support
{
	/// <summary>
	/// Use this enum for the creating the accept header or checking the content-type
	/// </summary>
	public enum MediaTypes
	{
		[EnumMember(Value = "application/json")]
		Json,
		[EnumMember(Value = "application/xml")]
		Xml,
		[EnumMember(Value = "text/xml")]
		XmlReadable,
		[EnumMember(Value = "text/html")]
		Html,
		[EnumMember(Value = "text/plain")]
		Txt,
		[EnumMember(Value = "application/x-www-form-urlencoded")]
		WwwFormUrlEncoded,
		[EnumMember(Value = "image/gif")]
		Gif,
		[EnumMember(Value = "image/jpeg")]
		Jpeg,
		[EnumMember(Value = "image/png")]
		Png,
		[EnumMember(Value = "image/bmp")]
		Bmp,
		[EnumMember(Value = "image/tiff")]
		Tiff,
		[EnumMember(Value = "image/x-icon")]
		Icon,
		[EnumMember(Value = "image/svg+xml")]
		Svg
	}
}
