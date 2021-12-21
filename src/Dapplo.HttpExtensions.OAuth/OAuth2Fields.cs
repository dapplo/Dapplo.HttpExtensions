// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.HttpExtensions.OAuth;

/// <summary>
///     This enum is used internally for the mapping of the field names
/// </summary>
internal enum OAuth2Fields
{
    [EnumMember(Value = "refresh_token")] RefreshToken,
    [EnumMember(Value = "code")] Code,
    [EnumMember(Value = "client_id")] ClientId,
    [EnumMember(Value = "client_secret")] ClientSecret,
    [EnumMember(Value = "grant_type")] GrantType,
    [EnumMember(Value = "redirect_uri")] RedirectUri,
    [EnumMember(Value = "error")] Error,
    [EnumMember(Value = "error_description")] ErrorDescription
}