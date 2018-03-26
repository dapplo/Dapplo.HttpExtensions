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
using System.Runtime.Serialization;

#endregion

namespace Dapplo.HttpExtensions.Tests.TestEntities
{
    /// <summary>
    ///     Container for the release information from GitHub
    /// </summary>
    [DataContract]
    public class GitHubRelease
    {
        [DataMember(Name = "html_url", EmitDefaultValue = false)]
        public string HtmlUrl { get; set; }

        [DataMember(Name = "prerelease", EmitDefaultValue = false)]
        public bool Prerelease { get; set; }

        [DataMember(Name = "published_at")]
        public DateTime PublishedAt { get; set; }

        [DataMember(Name = "releaseType")]
        public ReleaseTypes ReleaseType { get; set; }
    }
}