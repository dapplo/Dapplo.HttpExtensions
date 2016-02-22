﻿/*
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

using Dapplo.LogFacade;
using Dapplo.HttpExtensions.Support;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Dapplo.HttpExtensions.ContentConverter
{
	/// <summary>
	/// This can convert HttpContent from/to a SyndicationFeed
	/// </summary>
	public class SyndicationFeedHttpContentConverter : IHttpContentConverter
	{
		private static readonly LogSource Log = new LogSource();
		public static readonly SyndicationFeedHttpContentConverter Instance = new SyndicationFeedHttpContentConverter();

		public int Order => 0;

		public bool CanConvertFromHttpContent<TResult>(HttpContent httpContent) where TResult : class
		{
			return CanConvertFromHttpContent(typeof(TResult), httpContent);
		}

		public bool CanConvertFromHttpContent(Type typeToConvertTo, HttpContent httpContent)
		{
			return typeToConvertTo == typeof(SyndicationFeed);
		}

		public async Task<TResult> ConvertFromHttpContentAsync<TResult>(HttpContent httpContent, CancellationToken token = default(CancellationToken)) where TResult : class
		{
			return await ConvertFromHttpContentAsync(typeof (TResult), httpContent, token).ConfigureAwait(false) as TResult;
		}

		public async Task<object> ConvertFromHttpContentAsync(Type resultType, HttpContent httpContent, CancellationToken token = default(CancellationToken))
		{
			if (!CanConvertFromHttpContent(resultType, httpContent))
			{
				throw new NotSupportedException("CanConvertFromHttpContent resulted in false, this is not supposed to be called.");
			}
			Log.Debug().WriteLine("Retrieving the content as SyndicationFeed, Content-Type: {0}", httpContent.Headers.ContentType);

			using (var contentStream = await httpContent.ReadAsStreamAsync().ConfigureAwait(false))
			{
				using (XmlReader reader = XmlReader.Create(contentStream))
				{
					return SyndicationFeed.Load(reader);
				}
			}
		}

		public bool CanConvertToHttpContent(Type typeToConvert, object content)
		{
			return typeToConvert == typeof(SyndicationFeed);
		}

		public bool CanConvertToHttpContent<TInput>(TInput content) where TInput : class
		{
			return CanConvertToHttpContent(typeof(TInput), content);
		}

		public HttpContent ConvertToHttpContent(Type typeToConvert, object content)
		{
			var feed = content as SyndicationFeed;
			using (var stringWriter = new StringWriter())
			using (var xmlTextWriter = new XmlTextWriter(stringWriter))
			{
				var rss2Formatter = feed.GetRss20Formatter();
				rss2Formatter.WriteTo(xmlTextWriter);
				var httpContent = new StringContent(stringWriter.ToString());
				httpContent.SetContentType($"{MediaTypes.Rss.EnumValueOf()}; charset={stringWriter.Encoding.EncodingName}");
                return httpContent;
			}
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
			if (resultType != typeof(SyndicationFeed))
			{
				return;
			}
			httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypes.Rss.EnumValueOf()));
			Log.Debug().WriteLine("Modified the header(s) of the HttpRequestMessage: Accept: {0}", httpRequestMessage.Headers.Accept);
		}
	}
}
