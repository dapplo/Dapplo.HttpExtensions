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

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// Extensions for the HttpContent
	/// </summary>
	public static class HttpContentExtensions
	{
		/// <summary>
		/// Extension method reading the httpContent to a Type object
		/// Currently we support Json objects which are annotated with the DataContract/DataMember attributes
		/// We might support other object, e.g MemoryStream, Bitmap etc soon
		/// </summary>
		/// <typeparam name="TResult">The Type to read into</typeparam>
		/// <param name="httpContent">HttpContent</param>
		/// <param name="HttpBehaviour">behaviour</param>
		/// <returns>the deserialized object of type T</returns>
		public static async Task<TResult> ReadAsAsync<TResult>(this HttpContent httpContent, HttpBehaviour httpBehaviour = null)
		{
			var jsonString = await httpContent.ReadAsStringAsync().ConfigureAwait(false);
			return SimpleJson.DeserializeObject<TResult>(jsonString);
		}

		/// <summary>
		/// Extension method for parsing the (json) response to a Type object
		/// </summary>
		/// <param name="httpContent">HttpContent</param>
		/// <param name="type">The Type to deserialize to, use null for dynamic json parsing</typeparam>
		/// <returns>the deserialized object of type T</returns>
		public static async Task<object> ReadAsAsync(this HttpContent httpContent, Type type)
		{
			var jsonString = await httpContent.ReadAsStringAsync().ConfigureAwait(false);
			return SimpleJson.DeserializeObject(jsonString, type);
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

			var contentType = httpContent.Headers.ContentType.MediaType;

			if (httpBehaviour.ValidateResponseContentType && contentType != mediaType.EnumValueOf())
			{
				throw new InvalidOperationException($"Expected response with Content-Type {MediaTypes.Json.EnumValueOf()} got {contentType}");
			}
			return contentType == mediaType.EnumValueOf();
		}
	}
}
