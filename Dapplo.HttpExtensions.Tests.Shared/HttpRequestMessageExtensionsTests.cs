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
using System.Threading.Tasks;
using Dapplo.HttpExtensions.Factory;
using Dapplo.Log.XUnit;
using Dapplo.Log;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace Dapplo.HttpExtensions.Tests
{
	/// <summary>
	///     Testing HttpRequestMessageExtensions
	/// </summary>
	public class HttpRequestMessageExtensionsTests
	{
		public HttpRequestMessageExtensionsTests(ITestOutputHelper testOutputHelper)
		{
			LogSettings.RegisterDefaultLogger<XUnitLogger>(LogLevels.Verbose, testOutputHelper);
		}

		/// <summary>
		///     Test getting the uri as Bitmap
		/// </summary>
		[Fact]
		public async Task TestSendAsync()
		{
			var testUri = new Uri("http://httpbin.org/xml");
			var httpRequestMessage = HttpRequestMessageFactory.CreateGet<string>(testUri);
			var result = await httpRequestMessage.SendAsync<string>();
			Assert.NotNull(result);
		}
	}
}