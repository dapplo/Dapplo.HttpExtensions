// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if NET461 || NETCOREAPP3_1

using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using Dapplo.HttpExtensions.Extensions;
using Dapplo.HttpExtensions.Support;

namespace Dapplo.HttpExtensions.Wpf.ContentConverter
{
    /// <summary>
    ///     This is a configuration class for the BitmapSourceHttpContentConverter
    /// </summary>
    public class BitmapSourceConfiguration : IHttpRequestConfiguration
    {
        /// <summary>
        ///     Specify the supported content types
        /// </summary>
        public IList<string> SupportedContentTypes { get; } = new List<string>
        {
            MediaTypes.Bmp.EnumValueOf(),
            MediaTypes.Gif.EnumValueOf(),
            MediaTypes.Jpeg.EnumValueOf(),
            MediaTypes.Png.EnumValueOf(),
            MediaTypes.Tiff.EnumValueOf(),
            MediaTypes.Icon.EnumValueOf()
        };

        /// <summary>
        ///     Name of the configuration, this should be unique and usually is the type of the object
        /// </summary>
        public string Name { get; } = nameof(BitmapSourceConfiguration);

        /// <summary>
        ///     Specify the format used to write the image to
        /// </summary>
        public ImageFormat Format { get; set; } = ImageFormat.Png;

        /// <summary>
        ///     Set the quality, for the Jpg format 0-100
        /// </summary>
        public int Quality { get; set; } = 80;

        /// <summary>
        /// This creates a BitmapEncoder
        /// </summary>
        /// <returns>BitmapEncoder</returns>
        public BitmapEncoder CreateEncoder()
        {
            if (Format.Guid == ImageFormat.Bmp.Guid)
            {
                return new BmpBitmapEncoder();
            }
            if (Format.Guid == ImageFormat.Gif.Guid)
            {
                return new GifBitmapEncoder();
            }
            if (Format.Guid == ImageFormat.Jpeg.Guid)
            {
                return new JpegBitmapEncoder
                {
                    QualityLevel = Quality
                };
            }
            if (Format.Guid == ImageFormat.Tiff.Guid)
            {
                return new TiffBitmapEncoder();
            }
            if (Format.Guid == ImageFormat.Png.Guid)
            {
                return new PngBitmapEncoder();
            }
            // There is no IconBitmapEncoder
            // http://www.codeproject.com/Articles/687057/A-High-Quality-IconBitmapEncoder-for-WPF
            // if (Format.Guid == ImageFormat.Icon.Guid)
            throw new NotSupportedException($"Unsupported image format {Format}");
        }
    }
}

#endif