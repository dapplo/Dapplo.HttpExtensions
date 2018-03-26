#region Dapplo 2016-2018 - GNU Lesser General Public License

// Dapplo - building blocks for .NET applications
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

#endregion

#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

#endregion

namespace Dapplo.HttpExtensions.Extensions
{
    /// <summary>
    ///     Formatwith extension for the string
    /// </summary>
    public static class StringExtensions
    {
        private static readonly Regex PropertyRegex = new Regex(@"(?<start>\{)+(?<property>[\w\.\[\]]+)(?<format>:[^}]+)?(?<end>\})+",
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        /// <summary>
        ///     Format the string "format" with the source
        /// </summary>
        /// <param name="format">String with formatting, like {name}</param>
        /// <param name="sources">
        ///     object [] with properties, if a property has the type IDictionary string,string it can used these
        ///     parameters too
        /// </param>
        /// <returns>Formatted string</returns>
        public static string FormatWith(this string format, params object[] sources)
        {
            return FormatWith(format, null, sources);
        }

        /// <summary>
        ///     Format the string "format" with the source
        /// </summary>
        /// <param name="format">String with formatting, like {name}</param>
        /// <param name="provider">IFormatProvider</param>
        /// <param name="sources">
        ///     object with properties, if a property has the type IDictionary string,string it can used these
        ///     parameters too
        /// </param>
        /// <returns>Formatted string</returns>
        public static string FormatWith(this string format, IFormatProvider provider, params object[] sources)
        {
            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }
            if (sources == null)
            {
                return format;
            }
            var properties = new Dictionary<string, object>();

            for (var index = 0; index < sources.Length; index++)
            {
                var source = sources[index];
                MapToProperties(properties, index, source);
            }

            var values = new List<object>();
            var rewrittenFormat = PropertyRegex.Replace(format, delegate(Match m)
            {
                var startGroup = m.Groups["start"];
                var propertyGroup = m.Groups["property"];
                var formatGroup = m.Groups["format"];
                var endGroup = m.Groups["end"];

                if (properties.TryGetValue(propertyGroup.Value, out var value))
                {
                    values.Add(value is Enum enumValue ? enumValue.EnumValueOf() : value);
                }
                else
                {
                    values.Add(propertyGroup.Value);
                }
                return new string('{', startGroup.Captures.Count) + (values.Count - 1) + formatGroup.Value + new string('}', endGroup.Captures.Count);
            });

            return string.Format(provider, rewrittenFormat, values.ToArray());
        }


        /// <summary>
        ///     Helper method to fill the properties with the values from the source
        /// </summary>
        /// <param name="properties">IDictionary with the possible properties</param>
        /// <param name="index">int with index in the current sources</param>
        /// <param name="source">object</param>
        private static void MapToProperties(IDictionary<string, object> properties, int index, object source)
        {
            if (source == null)
            {
                return;
            }
            var sourceType = source.GetType();
            if (sourceType.GetTypeInfo().IsPrimitive || sourceType == typeof(string))
            {
                properties[index.ToString()] = source;
                return;
            }

            if (properties.DictionaryToGenericDictionary(source as IDictionary))
            {
                return;
            }

            foreach (var propertyInfo in source.GetType().GetRuntimeProperties())
            {
                if (!propertyInfo.CanRead)
                {
                    continue;
                }

                var value = propertyInfo.GetValue(source, null);
                if (value == null)
                {
                    properties[propertyInfo.Name] = "";
                    continue;
                }

                if (properties.DictionaryToGenericDictionary(value as IDictionary))
                {
                    continue;
                }

                if (propertyInfo.PropertyType.GetTypeInfo().IsEnum)
                {
                    var enumValue = value as Enum;
                    properties[propertyInfo.Name] = enumValue.EnumValueOf();
                    continue;
                }
                properties[propertyInfo.Name] = value;
            }
        }

        /// <summary>
        ///     Map a dictionary to properties
        /// </summary>
        /// <param name="properties">IDictionary with properties to add to</param>
        /// <param name="dictionary">dictionary to process, or null due to "as" cast</param>
        /// <returns>false if dictionary was null</returns>
        private static bool DictionaryToGenericDictionary<TKey, TValue>(this IDictionary<TKey, TValue> properties, IDictionary dictionary)
        {
            if (dictionary == null)
            {
                return false;
            }

            var dictionaryType = dictionary.GetType().GetTypeInfo();
            if (!dictionaryType.IsGenericType || dictionaryType.GenericTypeArguments[0] != typeof(TKey))
            {
                return true;
            }

            foreach (DictionaryEntry item in dictionary)
            {
                var key = (TKey) item.Key;
                var value = (TValue) item.Value;
                properties[key] = value;
            }
            // Also return true if the dictionary didn't have keys of type string, as we don't know what to do with it.
            return true;
        }
    }
}