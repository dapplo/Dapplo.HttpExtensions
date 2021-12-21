// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dapplo.HttpExtensions.ContentConverter;
using Dapplo.HttpExtensions.Support;

namespace Dapplo.HttpExtensions;

/// <summary>
///     These are the globals for some of the important configurable settings
///     When a HttpBehaviour is created, some of the values from here will be copied. (unless diffently specified)
/// </summary>
public static class HttpExtensionsGlobals
{
    /// <summary>
    ///     The global default encoding
    /// </summary>
    public static Encoding DefaultEncoding { get; set; } = Encoding.UTF8;

    /// <summary>
    ///     The global list of HttpContent converters
    /// </summary>
    public static IList<IHttpContentConverter> HttpContentConverters { get; set; } = new List<IHttpContentConverter>
    {
#if !NETSTANDARD1_3
        SyndicationFeedHttpContentConverter.Instance.Value,
        XDocumentHttpContentConverter.Instance.Value,
#endif
        ByteArrayHttpContentConverter.Instance.Value,
        FormUriEncodedContentConverter.Instance.Value,
        DefaultJsonHttpContentConverter.Instance.Value,
        StreamHttpContentConverter.Instance.Value,
        StringHttpContentConverter.Instance.Value
    };

    /// <summary>
    /// The GLOBAL default function for the uri escaping, this is Uri.EscapeDataString
    /// Some projects might rather use Uri.EscapeUriString, be careful changing this!
    /// </summary>
    public static Func<string, string> DefaultUriEscapeFunc { get; } = Uri.EscapeDataString;

    /// <summary>
    ///     The global IHttpSettings
    /// </summary>
    public static IHttpSettings HttpSettings { get; set; } = new HttpSettings();

    /// <summary>
    ///     The global JsonSerializer
    /// </summary>
    public static IJsonSerializer JsonSerializer { get; set; }

    /// <summary>
    ///     This offset is used in the OAuth2Setting.IsAccessTokenExpired to check the OAuth2AccessTokenExpires
    ///     Now + this > OAuth2AccessTokenExpires
    /// </summary>
    public static int OAuth2ExpireOffset { get; set; } = 5;

    /// <summary>
    ///     The global read buffer-size
    /// </summary>
    public static int ReadBufferSize { get; set; } = 4096;

    /// <summary>
    ///     Global value for ThrowOnError, see IHttpBehaviour
    /// </summary>
    public static bool ThrowOnError { get; set; } = true;

    /// <summary>
    ///     Global value for UseProgressStream, see IHttpBehaviour
    /// </summary>
    public static bool UseProgressStream { get; set; } = false;

    /// <summary>
    ///     Global validate response content-type
    /// </summary>
    public static bool ValidateResponseContentType { get; set; } = true;
}