// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Dapplo.HttpExtensions
{
    /// <summary>
    ///     Misc extensions
    /// </summary>
    public static class MiscExtensions
    {
        /// <summary>
        ///     Create a query string from a list of keyValuePairs
        /// </summary>
        /// <typeparam name="T">type for the value, sometimes it's easier to let this method call ToString on your type.</typeparam>
        /// <param name="keyValuePairs">list of keyValuePair with string,T</param>
        /// <returns>name1=value1&amp;name2=value2 etc...</returns>
        public static string ToQueryString<T>(this IEnumerable<KeyValuePair<string, T>> keyValuePairs)
        {
            if (keyValuePairs is null)
            {
                throw new ArgumentNullException(nameof(keyValuePairs));
            }
            var queryBuilder = new StringBuilder();

            foreach (var keyValuePair in keyValuePairs)
            {
                queryBuilder.Append($"{keyValuePair.Key}");
                if (keyValuePair.Value != null)
                {
                    var encodedValue = Uri.EscapeDataString(keyValuePair.Value?.ToString());
                    queryBuilder.Append($"={encodedValue}");
                }
                queryBuilder.Append('&');
            }
            if (queryBuilder.Length > 0)
            {
                queryBuilder.Length -= 1;
            }
            return queryBuilder.ToString();
        }
    }
}