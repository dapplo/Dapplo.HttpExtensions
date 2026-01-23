// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
#if NETFRAMEWORK
using System.Net.Cache;
#endif
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Dapplo.HttpExtensions.JsonSimple;
using Dapplo.HttpExtensions.Support;
using Dapplo.HttpExtensions.SystemTextJson;
using Dapplo.HttpExtensions.WinForms.ContentConverter;
using Dapplo.HttpExtensions.Wpf.ContentConverter;
using Dapplo.Log;
using Dapplo.Log.XUnit;
using Xunit;

namespace Dapplo.HttpExtensions.Tests;

/// <summary>
///     Some tests which use http://httpbin.org/
/// </summary>
public class UriActionExtensionsTests
{
#pragma warning disable IDE0090 // Use 'new(...)'
    private static readonly LogSource Log = new LogSource();
#pragma warning restore IDE0090 // Use 'new(...)'
    private readonly Uri _httpBinUri = new("http://httpbin.org");

    public UriActionExtensionsTests(ITestOutputHelper testOutputHelper)
    {
        BitmapHttpContentConverter.RegisterGlobally();
        BitmapSourceHttpContentConverter.RegisterGlobally();
        var behaviour = HttpBehaviour.Current as IChangeableHttpBehaviour;
        Assert.NotNull(behaviour);
        behaviour.JsonSerializer = new SystemTextJsonSerializer();
        LogSettings.RegisterDefaultLogger<XUnitLogger>(LogLevels.Verbose, testOutputHelper);
#if NETFRAMEWORK
            HttpExtensionsGlobals.HttpSettings.RequestCacheLevel = RequestCacheLevel.NoCacheNoStore;
#endif
    }

    /// <summary>
    ///     Test basic authentication
    /// </summary>
    [Fact]
    public Task TestBasicAuthAsync()
    {
        const string password = @"pass\w";
        const string username = "usern";
        var authUri = _httpBinUri.AppendSegments("basic-auth", username, password).SetCredentials(username, password);
        return authUri.GetAsAsync<string>(TestContext.Current.CancellationToken);
    }

    /// <summary>
    ///     Test DELETE
    /// </summary>
    [Fact]
    public async Task TestDeleteAsync()
    {
        var result = await new Uri("https://httpbin.org/delete").DeleteAsync<string>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
    }

    /// <summary>
    ///     Test retrieval when a 204 (no content) is returned
    /// </summary>
    [Fact]
    public async Task TestGetAsAsync204()
    {
        var result = await new Uri("https://httpbin.org/status/204").GetAsAsync<string>(TestContext.Current.CancellationToken);
        Assert.Null(result);
    }

    /// <summary>
    ///     Test getting the uri as Bitmap
    /// </summary>
    [Fact]
    public async Task TestGetAsAsyncBitmap()
    {
        var downloadBehaviour = HttpBehaviour.Current.ShallowClone();

        var bitmapUri = _httpBinUri.AppendSegments("image", "png");
        downloadBehaviour.UseProgressStream = true;
        downloadBehaviour.DownloadProgress += progress => { Log.Info().WriteLine("Progress {0}", (int) (progress * 100)); };
        downloadBehaviour.MakeCurrent();

        var bitmap = await bitmapUri.GetAsAsync<Bitmap>(TestContext.Current.CancellationToken);
        Assert.NotNull(bitmap);
        Assert.True(bitmap.Width > 0);
        Assert.True(bitmap.Height > 0);
    }

    /// <summary>
    ///     Test getting the Uri as BitmapSource
    /// </summary>
    [Fact]
    public async Task TestGetAsAsyncBitmapSource()
    {
        var uploadBehaviour = HttpBehaviour.Current.ShallowClone();
        var bitmapUri = _httpBinUri.AppendSegments("image", "png");
        bool hasProgress = false;

        uploadBehaviour.UseProgressStream = true;
        uploadBehaviour.DownloadProgress += progress =>
        {
            Log.Info().WriteLine("Progress {0}", (int) (progress * 100));
            hasProgress = true;
        };
        uploadBehaviour.MakeCurrent();

        var bitmap = await bitmapUri.GetAsAsync<BitmapSource>(TestContext.Current.CancellationToken);
        Assert.NotNull(bitmap);
        Assert.True(bitmap.Width > 0);
        Assert.True(bitmap.Height > 0);
        Assert.True(hasProgress);
    }

    /// <summary>
    ///     Test getting the Uri as MemoryStream
    /// </summary>
    [Fact]
    public async Task TestGetAsAsyncMemoryStream()
    {
        var bitmapUri = _httpBinUri.AppendSegments("image", "png");
        var stream = await bitmapUri.GetAsAsync<MemoryStream>(TestContext.Current.CancellationToken);
        Assert.NotNull(stream);
        Assert.True(stream.Length > 0);
    }

    /// <summary>
    ///     Test getting the uri as Feed
    /// </summary>
    [Fact]
    public async Task TestGetAsAsyncSyndicationFeed()
    {
        var feed = await new Uri("https://blogs.msdn.microsoft.com/dotnet/feed/").GetAsAsync<SyndicationFeed>(TestContext.Current.CancellationToken);
        Assert.NotNull(feed);
        Assert.True(feed.Items.Any());
    }

    /// <summary>
    ///     Test getting the uri as an XML
    /// </summary>
    [Fact]
    public async Task TestGetAsAsyncXDocument()
    {
        var xDocument = await new Uri("http://httpbin.org/xml").GetAsAsync<XDocument>(TestContext.Current.CancellationToken);
        Assert.NotNull(xDocument);
        Assert.True(xDocument.Nodes().Any());
    }

    /// <summary>
    ///     Test HandleErrorAsync
    /// </summary>
    [Fact]
    public async Task TestHandleErrorAsync()
    {
        await Assert.ThrowsAsync<HttpRequestException>(async () => await new Uri("https://httpbin.orgf").HeadAsync(TestContext.Current.CancellationToken));
    }

    /// <summary>
    ///     Test HEAD
    /// </summary>
    [Fact]
    public async Task TestHead()
    {
        var result = await new Uri("https://httpbin.org").HeadAsync(TestContext.Current.CancellationToken);
        Assert.Contains("text/html", result.ContentType.MediaType);
    }

    /// <summary>
    ///     Test LastModified
    /// </summary>
    [Fact]
    public Task TestLastModified()
    {
        return new Uri("http://nu.nl").LastModifiedAsync(TestContext.Current.CancellationToken);
    }

    /// <summary>
    ///     Test POST
    /// </summary>
    [Fact]
    public Task TestPost()
    {
        return new Uri("https://httpbin.org/post").PostAsync(null, TestContext.Current.CancellationToken);
    }

    /// <summary>
    ///     Test PUT without response
    /// </summary>
    [Fact]
    public Task TestPut()
    {
        return new Uri("https://httpbin.org/put").PutAsync(null, TestContext.Current.CancellationToken);
    }

    /// <summary>
    ///     Test PUT
    /// </summary>
    [Fact]
    public async Task TestPut_Response()
    {
        var behaviour = HttpBehaviour.Current as IChangeableHttpBehaviour;
        Assert.NotNull(behaviour);
        behaviour.JsonSerializer = new SimpleJsonSerializer();
        var result = await new Uri("https://httpbin.org/put").PutAsync<dynamic>(null, TestContext.Current.CancellationToken);
        Assert.NotNull(result);
    }

    /// <summary>
    ///     Test redirecting
    /// </summary>
    //[Fact]
    // Disabled as the link didn't work at the time of testing
    public async Task TestRedirectAndFollow()
    {
        var result = await new Uri("https://nghttp2.org/httpbin/redirect/5").GetAsAsync<string>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
    }

    /// <summary>
    ///     Test NOT redirecting, also testing the MakeCurrent of the behavior
    /// </summary>
    [Fact]
    public async Task TestRedirectAndNotFollow()
    {
        var behavior = new HttpBehaviour
        {
            HttpSettings = new HttpSettings()
        };
        behavior.HttpSettings.AllowAutoRedirect = false;
        behavior.MakeCurrent();
        await Assert.ThrowsAsync<HttpRequestException>(async () => await new Uri("https://httpbin.org/redirect/2").GetAsAsync<string>(TestContext.Current.CancellationToken));
    }

    /// <summary>
    ///     Test user-agent
    /// </summary>
    [Fact]
    public async Task TestUserAgent()
    {
        var result = await new Uri("https://httpbin.org/user-agent").GetAsAsync<IDictionary<string, string>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        Assert.Equal(HttpExtensionsGlobals.HttpSettings.DefaultUserAgent, result["user-agent"]);
    }

    /// <summary>
    ///     Test user-agent
    /// </summary>
    [Fact]
    public async Task TestUrlEncodedFormData()
    {
        Dictionary<string, string> values = new();
        //var f = "h5=1&_time=1626124814580&alias=liveme&videoid=16261222677467284812&area=us&vali=XjMJleNrbmw2ybp&risk_token=t345a9757d0ab68f1feb87c38417e5230";
        values["h5"] = "1";
        values["time"] = "1626124814580";
        values["risk_token"] = "t345a9757d0ab68f1feb87c38417e5230";
        values["alias"] = "liveme";
        values["area"] = "us";
        values["videoid"] = "16261222677467284812";


        var result = await new Uri("https://httpbin.org/anything").PostAsync<string>(values, TestContext.Current.CancellationToken);

        Assert.NotNull(result);
    }
}