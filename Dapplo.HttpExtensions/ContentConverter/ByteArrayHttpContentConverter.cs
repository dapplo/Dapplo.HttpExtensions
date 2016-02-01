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

using Dapplo.LogFacade;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Dapplo.HttpExtensions.ContentConverter
{
	/// <summary>
	/// This can convert HttpContent from/to a byte[]
	/// </summary>
	public class ByteArrayHttpContentConverter : IHttpContentConverter
	{
		private static readonly LogSource Log = new LogSource();
		public static readonly ByteArrayHttpContentConverter Instance = new ByteArrayHttpContentConverter();

		public int Order => 0;

		public bool CanConvertFromHttpContent<TResult>(HttpContent httpContent, IHttpBehaviour httpBehaviour = null) where TResult : class
		{
			return CanConvertFromHttpContent(typeof(TResult), httpContent, httpBehaviour);
		}

		public bool CanConvertFromHttpContent(Type typeToConvertTo, HttpContent httpContent, IHttpBehaviour httpBehaviour = null)
		{
			return typeToConvertTo == typeof(byte[]);
		}

		public async Task<TResult> ConvertFromHttpContentAsync<TResult>(HttpContent httpContent, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken)) where TResult : class
		{
			return await ConvertFromHttpContentAsync(typeof(TResult), httpContent, httpBehaviour, token).ConfigureAwait(false) as TResult;
		}

		public async Task<object> ConvertFromHttpContentAsync(Type resultType, HttpContent httpContent, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken))
		{
			httpBehaviour = httpBehaviour ?? new HttpBehaviour();
			if (!CanConvertFromHttpContent(resultType, httpContent, httpBehaviour))
			{
				throw new NotSupportedException("CanConvertFromHttpContent resulted in false, this is not supposed to be called.");
			}
			Log.Debug().WriteLine("Retrieving the content as byte[], Content-Type: {0}", httpContent.Headers.ContentType);

			return await httpContent.ReadAsByteArrayAsync().ConfigureAwait(false);
		}

		public bool CanConvertToHttpContent(Type typeToConvert, object content, IHttpBehaviour httpBehaviour = null)
		{
			return typeToConvert == typeof(byte[]);
		}

		public bool CanConvertToHttpContent<TInput>(TInput content, IHttpBehaviour httpBehaviour = null) where TInput : class
		{
			return CanConvertToHttpContent(typeof(TInput), content, httpBehaviour);
		}

		public HttpContent ConvertToHttpContent(Type typeToConvert, object content, IHttpBehaviour httpBehaviour = null)
		{
			var byteArray = content as byte[];
			return new ByteArrayContent(byteArray);
		}

		public HttpContent ConvertToHttpContent<TInput>(TInput content, IHttpBehaviour httpBehaviour = null) where TInput : class
		{
			return ConvertToHttpContent(typeof(TInput), content, httpBehaviour);
		}

		public void AddAcceptHeadersForType(Type resultType, HttpRequestMessage httpRequestMessage, IHttpBehaviour httpBehaviour = null)
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
