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
	along with Dapplo.HttpExtensions. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// Write extension method for the string here
	/// </summary>
	public static class StringExtensions
	{
		private static readonly Regex PropertyRegex =
			new Regex(@"(?<start>\{)+(?<property>[\w\.\[\]]+)(?<format>:[^}]+)?(?<end>\})+",
				RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

		/// <summary>
		/// Format the string "format" with the source
		/// </summary>
		/// <param name="format">String with formatting, like {name}</param>
		/// <param name="source">object with properties, if a property has the type IDictionary string,string it can used these parameters too</param>
		/// <param name="provider">IFormatProvider</param>
		/// <returns>Formatted string</returns>
		public static string FormatWith(this string format, object source, IFormatProvider provider = null)
		{
			if (format == null)
			{
				throw new ArgumentNullException(nameof(format));
			}

			var properties = new Dictionary<string, object>();
			foreach (var propertyInfo in source.GetType().GetProperties())
			{
				if (propertyInfo.CanRead && propertyInfo.CanWrite)
				{
					var value = propertyInfo.GetValue(source, null);
					if (propertyInfo.PropertyType != typeof (IDictionary<string, string>))
					{
						properties.Add(propertyInfo.Name, value);
					}
					else
					{
						var dictionary = (IDictionary<string, string>) value;
						foreach (var propertyKey in dictionary.Keys)
						{
							properties.Add(propertyKey, dictionary[propertyKey]);
						}
					}
				}
			}


			var values = new List<object>();
			var rewrittenFormat = PropertyRegex.Replace(format, delegate(Match m)
			{
				var startGroup = m.Groups["start"];
				var propertyGroup = m.Groups["property"];
				var formatGroup = m.Groups["format"];
				var endGroup = m.Groups["end"];

				object value;
				values.Add(properties.TryGetValue(propertyGroup.Value, out value) ? value : source);
				return new string('{', startGroup.Captures.Count) + (values.Count - 1) + formatGroup.Value +
				       new string('}', endGroup.Captures.Count);
			});

			return string.Format(provider, rewrittenFormat, values.ToArray());
		}
	}
}
