//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2015-2017 Dapplo
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

using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using Dapplo.HttpExtensions.Extensions;
using Dapplo.HttpExtensions.Support;
using Dapplo.Log;

#endregion

namespace Dapplo.HttpExtensions.ContentConverter
{
    /// <summary>
    ///     This is a configuration class for the BitmapHttpContentConverter
    /// </summary>
    public class BitmapConfiguration : IHttpRequestConfiguration
    {
        private static readonly LogSource Log = new LogSource();
        private int _quality = 80;

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
        public string Name { get; } = nameof(BitmapConfiguration);

        /// <summary>
        ///     Check the parameters for the encoder, like setting Jpg quality
        /// </summary>
        public IList<EncoderParameter> EncoderParameters { get; } = new List<EncoderParameter>();

        /// <summary>
        ///     Specify the format used to write the image to
        /// </summary>
        public ImageFormat Format { get; set; } = ImageFormat.Png;

        /// <summary>
        ///     Default constructor
        /// </summary>
        public BitmapConfiguration()
        {
            // Set the quality from the default
            EncoderParameters.Add(new EncoderParameter(Encoder.Quality, _quality));
        }

        /// <summary>
        ///     Set the quality EncoderParameter, for the Jpg format 0-100
        /// </summary>
        public int Quality
        {
            get { return _quality; }
            set
            {
                _quality = value;
                Log.Verbose().WriteLine("Setting Quality to {0}", value);
                var qualityParameter = EncoderParameters.FirstOrDefault(x => x.Encoder.Guid == Encoder.Quality.Guid);
                if (qualityParameter != null)
                {
                    EncoderParameters.Remove(qualityParameter);
                }
                EncoderParameters.Add(new EncoderParameter(Encoder.Quality, value));
            }
        }
    }
}

#endif