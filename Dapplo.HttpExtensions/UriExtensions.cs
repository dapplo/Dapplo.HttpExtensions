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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// Extensions for the Uri class
	/// </summary>
	public static class UriExtensions
	{
		/// <summary>
		/// Create a query string from a list of tuples
		/// </summary>
		/// <param name="nameValueCollection">list of tuple string,string</param>
		/// <returns>name1=value1&amp;name2=value2 etc...</returns>
		private static string ToQueryString(this IEnumerable<Tuple<string, string>> nameValueCollection)
		{
			var queryBuilder = new StringBuilder();

			foreach (var tuple in nameValueCollection)
			{
				queryBuilder.AppendFormat(tuple.Item2 != null ? $"{tuple.Item1}={tuple.Item2}&" : $"{tuple.Item1}");
			}
			queryBuilder.Length -= 1;
			return queryBuilder.ToString();
		}

		/// <summary>
		/// QueryToDictionary creates a IDictionary with name-values
		/// </summary>
		/// <param name="uri">Uri of which the query is processed</param>
		/// <returns>IDictionary string, string</returns>
		public static IDictionary<string, string> QueryToDictionary(this Uri uri)
		{
			var parameters = new SortedDictionary<string, string>();

			foreach (var tuple in uri.QueryToTuples())
			{
				if (parameters.ContainsKey(tuple.Item1))
				{
					parameters[tuple.Item1] = tuple.Item2;
				}
				else
				{
					parameters.Add(tuple.Item1, tuple.Item2);
				}
			}
			return parameters;
		}

		/// <summary>
		/// QueryToTuples creates a List with Tuples which have the name-values
		/// </summary>
		/// <param name="uri">Uri of which the query is processed</param>
		/// <returns>List Tuple string, string</returns>
		public static List<Tuple<string, string>> QueryToTuples(this Uri uri)
		{
			var parameters = new List<Tuple<string, string>>();
			var queryString = uri.Query;
			// remove anything other than query string from uri
			if (queryString.StartsWith("?"))
			{
				queryString = queryString.Substring(1);
			}
			foreach (string vp in Regex.Split(queryString, "&"))
			{
				if (string.IsNullOrEmpty(vp))
				{
					continue;
				}
				var singlePair = Regex.Split(vp, "=");
				parameters.Add(new Tuple<string, string>(singlePair[0], singlePair.Length == 2 ? singlePair[1] : string.Empty));
			}
			return parameters;
		}

		/// <summary>
		/// QueryToLookup creates a ILookup with name-values
		/// </summary>
		/// <param name="uri">Uri of which the query is processed</param>
		/// <returns>ILookup string, string</returns>
		public static ILookup<string, string> QueryToLookup(this Uri uri)
		{
			var parameters = uri.QueryToTuples();
			return parameters.ToLookup(k => k.Item1, e => e.Item2);
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
		/// <param name="name">string name of value</param>
		/// <param name="value">value</param>
		/// <returns>Uri with extended query</returns>
		public static Uri ExtendQuery<T>(this Uri uri, string name, T value)
		{
			var tuples = uri.QueryToTuples();
			tuples.Add(new Tuple<string, string>(name, value?.ToString()));

			var uriBuilder = new UriBuilder(uri);
			if (!tuples.Any())
			{
				return uri;
			}
			uriBuilder.Query = tuples.ToQueryString();
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
			var tuples = uri.QueryToTuples();
			tuples.AddRange(values.Select(nameValue => new Tuple<string, string>(nameValue.Key, nameValue.Value?.ToString())));

			var uriBuilder = new UriBuilder(uri);
			if (!tuples.Any())
			{
				return uri;
			}
			uriBuilder.Query = tuples.ToQueryString();
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
			var tuples = uri.QueryToTuples();
			tuples.AddRange(from kvp in values from value in kvp select new Tuple<string, string>(kvp.Key, value?.ToString()));

			var uriBuilder = new UriBuilder(uri);
			if (!tuples.Any())
			{
				return uri;
			}
			uriBuilder.Query = tuples.ToQueryString();
			return uriBuilder.Uri;
		}

		/// <summary>
		/// Normalize the URI by replacing http...80 and https...443 without the port.
		/// Is needed for OAuth 1.0(a)
		/// </summary>
		/// <param name="uri">Uri to normalize</param>
		/// <returns>Uri</returns>
		public static Uri Normalize(this Uri uri)
		{
			string normalizedUrl = string.Format(CultureInfo.InvariantCulture, "{0}://{1}", uri.Scheme, uri.Host);
			if (!((uri.Scheme == "http" && uri.Port == 80) || (uri.Scheme == "https" && uri.Port == 443)))
			{
				normalizedUrl += ":" + uri.Port;
			}
			normalizedUrl += uri.AbsolutePath;
			return new Uri(normalizedUrl);
		}

		/// <summary>
		/// Get LastModified for a URI
		/// </summary>
		/// <param name="uri">Uri</param>
		/// <param name="throwErrorOnNonSuccess">true to throw an exception when an error is returned, else DateTimeOffset.MinValue is returned</param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>DateTime</returns>
		public static async Task<DateTimeOffset> LastModifiedAsync(this Uri uri, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			try
			{
				var headers = await uri.HeadAsync(throwErrorOnNonSuccess, token, httpSettings).ConfigureAwait(false);
				if (headers.LastModified.HasValue)
				{
					return headers.LastModified.Value;
				}
			}
			catch
			{
				// Ignore
			}
			// Pretend it is old
			return DateTimeOffset.MinValue;
		}

		/// <summary>
		/// Retrieve only the content headers, by using the HTTP HEAD method
		/// </summary>
		/// <param name="uri">Uri to get HEAD for</param>
		/// <param name="throwErrorOnNonSuccess">true to throw an exception when an error is returned, else the headers are returned</param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>HttpContentHeaders</returns>
		public static async Task<HttpContentHeaders> HeadAsync(this Uri uri, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			using (var client = HttpClientFactory.CreateHttpClient(httpSettings, uri))
			using (var request = new HttpRequestMessage(HttpMethod.Head, uri))
			{
				var responseMessage = await client.SendAsync(request, token).ConfigureAwait(false);
				await responseMessage.HandleErrorAsync(throwErrorOnNonSuccess, token);
				return responseMessage.Content.Headers;
			}
		}

		/// <summary>
		/// Method to Post without content
		/// </summary>
		/// <param name="uri">Uri to post to</param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>HttpResponseMessage</returns>
		public static async Task<HttpResponseMessage> PostAsync(this Uri uri, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			using (var client = HttpClientFactory.CreateHttpClient(httpSettings, uri))
			{
				return await client.PostAsync(uri, token);
			}
		}

		/// <summary>
		/// Method to Post content
		/// </summary>
		/// <param name="uri">Uri to post to</param>
		/// <param name="content">HttpContent to post</param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>HttpResponseMessage</returns>
		public static async Task<HttpResponseMessage> PostAsync(this Uri uri, HttpContent content, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			using (var client = HttpClientFactory.CreateHttpClient(httpSettings, uri))
			{
				return await client.PostAsync(uri, content, token);
			}
		}

		/// <summary>
		/// Simple extension to post Form-URLEncoded Content
		/// </summary>
		/// <param name="uri">Uri to post to</param>
		/// <param name="formContent">Dictionary with the values</param>
		/// <param name="token">Cancellationtoken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>HttpResponseMessage</returns>
		public static async Task<HttpResponseMessage> PostFormUrlEncodedAsync(this Uri uri, IDictionary<string, string> formContent, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			using (var content = new FormUrlEncodedContent(formContent))
			using (var client = HttpClientFactory.CreateHttpClient(httpSettings, uri))
			{
				return await client.PostAsync(uri, content, token);
			}
		}

		/// <summary>
		/// Download a uri response as string
		/// </summary>
		/// <param name="uri">An Uri to specify the download location</param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>HttpResponseMessage</returns>
		public static async Task<HttpResponseMessage> GetAsync(this Uri uri, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			using (var client = HttpClientFactory.CreateHttpClient(httpSettings, uri))
			{
				return await client.GetAsync(uri, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Download a Json response
		/// </summary>
		/// <param name="uri">An Uri to specify the download location</param>
		/// <param name="throwErrorOnNonSuccess"></param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>dynamic created with SimpleJson</returns>
		public static async Task<dynamic> GetAsJsonAsync(this Uri uri, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			using (var reponse = await uri.GetAsync(token, httpSettings).ConfigureAwait(false))
			{
				return await reponse.GetAsJsonAsync(throwErrorOnNonSuccess, token).ConfigureAwait(false);
            }
		}

		/// <summary>
		/// Download a Json response
		/// </summary>
		/// <typeparam name="T">Type to deserialize into</typeparam>
		/// <param name="uri">An Uri to specify the download location</param>
		/// <param name="throwErrorOnNonSuccess"></param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>T created with SimpleJson</returns>
		public static async Task<T> GetAsJsonAsync<T>(this Uri uri, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			using (var reponse = await uri.GetAsync(token, httpSettings).ConfigureAwait(false))
			{
				return await reponse.GetAsJsonAsync<T>(throwErrorOnNonSuccess, token).ConfigureAwait(false);
			}
		}


		/// <summary>
		/// Method to post JSON
		/// </summary>
		/// <typeparam name="T">Type to post</typeparam>
		/// <param name="uri">Uri to post json to</param>
		/// <param name="jsonContent">T</param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>HttpResponseMessage</returns>
		public static async Task<HttpResponseMessage> PostJsonAsync<T>(this Uri uri, T jsonContent, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			using (var client = HttpClientFactory.CreateHttpClient(httpSettings, uri))
			{
				return await client.PostJsonAsync(uri, jsonContent, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Method to post with JSON, and get JSON
		/// </summary>
		/// <typeparam name="T1">Type to post</typeparam>
		/// <typeparam name="T2">Type to read from the response</typeparam>
		/// <param name="uri">Uri to post to</param>
		/// <param name="jsonContent">T1</param>
		/// <param name="throwErrorOnNonSuccess">true to throw an exception when an error occurse, else null is returned</param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>T2</returns>
		public static async Task<T2> PostJsonAsync<T1, T2>(this Uri uri, T1 jsonContent, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			using (var client = HttpClientFactory.CreateHttpClient(httpSettings, uri))
			{
				return await client.PostJsonAsync<T1, T2>(uri, jsonContent, throwErrorOnNonSuccess, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Download a uri response as string
		/// </summary>
		/// <param name="uri">An Uri to specify the download location</param>
		/// <param name="throwErrorOnNonSuccess">true to throw an exception when an error occurse, else null is returned</param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>string with the content</returns>
		public static async Task<string> GetAsStringAsync(this Uri uri, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			using (var client = HttpClientFactory.CreateHttpClient(httpSettings, uri))
			using (var response = await client.GetAsync(uri, HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(false))
			{
				return await response.GetAsStringAsync(throwErrorOnNonSuccess, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Get the content as a MemoryStream
		/// </summary>
		/// <param name="uri">Uri</param>
		/// <param name="throwErrorOnNonSuccess">true to throw an exception when an error occurse, else null is returned</param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>MemoryStream</returns>
		public static async Task<MemoryStream> GetAsMemoryStreamAsync(this Uri uri, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			using (var client = HttpClientFactory.CreateHttpClient(httpSettings, uri))
			{
				return await client.GetAsMemoryStreamAsync(uri, throwErrorOnNonSuccess, token);
			}
		}

		/// <summary>
		/// Append path segment(s) to the specified Uri
		/// </summary>
		/// <param name="uri">Uri to extend</param>
		/// <param name="segments">array of objects which will be added after converting them to strings</param>
		/// <returns>new Uri with segments added to the path</returns>
		public static Uri AppendSegments(this Uri uri, params object[] segments)
		{
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
