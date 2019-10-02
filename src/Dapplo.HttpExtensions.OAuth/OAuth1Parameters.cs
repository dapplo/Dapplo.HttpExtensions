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