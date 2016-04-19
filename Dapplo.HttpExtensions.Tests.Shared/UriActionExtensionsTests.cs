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
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Dapplo.LogFacade;
using Xunit;
using Xunit.Abstractions;
using Dapplo.HttpExtensions.Tests.Logger;

#endregion

namespace Dapplo.HttpExtensions.Tests
{
	/// <summary>
	///     Some tests which use http://httpbin.org/
	/// </summary>
	public class UriActionExtensionsTests
	{
		private readonly Uri _bitmapUri = new Uri("http://httpbin.org/image/png");

		public UriActionExtensionsTests(ITestOutputHelper testOutputHelper)
		{
			XUnitLogger.RegisterLogger(testOutputHelper, LogLevel.Verbose);
		}

		/// <summary>
		///     Test getting the uri as Bitmap
		/// </summary>
		[Fact]
		public async Task TestGetAsAsyncBitmap()
		{
			var bitmap = await _bitmapUri.GetAsAsync<Bitmap>();
			Assert.NotNull(bitmap);
			Assert.True(bitmap.Width > 0);
			Assert.True(bitmap.Height > 0);
		}

		/// <summary>
		///     Test getting the Uri as BitmapSource
		/// </summary>
		[Fact]
		public async Task TestGetAsAsyncBitmapSource()
		{
			var bitmap = await _bitmapUri.GetAsAsync<BitmapSource>();
			Assert.NotNull(bitmap);
			Assert.True(bitmap.Width > 0);
			Assert.True(bitmap.Height > 0);
		}

		/// <summary>
		///     Test getting the Uri as MemoryStream
		/// </summary>
		public async Task TestGetAsAsyncMemoryStream()
		{
			var stream = await _bitmapUri.GetAsAsync<MemoryStream>();
			Assert.NotNull(stream);
			Assert.True(stream.Length > 0);
		}

		/// <summary>
		///     Test getting the uri as Feed
		/// </summary>
		[Fact]
		public async Task TestGetAsAsyncSyndicationFeed()
		{
			var feed = await new Uri("http://lorem-rss.herokuapp.com/feed").GetAsAsync<SyndicationFeed>();
			Assert.NotNull(feed);
			Assert.True(feed.Items.Any());
		}

		/// <summary>
		///     Test getting the uri as an XML
		/// </summary>
		[Fact]
		public async Task TestGetAsAsyncXDocument()
		{
			var xDocument = await new Uri("http://httpbin.org/xml").GetAsAsync<XDocument>();
			Assert.NotNull(xDocument);
			Assert.True(xDocument.Nodes().Any());
		}

		/// <summary>
		///     Test retrieval when a 204 (no content) is returned
		/// </summary>
		[Fact]
		public async Task TestGetAsAsync204()
		{
			var result = await new Uri("https://httpbin.org/status/204").GetAsAsync<string>();
			Assert.Null(result);
		}

		/// <summary>
		///     Test user-agent
		/// </summary>
		[Fact]
		public async Task TestUserAgent()
		{
			var result = await new Uri("https://httpbin.org/user-agent").GetAsAsync<IDictionary<string, string>>();
			Assert.NotNull(result);
			Assert.True(result.ContainsKey("user-agent"));
			Assert.Equal(HttpExtensionsGlobals.HttpSettings.DefaultUserAgent, result["user-agent"]);
		}
	}
}