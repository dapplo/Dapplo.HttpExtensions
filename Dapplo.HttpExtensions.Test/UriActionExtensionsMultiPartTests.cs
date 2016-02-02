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
using System.Threading.Tasks;
using System.IO;
using Dapplo.LogFacade;
using Dapplo.LogFacade.Loggers;

namespace Dapplo.HttpExtensions.Test
{
	/// <summary>
	/// Should write some tests which use http://httpbin.org/
	/// </summary>
	[TestClass]
	public class UriActionExtensionsMultiPartTests
	{
		private readonly Uri _bitmapUri = new Uri("http://beta.getgreenshot.org/assets/greenshot-logo.png");

		[TestInitialize]
		public void Init()
		{
			// Make sure the logger is set for debugging
			LogSettings.Logger = new TraceLogger();
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

	}
}
