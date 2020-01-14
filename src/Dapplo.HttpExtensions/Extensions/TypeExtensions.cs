// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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