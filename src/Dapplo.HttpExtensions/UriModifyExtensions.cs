// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;

namespace Dapplo.HttpExtensions;

/// <summary>
///     Uri extensions which modify an Uri (return a new one)
/// </summary>
public static class UriModifyExtensions
{
    /// <summary>
    ///     Append path segment(s) to the specified Uri
    ///     When adding a segment the logic takes care that there is always one single slash separating the segments.
    ///     Null or empty segments are ignored.
    /// </summary>
    /// <param name="uri">Uri to extend</param>
    /// <param name="segments">params with objects which will be converted to string and adding them as segments</param>
    /// <returns>new Uri with segments added to the path</returns>
    public static Uri AppendSegments(this Uri uri, params object[] segments)
    {
        return uri.AppendSegments(segments.Select(o => o?.ToString()));
    }

    /// <summary>
    ///     Append path segment(s) to the specified Uri
    ///     When adding a segment the logic takes care that there is always one single slash separating the segments.
    ///     Null or empty segments are ignored.
    /// </summary>
    /// <param name="uri">Uri to extend</param>
    /// <param name="urlEscapeFunc">Func used for the encoding</param>
    /// <param name="segments">params with objects which will be converted to string and adding them as segments</param>
    /// <returns>new Uri with segments added to the path</returns>
    public static Uri AppendSegments(this Uri uri, Func<string, string> urlEscapeFunc, params object[] segments)
    {
        return uri.AppendSegments(urlEscapeFunc, segments.Select(o => o?.ToString()));
    }

    /// <summary>
    ///     Append segment(s) to the path of the specified Uri.
    ///     When adding a segment the logic takes care that there is always one single slash separating the segments.
    ///     Null or empty segments are ignored.
    /// </summary>
    /// <param name="uri">Uri to extend</param>
    /// <param name="urlEscapeFunc">Func used for the encoding</param>
    /// <param name="segments">IEnumerable of string with the segments which need to be added</param>
    /// <returns>new Uri with segments added to the path</returns>
    public static Uri AppendSegments(this Uri uri, Func<string, string> urlEscapeFunc, IEnumerable<string> segments)
    {
        if (uri is null)
        {
            throw new ArgumentNullException(nameof(uri));
        }

        if (segments is null)
        {
            return uri;
        }

        var uriBuilder = new UriBuilder(uri);

        // Start with the content of the path of the Uri
        var stringBuilder = new StringBuilder(uriBuilder.Path);
        // Append a / when there wasn't one
        if (!uriBuilder.Path.EndsWith("/"))
        {
            stringBuilder.Append("/");
        }
        // Create the path of the segments, removing all starting and ending /, and encode the part
        var path = string.Join("/", segments
            .Where(s => !string.IsNullOrEmpty(s))
            .Select(s => s.TrimStart('/').TrimEnd('/'))
            .Where(s => !string.IsNullOrEmpty(s))
            .Select(urlEscapeFunc));

        // Append this to the string builder
        stringBuilder.Append(path);

        // Set the newly build path to the UriBuilder's path
        uriBuilder.Path = stringBuilder.ToString();

        // Create the Uri and return it
        return uriBuilder.Uri;
    }

    /// <summary>
    ///     Append segment(s) to the path of the specified Uri.
    ///     When adding a segment the logic takes care that there is always one single slash separating the segments.
    ///     Null or empty segments are ignored.
    /// </summary>
    /// <param name="uri">Uri to extend</param>
    /// <param name="segments">IEnumerable of string with the segments which need to be added</param>
    /// <returns>new Uri with segments added to the path</returns>
    public static Uri AppendSegments(this Uri uri, IEnumerable<string> segments)
    {
        return uri.AppendSegments(HttpExtensionsGlobals.DefaultUriEscapeFunc, segments);
    }

    /// <summary>
    ///     Adds query string value to an existing url, both absolute and relative URI's are supported.
    /// </summary>
    /// <code>
    ///     // returns "www.domain.com/test?param1=val1&amp;param2=val2&amp;param3=val3"
    ///     new Uri("www.domain.com/test?param1=val1").ExtendQuery(new Dictionary&lt;string, string&gt; { { "param2", "val2" }, { "param3", "val3" } }); 
    /// 
    ///     // returns "/test?param1=val1&amp;param2=val2&amp;param3=val3"
    ///     new Uri("/test?param1=val1").ExtendQuery(new Dictionary&lt;string, string&gt; { { "param2", "val2" }, { "param3", "val3" } }); 
    /// </code>
    /// <param name="uri">Uri to extend</param>
    /// <param name="name">string name of value</param>
    /// <param name="value">value</param>
    /// <returns>Uri with extended query</returns>
    public static Uri ExtendQuery<T>(this Uri uri, string name, T value)
    {
        var keyValuePairs = uri.QueryToKeyValuePairs().Concat(new[] {new KeyValuePair<string, string>(name, value?.ToString())});

        var uriBuilder = new UriBuilder(uri)
        {
            Query = keyValuePairs.ToQueryString()
        };
        return uriBuilder.Uri;
    }

    /// <summary>
    ///     Adds query string value to an existing url, both absolute and relative URI's are supported.
    /// </summary>
    /// <code>
    ///     // returns "www.domain.com/test?param1=val1&amp;param2=val2&amp;param3=val3"
    ///     new Uri("www.domain.com/test?param1=val1").ExtendQuery(new Dictionary&lt;string, string&gt; { { "param2", "val2" }, { "param3", "val3" } }); 
    /// 
    ///     // returns "/test?param1=val1&amp;param2=val2&amp;param3=val3"
    ///     new Uri("/test?param1=val1").ExtendQuery(new Dictionary&lt;string, string&gt; { { "param2", "val2" }, { "param3", "val3" } }); 
    /// </code>
    /// <param name="uri">Uri to extend</param>
    /// <param name="values">IDictionary with values</param>
    /// <returns>Uri with extended query</returns>
    public static Uri ExtendQuery<T>(this Uri uri, IDictionary<string, T> values)
    {
        if (values is null)
        {
            throw new ArgumentNullException(nameof(values));
        }
        return uri.ExtendQuery(values.Select(item => item));
    }

    /// <summary>
    ///     Adds query string value to an existing url, both absolute and relative URI's are supported.
    /// </summary>
    /// <param name="uri">Uri to extend</param>
    /// <param name="values">IEnumerable of KeyValuePair with values</param>
    /// <returns>Uri with extended query</returns>
    public static Uri ExtendQuery<T>(this Uri uri, IEnumerable<KeyValuePair<string, T>> values)
    {
        if (values is null)
        {
            throw new ArgumentNullException(nameof(values));
        }
        var keyValuePairs = uri.QueryToKeyValuePairs()
            .Concat(values
                .Select(nameValue => new KeyValuePair<string, string>(nameValue.Key, nameValue.Value?.ToString())));

        var uriBuilder = new UriBuilder(uri)
        {
            Query = keyValuePairs.ToQueryString()
        };
        return uriBuilder.Uri;
    }

    /// <summary>
    ///     Adds query string value to an existing url, both absolute and relative URI's are supported.
    /// </summary>
    /// <code>
    ///     // returns "www.domain.com/test?param1=val1&amp;param2=val2&amp;param3=val3"
    ///     new Uri("www.domain.com/test?param1=val1").ExtendQuery(new Dictionary&lt;string, string&gt; { { "param2", "val2" }, { "param3", "val3" } }); 
    /// 
    ///     // returns "/test?param1=val1&amp;param2=val2&amp;param3=val3"
    ///     new Uri("/test?param1=val1").ExtendQuery(new Dictionary&lt;string, string&gt; { { "param2", "val2" }, { "param3", "val3" } }); 
    /// </code>
    /// <param name="uri">Uri to extend the query for</param>
    /// <param name="values">ILookup with values</param>
    /// <returns>Uri with extended query</returns>
    public static Uri ExtendQuery<T>(this Uri uri, ILookup<string, T> values)
    {
        var keyValuePairs = uri.QueryToKeyValuePairs()
            .Concat(from kvp in values from value in kvp select new KeyValuePair<string, string>(kvp.Key, value?.ToString()));

        var uriBuilder = new UriBuilder(uri)
        {
            Query = keyValuePairs.ToQueryString()
        };
        return uriBuilder.Uri;
    }
}