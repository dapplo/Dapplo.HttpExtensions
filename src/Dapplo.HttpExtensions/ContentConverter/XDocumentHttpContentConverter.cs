// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if !NETSTANDARD1_3

using System.IO;
using System.Net.Http.Headers;
using System.Xml;
using System.Xml.Linq;
using Dapplo.HttpExtensions.Extensions;
using Dapplo.HttpExtensions.Support;

namespace Dapplo.HttpExtensions.ContentConverter
{
    /// <summary>
    ///     This can convert HttpContent from/to a SyndicationFeed
    /// </summary>
    public class XDocumentHttpContentConverter : IHttpContentConverter
    {
        private static readonly LogSource Log = new LogSource();

        /// <summary>
        ///     Instance of this IHttpContentConverter for reusing
        /// </summary>
        public static Lazy<XDocumentHttpContentConverter> Instance { get; } = new Lazy<XDocumentHttpContentConverter>(() => new XDocumentHttpContentConverter());

        /// <inheritdoc />
        public int Order => 0;

        /// <inheritdoc />
        public bool CanConvertFromHttpContent(Type typeToConvertTo, HttpContent httpContent)
        {
            return typeToConvertTo == typeof(XDocument);
        }

        /// <inheritdoc />
        public async Task<object> ConvertFromHttpContentAsync(Type resultType, HttpContent httpContent, CancellationToken cancellationToken = default)
        {
            if (!CanConvertFromHttpContent(resultType, httpContent))
            {
                throw new NotSupportedException("CanConvertFromHttpContent resulted in false, this is not supposed to be called.");
            }
            Log.Debug().WriteLine("Retrieving the content as XDocument, Content-Type: {0}", httpContent.Headers.ContentType);

            using var contentStream = await httpContent.ReadAsStreamAsync().ConfigureAwait(false);
            return XDocument.Load(contentStream);
        }

        /// <inheritdoc />
        public bool CanConvertToHttpContent(Type typeToConvert, object content)
        {
            return typeToConvert == typeof(XDocument);
        }

        /// <inheritdoc />
        public HttpContent ConvertToHttpContent(Type typeToConvert, object content)
        {
            if (!(content is XDocument xDocument))
            {
                return null;
            }

            using var stringWriter = new StringWriter();
            using var xmlTextWriter = new XmlTextWriter(stringWriter);
            xDocument.WriteTo(xmlTextWriter);
            var httpContent = new StringContent(stringWriter.ToString());
            httpContent.SetContentType($"{MediaTypes.Xml.EnumValueOf()}; charset={stringWriter.Encoding.EncodingName}");
            return httpContent;
        }

        /// <inheritdoc />
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
            if (resultType != typeof(XDocument))
            {
                return;
            }
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypes.Xml.EnumValueOf()));
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypes.XmlReadable.EnumValueOf()));
            Log.Debug().WriteLine("Modified the header(s) of the HttpRequestMessage: Accept: {0}", httpRequestMessage.Headers.Accept);
        }
    }
}

#endif