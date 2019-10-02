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

using System;
using System.Collections.Generic;
using System.Text;
using Dapplo.HttpExtensions.ContentConverter;
using Dapplo.HttpExtensions.Support;

namespace Dapplo.HttpExtensions
{
    /// <summary>
    ///     These are the globals for some of the important configurable settings
    ///     When a HttpBehaviour is created, some of the values from here will be copied. (unless diffently specified)
    /// </summary>
    public static class HttpExtensionsGlobals
    {
        /// <summary>
        ///     The global default encoding
        /// </summary>
        public static Encoding DefaultEncoding { get; set; } = Encoding.UTF8;

        /// <summary>
        ///     The global list of HttpContent converters
        /// </summary>
        public static IList<IHttpContentConverter> HttpContentConverters { get; set; } = new List<IHttpContentConverter>
        {
#if NET461
            BitmapHttpContentConverter.Instance.Value,
            BitmapSourceHttpContentConverter.Instance.Value,
#endif
#if !NETSTANDARD1_3
            SyndicationFeedHttpContentConverter.Instance.Value,
            XDocumentHttpContentConverter.Instance.Value,
#endif
            ByteArrayHttpContentConverter.Instance.Value,
            FormUriEncodedContentConverter.Instance.Value,
            DefaultJsonHttpContentConverter.Instance.Value,
            StreamHttpContentConverter.Instance.Value,
            StringHttpContentConverter.Instance.Value
        };

        /// <summary>
        /// The GLOBAL default function for the uri escaping, this is Uri.EscapeDataString
        /// Some projects might rather use Uri.EscapeUriString, be careful changing this!
        /// </summary>
        public static Func<string, string> DefaultUriEscapeFunc { get; } = Uri.EscapeDataString;

        /// <summary>
        ///     The global IHttpSettings
        /// </summary>
        public static IHttpSettings HttpSettings { get; set; } = new HttpSettings();

        /// <summary>
        ///     The global JsonSerializer
        /// </summary>
        public static IJsonSerializer JsonSerializer { get; set; }

        /// <summary>
        ///     This offset is used in the OAuth2Setting.IsAccessTokenExpired to check the OAuth2AccessTokenExpires
        ///     Now + this > OAuth2AccessTokenExpires
        /// </summary>
        public static int OAuth2ExpireOffset { get; set; } = 5;

        /// <summary>
        ///     The global read buffer-size
        /// </summary>
        public static int ReadBufferSize { get; set; } = 4096;

        /// <summary>
        ///     Global value for ThrowOnError, see IHttpBehaviour
        /// </summary>
        public static bool ThrowOnError { get; set; } = true;

        /// <summary>
        ///     Global value for UseProgressStream, see IHttpBehaviour
        /// </summary>
        public static bool UseProgressStream { get; set; } = false;

        /// <summary>
        ///     Global validate response content-type
        /// </summary>
        public static bool ValidateResponseContentType { get; set; } = true;
    }
}