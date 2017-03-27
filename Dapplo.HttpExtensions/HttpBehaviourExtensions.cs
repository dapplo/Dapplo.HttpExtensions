//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2015-2017 Dapplo
// 
//  For more information see: http://dapplo.net/
//  Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
//  This file is part of Dapplo.HttpExtensions
// 
//  Dapplo.HttpExtensions is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  Dapplo.HttpExtensions is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have a copy of the GNU Lesser General Public License
//  along with Dapplo.HttpExtensions. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

#region using

using System;
using System.Net.Http;

#endregion

namespace Dapplo.HttpExtensions
{
	/// <summary>
	///     Extensions for the HttpBehaviour class
	/// </summary>
	public static class HttpBehaviourExtensions
	{
		/// <summary>
		///     Chain the current OnHttpContentCreated
		/// </summary>
		/// <param name="changeableHttpBehaviour"></param>
		/// <param name="newOnHttpContentCreated"></param>
		/// <returns>IChangeableHttpBehaviour for fluent usage</returns>
		public static IChangeableHttpBehaviour ChainOnHttpContentCreated(this IChangeableHttpBehaviour changeableHttpBehaviour, Func<HttpContent, HttpContent> newOnHttpContentCreated)
		{
			var oldOnHttpContentCreated = changeableHttpBehaviour.OnHttpContentCreated;

			changeableHttpBehaviour.OnHttpContentCreated = httpContent =>
			{
				if (oldOnHttpContentCreated != null)
				{
					httpContent = oldOnHttpContentCreated(httpContent);
				}
				return newOnHttpContentCreated(httpContent);
			};
			return changeableHttpBehaviour;
		}

		/// <summary>
		///     Chain the current OnHttpMessageHandlerCreated
		/// </summary>
		/// <param name="changeableHttpBehaviour"></param>
		/// <param name="newOnHttpMessageHandlerCreated">function which accepts a HttpMessageHandler and also returns one</param>
		/// <returns>IChangeableHttpBehaviour for fluent usage</returns>
		public static IChangeableHttpBehaviour ChainOnHttpMessageHandlerCreated(this IChangeableHttpBehaviour changeableHttpBehaviour,
			Func<HttpMessageHandler, HttpMessageHandler> newOnHttpMessageHandlerCreated)
		{
			var oldOnHttpMessageHandlerCreated = changeableHttpBehaviour.OnHttpMessageHandlerCreated;

			changeableHttpBehaviour.OnHttpMessageHandlerCreated = httpMessageHandler =>
			{
				if (oldOnHttpMessageHandlerCreated != null)
				{
					httpMessageHandler = oldOnHttpMessageHandlerCreated(httpMessageHandler);
				}
				return newOnHttpMessageHandlerCreated(httpMessageHandler);
			};
			return changeableHttpBehaviour;
		}

		/// <summary>
		///     Chain the current OnHttpRequestMessageCreated function
		/// </summary>
		/// <param name="changeableHttpBehaviour">IChangeableHttpBehaviour</param>
		/// <param name="newOnHttpRequestMessageCreated">Function which accepts and returns HttpRequestMessage</param>
		/// <returns>IChangeableHttpBehaviour for fluent usage</returns>
		public static IChangeableHttpBehaviour ChainOnHttpRequestMessageCreated(this IChangeableHttpBehaviour changeableHttpBehaviour,
			Func<HttpRequestMessage, HttpRequestMessage> newOnHttpRequestMessageCreated)
		{
			var onHttpRequestMessageCreated = changeableHttpBehaviour.OnHttpRequestMessageCreated;

			changeableHttpBehaviour.OnHttpRequestMessageCreated = httpRequestMessage =>
			{
				if (onHttpRequestMessageCreated != null)
				{
					httpRequestMessage = onHttpRequestMessageCreated(httpRequestMessage);
				}
				return newOnHttpRequestMessageCreated(httpRequestMessage);
			};
			return changeableHttpBehaviour;
		}
	}
}