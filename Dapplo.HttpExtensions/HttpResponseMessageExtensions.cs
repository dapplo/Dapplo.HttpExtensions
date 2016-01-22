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
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// Extensions for the HttpResponseMessage class
	/// </summary>
	public static class HttpResponseMessageExtensions
	{
		/// <summary>
		/// Read the response as a string
		/// </summary>
		/// <param name="response">HttpResponseMessage</param>
		/// <param name="httpBehaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>string</returns>
		public static async Task<string> GetAsStringAsync(this HttpResponseMessage response, HttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken))
		{
			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			}
			await response.HandleErrorAsync(httpBehaviour, token).ConfigureAwait(false);
			return null;
		}

		/// <summary>
		/// Get the content as a MemoryStream
		/// </summary>
		/// <param name="response">HttpResponseMessage</param>
		/// <param name="httpBehaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>MemoryStream</returns>
		public static async Task<MemoryStream> GetAsMemoryStreamAsync(this HttpResponseMessage response, HttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken))
		{
			if (response.IsSuccessStatusCode)
			{
				return await response.Content.GetAsMemoryStreamAsync(httpBehaviour, token).ConfigureAwait(false);
			}
			await response.HandleErrorAsync(httpBehaviour, token).ConfigureAwait(false);
			return null;
		}

		/// <summary>
		/// Extension method reading the HttpResponseMessage to a Type object
		/// Currently we support Json objects which are annotated with the DataContract/DataMember attributes
		/// We might support other object, e.g MemoryStream, Bitmap etc soon
		/// </summary>
		/// <typeparam name="TResult">The Type to read into</typeparam>
		/// <param name="response">HttpResponseMessage</param>
		/// <param name="httpBehaviour">HttpBehaviour</param>
		/// <param name="token"></param>
		/// <returns>the deserialized object of type T or default(T)</returns>
		public static async Task<TResult> ReadAsAsync<TResult>(this HttpResponseMessage response, HttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken)) where TResult : class
		{
			if (response.IsSuccessStatusCode)
			{
				var content = response.Content;
				return await content.ReadAsAsync<TResult>(httpBehaviour).ConfigureAwait(false);
			}
			await response.HandleErrorAsync(httpBehaviour, token).ConfigureAwait(false);
			return default(TResult);
		}

		/// <summary>
		/// Simplified error handling, this makes sure the uri & response are logged
		/// </summary>
		/// <param name="responseMessage">HttpResponseMessage</param>
		/// <param name="httpBehaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>string with the error content if HttpBehaviour.ThrowErrorOnNonSuccess = false</returns>
		public static async Task<string> HandleErrorAsync(this HttpResponseMessage responseMessage, HttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken))
		{
			Exception throwException = null;
			string errorContent = null;
			Uri requestUri = null;
			try
			{
				if (!responseMessage.IsSuccessStatusCode)
				{
					requestUri = responseMessage.RequestMessage.RequestUri;
					try
					{
						// try reading the content, so this is not lost
						errorContent = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
					}
					catch
					{
						// Ignore
					}
					responseMessage.EnsureSuccessStatusCode();
				}
			}
			catch (Exception ex)
			{
				throwException = ex;
				throwException.Data.Add("uri", requestUri);
				if (errorContent != null)
				{
					throwException.Data.Add("response", errorContent);
				}
			}
			httpBehaviour = httpBehaviour ?? HttpBehaviour.GlobalHttpBehaviour;
			if (httpBehaviour.ThrowErrorOnNonSuccess && throwException != null)
			{
				throw throwException;
			}
			return errorContent;
		}
	}
}
