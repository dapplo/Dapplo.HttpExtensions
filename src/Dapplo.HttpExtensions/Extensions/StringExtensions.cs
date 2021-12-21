// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Dapplo.HttpExtensions.Extensions;

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
        if (format is null)
        {
            throw new ArgumentNullException(nameof(format));
        }
        if (sources is null)
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
        if (source is null)
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
            if (value is null)
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
        if (dictionary is null)
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