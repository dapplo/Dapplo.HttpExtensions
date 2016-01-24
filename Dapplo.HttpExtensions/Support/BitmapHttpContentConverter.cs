/*
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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Dapplo.HttpExtensions.Support
{
	/// <summary>
	/// This can convert HttpContent from/to a GDI Bitmap
	/// </summary>
	public class BitmapHttpContentConverter : IHttpContentConverter
	{
		private static readonly IList<string> SupportedContentTypes = new List<string>();
		public static readonly BitmapHttpContentConverter Instance = new BitmapHttpContentConverter();

		static BitmapHttpContentConverter()
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

		/// <summary>
		/// Check the parameters for the encoder, like setting Jpg quality
		/// </summary>
		public IList<EncoderParameter> EncoderParameters { get; } = new List<EncoderParameter>();

		/// <summary>
		/// Set the quality EncoderParameter, for the Jpg format 0-100
		/// </summary>
		public int Quality
		{
			set
			{
				var qualityParameter = EncoderParameters.FirstOrDefault(x => x.Encoder.Guid == Encoder.Quality.Guid);
				if (qualityParameter != null)
				{
					EncoderParameters.Remove(qualityParameter);
				}
				EncoderParameters.Add(new EncoderParameter(Encoder.Quality, value));
			}
		}

		public BitmapHttpContentConverter()
		{
			// Default quality
			Quality = 80;
		}

		/// <summary>
		/// This checks if the HttpContent can be converted to a Bitmap and is assignable to the specified Type 
		/// </summary>
		/// <typeparam name="TResult">Type to convert to</typeparam>
		/// <param name="httpContent">HttpContent to process</param>
		/// <param name="httpBehaviour">HttpBehaviour</param>
		/// <returns>true if it can convert</returns>
		public bool CanConvertFromHttpContent<TResult>(HttpContent httpContent, IHttpBehaviour httpBehaviour = null) where TResult : class
		{
			return CanConvertFromHttpContent(typeof (TResult), httpContent, httpBehaviour);
		}

		/// <summary>
		/// This checks if the HttpContent can be converted to a Bitmap and is assignable to the specified Type 
		/// </summary>
		/// <param name="typeToConvertTo">This should be something we can assign Bitmap to</param>
		/// <param name="httpContent">HttpContent to process</param>
		/// <param name="httpBehaviour">IHttpBehaviour which supports the behaviour</param>
		/// <returns>true if it can convert</returns>
		public bool CanConvertFromHttpContent(Type typeToConvertTo, HttpContent httpContent, IHttpBehaviour httpBehaviour = null)
		{
			if (!typeToConvertTo.IsAssignableFrom(typeof (Bitmap)))
			{
				return false;
			}
			httpBehaviour = httpBehaviour ?? new HttpBehaviour();
			return !httpBehaviour.ValidateResponseContentType || SupportedContentTypes.Contains(httpContent.ContentType());
		}

		public async Task<TResult> ConvertFromHttpContentAsync<TResult>(HttpContent httpContent, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken)) where TResult : class
		{
			return await ConvertFromHttpContentAsync(typeof(TResult), httpContent, httpBehaviour, token).ConfigureAwait(false) as TResult;
		}

		public async Task<object> ConvertFromHttpContentAsync(Type resultType, HttpContent httpContent, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken))
		{
			if (!CanConvertFromHttpContent(resultType, httpContent, httpBehaviour))
			{
				throw new NotSupportedException("CanConvertFromHttpContent resulted in false, this is not supposed to be called.");
			}
			var memoryStream = await StreamHttpContentConverter.Instance.ConvertFromHttpContentAsync<MemoryStream>(httpContent, httpBehaviour, token).ConfigureAwait(false);
			return new Bitmap(memoryStream);
		}

		public bool CanConvertToHttpContent<TInput>(TInput content, IHttpBehaviour httpBehaviour = null) where TInput : class
		{
			return CanConvertToHttpContent(typeof(TInput), content, httpBehaviour);
		}

		public bool CanConvertToHttpContent(Type typeToConvert, object content, IHttpBehaviour httpBehaviour = null)
		{
			return typeof(Bitmap).IsAssignableFrom(typeToConvert) && content != null;
		}

		public HttpContent ConvertToHttpContent<TInput>(TInput content, IHttpBehaviour httpBehaviour = null) where TInput : class
		{
			return ConvertToHttpContent(typeof(TInput), content, httpBehaviour);
		}

		public HttpContent ConvertToHttpContent(Type typeToConvert, object content, IHttpBehaviour httpBehaviour = null)
		{
			if (CanConvertToHttpContent(typeToConvert, content, httpBehaviour))
			{
				var bitmap = content as Bitmap;
				if (bitmap == null) return null;

				var memoryStream = new MemoryStream();
				var encoder = ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.FormatID == Format.Guid);
				if (encoder != null)
				{
					var parameters = new EncoderParameters(EncoderParameters.Count);
					int index = 0;
					EncoderParameters.ForEach(parameter => parameters.Param[index++] = parameter);
					bitmap.Save(memoryStream, encoder, parameters);
				}
				else
				{
					throw new NotSupportedException($"Can't find an encoder for {Format}");
				}
				memoryStream.Seek(0, SeekOrigin.Begin);
				var httpContent = new StreamContent(memoryStream);
				httpContent.Headers.Add("Content-Type", "image/" + Format.ToString().ToLowerInvariant());
				return httpContent;
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
			if (!resultType.IsAssignableFrom(typeof (Bitmap)))
			{
				return;
			}
			httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypes.Png.EnumValueOf()));
			httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypes.Jpeg.EnumValueOf()));
			httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypes.Tiff.EnumValueOf()));
			httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypes.Bmp.EnumValueOf()));
			httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypes.Gif.EnumValueOf()));
			httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypes.Icon.EnumValueOf()));
		}
	}
}
