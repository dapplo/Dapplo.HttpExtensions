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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Dapplo.HttpExtensions.SpecializedHttpContent
{
	/// <summary>
	/// This can convert HttpContent from/to a GDI Bitmap
	/// </summary>
	public class BitmapHttpContentConverter : IHttpContentConverter
	{
		private static readonly IList<string> SupportedContentTypes = new List<string>();
		public static readonly BitmapHttpContentConverter Instance = new BitmapHttpContentConverter();

		public int Order => 0;

		static BitmapHttpContentConverter()
		{
			SupportedContentTypes.Add(MediaTypes.Bmp.EnumValueOf());
			SupportedContentTypes.Add(MediaTypes.Gif.EnumValueOf());
			SupportedContentTypes.Add(MediaTypes.Jpeg.EnumValueOf());
			SupportedContentTypes.Add(MediaTypes.Png.EnumValueOf());
			SupportedContentTypes.Add(MediaTypes.Tiff.EnumValueOf());
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
			httpBehaviour = httpBehaviour ?? HttpBehaviour.GlobalHttpBehaviour;
			return !httpBehaviour.ValidateResponseContentType || SupportedContentTypes.Contains(httpContent.ContentType());
		}

		public async Task<TResult> ConvertFromHttpContentAsync<TResult>(HttpContent httpContent, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken)) where TResult : class
		{
			return await ConvertFromHttpContentAsync(typeof(TResult), httpContent, httpBehaviour, token) as TResult;
		}

		public async Task<object> ConvertFromHttpContentAsync(Type resultType, HttpContent httpContent, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken))
		{
			if (!CanConvertFromHttpContent(resultType, httpContent, httpBehaviour))
			{
				throw new NotSupportedException("CanConvertFromHttpContent resulted in false, this is not supposed to be called.");
			}
			var memoryStream = await MemoryStreamHttpContentConverter.Instance.ConvertFromHttpContentAsync<MemoryStream>(httpContent, httpBehaviour, token).ConfigureAwait(false);
			return new Bitmap(memoryStream);
		}

		public bool CanConvertToHttpContent<TInput>(TInput content, IHttpBehaviour httpBehaviour = null) where TInput : class
		{
			return CanConvertToHttpContent(typeof(TInput), content, httpBehaviour);
		}

		public bool CanConvertToHttpContent(Type typeToConvert, object content, IHttpBehaviour httpBehaviour = null)
		{
			return typeToConvert == typeof(Bitmap) && content != null;
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
				if (bitmap != null)
				{
					var memoryStream = new MemoryStream();
					bitmap.Save(memoryStream, ImageFormat.Png);
					memoryStream.Seek(0, SeekOrigin.Begin);
					return new StreamContent(memoryStream);
				}
			}
			return null;
		}
	}
}
