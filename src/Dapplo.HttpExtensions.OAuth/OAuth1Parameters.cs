// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.Serialization;

namespace Dapplo.HttpExtensions.OAuth
{
    /// <summary>
    ///     Enum with all the parameters for OAuth 1
    /// </summary>
    public enum OAuth1Parameters
    {
        /// <summary>
        ///     Consumer key is the key which a service provides for it's consumers
        /// </summary>
        [EnumMember(Value = "oauth_consumer_key")] ConsumerKey,

        /// <summary>
        ///     Callback Uri
        /// </summary>
        [EnumMember(Value = "oauth_callback")] Callback,

        /// <summary>
        ///     Used version
        /// </summary>
        [EnumMember(Value = "oauth_version")] Version,

        /// <summary>
        ///     Signing method
        /// </summary>
        [EnumMember(Value = "oauth_signature_method")] SignatureMethod,

        /// <summary>
        ///     Timestamp of the request
        /// </summary>
        [EnumMember(Value = "oauth_timestamp")] Timestamp,

        /// <summary>
        ///     A unique code
        /// </summary>
        [EnumMember(Value = "oauth_nonce")] Nonce,

        /// <summary>
        ///     Token
        /// </summary>
        [EnumMember(Value = "oauth_token")] Token,

        /// <summary>
        ///     Token verifier
        /// </summary>
        [EnumMember(Value = "oauth_verifier")] Verifier,

        /// <summary>
        ///     Token secret
        /// </summary>
        [EnumMember(Value = "oauth_token_secret")] TokenSecret,

        /// <summary>
        ///     Signature for the request
        /// </summary>
        [EnumMember(Value = "oauth_signature")] Signature
    }
}