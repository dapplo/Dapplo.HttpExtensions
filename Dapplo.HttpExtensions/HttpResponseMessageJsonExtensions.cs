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
using System.Threading;
using System.Threading.Tasks;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// Extensions for the HttpResponseMessage class which handle Json
	/// </summary>
	public static class HttpResponseMessageJsonExtensions
	{
		/// <summary>
		/// Internal extension method for parsing the response to a Json object
		/// </summary>
		/// <typeparam name="TResult">The type to deserialize to</typeparam>
		/// <param name="httpResponseMessage">HttpResponseMessage</param>
		/// <returns>the deserialized object of type T</returns>
		private static async Task<TResult> DeserializeObject<TResult>(this HttpResponseMessage httpResponseMessage)
		{
			var jsonString = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
			return SimpleJson.DeserializeObject<TResult>(jsonString);
		}

		/// <summary>
		/// Internal extension method for checking IsSuccessStatusCode and eventually a validation is done to see if the returned content-type fits
		/// </summary>
		/// <param name="httpResponseMessage">HttpResponseMessage</param>
		/// <param name="behaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <returns>true</returns>
		private static bool IsSuccessAndContentTypeJson(this HttpResponseMessage httpResponseMessage, HttpBehaviour behaviour = null)
		{
			if (httpResponseMessage.IsSuccessStatusCode)
			{
				// Make sure we have HttpBehaviour
				behaviour = behaviour ?? HttpBehaviour.GlobalHttpBehaviour;

				if (behaviour.ValidateResponseContentType && httpResponseMessage.Content.Headers.ContentType.MediaType != MediaTypes.Json.EnumValueOf())
				{
					throw new InvalidOperationException($"Expected content-type of ${MediaTypes.Json.EnumValueOf()} got {httpResponseMessage.Content.Headers.ContentType.MediaType}");
				}
				return true;
			}
			return false;
		}


		/// <summary>
		/// Get Json from the httpResponseMessage
		/// </summary>
		/// <param name="httpResponseMessage">HttpResponseMessage</param>
		/// <param name="behaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>dynamic created with SimpleJson</returns>
		public static async Task<dynamic> GetAsJsonAsync(this HttpResponseMessage httpResponseMessage, HttpBehaviour behaviour = null, CancellationToken token = default(CancellationToken))
		{
			if (httpResponseMessage.IsSuccessAndContentTypeJson())
			{
				return await httpResponseMessage.DeserializeObject<dynamic>().ConfigureAwait(false);
			}
			await httpResponseMessage.HandleErrorAsync(behaviour, token).ConfigureAwait(false);
			return null;
		}

		/// <summary>
		/// GetAsJsonAsync will use DataMember / DataContract to parse the object into
		/// </summary>
		/// <typeparam name="TResult">Type to parse to</typeparam>
		/// <param name="httpResponseMessage"></param>
		/// <param name="behaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>T created with SimpleJson</returns>
		public static async Task<TResult> GetAsJsonAsync<TResult>(this HttpResponseMessage httpResponseMessage, HttpBehaviour behaviour = null, CancellationToken token = default(CancellationToken))
		{
			if (httpResponseMessage.IsSuccessAndContentTypeJson())
			{
				return await httpResponseMessage.DeserializeObject<TResult>().ConfigureAwait(false);
			}
			await httpResponseMessage.HandleErrorAsync(behaviour, token).ConfigureAwait(false);
			return default(TResult);
		}

		/// <summary>
		/// GetAsJsonAsync will use DataMember / DataContract to parse the object into
		/// </summary>
		/// <typeparam name="TNormal">Type to parse to</typeparam>
		/// <typeparam name="TError">Type to parse to if the httpResponseMessage has an error</typeparam>
		/// <param name="httpResponseMessage"></param>
		/// <param name="behaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>HttpResponse of TNormal and TError filled by SimpleJson</returns>
		public static async Task<HttpResponse<TNormal, TError>> GetAsJsonAsync<TNormal, TError>(this HttpResponseMessage httpResponseMessage, HttpBehaviour behaviour = null, CancellationToken token = default(CancellationToken))
		{
			var response = new HttpResponse<TNormal, TError>
			{
				StatusCode = httpResponseMessage.StatusCode
			};

			if (httpResponseMessage.IsSuccessAndContentTypeJson(behaviour))
			{
				response.Result = await httpResponseMessage.DeserializeObject<TNormal>().ConfigureAwait(false);
			}
			else
			{
				// ThrowErrorOnNonSuccess NEEDS to be false
				behaviour = behaviour ?? HttpBehaviour.GlobalHttpBehaviour;
				var specialBehaviour = behaviour.Clone();
				specialBehaviour.ThrowErrorOnNonSuccess = false;
				var jsonErrorResponse = await httpResponseMessage.HandleErrorAsync(specialBehaviour, token).ConfigureAwait(false);
				response.ErrorResponse = SimpleJson.DeserializeObject<TError>(jsonErrorResponse);
			}
			return response;
		}
	}
}
