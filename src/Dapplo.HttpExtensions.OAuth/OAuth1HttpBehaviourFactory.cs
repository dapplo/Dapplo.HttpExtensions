// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.HttpExtensions.OAuth;

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