// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.HttpExtensions.OAuth;

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