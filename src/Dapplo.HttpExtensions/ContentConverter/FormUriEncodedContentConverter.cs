// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.Extensions;
using Dapplo.HttpExtensions.Support;
using Dapplo.Log;

#if NETSTANDARD1_3
using System.Reflection;
#endif

namespace Dapplo.HttpExtensions.ContentConverter
{
    /// <summary>
    ///     This can convert HttpContent from/to a IEnumerable keyvaluepair string-string or IDictionary string,string
    ///     A common usage is the oauth2 token request as described here:
    ///     https://developers.google.com/identity/protocols/OAuth2InstalledApp
    ///     (the response would be json, that is for the JsonHttpContentConverter)
    /// </summary>
    public class FormUriEncodedContentConverter : IHttpContentConverter
    {
        private static readonly LogSource Log = new LogSource();

        /// <summary>
        ///     Instance of this IHttpContentConverter for reusing
        /// </summary>
        public static Lazy<FormUriEncodedContentConverter> Instance { get; } = new Lazy<FormUriEncodedContentConverter>(() => new FormUriEncodedContentConverter());

        /// <inheritdoc />
        public int Order => 0;

        /// <inheritdoc />
        public bool CanConvertFromHttpContent(Type typeToConvertTo, HttpContent httpContent)
        {
            // Check if the return-type can be assigned
            if (typeToConvertTo == typeof(object) || !typeToConvertTo.IsAssignableFrom(typeof(IEnumerable<KeyValuePair<string, string>>)))
            {
                return false;
            }
            return httpContent.GetContentType() == MediaTypes.WwwFormUrlEncoded.EnumValueOf();
        }

        /// <inheritdoc />
        public async Task<object> ConvertFromHttpContentAsync(Type resultType, HttpContent httpContent, CancellationToken cancellationToken = default)
        {
            if (resultType is null)
            {
                throw new ArgumentNullException(nameof(resultType));
            }
            if (httpContent is null)
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

        /// <inheritdoc />
        public bool CanConvertToHttpContent(Type typeToConvert, object content)
        {
            return typeof(IEnumerable<KeyValuePair<string, string>>).IsAssignableFrom(typeToConvert);
        }

        /// <inheritdoc />
        public HttpContent ConvertToHttpContent(Type typeToConvert, object content)
        {
            var nameValueCollection = content as IEnumerable<KeyValuePair<string, string>>;
            Log.Debug().WriteLine("Created HttpContent with WwwUriEncodedForm data: {0}", nameValueCollection);
            return new FormUrlEncodedContent(nameValueCollection);
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
            if (resultType == typeof(object) || !resultType.IsAssignableFrom(typeof(IEnumerable<KeyValuePair<string, string>>)))
            {
                return;
            }
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypes.WwwFormUrlEncoded.EnumValueOf()));
            Log.Debug().WriteLine("Modified the header(s) of the HttpRequestMessage: Accept: {0}", httpRequestMessage.Headers.Accept);
        }
    }
}