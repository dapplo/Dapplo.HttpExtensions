/*
	Dapplo - building blocks for desktop applications
	Copyright (C) 2015-2016 Dapplo

	For more information see: http://dapplo.net/
	Dapplo repositories are hosted on GitHub: https://github.com/dapplo

	This file is part of Dapplo.HttpExtensions.

	Dapplo.HttpExtensions is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	Dapplo.HttpExtensions is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// Misc extensions
	/// </summary>
	public static class MiscExtensions
	{
		/// <summary>
		/// Create a query string from a list of keyValuePairs
		/// </summary>
		/// <typeparam name="T">type for the value, sometimes it's easier to let this method call ToString on your type.</typeparam>
		/// <param name="keyValuePairs">list of keyValuePair with string,T</param>
		/// <returns>name1=value1&amp;name2=value2 etc...</returns>
		public static string ToQueryString<T>(this IEnumerable<KeyValuePair<string, T>> keyValuePairs)
		{
			if (keyValuePairs == null)
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
			queryBuilder.Length -= 1;
			return queryBuilder.ToString();
		}

		/// <summary>
		/// As the BCL doesn't include a ForEach on the IEnumerable, this extension was added
		/// </summary>
		/// <typeparam name="T">Type of the IEnumerable</typeparam>
		/// <param name="source">The IEnumerable</param>
		/// <param name="action">Action to call for each item</param>
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			if (source == null)
			{
				throw new ArgumentNullException(nameof(source));
			}

			if (action == null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			foreach (T item in source)
			{
				action(item);
			}
		}
	}
}
