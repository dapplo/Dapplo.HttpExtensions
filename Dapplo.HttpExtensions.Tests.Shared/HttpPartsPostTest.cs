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
using Dapplo.HttpExtensions.Tests.TestEntities;

#endregion

namespace Dapplo.HttpExtensions.Tests
{
	/// <summary>
	///     Should write some tests which use http://requestb.in/
	/// </summary>
	public class HttpPartsPostTest
	{
		private static readonly Uri RequestBinUri = new Uri("http://requestb.in");

		/// <summary>
		///     This method sends something to requestb.in, which doesn't make sense to do in a unit test.
		///     Therefor the [Fact] is commented out.
		/// </summary>
		//[Fact]
		public async Task TestPost()
		{
			const string binId = "1ajltg01";
			var testUri = RequestBinUri.AppendSegments(binId);

			var testObject = new MyMultiPartRequest
			{
				BitmapContentName = "MyBitmapContent",
				BitmapFileName = "MyBitmapFilename",
				OurBitmap = new Bitmap(10, 10),
				JsonInformation = new GitHubError {DocumentationUrl = "http://test.de", Message = "Hello"}
			};
			testObject.Headers.Add("Name", "Dapplo");
			await testUri.PostAsync(testObject);
		}
	}
}