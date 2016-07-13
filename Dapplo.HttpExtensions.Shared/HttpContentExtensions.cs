//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2015-2016 Dapplo
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

#region using

#region using

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.Support;
using Dapplo.Log.Facade;
using Dapplo.Utils.Extensions;
using System.IO;
using Dapplo.Utils;

#endregion

#if _PCL_
using System.Reflection;

#endif

#endregion

namespace Dapplo.HttpExtensions
{
	/// <summary>
	///     Extensions for the HttpContent
	/// </summary>
	public static class HttpContentExtensions
	{
		private static readonly LogSource Log = new LogSource();

		/// <summary>
		///     Extension method reading the httpContent to a Typed object, depending on the returned content-type
		///     Currently we support:
		///     Json objects which are annotated with the DataContract/DataMember attributes
		/// </summary>
		/// <typeparam name="TResult">The Type to read into</typeparam>
		/// <param name="httpContent">HttpContent</param>
		/// <param name="httpStatusCode">HttpStatusCode</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>the deserialized object of type T</returns>
		public static async Task<TResult> GetAsAsync<TResult>(this HttpContent httpContent, HttpStatusCode httpStatusCode, CancellationToken token = default(CancellationToken)) where TResult : class
		{
			return (TResult) await httpContent.GetAsAsync(typeof (TResult), httpStatusCode, token).ConfigureAwait(false);
		}

		/// <summary>
		///     Extension method reading the httpContent to a Typed object, depending on the returned content-type
		///     Currently we support:
		///     Json objects which are annotated with the DataContract/DataMember attributes
		/// </summary>
		/// <param name="httpContent">HttpContent</param>
		/// <param name="resultType">The Type to read into</param>
		/// <param name="httpStatusCode">HttpStatusCode</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>the deserialized object of type T</returns>
		public static async Task<object> GetAsAsync(this HttpContent httpContent, Type resultType, HttpStatusCode httpStatusCode, CancellationToken token = default(CancellationToken))
		{
			// Quick exit when the requested type is from HttpContent
			if (typeof (HttpContent).IsAssignableFrom(resultType))
			{
				return httpContent;
			}
			if (httpStatusCode == HttpStatusCode.NoContent)
			{
				return resultType.Default();
			}
			var httpBehaviour = HttpBehaviour.Current;
			var converter = httpBehaviour.HttpContentConverters.OrderBy(x => x.Order).FirstOrDefault(x => x.CanConvertFromHttpContent(resultType, httpContent));
			if (converter != null)
			{
				return await converter.ConvertFromHttpContentAsync(resultType, httpContent, token).ConfigureAwait(false);
			}

			// For everything that comes here, a fitting converter should be written, or the ValidateResponseContentType can be set to false
			var contentType = httpContent.GetContentType();
			Log.Error().WriteLine($"Unsupported result type {resultType} & {contentType} combination.");
			if (MediaTypes.Txt.EnumValueOf() == contentType && Log.IsErrorEnabled())
			{
				Log.Error().WriteLine("Unprocessable result: {0}", await httpContent.ReadAsStringAsync().ConfigureAwait(false));
			}
			return null;
		}

		/// <summary>
		///     Get the Content-stream of the HttpContent, wrap it in ProgressStream if this is specified
		/// </summary>
		/// <param name="httpContent"></param>
		/// <returns>Stream from ReadAsStreamAsync eventually wrapped by ProgressStream</returns>
		public static async Task<Stream> GetContentStream(this HttpContent httpContent)
		{
			var contentStream = await httpContent.ReadAsStreamAsync().ConfigureAwait(false);
			var hasContentLength = httpContent.Headers.Any(h => h.Key.Equals("Content-Length"));
			if (hasContentLength)
			{
				var contentLength = int.Parse(httpContent.Headers.First(h => h.Key.Equals("Content-Length")).Value.First());
				var httpBehaviour = HttpBehaviour.Current;
				// Add progress support, if this is enabled
				if (httpBehaviour.UseProgressStream && contentLength > 0)
				{
					var progressStream = new ProgressStream(contentStream);
					long position = 0;
					progressStream.BytesRead += (sender, eventArgs) =>
					{
						position += eventArgs.BytesMoved;
						if (httpBehaviour.CallProgressOnUiContext)
						{
							UiContext.RunOn(() =>
							{
								httpBehaviour.DownloadProgress?.Invoke((float)position / contentLength);
							});
						}
						else
						{
							httpBehaviour.DownloadProgress?.Invoke((float)position / contentLength);
						}
					};
					contentStream = progressStream;
				}
			}
			return contentStream;
		}

		/// <summary>
		///     Simply return the content type of the HttpContent
		/// </summary>
		/// <param name="httpContent">HttpContent</param>
		/// <returns>string with the content type</returns>
		public static string GetContentType(this HttpContent httpContent)
		{
			return httpContent?.Headers?.ContentType?.MediaType;
		}

		/// <summary>
		///     Simply set the content type of the HttpContent
		/// </summary>
		/// <param name="httpContent">HttpContent</param>
		/// <param name="contentType">Content-Type to set</param>
		public static void SetContentType(this HttpContent httpContent, string contentType)
		{
			httpContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
		}
	}
}