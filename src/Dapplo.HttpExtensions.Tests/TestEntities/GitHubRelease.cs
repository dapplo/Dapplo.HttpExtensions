// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Dapplo.HttpExtensions.Tests.TestEntities
{
    /// <summary>
    ///     Container for the release information from GitHub
    /// </summary>
    [DataContract]
    public class GitHubRelease
    {
        [DataMember(Name = "html_url", EmitDefaultValue = false)]
        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; }

        [DataMember(Name = "prerelease", EmitDefaultValue = false)]
        [JsonPropertyName("prerelease")]
        public bool Prerelease { get; set; }

        [DataMember(Name = "published_at")]
        [JsonPropertyName("published_at")]
        public DateTime PublishedAt { get; set; }

        [DataMember(Name = "releaseType")]
        [JsonPropertyName("releaseType")]
        public ReleaseTypes ReleaseType { get; set; }
    }
}