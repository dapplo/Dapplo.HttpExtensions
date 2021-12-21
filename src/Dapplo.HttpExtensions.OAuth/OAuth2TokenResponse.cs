// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.HttpExtensions.OAuth;

/// <summary>
///     Container for the OAuth token / refresh response
/// </summary>
[DataContract]
public class OAuth2TokenResponse
{
    private readonly DateTimeOffset _responseTime = DateTimeOffset.Now;

    /// <summary>
    ///     The access token
    /// </summary>
    [DataMember(Name = "access_token")]
    public string AccessToken { get; set; }

    /// <summary>
    ///     The error
    /// </summary>
    [DataMember(Name = "error")]
    public string Error { get; set; }

    /// <summary>
    ///     Details to the error
    /// </summary>
    [DataMember(Name = "error_description")]
    public string ErrorDescription { get; set; }

    /// <summary>
    ///     Returns the time that the token expires
    /// </summary>
    public DateTimeOffset Expires => _responseTime.AddSeconds(ExpiresInSeconds);

    /// <summary>
    ///     DateTimeOffset.Now.AddSeconds(expiresIn)
    /// </summary>
    [DataMember(Name = "expires_in")]
    public long ExpiresInSeconds { get; set; }

    /// <summary>
    ///     Test if the response has an error
    /// </summary>
    public bool HasError => !string.IsNullOrEmpty(Error);

    /// <summary>
    ///     Test if the error is an invalid grant
    /// </summary>
    public bool IsInvalidGrant => HasError && Error == "invalid_grant";

    /// <summary>
    ///     Refresh token, used to get a new access token
    /// </summary>
    [DataMember(Name = "refresh_token")]
    public string RefreshToken { get; set; }

    /// <summary>
    ///     Type for the token
    /// </summary>
    [DataMember(Name = "token_type")]
    public string TokenType { get; set; }
}