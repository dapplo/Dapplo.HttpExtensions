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

#region Usings

using System.Runtime.Serialization;

#endregion

namespace Dapplo.HttpExtensions.OAuth
{
    /// <summary>
    ///     Provides a predefined set of algorithms that are supported officially by the OAuth 1.x protocol
    /// </summary>
    public enum OAuth1SignatureTypes
    {
        /// <summary>
        ///     Hash-based Message Authentication Code (HMAC) using the SHA1 hash function.
        /// </summary>
        [EnumMember(Value = "HMAC-SHA1")] HMacSha1,

        /// <summary>
        ///     The PLAINTEXT method does not provide any security protection and SHOULD only be used over a secure channel such as
        ///     HTTPS. It does not use the Signature Base String.
        /// </summary>
        [EnumMember(Value = "PLAINTEXT")] PlainText,

        /// <summary>
        ///     RSA-SHA1 signature method uses the RSASSA-PKCS1-v1_5 signature algorithm as defined in [RFC3447] section 8.2 (more
        ///     simply known as PKCS#1)
        /// </summary>
        [EnumMember(Value = "RSA-SHA1")] RsaSha1
    }
}