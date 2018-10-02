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
using System.IO;
using System.Linq;

#if NET461
using System.Drawing;
using System.Windows.Media.Imaging;
#endif

#endregion

namespace Dapplo.HttpExtensions.JsonSimple
{
    /// <summary>
    ///     This defines the default way how Json is de-/serialized.
    /// </summary>
    public class SimpleJsonSerializer : IJsonSerializer
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
        ///     Register this IJsonSerializer globally
        /// </summary>
        /// <param name="force">bool to specify if this also needs to be set when another serializer is already specified</param>
        public static void RegisterGlobally(bool force = true)
        {
            if (force || HttpExtensionsGlobals.JsonSerializer != null)
            {
                HttpExtensionsGlobals.JsonSerializer = new SimpleJsonSerializer();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="targetType">Type to deserialize from a json string</param>
        /// <param name="jsonString">json</param>
        /// <returns>Deserialized json as targetType</returns>
        public object Deserialize(Type targetType, string jsonString)
        {
            return SimpleJson.DeserializeObject(jsonString, targetType);
        }

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
        /// </summary>
        /// <typeparam name="TContent">Type to serialize to json string</typeparam>
        /// <param name="jsonObject">The actual object</param>
        /// <returns>string with json</returns>
        public string Serialize<TContent>(TContent jsonObject)
        {
            return SimpleJson.SerializeObject(jsonObject);
        }
    }
}