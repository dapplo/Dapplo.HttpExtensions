// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.Extensions;
using Dapplo.HttpExtensions.Support;
using Dapplo.Log;

namespace Dapplo.HttpExtensions.JsonSimple
{
    /// <summary>
    ///     This can convert HttpContent from/to Json
    ///     TODO: add JsonObject from SimpleJson for more clear generic code..
    /// </summary>
    public class DefaultJsonHttpContentConverter : IHttpContentConverter
    {
        private static readonly LogSource Log = new LogSource();

        /// <summary>
        ///     Instance of this IHttpContentConverter for reusing
        /// </summary>
        public static Lazy<DefaultJsonHttpContentConverter> Instance { get; } = new Lazy<DefaultJsonHttpContentConverter>(() => new DefaultJsonHttpContentConverter());

        private static readonly IList<string> SupportedContentTypes = new List<string>();

        static DefaultJsonHttpContentConverter()
        {
            SupportedContentTypes.Add(MediaTypes.Json.EnumValueOf());
        }

        /// <summary>
        ///     If the json content is any longer than LogThreshold AppendedWhenCut is appended to the cut string
        /// </summary>
        public string AppendedWhenCut { get; set; } = "...";

        /// <summary>
        ///     This is the amount of characters that are written to the log, if the json content is any longer that it will be cut
        ///     (and AppendedWhenCut is appended)
        /// </summary>
        public int LogThreshold { get; set; } = 256;

        /// <inheritdoc />
        public int Order => int.MaxValue;

        /// <inheritdoc />
        public bool CanConvertFromHttpContent(Type typeToConvertTo, HttpContent httpContent)
        {
            var httpBehaviour = HttpBehaviour.Current;
            if (httpBehaviour.JsonSerializer is null || !httpBehaviour.JsonSerializer.CanDeserializeFrom(typeToConvertTo))
            {
                return false;
            }
            if (!typeToConvertTo.GetTypeInfo().IsClass && !typeToConvertTo.GetTypeInfo().IsInterface)
            {
                return false;
            }
            return !httpBehaviour.ValidateResponseContentType || SupportedContentTypes.Contains(httpContent.GetContentType());
        }

        /// <inheritdoc />
        public async Task<object> ConvertFromHttpContentAsync(Type resultType, HttpContent httpContent, CancellationToken cancellationToken = default)
        {
            if (!CanConvertFromHttpContent(resultType, httpContent))
            {
                throw new NotSupportedException("CanConvertFromHttpContent resulted in false, this is not supposed to be called.");
            }

            var jsonString = await httpContent.ReadAsStringAsync().ConfigureAwait(false);
            // Check if verbose is enabled, if so log but only up to a certain size
            if (Log.IsVerboseEnabled())
            {
                if (LogThreshold > 0)
                {
                    Log.Verbose()
                        .WriteLine("Read Json content: {0}{1}", jsonString.Substring(0, Math.Min(jsonString.Length, LogThreshold)),
                            jsonString.Length > LogThreshold ? AppendedWhenCut : string.Empty);
                }
                else
                {
                    Log.Verbose().WriteLine("Read Json content: {0}", jsonString);
                }
            }
            // Check if we can just pass it back, as the target is string
            if (resultType == typeof(string))
            {
                return jsonString;
            }
            // empty json should return the default of the resultType
            if (string.IsNullOrEmpty(jsonString))
            {
                if (resultType.GetTypeInfo().IsValueType)
                {
                    return Activator.CreateInstance(resultType);
                }
                return null;
            }
            var httpBehaviour = HttpBehaviour.Current;
            try
            {
                return httpBehaviour.JsonSerializer.Deserialize(resultType == typeof(object) ? null : resultType, jsonString);
            }
            catch (SerializationException sEx)
            {
                // Make sure that the content which can't be deserialized is visible in the log.
                Log.Error().WriteLine(sEx, "Can't deserialize the JSON from the server.");
                Log.Error().WriteLine(jsonString);
                throw;
            }
        }

        /// <inheritdoc />
        public bool CanConvertToHttpContent(Type typeToConvert, object content)
        {
            var httpBehaviour = HttpBehaviour.Current;
            if (httpBehaviour.JsonSerializer is null || !httpBehaviour.JsonSerializer.CanSerializeTo(typeToConvert))
            {
                return false;
            }

            return typeToConvert != typeof(string);
        }

        /// <inheritdoc />
        public HttpContent ConvertToHttpContent(Type typeToConvert, object content)
        {
            var httpBehaviour = HttpBehaviour.Current;
            var jsonString = httpBehaviour.JsonSerializer.Serialize(content);
            Log.Debug().WriteLine("Created HttpContent for Json: {0}", jsonString);
            return new StringContent(jsonString, httpBehaviour.DefaultEncoding, MediaTypes.Json.EnumValueOf());
        }

        /// <summary>
        ///     Add Accept-Headers to the HttpRequestMessage, depending on the passt resultType.
        ///     This tries to hint the Http server what we can accept, which depends on the type of the return value
        /// </summary>
        /// <param name="resultType">Result type, this where to a conversion from HttpContent is made</param>
        /// <param name="httpRequestMessage">HttpRequestMessage</param>
        public void AddAcceptHeadersForType(Type resultType, HttpRequestMessage httpRequestMessage)
        {
            if (resultType is null)
            {
                throw new ArgumentNullException(nameof(resultType));
            }
            if (httpRequestMessage is null)
            {
                throw new ArgumentNullException(nameof(httpRequestMessage));
            }
            var httpBehaviour = HttpBehaviour.Current;
            if (httpBehaviour.JsonSerializer?.CanSerializeTo(resultType) != true)
            {
                return;
            }
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypes.Json.EnumValueOf()));
            Log.Debug().WriteLine("Modified the header(s) of the HttpRequestMessage: Accept: {0}", httpRequestMessage.Headers.Accept);
        }
    }
}