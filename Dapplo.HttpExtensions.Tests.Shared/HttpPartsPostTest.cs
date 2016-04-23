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
using System.Drawing;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.Tests.Logger;
using Dapplo.HttpExtensions.Tests.TestEntities;
using Dapplo.LogFacade;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace Dapplo.HttpExtensions.Tests
{
	/// <summary>
	/// Test posting parts
	/// </summary>
	public class HttpPartsPostTest
	{
		private static readonly LogSource Log = new LogSource();
		private static readonly Uri RequestBinUri = new Uri("http://httpbin.org");

		public HttpPartsPostTest(ITestOutputHelper testOutputHelper)
		{
			XUnitLogger.RegisterLogger(testOutputHelper, LogLevel.Verbose);
		}

		/// <summary>
		/// Test posting
		/// </summary>
		[Fact]
		public async Task TestPost()
		{
			var testUri = RequestBinUri.AppendSegments("post");
			var uploadBehaviour = HttpBehaviour.Current.Clone();

			uploadBehaviour.UseProgressStream = true;
			uploadBehaviour.UploadProgress += progress => {
				Log.Info().WriteLine("Progress {0}", (int)(progress * 100));
			};
			uploadBehaviour.MakeCurrent();
			var testObject = new MyMultiPartRequest
			{
				BitmapContentName = "MyBitmapContent",
				BitmapFileName = "MyBitmapFilename",
				OurBitmap = new Bitmap(10, 10),
				JsonInformation = new GitHubError {DocumentationUrl = "http://test.de", Message = "Hello"}
			};
			testObject.Headers.Add("Name", "Dapplo");
			var result = await testUri.PostAsync<dynamic>(testObject);
			Assert.NotNull(result);
		}
	}
}