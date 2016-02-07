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
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.LogFacade;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// Extensions for the HttpContent
	/// </summary>
	public static class HttpContentExtensions
	{
		private static readonly LogSource Log = new LogSource();

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
			var resultType = typeof(TResult);
			return (TResult) await httpContent.GetAsAsync(resultType, httpBehaviour, token);
		}

		/// <summary>
		/// Extension method reading the httpContent to a Typed object, depending on the returned content-type
		/// Currently we support:
		///		Json objects which are annotated with the DataContract/DataMember attributes
		/// </summary>
		/// <param name="httpContent">HttpContent</param>
		/// <param name="resultType">The Type to read into</param>
		/// <param name="httpBehaviour">HttpBehaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>the deserialized object of type T</returns>
		public static async Task<object> GetAsAsync(this HttpContent httpContent, Type resultType, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken))
		{
			// Quick exit when the requested type is from HttpContent
			if (typeof(HttpContent).IsAssignableFrom(resultType))
			{
				return httpContent;
			}
			httpBehaviour = httpBehaviour ?? new HttpBehaviour();
			var converter = httpBehaviour.HttpContentConverters.OrderBy(x => x.Order).FirstOrDefault(x => x.CanConvertFromHttpContent(resultType, httpContent, httpBehaviour));
			if (converter != null)
			{
				return await converter.ConvertFromHttpContentAsync(resultType, httpContent, httpBehaviour, token).ConfigureAwait(false);
			}

			// For everything that comes here, a fitting converter should be written, or the ValidateResponseContentType can be set to false
			var contentType = httpContent.GetContentType();
			var message = $"Unsupported result type {resultType} / {contentType} combination.";
			Log.Error().WriteLine(message);
			throw new NotSupportedException(message);
		}

		/// <summary>
		/// Simply return the content type of the HttpContent
		/// </summary>
		/// <param name="httpContent">HttpContent</param>
		/// <returns>string with the content type</returns>
		public static string GetContentType(this HttpContent httpContent)
		{
			return httpContent?.Headers?.ContentType?.MediaType;
		}

		/// <summary>
		/// Simply set the content type of the HttpContent
		/// </summary>
		/// <param name="httpContent">HttpContent</param>
		/// <param name="contentType">Content-Type to set</param>
		public static void SetContentType(this HttpContent httpContent, string contentType)
		{
			httpContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
		}
	}
}
