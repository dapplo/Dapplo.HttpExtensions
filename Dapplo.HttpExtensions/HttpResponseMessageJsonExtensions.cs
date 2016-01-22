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

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// Extensions for the HttpResponseMessage class which handle Json
	/// These are explicitly for JSON, but could be ignored if you use ReadAsSync which does the same
	/// </summary>
	public static class HttpResponseMessageJsonExtensions
	{
		/// <summary>
		/// Get Json from the httpResponseMessage
		/// </summary>
		/// <param name="httpResponseMessage">HttpResponseMessage</param>
		/// <param name="httpBehaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>dynamic created with SimpleJson</returns>
		public static async Task<dynamic> GetAsJsonAsync(this HttpResponseMessage httpResponseMessage, HttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken))
		{
			var content = httpResponseMessage.Content;
			if (content.ExpectContentType(MediaTypes.Json, httpBehaviour))
			{
				// Currently we can't specify that we need a dynamic, but this will be automatically if we use null for a type.
				return await content.ReadAsAsync(null).ConfigureAwait(false);
			}
			await httpResponseMessage.HandleErrorAsync(httpBehaviour, token).ConfigureAwait(false);
			return null;
		}

		/// <summary>
		/// GetAsJsonAsync will use DataMember / DataContract to parse the object into
		/// </summary>
		/// <typeparam name="TResult">Type to parse to</typeparam>
		/// <param name="httpResponseMessage"></param>
		/// <param name="httpBehaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>T created with SimpleJson</returns>
		public static async Task<TResult> GetAsJsonAsync<TResult>(this HttpResponseMessage httpResponseMessage, HttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken)) where TResult : class
		{
			var content = httpResponseMessage.Content;
			if (content.ExpectContentType(MediaTypes.Json, httpBehaviour))
			{
				return await content.ReadAsAsync<TResult>().ConfigureAwait(false);
			}
			await httpResponseMessage.HandleErrorAsync(httpBehaviour, token).ConfigureAwait(false);
			return default(TResult);
		}

		/// <summary>
		/// GetAsJsonAsync will use DataMember / DataContract to parse the object into
		/// </summary>
		/// <typeparam name="TResult">Type to parse to</typeparam>
		/// <typeparam name="TError">Type to parse to if the httpResponseMessage has an error</typeparam>
		/// <param name="httpResponseMessage"></param>
		/// <param name="httpBehaviour">HttpBehaviour which specifies the IHttpSettings and other non default behaviour</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>HttpResponse of TNormal and TError filled by SimpleJson</returns>
		public static async Task<HttpResponse<TResult, TError>> GetAsJsonAsync<TResult, TError>(this HttpResponseMessage httpResponseMessage, HttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken)) where TResult : class
		{
			var response = new HttpResponse<TResult, TError>
			{
				StatusCode = httpResponseMessage.StatusCode
			};

			var content = httpResponseMessage.Content;
			if (content.ExpectContentType(MediaTypes.Json, httpBehaviour))
			{
				response.Result = await content.ReadAsAsync<TResult>().ConfigureAwait(false);
			}
			else
			{
				// ThrowErrorOnNonSuccess NEEDS to be false
				httpBehaviour = httpBehaviour ?? HttpBehaviour.GlobalHttpBehaviour;
				var specialBehaviour = httpBehaviour.Clone();
				specialBehaviour.ThrowErrorOnNonSuccess = false;
				var jsonErrorResponse = await httpResponseMessage.HandleErrorAsync(specialBehaviour, token).ConfigureAwait(false);
				response.ErrorResponse = SimpleJson.DeserializeObject<TError>(jsonErrorResponse);
			}
			return response;
		}
	}
}
