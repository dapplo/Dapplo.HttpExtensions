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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.Support;
using Dapplo.LogFacade;
using Dapplo.Utils.Extensions;

#endregion

namespace Dapplo.HttpExtensions.ContentConverter
{
	/// <summary>
	///     This can convert HttpContent from/to a string
	/// </summary>
	public class StringHttpContentConverter : IHttpContentConverter
	{
		public static readonly StringHttpContentConverter Instance = new StringHttpContentConverter();
		private static readonly LogSource Log = new LogSource();
		private static readonly IList<string> SupportedContentTypes = new List<string>();

		static StringHttpContentConverter()
		{
			// Store the Content-Types this converter supports
			SupportedContentTypes.Add(MediaTypes.Txt.EnumValueOf());
			SupportedContentTypes.Add(MediaTypes.Html.EnumValueOf());
			SupportedContentTypes.Add(MediaTypes.Xml.EnumValueOf());
			SupportedContentTypes.Add(MediaTypes.XmlReadable.EnumValueOf());
		}

		public int Order => int.MaxValue;

		public bool CanConvertFromHttpContent(Type typeToConvertTo, HttpContent httpContent)
		{
			if (typeToConvertTo != typeof (string))
			{
				return false;
			}
			var httpBehaviour = HttpBehaviour.Current;
			// Set ValidateResponseContentType to false to "catch" all
			return !httpBehaviour.ValidateResponseContentType || SupportedContentTypes.Contains(httpContent.GetContentType());
		}

		public async Task<object> ConvertFromHttpContentAsync(Type resultType, HttpContent httpContent, CancellationToken token = default(CancellationToken))
		{
			if (!CanConvertFromHttpContent(resultType, httpContent))
			{
				throw new NotSupportedException("CanConvertFromHttpContent resulted in false, this is not supposed to be called.");
			}
			return await httpContent.ReadAsStringAsync().ConfigureAwait(false);
		}

		public bool CanConvertToHttpContent(Type typeToConvert, object content)
		{
			return typeof (string) == typeToConvert;
		}

		public HttpContent ConvertToHttpContent(Type typeToConvert, object content)
		{
			// TODO:
			return new StringContent(content as string);
		}

		/// <summary>
		///     Add Accept-Headers to the HttpRequestMessage, depending on the passt resultType.
		///     This tries to hint the Http server what we can accept, which depends on the type of the return value
		/// </summary>
		/// <param name="typeToConvertTo">Result type, this where to a conversion from HttpContent is made</param>
		/// <param name="httpRequestMessage">HttpRequestMessage</param>
		public void AddAcceptHeadersForType(Type typeToConvertTo, HttpRequestMessage httpRequestMessage)
		{
			if (typeToConvertTo == null)
			{
				throw new ArgumentNullException(nameof(typeToConvertTo));
			}
			if (httpRequestMessage == null)
			{
				throw new ArgumentNullException(nameof(httpRequestMessage));
			}
			if (typeToConvertTo != typeof (string))
			{
				return;
			}
			// TODO: Encoding header?
			httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypes.Txt.EnumValueOf()));
			Log.Debug().WriteLine("Modified the header(s) of the HttpRequestMessage: Accept: {0}", httpRequestMessage.Headers.Accept);
		}
	}
}