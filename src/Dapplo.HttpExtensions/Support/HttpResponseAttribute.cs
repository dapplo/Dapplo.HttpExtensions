// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.HttpExtensions.Support;

/// <summary>
///     This attribute marks a class as "http content" for a response
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class HttpResponseAttribute : Attribute
{
}