// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.Serialization;

namespace Dapplo.HttpExtensions.Tests.TestEntities;

/// <summary>
///     Container for errors from GitHub
/// </summary>
[DataContract]
public class GitHubError
{
    [DataMember(Name = "documentation_url")]
    public string DocumentationUrl { get; set; }

    [DataMember(Name = "message")]
    public string Message { get; set; }
}