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

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.LogFacade;
using Dapplo.HttpExtensions.Support;

namespace Dapplo.HttpExtensions.ContentConverter
{
	/// <summary>
	/// This can convert HttpContent from/to a IEnumerable keyvaluepair string-string or IDictionary string,string
	/// A common usage is the oauth2 token request as described here:
	/// https://developers.google.com/identity/protocols/OAuth2InstalledApp
	/// (the response would be json, that is for the JsonHttpContentConverter)
	/// </summary>
	public class FormUriEncodedContentConverter : IHttpContentConverter
	{
		private static readonly LogSource Log = new LogSource();
		public static readonly FormUriEncodedContentConverter Instance = new FormUriEncodedContentConverter();

		public int Order => 0;

		public bool CanConvertFromHttpContent<TResult>(HttpContent httpContent) where TResult : class
		{
			return CanConvertFromHttpContent(typeof(TResult), httpContent);
		}

		public bool CanConvertFromHttpContent(Type typeToConvertTo, HttpContent httpContent)
		{
			// Check if the return-type can be assigned
			if (typeToConvertTo == typeof(object) || !typeToConvertTo.IsAssignableFrom(typeof(IEnumerable<KeyValuePair<string, string>>)))
			{
				return false;
			}
			return httpContent.GetContentType() == MediaTypes.WwwFormUrlEncoded.EnumValueOf();
		}

		public async Task<TResult> ConvertFromHttpContentAsync<TResult>(HttpContent httpContent, CancellationToken token = default(CancellationToken)) where TResult : class
		{
			return await ConvertFromHttpContentAsync(typeof (TResult), httpContent, token).ConfigureAwait(false) as TResult;
		}

		public async Task<object> ConvertFromHttpContentAsync(Type resultType, HttpContent httpContent, CancellationToken token = default(CancellationToken))
		{
			if (resultType == null)
			{
				throw new ArgumentNullException(nameof(resultType));
			}
			if (httpContent == null)
			{
				throw new ArgumentNullException(nameof(httpContent));
			}
			if (!CanConvertFromHttpContent(resultType, httpContent))
			{
				throw new NotSupportedException("Don't calls this, when the CanConvertFromHttpContent returns false!");
			}
			// Get the string from the content
			var formUriEncodedString = await httpContent.ReadAsStringAsync().ConfigureAwait(false);
			// Use code in the UriParseExtensions to parse the query string
			Log.Debug().WriteLine("Read WwwUriEncodedForm data: {0}", formUriEncodedString);
			// This returns an IEnumerable<KeyValuePair<string, string>>
			return UriParseExtensions.QueryStringToKeyValuePairs(formUriEncodedString);
		}

		public bool CanConvertToHttpContent(Type typeToConvert, object content)
		{
			return typeof(IEnumerable<KeyValuePair<string, string>>).IsAssignableFrom(typeToConvert);
		}

		public bool CanConvertToHttpContent<TInput>(TInput content) where TInput : class
		{
			return CanConvertToHttpContent(typeof(TInput), content);
		}

		public HttpContent ConvertToHttpContent(Type typeToConvert, object content)
		{
			var nameValueCollection = content as IEnumerable<KeyValuePair<string, string>>;
			Log.Debug().WriteLine("Created HttpContent with WwwUriEncodedForm data: {0}", nameValueCollection);
			return new FormUrlEncodedContent(nameValueCollection);
		}

		public HttpContent ConvertToHttpContent<TInput>(TInput content) where TInput : class
		{
			return ConvertToHttpContent(typeof(TInput), content);
		}

		/// <summary>
		/// Add Accept-Headers to the HttpRequestMessage, depending on the passt resultType.
		/// This tries to hint the Http server what we can accept, which depends on the type of the return value
		/// </summary>
		/// <param name="resultType">Result type, this where to a conversion from HttpContent is made</param>
		/// <param name="httpRequestMessage">HttpRequestMessage</param>
		public void AddAcceptHeadersForType(Type resultType, HttpRequestMessage httpRequestMessage)
		{
			if (resultType == null)
			{
				throw new ArgumentNullException(nameof(resultType));
			}
			if (httpRequestMessage == null)
			{
				throw new ArgumentNullException(nameof(httpRequestMessage));
			}
			if (resultType == typeof(object) || !resultType.IsAssignableFrom(typeof(IEnumerable<KeyValuePair<string, string>>)))
			{
				return;
			}
			httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypes.WwwFormUrlEncoded.EnumValueOf()));
			Log.Debug().WriteLine("Modified the header(s) of the HttpRequestMessage: Accept: {0}", httpRequestMessage.Headers.Accept);
		}
	}
}