//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2019 Dapplo
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
using System.IO;
using System.Linq;
using Newtonsoft.Json;
#if NET461
using System.Drawing;
using System.Windows.Media.Imaging;
#endif

#endregion

namespace Dapplo.HttpExtensions.JsonNet
{
    /// <summary>
    ///     Made to have Dapplo.HttpExtension use Json.NET
    /// </summary>
    public class JsonNetJsonSerializer : IJsonSerializer
    {
        private static readonly Type[] NotSerializableTypes =
        {
#if NET461
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