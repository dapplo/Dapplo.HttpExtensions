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

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Dapplo.HttpExtensions.Tests.TestEntities
{
    /// <summary>
    ///     Container for the release information from GitHub
    /// </summary>
    [DataContract]
    public class SerializeTestEntity
    {
        [DataMember(Name = "valueEmitDefaultFalse", EmitDefaultValue = false)]
        public string ValueEmitDefaultFalse { get; set; }

        [DataMember(Name = "valueNormal")]
        public string ValueNormal { get; set; }

        [DataMember(Name = "valueNotReadOnly")]
        [ReadOnly(false)]
        public string ValueNotReadOnly { get; set; }

        [DataMember(Name = "valueReadOnly")]
        [ReadOnly(true)]
        public string ValueReadOnly { get; set; }
    }
}