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
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.Support;
using Dapplo.Log.Facade;
using Dapplo.Utils;

#endregion

#if _PCL_
using System.Reflection;

#endif

#endregion

namespace Dapplo.HttpExtensions.ContentConverter
{
	/// <summary>
	///     This can convert HttpContent from/to a MemoryStream
	/// </summary>
	public class StreamHttpContentConverter : IHttpContentConverter
	{
		private static readonly LogSource Log = new LogSource();

		/// <summary>
		///     Singleton instance for reuse
		/// </summary>
		public static readonly StreamHttpContentConverter Instance = new StreamHttpContentConverter();

		/// <inheritdoc />
		public int Order => 0;

		/// <inheritdoc />
		public bool CanConvertFromHttpContent(Type typeToConvertTo, HttpContent httpContent)
		{
			return typeToConvertTo != typeof (object) && typeToConvertTo.IsAssignableFrom(typeof (MemoryStream));
		}

		/// <inheritdoc />
		public async Task<object> ConvertFromHttpContentAsync(Type resultType, HttpContent httpContent, CancellationToken token = default(CancellationToken))
		{
			if (!CanConvertFromHttpContent(resultType, httpContent))
			{
				throw new NotSupportedException("CanConvertFromHttpContent resulted in false, this is not supposed to be called.");
			}
			Log.Debug().WriteLine("Retrieving the content as MemoryStream, Content-Type: {0}", httpContent.Headers.ContentType);
			var httpBehaviour = HttpBehaviour.Current;

			var memoryStream = new MemoryStream();
			using (var contentStream = await httpContent.GetContentStream())
			{
				await contentStream.CopyToAsync(memoryStream, httpBehaviour.ReadBufferSize, token).ConfigureAwait(false);
			}
			// Make sure the memory stream position is at the beginning,
			// so the processing code can read right away.
			memoryStream.Position = 0;
			return memoryStream;
		}

		/// <inheritdoc />
		public bool CanConvertToHttpContent(Type typeToConvert, object content)
		{
			return typeof (Stream).IsAssignableFrom(typeToConvert);
		}

		/// <inheritdoc />
		public HttpContent ConvertToHttpContent(Type typeToConvert, object content)
		{
			var httpBehaviour = HttpBehaviour.Current;

			var stream = content as Stream;

			// Add progress support, if this is enabled
			if (httpBehaviour.UseProgressStream)
			{
				var progressStream = new ProgressStream(stream);
				progressStream.BytesRead += (sender, eventArgs) =>
				{
					if (httpBehaviour.CallProgressOnUiContext)
					{
						UiContext.RunOn(() =>
						{
							httpBehaviour.UploadProgress?.Invoke((float)eventArgs.StreamPosition / eventArgs.StreamLength);
						});
					}
					else
					{
						httpBehaviour.UploadProgress?.Invoke((float)eventArgs.StreamPosition / eventArgs.StreamLength);
					}
				};

				stream = progressStream;
			}
			return new StreamContent(stream);
		}

		/// <summary>
		///     Add Accept-Headers to the HttpRequestMessage, depending on the passt resultType.
		///     This tries to hint the Http server what we can accept, which depends on the type of the return value
		/// </summary>
		/// <param name="resultType">Result type, this where to a conversion from HttpContent is made</param>
		/// <param name="httpRequestMessage">HttpRequestMessage</param>
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
		}
	}
}