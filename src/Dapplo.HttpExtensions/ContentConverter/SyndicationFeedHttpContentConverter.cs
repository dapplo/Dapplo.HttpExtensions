//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2018 Dapplo
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

#if NET45 || NET46

#region Usings

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Dapplo.HttpExtensions.Extensions;
using Dapplo.HttpExtensions.Support;
using Dapplo.Log;

#endregion

namespace Dapplo.HttpExtensions.ContentConverter
{
    /// <summary>
    ///     This can convert HttpContent from/to a SyndicationFeed
    /// </summary>
    public class SyndicationFeedHttpContentConverter : IHttpContentConverter
    {
        private static readonly LogSource Log = new LogSource();

        /// <summary>
        ///     Instance of this IHttpContentConverter for reusing
        /// </summary>
        public static Lazy<SyndicationFeedHttpContentConverter> Instance { get; } =
            new Lazy<SyndicationFeedHttpContentConverter>(() => new SyndicationFeedHttpContentConverter());

        /// <inheritdoc />
        public int Order => 0;

        /// <inheritdoc />
        public bool CanConvertFromHttpContent(Type typeToConvertTo, HttpContent httpContent)
        {
            return typeToConvertTo == typeof(SyndicationFeed);
        }

        /// <inheritdoc />
        public async Task<object> ConvertFromHttpContentAsync(Type resultType, HttpContent httpContent, CancellationToken cancellationToken = default)
        {
            if (!CanConvertFromHttpContent(resultType, httpContent))
            {
                throw new NotSupportedException("CanConvertFromHttpContent resulted in false, this is not supposed to be called.");
            }
            Log.Debug().WriteLine("Retrieving the content as SyndicationFeed, Content-Type: {0}", httpContent.Headers.ContentType);

            using (var contentStream = await httpContent.ReadAsStreamAsync().ConfigureAwait(false))
            {
                using (var reader = XmlReader.Create(contentStream))
                {
                    return SyndicationFeed.Load(reader);
                }
            }
        }

        /// <inheritdoc />
        public bool CanConvertToHttpContent(Type typeToConvert, object content)
        {
            return typeToConvert == typeof(SyndicationFeed);
        }

        /// <inheritdoc />
        public HttpContent ConvertToHttpContent(Type typeToConvert, object content)
        {
            if (!(content is SyndicationFeed feed))
            {
                return null;
            }
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

        /// <summary>
        ///     Add Accept-Headers to the HttpRequestMessage, depending on the passt resultType.
        ///     This tries to hint the Http server what we can accept, which depends on the type of the return value
        /// </summary>
        /// <param name="resultType">Result type, this where to a conversion from HttpContent is made</param>
        /// <param name="httpRequestMessage">HttpRequestMessage</param>
        public void AddAcceptHeadersForType(Type resultType, HttpRequestMessage httpRequestMessage)
        {
            if (resultType is null)
            {
                throw new ArgumentNullException(nameof(resultType));
            }
            if (httpRequestMessage is null)
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

#endif