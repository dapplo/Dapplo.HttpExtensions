// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Dapplo.HttpExtensions.JsonSimple;
using Dapplo.HttpExtensions.Tests.TestEntities;
using Dapplo.Log;
using Dapplo.Log.XUnit;
using Xunit;
using Xunit.Abstractions;

namespace Dapplo.HttpExtensions.Tests;

/// <summary>
///     Test posting parts
/// </summary>
public class HttpPartsPostTest
{
    private static readonly LogSource Log = new();
    private static readonly Uri RequestBinUri = new("http://httpbin.org");

    public HttpPartsPostTest(ITestOutputHelper testOutputHelper)
    {
        LogSettings.RegisterDefaultLogger<XUnitLogger>(LogLevels.Verbose, testOutputHelper);
        SimpleJsonSerializer.RegisterGlobally();
    }

    /// <summary>
    ///     Test posting, using Bitmap
    /// </summary>
    [Fact]
    public async Task TestPost_Bitmap()
    {
        var testUri = RequestBinUri.AppendSegments("post");
        var uploadBehaviour = HttpBehaviour.Current.ShallowClone();

        bool hasProgress = false;

        uploadBehaviour.UseProgressStream = true;
        uploadBehaviour.UploadProgress += progress =>
        {
            Log.Info().WriteLine("Progress {0}", (int) (progress * 100));
            hasProgress = true;
        };
        uploadBehaviour.MakeCurrent();
        var testObject = new MyMultiPartRequest<Bitmap>
        {
            BitmapContentName = "MyBitmapContent",
            BitmapFileName = "MyBitmapFilename",
            OurBitmap = new Bitmap(10, 10),
            JsonInformation = new GitHubError {DocumentationUrl = "http://test.de", Message = "Hello"}
        };
        testObject.Headers.Add("Name", "Dapplo");
        var result = await testUri.PostAsync<dynamic>(testObject);
        Assert.NotNull(result);
        Assert.True(hasProgress);
    }

    /// <summary>
    ///     Test posting, this time use a BitmapSource
    /// </summary>
    [Fact]
    public async Task TestPost_BitmapSource()
    {
        var testUri = RequestBinUri.AppendSegments("post");
        var uploadBehaviour = HttpBehaviour.Current.ShallowClone();

        bool hasProgress = false;

        uploadBehaviour.UseProgressStream = true;
        uploadBehaviour.UploadProgress += progress =>
        {
            Log.Info().WriteLine("Progress {0}", (int) (progress * 100));
            hasProgress = true;
        };
        uploadBehaviour.MakeCurrent();
        var testObject = new MyMultiPartRequest<BitmapSource>
        {
            BitmapContentName = "MyBitmapContent",
            BitmapFileName = "MyBitmapFilename",
            OurBitmap = BitmapSource.Create(1, 1, 96, 96, PixelFormats.Bgr24, null, new byte[] {0, 0, 0}, 3),
            JsonInformation = new GitHubError {DocumentationUrl = "http://test.de", Message = "Hello"}
        };
        testObject.Headers.Add("Name", "Dapplo");
        var result = await testUri.PostAsync<dynamic>(testObject);
        Assert.NotNull(result);
        Assert.True(hasProgress);
    }
}