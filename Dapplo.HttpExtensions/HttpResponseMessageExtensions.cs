/*
 * dapplo - building blocks for desktop applications
 * Copyright (C) 2015 Robin Krom
 * 
 * For more information see: http://dapplo.net/
 * dapplo repositories are hosted on GitHub: https://github.com/dapplo
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
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
		/// ReadAsStringAsync
		/// </summary>
		/// <param name="response"></param>
		/// <param name="throwErrorOnNonSuccess"></param>
		/// <param name="token"></param>
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
		/// GetAsJsonAsync
		/// </summary>
		/// <param name="response"></param>
		/// <param name="throwErrorOnNonSuccess"></param>
		/// <param name="token"></param>
		/// <returns>dynamic created with SimpleJson</returns>
		public static async Task<dynamic> GetJsonAsync(this HttpResponseMessage response, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken))
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
		/// Get the content as a MemoryStream
		/// </summary>
		/// <param name="response">HttpResponseMessage</param>
		/// <param name="throwErrorOnNonSuccess">bool</param>
		/// <param name="token"></param>
		/// <returns>MemoryStream</returns>
		public static async Task<MemoryStream> GetAsMemoryStreamAsync(this HttpResponseMessage response, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken))
		{
			if (response.IsSuccessStatusCode)
			{
				using (var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
				{
					var memoryStream = new MemoryStream();
					await contentStream.CopyToAsync(memoryStream, 4096, token).ConfigureAwait(false);
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
		/// <param name="token">CancellationToken</param>
		/// <param name="throwErrorOnNonSuccess">bool</param>
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
