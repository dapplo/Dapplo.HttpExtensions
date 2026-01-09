// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if NETFRAMEWORK || NET10_0 || NET8_0

using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using Dapplo.HttpExtensions.Extensions;
using Dapplo.HttpExtensions.Support;
using Dapplo.Log;

namespace Dapplo.HttpExtensions.WinForms.ContentConverter
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
            get => _quality;
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