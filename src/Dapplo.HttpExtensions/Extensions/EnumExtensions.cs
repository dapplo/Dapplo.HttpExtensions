// Dapplo - building blocks for .NET applications
// Copyright (C) 2016-2019 Dapplo
// 
// For more information see: http://dapplo.net/
// Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
// This file is part of Dapplo.HttpExtensions
// 
// Dapplo.HttpExtensions is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Dapplo.HttpExtensions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have a copy of the GNU Lesser General Public License
// along with Dapplo.HttpExtensions. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Dapplo.HttpExtensions.Extensions
{
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
}