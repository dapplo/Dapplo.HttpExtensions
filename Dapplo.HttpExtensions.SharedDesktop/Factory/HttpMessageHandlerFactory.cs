﻿//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2015-2016 Dapplo
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

using Dapplo.LogFacade;
using System.Net.Cache;
using System.Net.Http;

#endregion

namespace Dapplo.HttpExtensions.Factory
{
	/// <summary>
	///     Creating a HttpMessageHandler is not very straightforward, that is why the logic is capsulated in the
	///     HttpMessageHandlerFactory.
	/// </summary>
	public static partial class HttpMessageHandlerFactory
	{
		/// <summary>
		///     This creates an advanced HttpMessageHandler, used in desktop applications
		///     Should be preferred
		/// </summary>
		/// <returns>HttpMessageHandler (WebRequestHandler)</returns>
		private static HttpMessageHandler CreateHandler()
		{
			var webRequestHandler = new WebRequestHandler();
			SetDefaults(webRequestHandler);
			return webRequestHandler;
		}

		/// <summary>
		///     Apply settings on the WebRequestHandler, this also calls the SetDefaults for the underlying HttpClientHandler
		/// </summary>
		/// <param name="webRequestHandler">WebRequestHandler to set the defaults to</param>
		private static void SetDefaults(WebRequestHandler webRequestHandler)
		{
			var httpBehaviour = HttpBehaviour.Current;
			SetDefaults(webRequestHandler as HttpClientHandler);

			var httpSettings = httpBehaviour.HttpSettings ?? HttpExtensionsGlobals.HttpSettings;

			webRequestHandler.AllowPipelining = httpSettings.AllowPipelining;
			webRequestHandler.ReadWriteTimeout = httpSettings.ReadWriteTimeout;
			webRequestHandler.AuthenticationLevel = httpSettings.AuthenticationLevel;
			webRequestHandler.ContinueTimeout = httpSettings.ContinueTimeout;
			webRequestHandler.ImpersonationLevel = httpSettings.ImpersonationLevel;
			webRequestHandler.MaxResponseHeadersLength = httpSettings.MaxResponseHeadersLength;
			webRequestHandler.CachePolicy = new RequestCachePolicy(httpSettings.RequestCacheLevel);
			webRequestHandler.Proxy = httpSettings.UseProxy ? WebProxyFactory.Create() : null;

			if (httpSettings.IgnoreSslCertificateErrors)
			{
				webRequestHandler.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
				{
					if (sslPolicyErrors != System.Net.Security.SslPolicyErrors.None)
					{
						Log.Warn().WriteLine("Ssl policy error {0}", sslPolicyErrors);
					}
					return true;
				};
			}
		}
	}
}