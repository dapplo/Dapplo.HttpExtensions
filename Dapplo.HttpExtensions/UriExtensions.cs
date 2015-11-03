/*
 * dapplo - building blocks for desktop applications
 * Copyright (C) 2015 Robin Krom
 * 
 * For more information see: http://dapplo.net/
 * dapplo repositories are hosted on GitHub: https://github.com/dapplo
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// Extensions for the Uri class
	/// </summary>
	public static class UriExtensions
	{
		/// <summary>
		/// Create a NameValueCollection from the query part of the uri
		/// </summary>
		/// <param name="uri"></param>
		/// <returns>NameValueCollection</returns>
		public static NameValueCollection QueryToNameValues(this Uri uri)
		{
			if (!string.IsNullOrEmpty(uri.Query))
			{
				return HttpUtility.ParseQueryString(uri.Query);
			}
			return new NameValueCollection();
		}

		/// <summary>
		/// QueryToDictionary creates a IDictionary with name-values without using System.Web
		/// </summary>
		/// <param name="uri"></param>
		/// <returns>IDictionary string, string</returns>
		public static IDictionary<string, string> QueryToDictionary(this Uri uri)
		{
			var parameters = new SortedDictionary<string, string>();
			var queryString = uri.Query;
			// remove anything other than query string from uri
			if (queryString.Contains("?"))
			{
				queryString = queryString.Substring(queryString.IndexOf('?') + 1);
			}
			foreach (string vp in Regex.Split(queryString, "&"))
			{
				if (string.IsNullOrEmpty(vp))
				{
					continue;
				}
				string[] singlePair = Regex.Split(vp, "=");
				if (parameters.ContainsKey(singlePair[0]))
				{
					parameters.Remove(singlePair[0]);
				}
				parameters.Add(singlePair[0], singlePair.Length == 2 ? singlePair[1] : string.Empty);
			}
			return parameters;
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
		/// <param name="uri"></param>
		/// <param name="values"></param>
		/// <returns>Uri</returns>
		public static Uri ExtendQuery<T>(this Uri uri, IDictionary<string, T> values)
		{
			var queryCollection = uri.QueryToNameValues();
			foreach (var kvp in uri.QueryToDictionary())
			{
				queryCollection[kvp.Key] = kvp.Value;
			}

			var uriBuilder = new UriBuilder(uri);
			if (queryCollection.Count > 0)
			{
				uriBuilder.Query = queryCollection.ToQueryString();
			}
			return uriBuilder.Uri;
		}

		/// <summary>
		/// Normalize the URI by replacing http...80 and https...443 without the port.
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
		/// <param name="uri"></param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>HttpContentHeaders</returns>
		public static async Task<HttpContentHeaders> HeadAsync(this Uri uri, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			using (var client = HttpClientFactory.CreateHttpClient(httpSettings))
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
		/// <param name="uri"></param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>HttpResponseMessage</returns>
		public static async Task<HttpResponseMessage> PostAsync(this Uri uri, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			using (var client = HttpClientFactory.CreateHttpClient(httpSettings))
			{
				return await client.PostAsync(uri, token);
			}
		}

		/// <summary>
		/// Method to Post content
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>HttpResponseMessage</returns>
		public static async Task<HttpResponseMessage> PostAsync(this Uri uri, HttpContent content, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			using (var client = HttpClientFactory.CreateHttpClient(httpSettings))
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
			using (var client = HttpClientFactory.CreateHttpClient(httpSettings))
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
			using (var client = HttpClientFactory.CreateHttpClient(httpSettings))
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
		/// <param name="uri"></param>
		/// <param name="postData">T</param>
		/// <param name="token"></param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>HttpResponseMessage</returns>
		public static async Task<HttpResponseMessage> PostJsonAsync<T>(this Uri uri, T jsonContent, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			using (var client = HttpClientFactory.CreateHttpClient(httpSettings))
			{
				return await client.PostJsonAsync<T>(uri, jsonContent, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Method to post with JSON, and get JSON
		/// </summary>
		/// <typeparam name="T1">Type to post</typeparam>
		/// <typeparam name="T2">Type to read from the response</typeparam>
		/// <param name="uri"></param>
		/// <param name="postData">T1</param>
		/// <param name="throwErrorOnNonSuccess"></param>
		/// <param name="token"></param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>T2</returns>
		public static async Task<T2> PostJsonAsync<T1, T2>(this Uri uri, T1 jsonContent, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			using (var client = HttpClientFactory.CreateHttpClient(httpSettings))
			{
				return await client.PostJsonAsync<T1, T2>(uri, jsonContent, throwErrorOnNonSuccess, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Download a uri response as string
		/// </summary>
		/// <param name="uri">An Uri to specify the download location</param>
		/// <param name="throwErrorOnNonSuccess"></param>
		/// <param name="token">CancellationToken</param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>string with the content</returns>
		public static async Task<string> GetAsStringAsync(this Uri uri, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			using (var client = HttpClientFactory.CreateHttpClient(httpSettings))
			using (var response = await client.GetAsync(uri, HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(false))
			{
				return await response.GetAsStringAsync(throwErrorOnNonSuccess, token).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Get the content as a MemoryStream
		/// </summary>
		/// <param name="uri">Uri</param>
		/// <param name="throwErrorOnNonSuccess">bool</param>
		/// <param name="token"></param>
		/// <param name="httpSettings">IHttpSettings instance or null if the global settings need to be used</param>
		/// <returns>MemoryStream</returns>
		public static async Task<MemoryStream> GetAsMemoryStreamAsync(this Uri uri, bool throwErrorOnNonSuccess = true, CancellationToken token = default(CancellationToken), IHttpSettings httpSettings = null)
		{
			using (var client = HttpClientFactory.CreateHttpClient(httpSettings))
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
					if (!stringBuilder.ToString().EndsWith("/") && !segment.ToString().StartsWith("/"))
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
