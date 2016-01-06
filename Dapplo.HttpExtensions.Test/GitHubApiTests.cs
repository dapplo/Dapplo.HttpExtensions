/*
	Dapplo - building blocks for desktop applications
	Copyright (C) 2015-2016 Dapplo

	For more information see: http://dapplo.net/
	Dapplo repositories are hosted on GitHub: https://github.com/dapplo

	This file is part of Dapplo.HttpExtensions.

	Dapplo.HttpExtensions is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	Dapplo.HttpExtensions is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Dapplo.HttpExtensions.Test
{
	/// <summary>
	/// Container for the release information from GitHub
	/// </summary>
	[DataContract]
	public class GitHubRelease
	{
		[DataMember(Name = "prerelease")]
		public bool Prerelease { get; set; }

		[DataMember(Name = "published_at")]
		public DateTime PublishedAt { get; set; }

		[DataMember(Name = "html_url")]
		public string HtmlUrl { get; set; }
	}

	/// <summary>
	/// Container for errors from GitHub
	/// </summary>
	[DataContract]
	public class GitHubError
	{
		[DataMember(Name = "message")]
		public string Message { get; set; }

		[DataMember(Name = "documentation_url")]
		public string DocumentationUrl { get; set; }
	}

	[TestClass]
	public class GitHubApiTests
	{
		/// <summary>
		/// To make sure we test some of the functionality, we call the GitHub API to get the releases for this project.
		/// </summary>
		/// <returns></returns>
		[TestMethod]
		public async Task TestGitHubApiReleases()
		{
			var githubApiUri = new Uri("https://api.github.com");
			var releasesUri = githubApiUri.AppendSegments("repos", "dapplo", "Dapplo.HttpExtensions", "releases");
			var releases = await releasesUri.GetAsJsonAsync<List<GitHubRelease>, GitHubError>();
			Assert.IsFalse(releases.HasError, releases.ErrorResponse.Message);

			var latestRelease = releases.Response
					.Where(x => !x.Prerelease)
					.OrderByDescending(x => x.PublishedAt)
					.FirstOrDefault();
			Assert.IsNotNull(latestRelease);
		}
	}
}
