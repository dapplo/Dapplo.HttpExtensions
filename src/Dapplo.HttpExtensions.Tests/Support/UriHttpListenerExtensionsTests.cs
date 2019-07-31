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

using System.Threading.Tasks;
using Dapplo.HttpExtensions.Listener;
using Dapplo.Log;
using Dapplo.Log.XUnit;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace Dapplo.HttpExtensions.Tests.Support
{
    public class UriHttpListenerExtensionsTests
    {
        public UriHttpListenerExtensionsTests(ITestOutputHelper testOutputHelper)
        {
            LogSettings.RegisterDefaultLogger<XUnitLogger>(LogLevels.Verbose, testOutputHelper);
        }

        [Fact]
        public async Task TestListenAsync()
        {
            // Try listening on 8080, if this doesn't work take the first free port
            var listenUri = new[] {8080, 0}.CreateLocalHostUri().AppendSegments("AsyncHttpListenerTests");
            var listenTask = listenUri.ListenAsync(async httpListenerContext =>
            {
                // Process the request
                var httpListenerRequest = httpListenerContext.Request;
                var result = httpListenerRequest.Url.QueryToDictionary();
                await httpListenerContext.RespondAsync("OK");
                return result;
            });
            // Do we need a delay for the listener to be ready?
            //await Task.Delay(100);
            var testUri = listenUri.ExtendQuery("name", "dapplo");
            var okResponse = await testUri.GetAsAsync<string>();
            Assert.Equal("OK", okResponse);
            var actionResult = await listenTask;
            Assert.True(actionResult.ContainsKey("name"));
            Assert.True(actionResult["name"] == "dapplo");
        }
    }
}