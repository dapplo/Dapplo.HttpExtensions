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
using System.Net.Http;
using Dapplo.HttpExtensions.Support;

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

		/// <summary>
		///     Test DELETE
		/// </summary>
		[Fact]
		public async Task TestDelete()
		{
			var result = await new Uri("https://httpbin.org/delete").DeleteAsync<dynamic>();
			Assert.NotNull(result);
		}

		/// <summary>
		///     Test PUT
		/// </summary>
		[Fact]
		public async Task TestPut()
		{
			var result = await new Uri("https://httpbin.org/put").PutAsync<dynamic>(null);
			Assert.NotNull(result);
		}

		/// <summary>
		///     Test POST
		/// </summary>
		[Fact]
		public async Task TestPost()
		{
			await new Uri("https://httpbin.org/put").PostAsync(null);
		}

		/// <summary>
		///     Test HEAD
		/// </summary>
		[Fact]
		public async Task TestHead()
		{
			var result = await new Uri("https://httpbin.org").HeadAsync();
			Assert.Contains("text/html", result.ContentType.MediaType);
		}

		/// <summary>
		///     Test LastModified
		/// </summary>
		[Fact]
		public async Task TestLastModified()
		{
			await new Uri("http://nu.nl").LastModifiedAsync();
		}

		/// <summary>
		///     Test HandleErrorAsync
		/// </summary>
		[Fact]
		public async Task TestHandleErrorAsync()
		{
			await Assert.ThrowsAsync<HttpRequestException>(async () =>  await new Uri("https://httpbin.orgf").HeadAsync());
		}

		/// <summary>
		///     Test basic authentication
		/// </summary>
		[Fact]
		public async Task TestBasicAuthAsync()
		{
			await new Uri("https://usern:passw@httpbin.org/basic-auth/usern/passw").GetAsAsync<string>();
		}

		/// <summary>
		///     Test redirecting
		/// </summary>
		[Fact]
		public async Task TestRedirectAndFollow()
		{
			var result = await new Uri("https://httpbin.org/redirect/5").GetAsAsync<string>();
			Assert.NotNull(result);
		}

		/// <summary>
		///     Test NOT redirecting, also testing the MakeCurrent of the behavior
		/// </summary>
		[Fact]
		public async Task TestRedirectAndNotFollow()
		{
			var behavior = new HttpBehaviour
			{
				HttpSettings = new HttpSettings()
			};
			behavior.HttpSettings.AllowAutoRedirect = false;
			behavior.MakeCurrent();
			await Assert.ThrowsAsync<HttpRequestException>(async () => await new Uri("https://httpbin.org/redirect/5").GetAsAsync<string>());
		}
	}
}