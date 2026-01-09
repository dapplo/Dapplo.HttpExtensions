// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
#if NETFRAMEWORK
using System.Net.Cache;
#endif
using System.Threading.Tasks;
using Dapplo.Log;
using Dapplo.Log.XUnit;
using Xunit;

namespace Dapplo.HttpExtensions.Tests;

/// <summary>
///     Should write some tests which use http://httpbin.org/
/// </summary>
public class UriActionExtensionsMultiPartTests
{
    private readonly Uri _bitmapUri = new Uri("http://getgreenshot.org/assets/greenshot-logo.png");

    public UriActionExtensionsMultiPartTests(ITestOutputHelper testOutputHelper)
    {
        LogSettings.RegisterDefaultLogger<XUnitLogger>(LogLevels.Verbose, testOutputHelper);
#if NETFRAMEWORK
            HttpExtensionsGlobals.HttpSettings.RequestCacheLevel = RequestCacheLevel.NoCacheNoStore;
#endif
    }

    /// <summary>
    ///     Test getting the Uri as MemoryStream
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task TestGetAsAsyncMemoryStream()
    {
        var stream = await _bitmapUri.GetAsAsync<MemoryStream>(TestContext.Current.CancellationToken);
        Assert.NotNull(stream);
        Assert.True(stream.Length > 0);
    }
}