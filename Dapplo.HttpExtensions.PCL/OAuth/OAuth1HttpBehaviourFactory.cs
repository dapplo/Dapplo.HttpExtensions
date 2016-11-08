//  Dapplo - building blocks for desktop applications
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
#if NET45 || NET46
namespace Dapplo.HttpExtensions.OAuth
{
	/// <summary>
	///     This factory can be used to create a IHttpBehaviour which handles OAuth requests
	/// </summary>
	public static class OAuth1HttpBehaviourFactory
	{
		/// <summary>
		///     Create a specify OAuth IHttpBehaviour
		/// </summary>
		/// <param name="oAuthSettings">OAuthSettings</param>
		/// <param name="fromHttpBehaviour">IHttpBehaviour or null</param>
		/// <returns>IHttpBehaviour</returns>
		public static OAuth1HttpBehaviour Create(OAuth1Settings oAuthSettings, IHttpBehaviour fromHttpBehaviour = null)
		{
			// Get a clone of a IHttpBehaviour (passed or current)
			var oauthHttpBehaviour = new OAuth1HttpBehaviour(fromHttpBehaviour);
			// Add a wrapper (delegate handler) which wraps all new HttpMessageHandlers
			oauthHttpBehaviour.ChainOnHttpMessageHandlerCreated(httpMessageHandler => new OAuth1HttpMessageHandler(oAuthSettings, oauthHttpBehaviour, httpMessageHandler));
			return oauthHttpBehaviour;
		}
	}
}
#endif