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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.Extensions;
using Dapplo.Log;

#endregion

namespace Dapplo.HttpExtensions.ContentConverter
{
	/// <summary>
	///     This can convert HttpContent from/to a string
	/// </summary>
	public class StringHttpContentConverter : IHttpContentConverter
	{
		private static readonly LogSource Log = new LogSource();


		/// <summary>
		/// Instance of this IHttpContentConverter for reusing
		/// </summary>
		public static Lazy<IHttpContentConverter> Instance
		{
			get;
		} = new Lazy<IHttpContentConverter>(() => new StringHttpContentConverter());

		/// <inheritdoc />
		public int Order => int.MaxValue;

		/// <inheritdoc />
		public bool CanConvertFromHttpContent(Type typeToConvertTo, HttpContent httpContent)
		{
			if (typeToConvertTo != typeof (string))
			{
				return false;
			}
			var httpBehaviour = HttpBehaviour.Current;
			var configuration = httpBehaviour.GetConfig<StringConfiguration>();
			// Set ValidateResponseContentType to false to "catch" all
			return !httpBehaviour.ValidateResponseContentType || configuration.SupportedContentTypes.Contains(httpContent.GetContentType());
		}

		/// <inheritdoc />
		public async Task<object> ConvertFromHttpContentAsync(Type resultType, HttpContent httpContent, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!CanConvertFromHttpContent(resultType, httpContent))
			{
				throw new NotSupportedException("CanConvertFromHttpContent resulted in false, this is not supposed to be called.");
			}
			return await httpContent.ReadAsStringAsync().ConfigureAwait(false);
		}

		/// <inheritdoc />
		public bool CanConvertToHttpContent(Type typeToConvert, object content)
		{
			return typeof (string) == typeToConvert;
		}

		/// <inheritdoc />
		public HttpContent ConvertToHttpContent(Type typeToConvert, object content)
		{
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
			var httpBehaviour = HttpBehaviour.Current;
			var configuration = httpBehaviour.GetConfig<StringConfiguration>();
			// add all supported content types
			foreach (var contentType in configuration.SupportedContentTypes)
			{
				httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
			}
			// TODO: Encoding header?
			Log.Debug().WriteLine("Modified the header(s) of the HttpRequestMessage: Accept: {0}", httpRequestMessage.Headers.Accept);
		}
	}
}