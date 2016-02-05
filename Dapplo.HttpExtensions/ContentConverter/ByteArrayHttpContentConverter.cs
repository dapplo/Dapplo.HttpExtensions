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

		/// <summary>
		/// Used to create the list of available IHttpContentConverter
		/// Can also be used to access the singleton to change the settings.
		/// </summary>
		public static readonly ByteArrayHttpContentConverter Instance = new ByteArrayHttpContentConverter();

		/// <summary>
		/// Order or priority of the IHttpContentConverter
		/// </summary>
		public int Order => 0;

		/// <summary>
		/// Check if we can convert from the HttpContent to a byte array
		/// </summary>
		/// <typeparam name="TResult">To what type will the result be assigned</typeparam>
		/// <param name="httpContent">HttpContent</param>
		/// <param name="httpBehaviour">IHttpBehaviour</param>
		/// <returns>true if we can convert the HttpContent to a ByteArray</returns>
		public bool CanConvertFromHttpContent<TResult>(HttpContent httpContent, IHttpBehaviour httpBehaviour = null) where TResult : class
		{
			return CanConvertFromHttpContent(typeof(TResult), httpContent, httpBehaviour);
		}

		/// <summary>
		/// Check if we can convert from the HttpContent to a byte array
		/// </summary>
		/// <param name="typeToConvertTo">To what type will the result be assigned</param>
		/// <param name="httpContent">HttpContent</param>
		/// <param name="httpBehaviour">IHttpBehaviour</param>
		/// <returns>true if we can convert the HttpContent to a ByteArray</returns>
		public bool CanConvertFromHttpContent(Type typeToConvertTo, HttpContent httpContent, IHttpBehaviour httpBehaviour = null)
		{
			return typeToConvertTo == typeof(byte[]);
		}

		/// <summary>
		/// Convert a HttpContent to the specified type
		/// </summary>
		/// <typeparam name="TResult">Type to convert to</typeparam>
		/// <param name="httpContent">HttpContent</param>
		/// <param name="httpBehaviour">IHttpBehaviour</param>
		/// <param name="token">CancellationToken, used as the HttpContent might be read async</param>
		/// <returns>Task with result</returns>
		public async Task<TResult> ConvertFromHttpContentAsync<TResult>(HttpContent httpContent, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken)) where TResult : class
		{
			return await ConvertFromHttpContentAsync(typeof(TResult), httpContent, httpBehaviour, token).ConfigureAwait(false) as TResult;
		}

		/// <summary>
		/// Convert a HttpContent to the specified type
		/// </summary>
		/// <param name="resultType">Type to convert to</param>
		/// <param name="httpContent">HttpContent</param>
		/// <param name="httpBehaviour">IHttpBehaviour</param>
		/// <param name="token">CancellationToken, used as the HttpContent might be read async</param>
		/// <returns>Task with result</returns>
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

		/// <summary>
		/// Checks if the content of type typeToConvert can be converted into a HttpContent
		/// </summary>
		/// <param name="typeToConvert">Type to convert to</param>
		/// <param name="content">some object</param>
		/// <param name="httpBehaviour">IHttpBehaviour</param>
		/// <returns>true if a conversion can be made</returns>
		public bool CanConvertToHttpContent(Type typeToConvert, object content, IHttpBehaviour httpBehaviour = null)
		{
			return typeToConvert == typeof(byte[]);
		}

		/// <summary>
		/// Checks if the content of type typeToConvert can be converted into a HttpContent
		/// </summary>
		/// <typeparam name="TInput">type to process</typeparam>
		/// <param name="content">some object</param>
		/// <param name="httpBehaviour">IHttpBehaviour</param>
		/// <returns>true if a conversion can be made</returns>
		public bool CanConvertToHttpContent<TInput>(TInput content, IHttpBehaviour httpBehaviour = null) where TInput : class
		{
			return CanConvertToHttpContent(typeof(TInput), content, httpBehaviour);
		}

		/// <summary>
		/// Actually convert the passed object into a HttpContent
		/// </summary>
		/// <param name="typeToConvert">Type to convert from</param>
		/// <param name="content">Some object</param>
		/// <param name="httpBehaviour">IHttpBehaviour</param>
		/// <returns>HttpContent</returns>
		public HttpContent ConvertToHttpContent(Type typeToConvert, object content, IHttpBehaviour httpBehaviour = null)
		{
			var byteArray = content as byte[];
			return new ByteArrayContent(byteArray);
		}

		/// <summary>
		/// Actually convert the passed object into a HttpContent
		/// </summary>
		/// <typeparam name="TInput">Type to convert from</typeparam>
		/// <param name="content">Some object</param>
		/// <param name="httpBehaviour">IHttpBehaviour</param>
		/// <returns>HttpContent</returns>
		public HttpContent ConvertToHttpContent<TInput>(TInput content, IHttpBehaviour httpBehaviour = null) where TInput : class
		{
			return ConvertToHttpContent(typeof(TInput), content, httpBehaviour);
		}

		/// <summary>
		/// Add Accept-Headers to the HttpRequestMessage, depending on the passt resultType.
		/// This tries to hint the Http server what we can accept, which depends on the type of the return value
		/// </summary>
		/// <param name="resultType">Result type, this where to a conversion from HttpContent is made</param>
		/// <param name="httpRequestMessage">HttpRequestMessage</param>
		/// <param name="httpBehaviour">IHttpBehaviour</param>
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
