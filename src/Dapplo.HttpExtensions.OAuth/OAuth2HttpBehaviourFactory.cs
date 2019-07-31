//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2019 Dapplo
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

namespace Dapplo.HttpExtensions.OAuth
{
    /// <summary>
    ///     This factory can be used to create a IHttpBehaviour which handles OAuth 2 requests
    /// </summary>
    public static class OAuth2HttpBehaviourFactory
    {
        /// <summary>
        ///     Create a specify OAuth2 IHttpBehaviour
        /// </summary>
        /// <param name="oAuth2Settings">OAuth2Settings</param>
        /// <param name="fromHttpBehaviour">IHttpBehaviour to clone, null if a new needs to be generated</param>
        /// <returns>IChangeableHttpBehaviour</returns>
        public static IChangeableHttpBehaviour Create(OAuth2Settings oAuth2Settings, IHttpBehaviour fromHttpBehaviour = null)
        {
            // Get a clone of a IHttpBehaviour (passed or current)
            var oauthHttpBehaviour = (fromHttpBehaviour ?? HttpBehaviour.Current).ShallowClone();
            // Add a wrapper (delegate handler) which wraps all new HttpMessageHandlers
            oauthHttpBehaviour.ChainOnHttpMessageHandlerCreated(httpMessageHandler => new OAuth2HttpMessageHandler(oAuth2Settings, oauthHttpBehaviour, httpMessageHandler));
            return oauthHttpBehaviour;
        }
    }
}