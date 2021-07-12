// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.Serialization;

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
        ///     Used for json
        /// </summary>
        [EnumMember(Value = "text/json")] TxtJson,

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