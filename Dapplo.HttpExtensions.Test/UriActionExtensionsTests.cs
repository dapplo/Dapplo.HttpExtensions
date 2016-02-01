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
	along with Dapplo.HttpExtensions. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Dapplo.HttpExtensions.Test.TestEntities;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using Dapplo.HttpExtensions.Support;
using System.ServiceModel.Syndication;
using System.Xml.Linq;

namespace Dapplo.HttpExtensions.Test
{
	[TestClass]
	public class UriActionExtensionsTests
	{
		private readonly Uri _bitmapUri = new Uri("http://beta.getgreenshot.org/assets/greenshot-logo.png");

		[TestInitialize]
		public void Init()
		{
			// Make sure the logger is set for debugging
			HttpExtensionsGlobals.Logger = new TraceLogger();
		}

		/// <summary>
		/// Test getting the uri as Bitmap
		/// </summary>
		/// <returns></returns>
		[TestMethod]
		public async Task TestGetAsAsyncBitmap()
		{
			var bitmap = await _bitmapUri.GetAsAsync<Bitmap>();
			Assert.IsNotNull(bitmap);
			Assert.IsTrue(bitmap.Width > 0);
			Assert.IsTrue(bitmap.Height > 0);
		}

		/// <summary>
		/// Test getting the Uri as BitmapSource
		/// </summary>
		/// <returns></returns>
		[TestMethod]
		public async Task TestGetAsAsyncBitmapSource()
		{
			var bitmap = await _bitmapUri.GetAsAsync<BitmapSource>();
			Assert.IsNotNull(bitmap);
			Assert.IsTrue(bitmap.Width > 0);
			Assert.IsTrue(bitmap.Height > 0);
		}

		/// <summary>
		/// Test getting the uri as Feed
		/// </summary>
		/// <returns></returns>
		[TestMethod]
		public async Task TestGetAsAsyncSyndicationFeed()
		{
			var feed = await new Uri("http://getgreenshot.org/project-feed/").GetAsAsync<SyndicationFeed>();
			Assert.IsNotNull(feed);
			Assert.IsTrue(feed.Items.Count() > 0);
		}

		/// <summary>
		/// Test getting the uri as Feed
		/// </summary>
		/// <returns></returns>
		[TestMethod]
		public async Task TestGetAsAsyncXDocument()
		{
			var xDocument = await new Uri("http://getgreenshot.org/project-feed/").GetAsAsync<XDocument>();
			Assert.IsNotNull(xDocument);
			Assert.IsTrue(xDocument.Nodes().Count() > 0);
		}

		/// <summary>
		/// Test getting the Uri as MemoryStream
		/// </summary>
		/// <returns></returns>
		[TestMethod]
		public async Task TestGetAsAsyncMemoryStream()
		{
			var stream = await _bitmapUri.GetAsAsync<MemoryStream>();
			Assert.IsNotNull(stream);
			Assert.IsTrue(stream.Length > 0);
		}

		/// <summary>
		/// To make sure we test some of the functionality, we call the GitHub API to get the releases for this project.
		/// </summary>
		/// <returns></returns>
		[TestMethod]
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

			var releases = await releasesUri.GetAsAsync<List<GitHubRelease>, GitHubError>();
			Assert.IsNotNull(releases);
			Assert.IsFalse(releases.HasError, $"{releases.StatusCode}: {releases.ErrorResponse?.Message} {releases.ErrorResponse?.DocumentationUrl}");

			var latestRelease = releases.Response
					.Where(x => !x.Prerelease)
					.OrderByDescending(x => x.PublishedAt)
					.FirstOrDefault();
			Assert.IsNotNull(latestRelease);
		}
	}
}
