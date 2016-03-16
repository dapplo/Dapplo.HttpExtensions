﻿/*
	Dapplo - building blocks for desktop applications
	Copyright (C) 2015-2016 Dapplo

	For more information see: http://dapplo.net/
	Dapplo repositories are hosted on GitHub: https://github.com/dapplo

	This file is part of Dapplo.HttpExtensions.

	Dapplo.HttpExtensions is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	Dapplo.HttpExtensions is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with Dapplo.HttpExtensions. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Dapplo.HttpExtensions.Support;
using Dapplo.LogFacade;

namespace Dapplo.HttpExtensions.ContentConverter
{
	/// <summary>
	/// This can convert HttpContent from/to a WPF BitmapImage
	/// </summary>
	public class BitmapSourceHttpContentConverter : IHttpContentConverter
	{
		private static readonly IList<string> SupportedContentTypes = new List<string>();
		private static readonly LogSource Log = new LogSource();
		public static readonly BitmapSourceHttpContentConverter Instance = new BitmapSourceHttpContentConverter();

		static BitmapSourceHttpContentConverter()
		{
			SupportedContentTypes.Add(MediaTypes.Bmp.EnumValueOf());
			SupportedContentTypes.Add(MediaTypes.Gif.EnumValueOf());
			SupportedContentTypes.Add(MediaTypes.Jpeg.EnumValueOf());
			SupportedContentTypes.Add(MediaTypes.Png.EnumValueOf());
			SupportedContentTypes.Add(MediaTypes.Tiff.EnumValueOf());
		}

		public int Order => 0;

		public ImageFormat Format
		{
			get;
			set;
		} = ImageFormat.Png;

		public int Quality { get; set; } = 80;

		private BitmapEncoder CreateEncoder()
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
				return new JpegBitmapEncoder();
			}
			if (Format.Guid == ImageFormat.Tiff.Guid)
			{
				return new TiffBitmapEncoder();
			}
			if (Format.Guid == ImageFormat.Png.Guid)
			{
				return new PngBitmapEncoder();
			}
			throw new NotSupportedException($"Unsupported image format {Format}");
		}

		/// <summary>
		/// This checks if the HttpContent can be converted to a Bitmap and is assignable to the specified Type 
		/// </summary>
		/// <param name="typeToConvertTo">This should be something we can assign Bitmap to</param>
		/// <param name="httpContent">HttpContent to process</param>
		/// <returns>true if it can convert</returns>
		public bool CanConvertFromHttpContent(Type typeToConvertTo, HttpContent httpContent)
		{
			if (typeToConvertTo == typeof(object) || !typeToConvertTo.IsAssignableFrom(typeof (BitmapImage)))
			{
				return false;
			}
			var httpBehaviour = HttpBehaviour.Current;
			return !httpBehaviour.ValidateResponseContentType || SupportedContentTypes.Contains(httpContent.GetContentType());
		}

		public async Task<object> ConvertFromHttpContentAsync(Type resultType, HttpContent httpContent, CancellationToken token = default(CancellationToken))
		{
			if (!CanConvertFromHttpContent(resultType, httpContent))
			{
				throw new NotSupportedException("CanConvertFromHttpContent resulted in false, this is not supposed to be called.");
			}
			using (var memoryStream = (MemoryStream)await StreamHttpContentConverter.Instance.ConvertFromHttpContentAsync(typeof(MemoryStream), httpContent, token).ConfigureAwait(false))
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

		public bool CanConvertToHttpContent(Type typeToConvert, object content)
		{
			return typeof(BitmapSource).IsAssignableFrom(typeToConvert)  && content != null;
		}

		public HttpContent ConvertToHttpContent(Type typeToConvert, object content)
		{
			if (CanConvertToHttpContent(typeToConvert, content))
			{
				var bitmapSource = content as BitmapSource;
				if (bitmapSource != null)
				{
					var memoryStream = new MemoryStream();
					var encoder = CreateEncoder();
					encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
					encoder.Save(memoryStream);
					memoryStream.Seek(0, SeekOrigin.Begin);
					HttpContent httpContent;
					var httpBehaviour = HttpBehaviour.Current;
					if (httpBehaviour.UseProgressStreamContent)
					{
						httpContent = new ProgressStreamContent(memoryStream, httpBehaviour.UploadProgress);
					}
					else
					{
						httpContent = new StreamContent(memoryStream);
					}
					httpContent.Headers.Add("Content-Type", "image/" + Format.ToString().ToLowerInvariant());
					return httpContent;
				}
			}
			return null;
		}

		public void AddAcceptHeadersForType(Type resultType, HttpRequestMessage httpRequestMessage)
		{
			if (resultType == null)
			{
				throw new ArgumentNullException(nameof(resultType));
			}
			if (httpRequestMessage == null)
			{
				throw new ArgumentNullException(nameof(httpRequestMessage));
			}
			if (resultType == typeof(object) || !resultType.IsAssignableFrom(typeof(BitmapSource)))
			{
				return;
			}
			httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypes.Png.EnumValueOf()));
			httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypes.Jpeg.EnumValueOf(), Quality/100d));
			httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypes.Tiff.EnumValueOf()));
			httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypes.Bmp.EnumValueOf()));
			httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypes.Gif.EnumValueOf()));
			httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypes.Icon.EnumValueOf()));
			Log.Debug().WriteLine("Modified the header(s) of the HttpRequestMessage: Accept: {0}", httpRequestMessage.Headers.Accept);
		}
	}
}