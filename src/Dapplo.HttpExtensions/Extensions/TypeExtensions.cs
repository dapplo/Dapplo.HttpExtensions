﻿// Dapplo - building blocks for .NET applications
// Copyright (C) 2016-2018 Dapplo
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

namespace Dapplo.HttpExtensions.Extensions
{
    /// <summary>
    ///     Extensions for the Type class
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        ///     Get the name of a type which is readable, even if generics are used.
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>string</returns>
        public static string FriendlyName(this Type type)
        {
            string friendlyName = type.Name;
            if (type.GetTypeInfo().IsGenericType)
            {
                int backtick = friendlyName.IndexOf('`');
                if (backtick > 0)
                {
                    friendlyName = friendlyName.Remove(backtick);
                }
                friendlyName += "<";
                Type[] typeParameters = type.GetGenericArguments();
                for (int i = 0; i < typeParameters.Length; i++)
                {
                    string typeParamName = typeParameters[i].FriendlyName();
                    friendlyName += i == 0 ? typeParamName : ", " + typeParamName;
                }
                friendlyName += ">";
            }

            if (type.IsArray)
            {
                return type.GetElementType().FriendlyName() + "[]";
            }

            return friendlyName;
        }
    }
}