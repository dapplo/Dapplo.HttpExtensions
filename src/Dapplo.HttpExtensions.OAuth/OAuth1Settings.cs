// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Net.Http;
using System.Security.Cryptography;

namespace Dapplo.HttpExtensions.OAuth
{
    /// <summary>
    ///     Settings for the OAuth protocol, if possible this should not be used and OAuth 2.0 is a better choice
    /// </summary>
    public class OAuth1Settings : BaseOAuthSettings
    {
        /// <summary>
        ///     The HttpMethod which is used for getting the access token
        /// </summary>
        public HttpMethod AccessTokenMethod { get; set; } = HttpMethod.Get;

        /// <summary>
        ///     The URL to get an access token
        /// </summary>
        public Uri AccessTokenUrl { get; set; }

        /// <summary>
        ///     OAuth authorize token
        /// </summary>
        public string AuthorizeToken { get; internal set; }

        /// <summary>
        ///     If this is set, the value of the verifier will be validated (not null)
        /// </summary>
        public bool CheckVerifier { get; set; } = true;


        /// <summary>
        ///     OAuth request token
        /// </summary>
        public string RequestToken { get; internal set; }

        /// <summary>
        ///     OAuth request token secret
        /// </summary>
        public string RequestTokenSecret { get; internal set; }

        /// <summary>
        ///     The type of signature that is used, mostly this is HMacSha1
        /// </summary>
        public OAuth1SignatureTypes SignatureType { get; set; } = OAuth1SignatureTypes.HMacSha1;

        /// <summary>
        ///     For OAuth1SignatureTypes.RsaSha1 set this
        /// </summary>
        public RSACryptoServiceProvider RsaSha1Provider { get; set; }

        /// <summary>
        ///     The actualy token information, placed in an interface for usage with the Dapplo.Config project
        ///     the OAuthToken, a default implementation is assigned when the settings are created.
        ///     When using a Dapplo.Config IIniSection for your settings, this property can/should be overwritten with an instance
        ///     of your interface by makeing it extend IOAuthToken
        /// </summary>
        public IOAuth1Token Token { get; set; } = new OAuth1Token();

        /// <summary>
        ///     The HttpMethod which is used for getting the token
        /// </summary>
        public HttpMethod TokenMethod { get; set; } = HttpMethod.Post;

        /// <summary>
        /// This defines the transport "way" which the OAuth signature takes.
        /// Default is OAuth1SignatureTransports.Headers, which is normal, but SOME systems want the information in the query parameters.
        /// (An example is Atlassians Confluence)
        /// </summary>
        public OAuth1SignatureTransports SignatureTransport { get; set; } = OAuth1SignatureTransports.Headers;
    }
}
