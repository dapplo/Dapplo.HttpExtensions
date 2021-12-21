// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Drawing.Imaging;
#if NETFRAMEWORK
using System.Net.Cache;
#endif
using Dapplo.HttpExtensions.Extensions;
using Dapplo.HttpExtensions.WinForms.ContentConverter;
using Dapplo.Log;
using Dapplo.Log.XUnit;
using Xunit;
using Xunit.Abstractions;

namespace Dapplo.HttpExtensions.Tests;

public class HttpRequestConfigurationTests
{
    public HttpRequestConfigurationTests(ITestOutputHelper testOutputHelper)
    {
        LogSettings.RegisterDefaultLogger<XUnitLogger>(LogLevels.Verbose, testOutputHelper);
#if NETFRAMEWORK
            HttpExtensionsGlobals.HttpSettings.RequestCacheLevel = RequestCacheLevel.NoCacheNoStore;
#endif
    }

    /// <summary>
    ///     Test posting, using Bitmap
    /// </summary>
    [Fact]
    public void Test_GetSet()
    {
        var httpBehaviour = HttpBehaviour.Current;
        var testConfig = new BitmapConfiguration {Format = ImageFormat.Gif};
        Assert.Equal(ImageFormat.Gif, testConfig.Format);
        httpBehaviour.SetConfig(testConfig);
        Assert.Equal(ImageFormat.Gif, testConfig.Format);
        var retrievedConfig = httpBehaviour.GetConfig<BitmapConfiguration>();
        Assert.Equal(ImageFormat.Gif, retrievedConfig.Format);
    }
}