// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.HttpExtensions.Support;

/// <summary>
///     This attribute marks a property in a HttpRequestAttributed or HttpResponseAttribute class as being a part for
///     processing
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class HttpPartAttribute : Attribute
{
    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="part">HttpParts</param>
    public HttpPartAttribute(HttpParts part)
    {
        Part = part;
    }

    /// <summary>
    ///     Order of the content when using multi-part content
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    ///     Use this to specify what the property is representing
    /// </summary>
    public HttpParts Part { get; set; }
}