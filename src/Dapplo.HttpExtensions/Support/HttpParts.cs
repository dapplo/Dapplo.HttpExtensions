// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.HttpExtensions.Support;

/// <summary>
///     Marker for the response
/// </summary>
public enum HttpParts
{
    /// <summary>
    ///     Default value.
    /// </summary>
    None,

    /// <summary>
    ///     The property specifies the boundary of a multi-part
    /// </summary>
    MultipartBoundary,

    /// <summary>
    ///     The property specifies the name of the content in a multi-part post
    /// </summary>
    RequestMultipartName,

    /// <summary>
    ///     The property specifies the filename of the content in a multi-part post
    /// </summary>
    RequestMultipartFilename,

    /// <summary>
    ///     Specifies the content for uploading
    /// </summary>
    RequestContent,

    /// <summary>
    ///     Specifies the content-type for uploading
    /// </summary>
    RequestContentType,

    /// <summary>
    ///     Specifies the request headers to send on the request, this should be of type IDictionary where key is string and
    ///     value is string
    /// </summary>
    RequestHeaders,

    /// <summary>
    ///     The property will get the response content, HttpResponseMessage can also be used
    /// </summary>
    ResponseContent,

    /// <summary>
    ///     The property will get the response content, when an error occured
    /// </summary>
    ResponseErrorContent,

    /// <summary>
    ///     Specifies the content-type, either for uploading or for the response
    /// </summary>
    ResponseContentType,

    /// <summary>
    ///     The Http-Status code, should be of type HttpStatusCode
    /// </summary>
    ResponseStatuscode,

    /// <summary>
    ///     Marks HttpResponseHeaders,
    /// </summary>
    ResponseHeaders
}