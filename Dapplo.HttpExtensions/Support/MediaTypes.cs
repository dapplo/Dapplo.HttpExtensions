//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2015-2017 Dapplo
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

using System.Runtime.Serialization;

#endregion

namespace Dapplo.HttpExtensions.Support
{
    /// <summary>
    ///     Use this enum for the creating the accept header or checking the content-type
    /// </summary>
    public enum MediaTypes
    {
        /// <summary>
        ///     Used for Json
        /// </summary>
        [EnumMember(Value = "application/json")] Json,

        /// <summary>
        ///     Used for Xml
        /// </summary>
        [EnumMember(Value = "application/xml")] Xml,

        /// <summary>
        ///     Used for Xml
        /// </summary>
        [EnumMember(Value = "text/xml")] XmlReadable,

        /// <summary>
        ///     Used for Html
        /// </summary>
        [EnumMember(Value = "text/html")] Html,

        /// <summary>
        ///     Used for Text
        /// </summary>
        [EnumMember(Value = "text/plain")] Txt,

        /// <summary>
        ///     Used for a www form which is url encoded
        /// </summary>
        [EnumMember(Value = "application/x-www-form-urlencoded")] WwwFormUrlEncoded,

        /// <summary>
        ///     Image type gif
        /// </summary>
        [EnumMember(Value = "image/gif")] Gif,

        /// <summary>
        ///     Image type jpeg
        /// </summary>
        [EnumMember(Value = "image/jpeg")] Jpeg,

        /// <summary>
        ///     Image type png
        /// </summary>
        [EnumMember(Value = "image/png")] Png,

        /// <summary>
        ///     Image type bmp
        /// </summary>
        [EnumMember(Value = "image/bmp")] Bmp,

        /// <summary>
        ///     Image type tiff
        /// </summary>
        [EnumMember(Value = "image/tiff")] Tiff,

        /// <summary>
        ///     Image type Icon (.ico)
        /// </summary>
        [EnumMember(Value = "image/x-icon")] Icon,

        /// <summary>
        ///     Image type SVG
        /// </summary>
        [EnumMember(Value = "image/svg+xml")] Svg,

        /// <summary>
        ///     Rss feed (not Atom)
        /// </summary>
        [EnumMember(Value = "application/rss+xml")] Rss
    }
}