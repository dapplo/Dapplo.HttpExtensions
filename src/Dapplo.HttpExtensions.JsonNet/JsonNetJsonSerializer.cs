// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
#if NETFRAMEWORK
using System.Drawing;
using System.Windows.Media.Imaging;
#endif

namespace Dapplo.HttpExtensions.JsonNet
{
    /// <summary>
    ///     Made to have Dapplo.HttpExtension use Json.NET
    /// </summary>
    public class JsonNetJsonSerializer : IJsonSerializer
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
                HttpExtensionsGlobals.JsonSerializer = new JsonNetJsonSerializer();
            }
        }

        /// <summary>
        ///     The JsonSerializerSettings used in the JsonNetJsonSerializer
        /// </summary>
        public JsonSerializerSettings Settings { get; set; } = new JsonSerializerSettings
        {
            DateParseHandling = DateParseHandling.None,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            ContractResolver = new ReadOnlyConsideringContractResolver()
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
        /// <param name="targetType"></param>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public object Deserialize(Type targetType, string jsonString)
        {
            return JsonConvert.DeserializeObject(jsonString, targetType, Settings);
        }

        /// <summary>
        /// Serialize the passed object into a json string
        /// </summary>
        /// <param name="jsonObject"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string Serialize<T>(T jsonObject)
        {
            return JsonConvert.SerializeObject(jsonObject, Settings);
        }
    }
}