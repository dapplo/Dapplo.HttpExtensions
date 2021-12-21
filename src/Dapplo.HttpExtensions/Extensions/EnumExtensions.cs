// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using System.Runtime.Serialization;

namespace Dapplo.HttpExtensions.Extensions;

/// <summary>
///     Extensions for enums
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    ///     The returns the Value from the EnumMemberAttribute, or a ToString on the element.
    ///     This can be used to create a lookup from string to enum element
    /// </summary>
    /// <param name="enumerationItem">Enum</param>
    /// <returns>string</returns>
    public static string EnumValueOf(this Enum enumerationItem)
    {
        if (enumerationItem is null)
        {
            return null;
        }
        var attributes =
            (EnumMemberAttribute[]) enumerationItem.GetType().GetRuntimeField(enumerationItem.ToString()).GetCustomAttributes(typeof(EnumMemberAttribute), false);
        return attributes.Length > 0 ? attributes[0].Value : enumerationItem.ToString();
    }
}