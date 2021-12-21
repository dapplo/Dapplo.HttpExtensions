// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
#if NETFRAMEWORK
using System.Net.Cache;
#endif
using System.Threading.Tasks;
using Dapplo.HttpExtensions.Factory;
using Dapplo.Log;
using Dapplo.Log.XUnit;
using Xunit;
using Xunit.Abstractions;

namespace Dapplo.HttpExtensions.Tests;

/// <summary>
///     Testing HttpRequestMessageExtensions
/// </summary>
public class HttpRequestMessageExtensionsTests
{
    public HttpRequestMessageExtensionsTests(ITestOutputHelper testOutputHelper)
    {
        LogSettings.RegisterDefaultLogger<XUnitLogger>(LogLevels.Verbose, testOutputHelper);
#if NETFRAMEWORK
            HttpExtensionsGlobals.HttpSettings.RequestCacheLevel = RequestCacheLevel.NoCacheNoStore;
#endif
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