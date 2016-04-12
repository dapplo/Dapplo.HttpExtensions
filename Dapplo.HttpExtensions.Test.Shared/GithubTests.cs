//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2015-2016 Dapplo
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

#region using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.Tests.TestEntities;
using Dapplo.LogFacade;
using Xunit;
using Xunit.Abstractions;
using Dapplo.HttpExtensions.Tests.Logger;

#endregion

namespace Dapplo.HttpExtensions.Tests
{
	/// <summary>
	///     Summary description for GithubTests
	/// </summary>
	public class GithubTests
	{
		public GithubTests(ITestOutputHelper testOutputHelper)
		{
			XUnitLogger.RegisterLogger(testOutputHelper, LogLevel.Verbose);
		}

		/// <summary>
		///     To make sure we test some of the functionality, we call the GitHub API to get the releases for this project.
		/// </summary>
		/// <returns></returns>
		[Fact]
		public async Task TestGetAsJsonAsync_GitHubApiReleases()
		{
			var githubApiUri = new Uri("https://api.github.com");
			var releasesUri = githubApiUri.AppendSegments("repos", "dapplo", "Dapplo.HttpExtensions", "releases");

			// This is needed when running in AppVeyor, as AppVeyor has multiple request to GitHub, the rate-limit is exceeded.
			var username = Environment.GetEnvironmentVariable("github_username");
			var password = Environment.GetEnvironmentVariable("github_token");
			if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
			{
				releasesUri = releasesUri.SetCredentials(username, password);
			}

			var releases = await releasesUri.GetAsAsync<HttpResponse<List<GitHubRelease>, GitHubError>>();
			Assert.NotNull(releases);
			Assert.False(releases.HasError, $"{releases.StatusCode}: {releases.ErrorResponse?.Message} {releases.ErrorResponse?.DocumentationUrl}");

			var latestRelease = releases.Response
				.Where(x => !x.Prerelease)
				.OrderByDescending(x => x.PublishedAt)
				.FirstOrDefault();
			Assert.NotNull(latestRelease);
		}
	}
}