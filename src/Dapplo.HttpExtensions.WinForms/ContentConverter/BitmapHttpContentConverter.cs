// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if NET461 || NETCOREAPP3_1

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.ContentConverter;
using Dapplo.HttpExtensions.Extensions;
using Dapplo.HttpExtensions.Support;
using Dapplo.Log;

namespace Dapplo.HttpExtensions.WinForms.ContentConverter
{
    /// <summary>
    ///     This can convert HttpContent from/to a GDI Bitmap
    /// </summary>
    public class BitmapHttpContentConverter : IHttpContentConverter
    {
        private static readonly LogSource Log = new LogSource();

        /// <summary>
        ///     Instance of this IHttpContentConverter for reusing
        /// </summary>
        public static Lazy<BitmapHttpContentConverter> Instance { get; } = new Lazy<BitmapHttpContentConverter>(() => new BitmapHttpContentConverter());

        /// <summary>
        /// Register this ContentConverter to the HttpExtensionsGlobals.HttpContentConverters
        /// </summary>
        /// <returns>false if it was already registered, true if it was added</returns>
        public static bool RegisterGlobally()
        {
            if (!HttpExtensionsGlobals.HttpContentConverters.Contains(Instance.Value))
            {
                HttpExtensionsGlobals.HttpContentConverters.Add(Instance.Value);
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public int Order => 0;

        /// <summary>
        ///     This checks if the HttpContent can be converted to a Bitmap and is assignable to the specified Type
        /// </summary>
        /// <param name="typeToConvertTo">This should be something we can assign Bitmap to</param>
        /// <param name="httpContent">HttpContent to process</param>
        /// <returns>true if it can convert</returns>
        public bool CanConvertFromHttpContent(Type typeToConvertTo, HttpContent httpContent)
        {
            if (typeToConvertTo == typeof(object) || !typeToConvertTo.IsAssignableFrom(typeof(Bitmap)))
            {
                return false;
            }
            var httpBehaviour = HttpBehaviour.Current;
            var configuration = httpBehaviour.GetConfig<BitmapConfiguration>();
            return !httpBehaviour.ValidateResponseContentType || configuration.SupportedContentTypes.Contains(httpContent.GetContentType());
        }

        /// <inheritdoc />
        public async Task<object> ConvertFromHttpContentAsync(Type resultType, HttpContent httpContent, CancellationToken cancellationToken = default)
        {
            if (!CanConvertFromHttpContent(resultType, httpContent))
            {
                var exMessage = "CanConvertFromHttpContent resulted in false, ConvertFromHttpContentAsync is not supposed to be called.";
                Log.Error().WriteLine(exMessage);
                throw new NotSupportedException(exMessage);
            }
            var memoryStream =
                (MemoryStream)
                await StreamHttpContentConverter.Instance.Value.ConvertFromHttpContentAsync(typeof(MemoryStream), httpContent, cancellationToken).ConfigureAwait(false);
            Log.Debug().WriteLine("Creating a Bitmap from the MemoryStream.");
            return new Bitmap(memoryStream);
        }

        /// <inheritdoc />
        public bool CanConvertToHttpContent(Type typeToConvert, object content)
        {
            return typeof(Bitmap).IsAssignableFrom(typeToConvert) && content != null;
        }

        /// <inheritdoc />
        public HttpContent ConvertToHttpContent(Type typeToConvert, object content)
        {
            if (!CanConvertToHttpContent(typeToConvert, content))
            {
                return null;
            }

            if (!(content is Bitmap bitmap))
            {
                return null;
            }

            Stream stream = new MemoryStream();
            var httpBehaviour = HttpBehaviour.Current;

            var configuration = httpBehaviour.GetConfig<BitmapConfiguration>();

            var encoder = ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.FormatID == configuration.Format.Guid);
            if (encoder != null)
            {
                var parameters = new EncoderParameters(configuration.EncoderParameters.Count);
                var index = 0;
                foreach (var encoderParameter in configuration.EncoderParameters)
                {
                    parameters.Param[index++] = encoderParameter;
                }
                bitmap.Save(stream, encoder, parameters);
            }
            else
            {
                var exMessage = $"Can't find an encoder for {configuration.Format}";
                Log.Error().WriteLine(exMessage);
                throw new NotSupportedException(exMessage);
            }
            stream.Seek(0, SeekOrigin.Begin);

            // Add progress support, if this is enabled

            if (httpBehaviour.UseProgressStream)
            {
                var progressStream = new ProgressStream(stream)
                {
                    BytesRead = (sender, eventArgs) => { httpBehaviour.UploadProgress?.Invoke((float) eventArgs.StreamPosition / eventArgs.StreamLength); }
                };
                stream = progressStream;
            }

            var httpContent = new StreamContent(stream);
            httpContent.Headers.Add("Content-Type", "image/" + configuration.Format.ToString().ToLowerInvariant());
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
            if (resultType == typeof(object) || !resultType.IsAssignableFrom(typeof(Bitmap)))
            {
                return;
            }
            var configuration = HttpBehaviour.Current.GetConfig<BitmapConfiguration>();
            foreach (var supportedContentType in configuration.SupportedContentTypes)
            {
                httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(supportedContentType, configuration.Quality / 100d));
            }
            Log.Debug().WriteLine("Modified the header(s) of the HttpRequestMessage: Accept: {0}", httpRequestMessage.Headers.Accept);
        }
    }
}

#endif