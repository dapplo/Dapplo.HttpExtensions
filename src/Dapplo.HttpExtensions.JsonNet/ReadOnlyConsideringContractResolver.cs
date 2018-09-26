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

using System.ComponentModel;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

#endregion

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