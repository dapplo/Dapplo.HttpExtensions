// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Dapplo.HttpExtensions.Tests.TestEntities;

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