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

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// Dapplo.HttpExtension uses the HttpRequestMessage to send the requests.
	/// This makes it a lot more flexible to use Accept headers and other stuff
	/// This is the factory for it.
	/// </summary>
	public static class HttpRequestMessageFactory
	{
		/// <summary>
		/// Create a HttpClient which is modified by the settings specified in the IHttpSettings of the HttpBehaviour.
		/// If nothing is passed, the GlobalSettings are used
		/// </summary>
		/// <param name="method">Method to create the request message for</param>
		/// <param name="requestUri">the target uri for this message</param>
		/// <param name="content">HttpContent</param>
		/// <param name="httpBehaviour">HttpBehaviour instance or null if the global settings need to be used</param>
		/// <returns>HttpRequestMessage</returns>
		public static HttpRequestMessage Create(HttpMethod method, Uri requestUri, HttpContent content, IHttpBehaviour httpBehaviour = null)
		{
			httpBehaviour = httpBehaviour ?? new HttpBehaviour();

			var httpRequestMessage = new HttpRequestMessage(method, requestUri)
			{
				Content = content
			};

			// Make sure the OnCreateHttpRequestMessage action is called
			httpBehaviour.OnHttpRequestMessageCreated?.Invoke(httpRequestMessage);
			return httpRequestMessage;
		}

		/// <summary>
		/// Create a HttpClient which is modified by the settings specified in the IHttpSettings of the HttpBehaviour.
		/// If nothing is passed, the GlobalSettings are used
		/// </summary>
		/// <param name="method">Method to create the request message for</param>
		/// <param name="requestUri">the target uri for this message</param>
		/// <param name="content">HttpContent</param>
		/// <param name="httpBehaviour">HttpBehaviour instance or null if the global settings need to be used</param>
		/// <returns>HttpRequestMessage</returns>
		public static HttpRequestMessage Create<TResult>(HttpMethod method, Uri requestUri, HttpContent content, IHttpBehaviour httpBehaviour = null) where TResult : class
		{
			httpBehaviour = httpBehaviour ?? new HttpBehaviour();
			//var httpSettings = httpBehaviour.HttpSettings ?? HttpSettings.GlobalHttpSettings;
			var httpRequestMessage = Create(method, requestUri, content, httpBehaviour);
			httpBehaviour.HttpContentConverters?.ForEach(x => x.AddAcceptHeadersForType<TResult>(httpRequestMessage));
			return httpRequestMessage;
		}
	}
}
