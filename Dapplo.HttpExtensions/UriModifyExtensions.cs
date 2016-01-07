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
using System.Linq;
using System.Text;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// Uri extensions which modify an Uri (return a new one)
	/// </summary>
	public static class UriModifyExtensions
	{
		/// <summary>
		///     Adds query string value to an existing url, both absolute and relative URI's are supported.
		/// </summary>
		/// <example>
		/// <code>
		///     // returns "www.domain.com/test?param1=val1&amp;param2=val2&amp;param3=val3"
		///     new Uri("www.domain.com/test?param1=val1").ExtendQuery(new Dictionary&lt;string, string&gt; { { "param2", "val2" }, { "param3", "val3" } }); 
		/// 
		///     // returns "/test?param1=val1&amp;param2=val2&amp;param3=val3"
		///     new Uri("/test?param1=val1").ExtendQuery(new Dictionary&lt;string, string&gt; { { "param2", "val2" }, { "param3", "val3" } }); 
		/// </code>
		/// </example>
		/// <param name="uri">Uri to extend</param>
		/// <param name="name">string name of value</param>
		/// <param name="value">value</param>
		/// <returns>Uri with extended query</returns>
		public static Uri ExtendQuery<T>(this Uri uri, string name, T value)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}
			var keyValuePairs = uri.QueryToKeyValuePairs().Concat(new[] { new KeyValuePair<string, string>(name, value?.ToString()) });

			var uriBuilder = new UriBuilder(uri);
			if (!keyValuePairs.Any())
			{
				return uri;
			}
			uriBuilder.Query = keyValuePairs.ToQueryString();
			return uriBuilder.Uri;
		}

		/// <summary>
		///     Adds query string value to an existing url, both absolute and relative URI's are supported.
		/// </summary>
		/// <example>
		/// <code>
		///     // returns "www.domain.com/test?param1=val1&amp;param2=val2&amp;param3=val3"
		///     new Uri("www.domain.com/test?param1=val1").ExtendQuery(new Dictionary&lt;string, string&gt; { { "param2", "val2" }, { "param3", "val3" } }); 
		/// 
		///     // returns "/test?param1=val1&amp;param2=val2&amp;param3=val3"
		///     new Uri("/test?param1=val1").ExtendQuery(new Dictionary&lt;string, string&gt; { { "param2", "val2" }, { "param3", "val3" } }); 
		/// </code>
		/// </example>
		/// <param name="uri">Uri to extend</param>
		/// <param name="values">IDictionary with values</param>
		/// <returns>Uri with extended query</returns>
		public static Uri ExtendQuery<T>(this Uri uri, IDictionary<string, T> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException(nameof(values));
			}
			var keyValuePairs = uri.QueryToKeyValuePairs().Concat(values.Select(nameValue => new KeyValuePair<string, string>(nameValue.Key, nameValue.Value?.ToString())));

			var uriBuilder = new UriBuilder(uri);
			if (!keyValuePairs.Any())
			{
				return uri;
			}
			uriBuilder.Query = keyValuePairs.ToQueryString();
			return uriBuilder.Uri;
		}

		/// <summary>
		///     Adds query string value to an existing url, both absolute and relative URI's are supported.
		/// </summary>
		/// <example>
		/// <code>
		///     // returns "www.domain.com/test?param1=val1&amp;param2=val2&amp;param3=val3"
		///     new Uri("www.domain.com/test?param1=val1").ExtendQuery(new Dictionary&lt;string, string&gt; { { "param2", "val2" }, { "param3", "val3" } }); 
		/// 
		///     // returns "/test?param1=val1&amp;param2=val2&amp;param3=val3"
		///     new Uri("/test?param1=val1").ExtendQuery(new Dictionary&lt;string, string&gt; { { "param2", "val2" }, { "param3", "val3" } }); 
		/// </code>
		/// </example>
		/// <param name="uri">Uri to extend the query for</param>
		/// <param name="values">ILookup with values</param>
		/// <returns>Uri with extended query</returns>
		public static Uri ExtendQuery<T>(this Uri uri, ILookup<string, T> values)
		{
			var keyValuePairs = uri.QueryToKeyValuePairs().Concat(from kvp in values from value in kvp select new KeyValuePair<string, string>(kvp.Key, value?.ToString()));

			var uriBuilder = new UriBuilder(uri);
			if (!keyValuePairs.Any())
			{
				return uri;
			}
			uriBuilder.Query = keyValuePairs.ToQueryString();
			return uriBuilder.Uri;
		}

		/// <summary>
		///Sets the userinfo of the Uri
		/// </summary>
		/// <param name="uri">Uri to extend</param>
		/// <param name="username">username of value</param>
		/// <param name="password">password for the user</param>
		/// <returns>Uri with extended query</returns>
		public static Uri SetCredentials(this Uri uri, string username, string password)
		{
			var uriBuilder = new UriBuilder(uri);
			uriBuilder.UserName = username;
			uriBuilder.Password = password;
			return uriBuilder.Uri;
		}

		/// <summary>
		/// Append path segment(s) to the specified Uri
		/// </summary>
		/// <param name="uri">Uri to extend</param>
		/// <param name="segments">array of objects which will be added after converting them to strings</param>
		/// <returns>new Uri with segments added to the path</returns>
		public static Uri AppendSegments(this Uri uri, params object[] segments)
		{
			if (uri == null)
			{
				throw new ArgumentNullException(nameof(uri));
			}
			var uriBuilder = new UriBuilder(uri);

			if (segments != null)
			{
				var stringBuilder = new StringBuilder();
				// Only add the path if it contains more that just a /
				if (!"/".Equals(uriBuilder.Path))
				{
					stringBuilder.Append(uriBuilder.Path);
                }
				foreach (var segment in segments)
				{
					// Do nothing with null segments
					if (segment == null)
					{
						continue;
					}

					// Add a / if the current path doesn't end with it and the segment doesn't have one
					bool hasPathTrailingSlash = stringBuilder.ToString().EndsWith("/");
					bool hasSegmentTrailingSlash = segment.ToString().StartsWith("/");
					if (hasPathTrailingSlash && hasSegmentTrailingSlash)
					{
						// Remove trailing slash
						stringBuilder.Length -= 1;
					}
					else if (!hasPathTrailingSlash && !hasSegmentTrailingSlash)
					{
						stringBuilder.Append("/");
					}

					// Add the segment
					stringBuilder.Append(segment);
				}
				uriBuilder.Path = stringBuilder.ToString();
			}
			return uriBuilder.Uri;
		}
	}
}
