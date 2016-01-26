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

using Dapplo.HttpExtensions.Internal;
using Dapplo.HttpExtensions.Support;
using System;
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
		private static readonly LogContext Log = LogContext.Create();

		/// <summary>
		/// Extension method reading the HttpResponseMessage to a Type object
		/// Currently we support Json objects which are annotated with the DataContract/DataMember attributes
		/// We might support other object, e.g MemoryStream, Bitmap etc soon
		/// </summary>
		/// <typeparam name="TResult">The Type to read into</typeparam>
		/// <param name="httpResponseMessage">HttpResponseMessage</param>
		/// <param name="httpBehaviour">HttpBehaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>the deserialized object of type T or default(T)</returns>
		public static async Task<TResult> GetAsAsync<TResult>(this HttpResponseMessage httpResponseMessage, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken)) where TResult : class
		{
			if (typeof (TResult) == typeof (HttpResponseMessage))
			{
				return httpResponseMessage as TResult;
			}
			await httpResponseMessage.HandleErrorAsync(httpBehaviour, token).ConfigureAwait(false);
			if (httpResponseMessage.IsSuccessStatusCode)
			{
				var httpContent = httpResponseMessage.Content;
				var result = await httpContent.GetAsAsync<TResult>(httpBehaviour, token).ConfigureAwait(false);
				// Make sure the httpContent is only disposed when it's not the return type
				if (!typeof(HttpContent).IsAssignableFrom(typeof(TResult)))
				{
					httpContent?.Dispose();
				}

				return result;
			}
			return default(TResult);
		}

		/// <summary>
		/// Extension method reading the HttpResponseMessage to a Type object
		/// Currently we support Json objects which are annotated with the DataContract/DataMember attributes
		/// We might support other object, e.g MemoryStream, Bitmap etc soon
		/// </summary>
		/// <typeparam name="TResponse">The Type to read into</typeparam>
		/// <typeparam name="TErrorResponse">The type to read into when an error occurs</typeparam>
		/// <param name="httpResponseMessage">HttpResponseMessage</param>
		/// <param name="httpBehaviour">HttpBehaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>HttpResponse</returns>
		public static async Task<IHttpResponse<TResponse,TErrorResponse>> GetAsAsync<TResponse, TErrorResponse>(this HttpResponseMessage httpResponseMessage, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken)) where TResponse : class where TErrorResponse : class
		{
			var response = new HttpResponse<TResponse, TErrorResponse>
			{
				StatusCode = httpResponseMessage.StatusCode,
				Headers = httpResponseMessage.Headers
			};

			var httpContent = httpResponseMessage.Content;
			if (httpResponseMessage.IsSuccessStatusCode)
			{
				// Write log for success
				Log.Debug().Write("Http response {0} ({1}) from {2}", (int)httpResponseMessage.StatusCode, httpResponseMessage.StatusCode, httpResponseMessage?.RequestMessage?.RequestUri);
				response.Response = await httpContent.GetAsAsync<TResponse>(httpBehaviour, token).ConfigureAwait(false);
				// Make sure the httpContent is only disposed when it's not the return type
				if (!typeof(HttpContent).IsAssignableFrom(typeof(TResponse)))
				{
					httpContent?.Dispose();
				}
			}
			else
			{
				// Write log if an error occured.
				Log.Error().Write("Http response {0} ({1}) from {2}", (int)httpResponseMessage.StatusCode, httpResponseMessage.StatusCode, httpResponseMessage?.RequestMessage?.RequestUri);
				response.ErrorResponse = await httpContent.GetAsAsync<TErrorResponse>(httpBehaviour, token).ConfigureAwait(false);

				// Make sure the httpContent is only disposed when it's not the return type
				if (!typeof(HttpContent).IsAssignableFrom(typeof(TErrorResponse)))
				{
					httpContent?.Dispose();
				}
			}
			return response;
		}

		/// <summary>
		/// Simplified error handling, this makes sure the uri & response are logged
		/// </summary>
		/// <param name="httpResponseMessage">HttpResponseMessage</param>
		/// <param name="httpBehaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>string with the error content if HttpBehaviour.ThrowErrorOnNonSuccess = false</returns>
		public static async Task<string> HandleErrorAsync(this HttpResponseMessage httpResponseMessage, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken))
		{
			if (httpResponseMessage == null)
			{
				throw new ArgumentNullException(nameof(httpResponseMessage));
			}
			Exception throwException = null;
			string errorContent = null;
			Uri requestUri = httpResponseMessage.RequestMessage?.RequestUri;
			try
			{
				if (!httpResponseMessage.IsSuccessStatusCode)
				{
					try
					{
						// try reading the content, so this is not lost
						errorContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
					}
					catch (Exception ex)
					{
						Log.Debug().Write("Error while reading the error content: {0}", ex.Message);
					}
					// Write log if an error occured.
					Log.Error().Write("Http response {0} ({1}) from {2}, details from website: {3}", (int)httpResponseMessage.StatusCode, httpResponseMessage.StatusCode, requestUri, errorContent);

					httpResponseMessage.EnsureSuccessStatusCode();
				}
				else
				{
					// Write log for success
					Log.Debug().Write("Http response {0} ({1}) from {2}", (int)httpResponseMessage.StatusCode, httpResponseMessage.StatusCode, requestUri);
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

			httpBehaviour = httpBehaviour ?? new HttpBehaviour();
			if (httpBehaviour.ThrowOnError && throwException != null)
			{
				throw throwException;
			}
			return errorContent;
		}
	}
}
