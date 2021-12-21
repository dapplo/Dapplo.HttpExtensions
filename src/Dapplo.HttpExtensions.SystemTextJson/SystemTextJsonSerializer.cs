// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
#if NETFRAMEWORK
using System.Drawing;
using System.Windows.Media.Imaging;
#endif

namespace Dapplo.HttpExtensions.SystemTextJson
{
    /// <summary>
    ///     Made to have Dapplo.HttpExtension use System.Text.Json
    /// </summary>
    public class SystemTextJsonSerializer : IJsonSerializer
    {
        private static readonly Type[] NotSerializableTypes =
        {
#if NETFRAMEWORK
            typeof(Bitmap),
            typeof(BitmapSource),
#endif
            typeof(Stream),
            typeof(MemoryStream)
        };

        /// <summary>
        ///     Register this IJsonSerializer
        /// </summary>
        /// <param name="force">bool to specify if this also needs to be set when another serializer is already specified</param>
        public static void RegisterGlobally(bool force = true)
        {
            if (force || HttpExtensionsGlobals.JsonSerializer != null)
            {
                HttpExtensionsGlobals.JsonSerializer = new SystemTextJsonSerializer();
            }
        }

        /// <summary>
        ///     The JsonSerializerOptions used in the JsonSerializer
        /// </summary>
        public JsonSerializerOptions Options { get; set; } = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };


        /// <summary>
        ///     Test if the specified type can be serialized to JSON
        /// </summary>
        /// <param name="sourceType">Type to check</param>
        /// <returns>bool</returns>
        public bool CanSerializeTo(Type sourceType)
        {
            return NotSerializableTypes.All(type => type != sourceType);
        }

        /// <summary>
        ///     Test if the specified type can be deserialized
        /// </summary>
        /// <param name="targetType">Type to check</param>
        /// <returns>bool</returns>
        public bool CanDeserializeFrom(Type targetType)
        {
            return NotSerializableTypes.All(type => type != targetType);
        }

        /// <summary>
        /// Deserialize the specified json string into the target type
        /// </summary>
        /// <param name="targetType">Type</param>
        /// <param name="jsonString">string</param>
        /// <returns>object</returns>
        public object Deserialize(Type targetType, string jsonString)
        {
            return JsonSerializer.Deserialize(jsonString, targetType, Options);
        }

        /// <summary>
        /// Serialize the passed object into a json string
        /// </summary>
        /// <param name="jsonObject">object of type T</param>
        /// <typeparam name="T">type for the object to serialize</typeparam>
        /// <returns>string</returns>
        public string Serialize<T>(T jsonObject)
        {
            return JsonSerializer.Serialize(jsonObject, Options);
        }
    }
}