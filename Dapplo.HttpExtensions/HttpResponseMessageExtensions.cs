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
		/// <param name="throwErrorOnNonSuccess">true to throw an exception when an error occurse, else null is returned</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>string</returns>
		public static async Task<string> GetAsStringAsync(this HttpResponseMessage response, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken))
		{
			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			}
			await response.HandleErrorAsync(throwErrorOnNonSuccess, token).ConfigureAwait(false);
			return null;
		}

		/// <summary>
		/// Get Json from the response
		/// </summary>
		/// <param name="response">HttpResponseMessage</param>
		/// <param name="throwErrorOnNonSuccess">true to throw an exception when an error occurse, else null is returned</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>dynamic created with SimpleJson</returns>
		public static async Task<dynamic> GetAsJsonAsync(this HttpResponseMessage response, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken))
		{
			if (response.IsSuccessStatusCode)
			{
				var jsonString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				return SimpleJson.DeserializeObject(jsonString);
			}
			await response.HandleErrorAsync(throwErrorOnNonSuccess, token).ConfigureAwait(false);
			return null;
		}

		/// <summary>
		/// GetAsJsonAsync&lt;T&gt; will use DataMember / DataContract to parse the object into
		/// </summary>
		/// <typeparam name="T">Type to parse to</typeparam>
		/// <param name="response"></param>
		/// <param name="throwErrorOnNonSuccess">true to throw an exception when an error occurse, else the default for T is returned</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>T created with SimpleJson</returns>
		public static async Task<T> GetAsJsonAsync<T>(this HttpResponseMessage response, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken))
		{
			if (response.IsSuccessStatusCode)
			{
				var jsonString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
				return SimpleJson.DeserializeObject<T>(jsonString);
			}
			await response.HandleErrorAsync(throwErrorOnNonSuccess, token).ConfigureAwait(false);
			return default(T);
		}

		/// <summary>
		/// Get the content as a MemoryStream
		/// </summary>
		/// <param name="response">HttpResponseMessage</param>
		/// <param name="throwErrorOnNonSuccess">true to throw an exception when an error occurse, else null is returned</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>MemoryStream</returns>
		public static async Task<MemoryStream> GetAsMemoryStreamAsync(this HttpResponseMessage response, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken))
		{
			if (response.IsSuccessStatusCode)
			{
				using (var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
				{
					var memoryStream = new MemoryStream();
					await contentStream.CopyToAsync(memoryStream, 4096, token).ConfigureAwait(false);
					// Make sure the memory stream position is at the beginning,
					// so the processing code can read right away.
					memoryStream.Position = 0;
					return memoryStream;
				}
			}
			await response.HandleErrorAsync(throwErrorOnNonSuccess, token).ConfigureAwait(false);
			return null;
		}

		/// <summary>
		/// Simplified error handling, this makes sure the uri & response are logged
		/// </summary>
		/// <param name="responseMessage">HttpResponseMessage</param>
		/// <param name="throwErrorOnNonSuccess">true to throw an exception when an error is returned, else the response text is returned</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>string with the error content if throwErrorOnNonSuccess = false</returns>
		public static async Task<string> HandleErrorAsync(this HttpResponseMessage responseMessage, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken))
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
			if (throwErrorOnNonSuccess && throwException != null)
			{
				throw throwException;
			}
			return errorContent;
		}
	}
}
