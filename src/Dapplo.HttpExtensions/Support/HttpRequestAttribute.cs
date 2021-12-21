// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.HttpExtensions.Support;

/// <summary>
///     This attribute marks a class as "http content" for a request
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class HttpRequestAttribute : Attribute
{
    /// <summary>
    ///     "Force" multi-part, even if there is only one content
    /// </summary>
    public bool MultiPart { get; set; }
}