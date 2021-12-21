// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Runtime.Serialization;
using Dapplo.HttpExtensions.JsonSimple;

namespace Dapplo.HttpExtensions.Tests.TestEntities;

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