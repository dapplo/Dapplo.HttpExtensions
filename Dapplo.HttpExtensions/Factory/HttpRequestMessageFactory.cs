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

using Dapplo.LogFacade;
using System;
using System.Collections;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Dapplo.HttpExtensions.Support;
using System.Collections.Generic;

namespace Dapplo.HttpExtensions.Factory
{
	/// <summary>
	/// Dapplo.HttpExtension uses the HttpRequestMessage to send the requests.
	/// This makes it a lot more flexible to use Accept headers and other stuff
	/// This is the factory for it.
	/// </summary>
	public static class HttpRequestMessageFactory
	{
		private static readonly LogSource Log = new LogSource();
		/// <summary>
		/// Create a HttpRequestMessage for the HEAD method
		/// </summary>
		/// <param name="requestUri">the target uri for this message</param>
		/// <returns>HttpRequestMessage</returns>
		public static HttpRequestMessage CreateHead(Uri requestUri)
		{
			return Create(HttpMethod.Head, requestUri);
		}

		/// <summary>
		/// Create a HttpRequestMessage for the GET method
		/// </summary>
		/// <param name="requestUri">the target uri for this message</param>
		/// <returns>HttpRequestMessage</returns>
		public static HttpRequestMessage CreateGet<TResponse>(Uri requestUri)
			where TResponse : class
		{
			return Create<TResponse>(HttpMethod.Get, requestUri);
		}

		/// <summary>
		/// Create a HttpRequestMessage for the POST method
		/// </summary>
		/// <typeparam name="TResponse">Type to return into, this only influences the Accept headers</typeparam>
		/// <typeparam name="TContent"></typeparam>
		/// <param name="requestUri">the target uri for this message</param>
		/// <param name="content">HttpContent</param>
		/// <returns>HttpRequestMessage</returns>
		public static HttpRequestMessage CreatePost<TResponse, TContent>(Uri requestUri, TContent content = default(TContent))
			where TResponse : class where TContent : class
		{
			return Create(HttpMethod.Post, requestUri, typeof(TResponse), typeof(TContent), content);
		}

		/// <summary>
		/// Create a HttpRequestMessage for the POST method
		/// </summary>
		/// <typeparam name="TContent"></typeparam>
		/// <param name="requestUri">the target uri for this message</param>
		/// <param name="content">HttpContent</param>
		/// <returns>HttpRequestMessage</returns>
		public static HttpRequestMessage CreatePost<TContent>(Uri requestUri, TContent content = default(TContent))
			where TContent : class
		{
			return Create(HttpMethod.Post, requestUri, null, typeof(TContent), content);
		}

		/// <summary>
		/// Create a HttpRequestMessage for the specified method
		/// </summary>
		/// <param name="method">Method to create the request message for</param>
		/// <param name="requestUri">the target uri for this message</param>
		/// <param name="resultType">Type</param>
		/// <param name="contentType">Type</param>
		/// <param name="content">content to convert to HttpContent</param>
		/// <returns>HttpRequestMessage</returns>
		public static HttpRequestMessage Create(HttpMethod method, Uri requestUri, Type resultType = null, Type contentType = null, object content = null)
		{
			Log.Verbose().WriteLine("Creating request for {0}", requestUri);
			var httpBehaviour = HttpBehaviour.Current;
			contentType = contentType ?? content?.GetType();

			var httpRequestMessage = new HttpRequestMessage(method, requestUri)
			{
				Content = HttpContentFactory.Create(contentType, content),
			};

			// if the type has a HttpAttribute with HttpPart.Request
			if (contentType != null && contentType.GetCustomAttribute<HttpAttribute>()?.Part == HttpParts.Request)
			{
				// And a property has a HttpAttribute with HttpPart.RequestHeaders
				var headersPropertyInfo = contentType.GetProperties().FirstOrDefault(t => t.GetCustomAttribute<HttpAttribute>()?.Part == HttpParts.RequestHeaders);
				var headersValue = headersPropertyInfo?.GetValue(content) as IDictionary<string, string>;
				if (headersValue != null)
				{
					foreach (var headerName in headersValue.Keys)
					{
						var headerValue = headersValue[headerName];
						httpRequestMessage.Headers.TryAddWithoutValidation(headerName, headerValue);
					}
				}
			}

			if (resultType != null)
			{
				httpBehaviour.HttpContentConverters?.ForEach(x => x.AddAcceptHeadersForType(resultType, httpRequestMessage));
			}

			// Make sure the OnCreateHttpRequestMessage function is called
			if (httpBehaviour.OnHttpRequestMessageCreated != null)
			{
				return httpBehaviour.OnHttpRequestMessageCreated.Invoke(httpRequestMessage);
			}
			return httpRequestMessage;
		}

		/// <summary>
		/// Create a HttpRequestMessage for the specified method
		/// </summary>
		/// <typeparam name="TResponse">The type for the response, this modifies the Accep headers</typeparam>
		/// <typeparam name="TContent"></typeparam>
		/// <param name="method">Method to create the request message for</param>
		/// <param name="requestUri">the target uri for this message</param>
		/// <param name="content">HttpContent</param>
		/// <returns>HttpRequestMessage</returns>
		public static HttpRequestMessage Create<TResponse, TContent>(HttpMethod method, Uri requestUri, TContent content = default(TContent))
			where TResponse : class where TContent : class
		{
			return Create(method, requestUri, typeof(TResponse), typeof(TContent), content);
		}

		/// <summary>
		/// Create a HttpRequestMessage for the specified method
		/// </summary>
		/// <typeparam name="TResponse">The type for the response, this modifies the Accep headers</typeparam>
		/// <param name="method">Method to create the request message for</param>
		/// <param name="requestUri">the target uri for this message</param>
		/// <returns>HttpRequestMessage</returns>
		public static HttpRequestMessage Create<TResponse>(HttpMethod method, Uri requestUri)
			where TResponse : class
		{
			return Create(method, requestUri, typeof(TResponse));
		}
	}
}
