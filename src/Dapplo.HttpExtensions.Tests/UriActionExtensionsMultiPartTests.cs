//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2019 Dapplo
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

#region Usings

using System;
using System.IO;
using System.Net.Cache;
using System.Threading.Tasks;
using Dapplo.Log;
using Dapplo.Log.XUnit;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace Dapplo.HttpExtensions.Tests
{
    /// <summary>
    ///     Should write some tests which use http://httpbin.org/
    /// </summary>
    public class UriActionExtensionsMultiPartTests
    {
        private readonly Uri _bitmapUri = new Uri("http://getgreenshot.org/assets/greenshot-logo.png");

        public UriActionExtensionsMultiPartTests(ITestOutputHelper testOutputHelper)
        {
            LogSettings.RegisterDefaultLogger<XUnitLogger>(LogLevels.Verbose, testOutputHelper);
            HttpExtensionsGlobals.HttpSettings.RequestCacheLevel = RequestCacheLevel.NoCacheNoStore;
        }

        /// <summary>
        ///     Test getting the Uri as MemoryStream
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TestGetAsAsyncMemoryStream()
        {
            var stream = await _bitmapUri.GetAsAsync<MemoryStream>();
            Assert.NotNull(stream);
            Assert.True(stream.Length > 0);
        }
    }
}