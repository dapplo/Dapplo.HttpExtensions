// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.HttpExtensions.OAuth;

/// <summary>
///     Enum values for the OAuth grant types
/// </summary>
public enum GrantTypes
{
    /// <summary>
    ///     Requesting a Password
    /// </summary>
    [EnumMember(Value = "password")] Password,

    /// <summary>
    ///     Requesting a refresh token
    /// </summary>
    [EnumMember(Value = "refresh_token")] RefreshToken,

    /// <summary>
    ///     Requesting a authorization code
    /// </summary>
    [EnumMember(Value = "authorization_code")] AuthorizationCode
}