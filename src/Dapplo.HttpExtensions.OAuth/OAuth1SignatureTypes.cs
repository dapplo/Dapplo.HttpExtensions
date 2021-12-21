// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.HttpExtensions.OAuth;

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