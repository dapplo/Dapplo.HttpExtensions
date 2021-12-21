// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dapplo.HttpExtensions.Listener;
using Dapplo.Log;
using Dapplo.Log.XUnit;
using Xunit;
using Xunit.Abstractions;

namespace Dapplo.HttpExtensions.Tests.Support;

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