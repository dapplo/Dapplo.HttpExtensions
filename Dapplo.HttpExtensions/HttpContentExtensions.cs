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
	along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;
using System.Threading;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// Extensions for the HttpContent
	/// </summary>
	public static class HttpContentExtensions
	{
		// TODO: Make this available in the HttpSettings or HttpBehaviour
		private const int BufferSize = 4096;

		/// <summary>
		/// Get the content as a MemoryStream
		/// </summary>
		/// <param name="httpContent">HttpContent</param>
		/// <param name="httpBehaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>MemoryStream</returns>
		public static async Task<MemoryStream> GetAsMemoryStreamAsync(this HttpContent httpContent, HttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken))
		{
			using (var contentStream = await httpContent.ReadAsStreamAsync().ConfigureAwait(false))
			{
				var memoryStream = new MemoryStream();
				await contentStream.CopyToAsync(memoryStream, 4096, token).ConfigureAwait(false);
				// Make sure the memory stream position is at the beginning,
				// so the processing code can read right away.
				memoryStream.Position = 0;
				return memoryStream;
			}
		}

		/// <summary>
		/// Extension method reading the httpContent to a Typed object, depending on the returned content-type
		/// Currently we support:
		///		Json objects which are annotated with the DataContract/DataMember attributes
		/// </summary>
		/// <typeparam name="TResult">The Type to read into</typeparam>
		/// <param name="httpContent">HttpContent</param>
		/// <param name="httpBehaviour">HttpBehaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>the deserialized object of type T</returns>
		public static async Task<TResult> ReadAsAsync<TResult>(this HttpContent httpContent, HttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken)) where TResult : class
		{
			httpBehaviour = httpBehaviour ?? HttpBehaviour.GlobalHttpBehaviour;
			var contentType = httpContent.ContentType();
			var mediaType = Enum.GetValues(typeof(MediaTypes)).Cast<MediaTypes>().Where(x => x.EnumValueOf() == contentType).FirstOrDefault();
			var resultType = typeof(TResult);

			if (httpBehaviour.ValidateResponseContentType)
			{
				switch (mediaType)
				{
					case MediaTypes.Bmp:
					case MediaTypes.Gif:
					case MediaTypes.Jpeg:
					case MediaTypes.Png:
					case MediaTypes.Tiff:
						if (!resultType.IsAssignableFrom(typeof(Bitmap)) && !resultType.IsAssignableFrom(typeof(BitmapImage)) && !resultType.IsAssignableFrom(typeof(MemoryStream)))
						{
							throw new ArgumentException($"Content-type {contentType} is not assignable from {resultType.Name}");
						}
						break;
					case MediaTypes.Txt:
					case MediaTypes.Html:
					case MediaTypes.Xml:
					case MediaTypes.XmlReadable:
						// Currently only string is supported
						if (resultType != typeof(string))
						{
							throw new ArgumentException($"Content-type {contentType} is readable as string.");
						}
						break;
				}

			}
			if (resultType.IsAssignableFrom(typeof(Bitmap)))
			{
				var memoryStream = await httpContent.GetAsMemoryStreamAsync(httpBehaviour, token).ConfigureAwait(false);
				return new Bitmap(memoryStream) as TResult;
			}

			if (resultType.IsAssignableFrom(typeof(BitmapImage)))
			{
				using (var contentStream = await httpContent.ReadAsStreamAsync().ConfigureAwait(false))
				{
					var bitmap = new BitmapImage();
					bitmap.BeginInit();
					bitmap.StreamSource = contentStream;
					bitmap.CacheOption = BitmapCacheOption.OnLoad;
					bitmap.EndInit();
					bitmap.Freeze();
					// TODO: Check if this is what was wanted
					return bitmap as TResult;
				}
			}
			if (resultType.IsAssignableFrom(typeof(MemoryStream)))
			{
				return await httpContent.GetAsMemoryStreamAsync(httpBehaviour, token).ConfigureAwait(false) as TResult;
			}

			if (resultType == typeof(string))
			{
				return await httpContent.ReadAsStringAsync().ConfigureAwait(false) as TResult;
			}

			// From here we assume that Json is wanted
			if (httpBehaviour.ValidateResponseContentType && mediaType != MediaTypes.Json)
			{
				throw new ArgumentException($"Unknown content-type {contentType}");
			}
			var jsonResponse = await httpContent.ReadAsStringAsync().ConfigureAwait(false);
			return SimpleJson.DeserializeObject<TResult>(jsonResponse);
		}

		/// <summary>
		/// Extension method for parsing the (json) response to a Type object
		/// </summary>
		/// <param name="httpContent">HttpContent</param>
		/// <param name="type">The Type to deserialize to, use null for dynamic json parsing</param>
		/// <returns>the deserialized object of type T</returns>
		public static async Task<object> ReadAsAsync(this HttpContent httpContent, Type type)
		{
			var jsonString = await httpContent.ReadAsStringAsync().ConfigureAwait(false);
			return SimpleJson.DeserializeObject(jsonString, type);
		}

		/// <summary>
		/// Simply return the content type of the HttpContent
		/// </summary>
		/// <param name="httpContent">HttpContent</param>
		/// <returns>string with the content type</returns>
		public static string ContentType(this HttpContent httpContent)
		{
			return httpContent?.Headers?.ContentType?.MediaType;
		}

		/// <summary>
		/// Validate the content type of the HttpContent
		/// If HttpBehaviour.ValidateResponseContentType is true, this will throw an exception if the type doesn't match
		/// </summary>
		/// <param name="httpContent"></param>
		/// <param name="mediaType">MediaTypes enum instance to validate against</param>
		/// <param name="httpBehaviour">HttpBehaviour</param>
		/// <returns>true if it fits, or if validation is turned off</returns>
		public static bool ExpectContentType(this HttpContent httpContent, MediaTypes mediaType, HttpBehaviour httpBehaviour = null)
		{
			httpBehaviour = httpBehaviour ?? HttpBehaviour.GlobalHttpBehaviour;

			var contentType = httpContent.ContentType();

			if (httpBehaviour.ValidateResponseContentType && contentType != mediaType.EnumValueOf())
			{
				throw new InvalidOperationException($"Expected response with Content-Type {MediaTypes.Json.EnumValueOf()} got {contentType}");
			}
			return contentType == mediaType.EnumValueOf();
		}
	}
}
