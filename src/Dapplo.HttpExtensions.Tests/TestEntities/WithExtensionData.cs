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

using System.Collections.Generic;
using System.Runtime.Serialization;
using Dapplo.HttpExtensions.JsonSimple;

namespace Dapplo.HttpExtensions.Tests.TestEntities
{
    /// <summary>
    ///     Container for a test with Extension data
    /// </summary>
    [DataContract]
    internal class WithExtensionData
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [JsonExtensionData(Pattern = "customstringfield_.*")]
        public IDictionary<string, string> StringExtensionData { get; set; } = new Dictionary<string, string>();

        [JsonExtensionData(Pattern = "customintfield_.*")]
        public IDictionary<string, int> IntExtensionData { get; set; } = new Dictionary<string, int>();

        [JsonExtensionData]
        public IDictionary<string, object> RestExtensionData { get; set; } = new Dictionary<string, object>();
    }
}