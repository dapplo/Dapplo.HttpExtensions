//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2018 Dapplo
// 
//  For more information see: http://dapplo.net/
//  Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
//  This file is part of Dapplo.HttpExtensions
// 
//  Dapplo.HttpExtensions is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  Dapplo.HttpExtensions is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have a copy of the GNU Lesser General Public License
//  along with Dapplo.HttpExtensions. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Dapplo.HttpExtensions.Support;
using Dapplo.Log;

#endregion

namespace Dapplo.HttpExtensions.Factory
{
    /// <summary>
    ///     Factory methods to create HttpContent
    /// </summary>
    public static class HttpContentFactory
    {
        private static readonly LogSource Log = new LogSource();

        /// <summary>
        ///     Create a HttpContent object from the supplied content
        /// </summary>
        /// <param name="inputType">type for the content</param>
        /// <param name="content">content</param>
        /// <returns>HttpContent</returns>
        public static HttpContent Create(Type inputType, object content)
        {
            if (content is null)
            {
                return null;
            }

            if (typeof(HttpContent).IsAssignableFrom(inputType))
            {
                return content as HttpContent;
            }

            var httpBehaviour = HttpBehaviour.Current;

            var httpContentAttribute = inputType.GetTypeInfo().GetCustomAttribute<HttpRequestAttribute>();
            // Process the input type
            if (httpContentAttribute is null)
            {
                return Create(httpBehaviour, inputType, content);
            }

            var contentItems = new List<ContentItem>();
            // We have a type which specifies the request content
            foreach (var propertyInfo in inputType.GetProperties())
            {
                var httpAttribute = propertyInfo.GetCustomAttribute<HttpPartAttribute>();
                if (httpAttribute is null)
                {
                    continue;
                }
                // skip all not request based HttpParts
                if (!httpAttribute.Part.ToString().StartsWith("Request"))
                {
                    continue;
                }

                var order = httpAttribute.Order;
                var contentItem = contentItems.FirstOrDefault(ci => ci.Order == order) ?? new ContentItem {Order = order};

                switch (httpAttribute.Part)
                {
                    case HttpParts.RequestContent:
                        var value = propertyInfo.GetValue(content);
                        if (value is null)
                        {
                            continue;
                        }
                        contentItem.Content = value;
                        break;
                    case HttpParts.RequestContentType:
                        contentItem.ContentType = propertyInfo.GetValue(content) as string;
                        break;
                    case HttpParts.RequestMultipartName:
                        contentItem.ContentName = propertyInfo.GetValue(content) as string;
                        break;
                    case HttpParts.RequestMultipartFilename:
                        contentItem.ContentFileName = propertyInfo.GetValue(content) as string;
                        break;
                    default:
                        // No know request value, go to the next
                        continue;
                }
                if (contentItems.All(x => x.Order != contentItem.Order))
                {
                    contentItems.Add(contentItem);
                }
            }
            // Having a HttpContentAttribute with MultiPart= true will skip this, even if one content is used
            if (contentItems.Count == 1 && !httpContentAttribute.MultiPart)
            {
                var contentItem = contentItems[0];
                return Create(httpBehaviour, contentItem);
            }

            if (contentItems.Count <= 0)
            {
                return Create(httpBehaviour, inputType, content);
            }

            var multipartContent = new MultipartFormDataContent();

            foreach (var contentItem in contentItems.OrderBy(x => x.Order))
            {
                // If the content would be null, and we would continue, a NullPointerReference would be thrown
                if (contentItem.Content is null)
                {
                    Log.Debug().WriteLine("Skipping content {0} as the content is null.", contentItem.ContentName);
                    continue;
                }
                var httpContent = Create(httpBehaviour, contentItem);
                if (contentItem.ContentName != null && contentItem.ContentFileName != null)
                {
                    multipartContent.Add(httpContent, contentItem.ContentName, contentItem.ContentFileName);
                }
                else if (contentItem.ContentName != null)
                {
                    multipartContent.Add(httpContent, contentItem.ContentName);
                }
                else
                {
                    multipartContent.Add(httpContent);
                }
            }
            return multipartContent;
        }

        /// <summary>
        ///     Helper method to create content
        /// </summary>
        /// <param name="httpBehaviour">IHttpBehaviour</param>
        /// <param name="contentItem"></param>
        /// <returns>HttpContent</returns>
        private static HttpContent Create(IHttpBehaviour httpBehaviour, ContentItem contentItem)
        {
            return Create(httpBehaviour, contentItem.Content.GetType(), contentItem.Content, contentItem.ContentType);
        }

        /// <summary>
        ///     Helper method to create content
        /// </summary>
        /// <param name="httpBehaviour">IHttpBehaviour</param>
        /// <param name="inputType">Type</param>
        /// <param name="content">object</param>
        /// <param name="contentType">if a specific content-Type is needed, specify it</param>
        /// <returns>HttpContent</returns>
        private static HttpContent Create(IHttpBehaviour httpBehaviour, Type inputType, object content, string contentType = null)
        {
            HttpContent resultHttpContent;

            if (typeof(HttpContent).IsAssignableFrom(inputType))
            {
                resultHttpContent = content as HttpContent;
            }
            else
            {
                var httpContentConverter = httpBehaviour.HttpContentConverters.OrderBy(x => x.Order).FirstOrDefault(x => x.CanConvertToHttpContent(inputType, content));
                if (httpContentConverter is null)
                {
                    return null;
                }

                resultHttpContent = httpContentConverter.ConvertToHttpContent(inputType, content);
            }

            if (contentType != null)
            {
                resultHttpContent?.SetContentType(contentType);
            }

            // Make sure the OnHttpContentCreated function is called
            if (httpBehaviour.OnHttpContentCreated != null)
            {
                return httpBehaviour.OnHttpContentCreated.Invoke(resultHttpContent);
            }
            return resultHttpContent;
        }

        /// <summary>
        ///     Create a HttpContent object from the supplied content
        /// </summary>
        /// <typeparam name="TInput">type for the content</typeparam>
        /// <param name="content">content</param>
        /// <returns>HttpContent</returns>
        public static HttpContent Create<TInput>(TInput content) where TInput : class
        {
            return Create(typeof(TInput), content);
        }
    }
}