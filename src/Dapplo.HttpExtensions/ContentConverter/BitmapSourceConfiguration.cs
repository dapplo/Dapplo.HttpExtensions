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
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using Dapplo.HttpExtensions.Extensions;
using Dapplo.HttpExtensions.Support;

#endregion

namespace Dapplo.HttpExtensions.ContentConverter
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

        /// <inheritdoc />
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