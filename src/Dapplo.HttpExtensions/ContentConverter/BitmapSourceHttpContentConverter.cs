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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Dapplo.HttpExtensions.Extensions;
using Dapplo.HttpExtensions.Support;
using Dapplo.Log;

#endregion

namespace Dapplo.HttpExtensions.ContentConverter
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
            using (
                var memoryStream =
                    (MemoryStream)
                    await StreamHttpContentConverter.Instance.Value.ConvertFromHttpContentAsync(typeof(MemoryStream), httpContent, cancellationToken)
                        .ConfigureAwait(false))
            {
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

            if (!(content is BitmapSource bitmapSource))
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