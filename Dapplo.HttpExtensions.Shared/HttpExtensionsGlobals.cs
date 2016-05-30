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
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.ContentConverter;
using Dapplo.HttpExtensions.Support;
using Dapplo.LogFacade;

#endregion

namespace Dapplo.HttpExtensions
{
	/// <summary>
	///     These are the globals for some of the important configurable settings
	///     When a HttpBehaviour is created, some of the values from here will be copied. (unless diffently specified)
	/// </summary>
	public static class HttpExtensionsGlobals
	{
		private static readonly LogSource Log = new LogSource();

		/// <summary>
		///     The glocal value which specifies if Progress actions are called with UiContext.RunOn
		/// </summary>
		public static bool CallProgressOnUiContext { get; internal set; } = true;

		/// <summary>
		///     The global default encoding
		/// </summary>
		public static Encoding DefaultEncoding { get; set; } = Encoding.UTF8;

		/// <summary>
		///     The global list of HttpContent converters
		/// </summary>
		public static IList<IHttpContentConverter> HttpContentConverters { get; set; } = new List<IHttpContentConverter>
		{
#if !_PCL_
			BitmapHttpContentConverter.Instance,
			BitmapSourceHttpContentConverter.Instance,
			SyndicationFeedHttpContentConverter.Instance,
			XDocumentHttpContentConverter.Instance,
#endif
			ByteArrayHttpContentConverter.Instance,
			FormUriEncodedContentConverter.Instance,
			JsonHttpContentConverter.Instance,
			StreamHttpContentConverter.Instance,
			StringHttpContentConverter.Instance
		};

		/// <summary>
		///     The global IHttpSettings
		/// </summary>
		public static IHttpSettings HttpSettings { get; set; } = new HttpSettings();

		/// <summary>
		///     The global JsonSerializer
		/// </summary>
		public static IJsonSerializer JsonSerializer { get; set; } = new SimpleJsonSerializer();

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