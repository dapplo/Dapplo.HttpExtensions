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
#region using

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.Extensions;
using Dapplo.HttpExtensions.SharedDesktop.ContentConverter;
using Dapplo.HttpExtensions.Support;
using Dapplo.Log;

#endregion

namespace Dapplo.HttpExtensions.ContentConverter
{
	/// <summary>
	///     This can convert HttpContent from/to a GDI Bitmap
	/// </summary>
	public class BitmapHttpContentConverter : IHttpContentConverter
	{
		private static readonly LogSource Log = new LogSource();

		/// <summary>
		/// Instance of this IHttpContentConverter for reusing
		/// </summary>
		public static Lazy<BitmapHttpContentConverter> Instance
		{
			get;
		} = new Lazy<BitmapHttpContentConverter>(() => new BitmapHttpContentConverter());

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
			if (typeToConvertTo == typeof (object) || !typeToConvertTo.IsAssignableFrom(typeof (Bitmap)))
			{
				return false;
			}
			var httpBehaviour = HttpBehaviour.Current;
			var configuration = httpBehaviour.GetConfig<BitmapConfiguration>();
			return !httpBehaviour.ValidateResponseContentType || configuration.SupportedContentTypes.Contains(httpContent.GetContentType());
		}

		/// <inheritdoc />
		public async Task<object> ConvertFromHttpContentAsync(Type resultType, HttpContent httpContent, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!CanConvertFromHttpContent(resultType, httpContent))
			{
				var exMessage = "CanConvertFromHttpContent resulted in false, ConvertFromHttpContentAsync is not supposed to be called.";
				Log.Error().WriteLine(exMessage);
				throw new NotSupportedException(exMessage);
			}
			var memoryStream = (MemoryStream) await StreamHttpContentConverter.Instance.Value.ConvertFromHttpContentAsync(typeof (MemoryStream), httpContent, cancellationToken).ConfigureAwait(false);
			Log.Debug().WriteLine("Creating a Bitmap from the MemoryStream.");
			return new Bitmap(memoryStream);
		}

		/// <inheritdoc />
		public bool CanConvertToHttpContent(Type typeToConvert, object content)
		{
			return typeof (Bitmap).IsAssignableFrom(typeToConvert) && content != null;
		}

		/// <inheritdoc />
		public HttpContent ConvertToHttpContent(Type typeToConvert, object content)
		{
			if (!CanConvertToHttpContent(typeToConvert, content))
			{
				return null;
			}

			var bitmap = content as Bitmap;
			if (bitmap == null) return null;

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
					BytesRead = (sender, eventArgs) => { httpBehaviour.UploadProgress?.Invoke((float) eventArgs.StreamPosition/eventArgs.StreamLength); }
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
			if (resultType == null)
			{
				throw new ArgumentNullException(nameof(resultType));
			}
			if (httpRequestMessage == null)
			{
				throw new ArgumentNullException(nameof(httpRequestMessage));
			}
			if (resultType == typeof (object) || !resultType.IsAssignableFrom(typeof (Bitmap)))
			{
				return;
			}
			var configuration = HttpBehaviour.Current.GetConfig<BitmapConfiguration>();
			foreach (var supportedContentType in configuration.SupportedContentTypes)
			{
				httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(supportedContentType, configuration.Quality/100d));
			}
			Log.Debug().WriteLine("Modified the header(s) of the HttpRequestMessage: Accept: {0}", httpRequestMessage.Headers.Accept);
		}
	}
}
#endif