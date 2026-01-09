// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if NETFRAMEWORK || NET10_0

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Dapplo.HttpExtensions.ContentConverter;
using Dapplo.HttpExtensions.Extensions;
using Dapplo.HttpExtensions.Support;
using Dapplo.Log;

namespace Dapplo.HttpExtensions.Wpf.ContentConverter
{
    /// <summary>
    ///     This can convert HttpContent from/to a WPF BitmapImage
    /// </summary>
    public class BitmapSourceHttpContentConverter : IHttpContentConverter
    {
        private static readonly LogSource Log = new LogSource();

        /// <summary>
        ///     Instance of this IHttpContentConverter for reusing
        /// </summary>
        public static Lazy<BitmapSourceHttpContentConverter> Instance { get; } = new Lazy<BitmapSourceHttpContentConverter>(() => new BitmapSourceHttpContentConverter());

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
            if (typeToConvertTo == typeof(object) || !typeToConvertTo.IsAssignableFrom(typeof(BitmapImage)))
            {
                return false;
            }
            var httpBehaviour = HttpBehaviour.Current;
            var configuration = httpBehaviour.GetConfig<BitmapSourceConfiguration>();
            return !httpBehaviour.ValidateResponseContentType || configuration.SupportedContentTypes.Contains(httpContent.GetContentType());
        }

        /// <inheritdoc />
        public async Task<object> ConvertFromHttpContentAsync(Type resultType, HttpContent httpContent, CancellationToken cancellationToken = default)
        {
            if (!CanConvertFromHttpContent(resultType, httpContent))
            {
                throw new NotSupportedException("CanConvertFromHttpContent resulted in false, this is not supposed to be called.");
            }

            using var memoryStream =
                (MemoryStream)
                await StreamHttpContentConverter.Instance.Value.ConvertFromHttpContentAsync(typeof(MemoryStream), httpContent, cancellationToken)
                    .ConfigureAwait(false);
            Log.Debug().WriteLine("Creating a BitmapImage from the MemoryStream.");
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = memoryStream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();

            // This is very important to make the bitmap usable in the UI thread:
            bitmap.Freeze();
            return bitmap;
        }

        /// <inheritdoc />
        public bool CanConvertToHttpContent(Type typeToConvert, object content)
        {
            return typeof(BitmapSource).IsAssignableFrom(typeToConvert) && content != null;
        }

        /// <inheritdoc />
        public HttpContent ConvertToHttpContent(Type typeToConvert, object content)
        {
            if (!CanConvertToHttpContent(typeToConvert, content))
            {
                return null;
            }

            if (content is not BitmapSource bitmapSource)
            {
                return null;
            }

            var httpBehaviour = HttpBehaviour.Current;
            var configuration = httpBehaviour.GetConfig<BitmapSourceConfiguration>();
            Stream stream = new MemoryStream();
            var encoder = configuration.CreateEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(stream);
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
            if (resultType == typeof(object) || !resultType.IsAssignableFrom(typeof(BitmapSource)))
            {
                return;
            }
            var httpBehaviour = HttpBehaviour.Current;

            var configuration = httpBehaviour.GetConfig<BitmapSourceConfiguration>();
            foreach (var supportedContentType in configuration.SupportedContentTypes)
            {
                httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(supportedContentType, configuration.Quality / 100d));
            }

            Log.Debug().WriteLine("Modified the header(s) of the HttpRequestMessage: Accept: {0}", httpRequestMessage.Headers.Accept);
        }
    }
}

#endif