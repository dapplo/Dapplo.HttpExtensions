// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using Dapplo.HttpExtensions.Support;

#if NETSTANDARD1_3
using System.Reflection;

#endif

namespace Dapplo.HttpExtensions.ContentConverter
{
    /// <summary>
    ///     This can convert HttpContent from/to a MemoryStream
    /// </summary>
    public class StreamHttpContentConverter : IHttpContentConverter
    {
        private static readonly LogSource Log = new LogSource();

        /// <summary>
        ///     Instance of this IHttpContentConverter for reusing
        /// </summary>
        public static Lazy<StreamHttpContentConverter> Instance { get; } = new Lazy<StreamHttpContentConverter>(() => new StreamHttpContentConverter());

        /// <inheritdoc />
        public int Order => 0;

        /// <inheritdoc />
        public bool CanConvertFromHttpContent(Type typeToConvertTo, HttpContent httpContent)
        {
            return typeToConvertTo != typeof(object) && typeToConvertTo.IsAssignableFrom(typeof(MemoryStream));
        }

        /// <inheritdoc />
        public async Task<object> ConvertFromHttpContentAsync(Type resultType, HttpContent httpContent, CancellationToken cancellationToken = default)
        {
            if (!CanConvertFromHttpContent(resultType, httpContent))
            {
                throw new NotSupportedException("CanConvertFromHttpContent resulted in false, this is not supposed to be called.");
            }
            Log.Debug().WriteLine("Retrieving the content as MemoryStream, Content-Type: {0}", httpContent.Headers.ContentType);
            var httpBehaviour = HttpBehaviour.Current;

            var memoryStream = new MemoryStream();
            using (var contentStream = await httpContent.GetContentStream().ConfigureAwait(false))
            {
                await contentStream.CopyToAsync(memoryStream, httpBehaviour.ReadBufferSize, cancellationToken).ConfigureAwait(false);
            }
            // Make sure the memory stream position is at the beginning,
            // so the processing code can read right away.
            memoryStream.Position = 0;
            return memoryStream;
        }

        /// <inheritdoc />
        public bool CanConvertToHttpContent(Type typeToConvert, object content)
        {
            return typeof(Stream).IsAssignableFrom(typeToConvert);
        }

        /// <inheritdoc />
        public HttpContent ConvertToHttpContent(Type typeToConvert, object content)
        {
            var httpBehaviour = HttpBehaviour.Current;

            var stream = content as Stream;

            // Add progress support, if this is enabled
            if (httpBehaviour.UseProgressStream)
            {
                var progressStream = new ProgressStream(stream)
                {
                    BytesRead = (sender, eventArgs) => { httpBehaviour.UploadProgress?.Invoke((float) eventArgs.StreamPosition / eventArgs.StreamLength); }
                };

                stream = progressStream;
            }
            return new StreamContent(stream);
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
        }
    }
}