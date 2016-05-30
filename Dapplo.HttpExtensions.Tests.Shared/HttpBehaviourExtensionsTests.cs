//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016 Dapplo
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
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Dapplo.HttpExtensions.Support;
using Dapplo.HttpExtensions.Tests.Logger;
using Dapplo.LogFacade;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace Dapplo.HttpExtensions.Tests
{
	/// <summary>
	///     Some tests which use http://httpbin.org/
	/// </summary>
	public class HttpBehaviourExtensionsTests
	{
		private static readonly LogSource Log = new LogSource();
		private readonly Uri _bitmapUri = new Uri("http://httpbin.org/image/png");

		public HttpBehaviourExtensionsTests(ITestOutputHelper testOutputHelper)
		{
			XUnitLogger.RegisterLogger(testOutputHelper, LogLevel.Verbose);
		}

		/// <summary>
		///     Test POST
		/// </summary>
		[Fact]
		public async Task TestHttpBehaviourChaining()
		{
			bool testChainOnHttpContentCreated1 = false;
			bool testChainOnHttpContentCreated2 = false;

			bool testChainOnHttpRequestMessageCreated1 = false;
			bool testChainOnHttpRequestMessageCreated2 = false;

			bool testChainOnHttpMessageHandlerCreated1 = false;
			bool testChainOnHttpMessageHandlerCreated2 = false;

			var testBehaviour = HttpBehaviour.Current.Clone();

			testBehaviour.ChainOnHttpContentCreated(x => {
				testChainOnHttpContentCreated1 = true;
				return x;
			});

			// Test if the chaining chains
			testBehaviour.ChainOnHttpContentCreated(x => {
				// The previous Func should be called before this
				Assert.True(testChainOnHttpContentCreated1);
				testChainOnHttpContentCreated2 = true;
				return x;
			});

			testBehaviour.ChainOnHttpRequestMessageCreated(x => {
				testChainOnHttpRequestMessageCreated1 = true;
				return x;
			});

			// Test if the chaining chains
			testBehaviour.ChainOnHttpRequestMessageCreated(x => {
				// The previous Func should be called before this
				Assert.True(testChainOnHttpRequestMessageCreated1);
				testChainOnHttpRequestMessageCreated2 = true;
				return x;
			});

			testBehaviour.ChainOnHttpMessageHandlerCreated(x => {
				testChainOnHttpMessageHandlerCreated1 = true;
				return x;
			});

			// Test if the chaining chains
			testBehaviour.ChainOnHttpMessageHandlerCreated(x => {
				// The previous Func should be called before this
				Assert.True(testChainOnHttpMessageHandlerCreated1);
				testChainOnHttpMessageHandlerCreated2 = true;
				return x;
			});

			testBehaviour.MakeCurrent();

			using (var bitmap = await _bitmapUri.GetAsAsync<Bitmap>())
			{
				await new Uri("https://httpbin.org/post").PostAsync(bitmap);
			}
			Assert.True(testChainOnHttpContentCreated1);
			Assert.True(testChainOnHttpContentCreated2);
			Assert.True(testChainOnHttpRequestMessageCreated1);
			Assert.True(testChainOnHttpRequestMessageCreated2);
			Assert.True(testChainOnHttpMessageHandlerCreated1);
			Assert.True(testChainOnHttpMessageHandlerCreated2);
		}
	}
}