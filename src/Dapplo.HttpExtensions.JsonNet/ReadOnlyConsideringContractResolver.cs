// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Dapplo.HttpExtensions.JsonNet
{
    /// <summary>
    ///     This takes care that the members are only serialized when there is no ReadOnlyAttribute
    /// </summary>
    public class ReadOnlyConsideringContractResolver : DefaultContractResolver
    {
        /// <summary>
        ///     This takes care that the members are only serialized when there is no ReadOnlyAttribute
        /// </summary>
        /// <param name="member">MemberInfo</param>
        /// <param name="memberSerialization">MemberSerialization</param>
        /// <returns>JsonProperty</returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            property.ShouldSerialize = o => member.GetCustomAttribute<ReadOnlyAttribute>() is null;
            return property;
        }
    }
}