// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Drawing;
#if NETFRAMEWORK
using System.Net.Cache;
#endif
using System.Threading.Tasks;
using Dapplo.HttpExtensions.WinForms.ContentConverter;
using Dapplo.Log;
using Dapplo.Log.XUnit;
using Xunit;
using Xunit.Abstractions;

namespace Dapplo.HttpExtensions.Tests;

/// <summary>
///     Some tests which use http://httpbin.org/
/// </summary>
public class HttpBehaviourExtensionsTests
{
    private readonly Uri _bitmapUri = new("http://httpbin.org/image/png");

    public HttpBehaviourExtensionsTests(ITestOutputHelper testOutputHelper)
    {
        LogSettings.RegisterDefaultLogger<XUnitLogger>(LogLevels.Verbose, testOutputHelper);
        BitmapHttpContentConverter.RegisterGlobally();
#if NETFRAMEWORK
            HttpExtensionsGlobals.HttpSettings.RequestCacheLevel = RequestCacheLevel.NoCacheNoStore;
#endif
    }

    /// <summary>
    ///     Test POST
    /// </summary>
    [Fact]
    public async Task TestHttpBehaviourChaining()
    {
        bool testChainOnHttpContentCreated1 = false;
        bool testChainOnHttpContentCreated2 = false;

        bool testChainOnHttpRequestMessageCreated1 = false;
        bool testChainOnHttpRequestMessageCreated2 = false;

        bool testChainOnHttpMessageHandlerCreated1 = false;
        bool testChainOnHttpMessageHandlerCreated2 = false;

        var testBehaviour = HttpBehaviour.Current.ShallowClone();

        testBehaviour.ChainOnHttpContentCreated(x =>
        {
            testChainOnHttpContentCreated1 = true;
            return x;
        });

        // Test if the chaining chains
        testBehaviour.ChainOnHttpContentCreated(x =>
        {
            // The previous Func should be called before this
            Assert.True(testChainOnHttpContentCreated1);
            testChainOnHttpContentCreated2 = true;
            return x;
        });

        testBehaviour.ChainOnHttpRequestMessageCreated(x =>
        {
            testChainOnHttpRequestMessageCreated1 = true;
            return x;
        });

        // Test if the chaining chains
        testBehaviour.ChainOnHttpRequestMessageCreated(x =>
        {
            // The previous Func should be called before this
            Assert.True(testChainOnHttpRequestMessageCreated1);
            testChainOnHttpRequestMessageCreated2 = true;
            return x;
        });

        testBehaviour.ChainOnHttpMessageHandlerCreated(x =>
        {
            testChainOnHttpMessageHandlerCreated1 = true;
            return x;
        });

        // Test if the chaining chains
        testBehaviour.ChainOnHttpMessageHandlerCreated(x =>
        {
            // The previous Func should be called before this
            Assert.True(testChainOnHttpMessageHandlerCreated1);
            testChainOnHttpMessageHandlerCreated2 = true;
            return x;
        });

        testBehaviour.MakeCurrent();

        using (var bitmap = await _bitmapUri.GetAsAsync<Bitmap>())
        {
            await new Uri("https://httpbin.org/post").PostAsync(bitmap);
        }
        Assert.True(testChainOnHttpContentCreated1);
        Assert.True(testChainOnHttpContentCreated2);
        Assert.True(testChainOnHttpRequestMessageCreated1);
        Assert.True(testChainOnHttpRequestMessageCreated2);
        Assert.True(testChainOnHttpMessageHandlerCreated1);
        Assert.True(testChainOnHttpMessageHandlerCreated2);
    }
}