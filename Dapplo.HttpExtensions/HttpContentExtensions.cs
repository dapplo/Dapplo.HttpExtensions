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
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using Dapplo.HttpExtensions.Support;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// Extensions for the HttpContent
	/// </summary>
	public static class HttpContentExtensions
	{
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
		public static async Task<TResult> GetAsAsync<TResult>(this HttpContent httpContent, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken)) where TResult : class
		{
			return await httpContent.GetAsAsync(typeof(TResult), httpBehaviour, token) as TResult;
		}

		/// <summary>
		/// Extension method for parsing the (json) response to a Type object
		/// </summary>
		/// <param name="httpContent">HttpContent</param>
		/// <param name="resultType">The Type to deserialize to, use null for dynamic json parsing</param>
		/// <param name="httpBehaviour">IHttpBehaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>the deserialized object of type T</returns>
		public static async Task<object> GetAsAsync(this HttpContent httpContent, Type resultType, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken))
		{
			if (resultType.IsInstanceOfType(httpContent))
			{
				return httpContent;
			}
			httpBehaviour = httpBehaviour ?? new HttpBehaviour();
			var converter = httpBehaviour.HttpContentConverters.OrderBy(x => x.Order).FirstOrDefault(x => x.CanConvertFromHttpContent(resultType, httpContent, httpBehaviour));
			if (converter != null)
			{
				return await converter.ConvertFromHttpContentAsync(resultType, httpContent, httpBehaviour, token).ConfigureAwait(false);
			}
			if (resultType == typeof(string))
			{
				return await httpContent.ReadAsStringAsync().ConfigureAwait(false);
			}

			throw new NotSupportedException($"Unsupported result type {resultType}");
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
		public static bool ExpectContentType(this HttpContent httpContent, MediaTypes mediaType, IHttpBehaviour httpBehaviour = null)
		{
			httpBehaviour = httpBehaviour ?? new HttpBehaviour();

			var contentType = httpContent.ContentType();

			if (httpBehaviour.ValidateResponseContentType && contentType != mediaType.EnumValueOf())
			{
				throw new InvalidOperationException($"Expected response with Content-Type {MediaTypes.Json.EnumValueOf()} got {contentType}");
			}
			return contentType == mediaType.EnumValueOf();
		}
	}
}
