﻿// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using Dapplo.HttpExtensions.JsonSimple;
using Dapplo.HttpExtensions.Tests.TestEntities;
using Dapplo.Log;
using Dapplo.Log.XUnit;
using Xunit;
using Xunit.Abstractions;

namespace Dapplo.HttpExtensions.Tests;

/// <summary>
///     Tests for (de)serializing Json
/// </summary>
public class SimpleJsonTests
{
    private static readonly LogSource Log = new LogSource();

    public SimpleJsonTests(ITestOutputHelper testOutputHelper)
    {
        LogSettings.RegisterDefaultLogger<XUnitLogger>(LogLevels.Verbose, testOutputHelper);
    }

    /// <summary>
    ///     Test SimpleJson EmitDefaultValue
    /// </summary>
    [Fact]
    public void TestSerializeObject_EmitDefaultValue()
    {
        var gitHubRelease = new GitHubRelease
        {
            HtmlUrl = "http://test.url",
            Prerelease = false,
            PublishedAt = DateTime.Now,
            ReleaseType = ReleaseTypes.Private
        };
        var jsonString = SimpleJson.SerializeObject(gitHubRelease);
        Assert.Contains("html_url", jsonString);
        Assert.DoesNotContain("prerelease", jsonString);
        Assert.Contains("private", jsonString);

        gitHubRelease.Prerelease = true;
        gitHubRelease.HtmlUrl = null;
        gitHubRelease.ReleaseType = ReleaseTypes.Public;
        jsonString = SimpleJson.SerializeObject(gitHubRelease);
        Assert.DoesNotContain("html_url", jsonString);
        Assert.DoesNotContain("private", jsonString);
        Assert.Contains("prerelease", jsonString);
    }

    /// <summary>
    ///     Test SimpleJson EmitDefaultValue
    /// </summary>
    [Fact]
    public void TestSerializeObject_Readonly()
    {
        var testObject = new SerializeTestEntity
        {
            ValueNormal = "normal",
            ValueEmitDefaultFalse = null,
            ValueReadOnly = "readonly",
            ValueNotReadOnly = "notReadonly"
        };
        var jsonString = SimpleJson.SerializeObject(testObject);
        Log.Info().WriteLine("Serialized: " + jsonString);
        Assert.Contains("valueNormal", jsonString);
        Assert.Contains("valueNotReadOnly", jsonString);
        Assert.DoesNotContain("valueEmitDefaultFalse", jsonString);
        Assert.DoesNotContain("valueReadOnly", jsonString);

        testObject.ValueEmitDefaultFalse = "something";
        jsonString = SimpleJson.SerializeObject(testObject);
        Assert.Contains("valueEmitDefaultFalse", jsonString);
    }

    /// <summary>
    ///     Test SimpleJson ReadOnly
    /// </summary>
    [Fact]
    public void TestDeserializeObject_Readonly()
    {
        var jsonString = "{\"valueNormal\":\"normal\",\"valueReadOnly\":\"readonly\"}";
        var jsonObject = SimpleJson.DeserializeObject<SerializeTestEntity>(jsonString);
        Assert.NotNull(jsonObject.ValueReadOnly);
        Assert.NotNull(jsonObject.ValueNormal);
    }

    [Fact]
    public void TestSimpleJson_DeserializeObjectWithExtensionData()
    {
        var json =
            "{\"customstringfield_array\": [],\"customstringfield_1\": \"testvalue1\",\"customstringfield_2\": \"testvalue2\",\"customintfield_1\": \"10\",\"customintfield_2\": 20,\"another_value\": \"testvalue3\",\"name\": \"Robin\"}";

        var jsonObject = SimpleJson.DeserializeObject<WithExtensionData>(json);
        Assert.True(jsonObject.StringExtensionData.Count == 2);
        Assert.True(jsonObject.IntExtensionData.Count == 2);
    }

    [Fact]
    public void TestSimpleJson_SerializeObjectWithExtensionData()
    {
        var objectWithExtensionData = new WithExtensionData
        {
            RestExtensionData = new Dictionary<string, object>
            {
                {"Blub", 100}
            }
        };

        var json = SimpleJson.SerializeObject(objectWithExtensionData);
        Log.Info().WriteLine(json);
        Assert.Contains("Blub", json);
    }


    [Fact]
    public void GivenNumberWithoutDecimalTooLargeForLongTypeIsDecimal()
    {
        decimal veryLargeInteger = long.MaxValue + 1M;
        var json = veryLargeInteger.ToString(CultureInfo.InvariantCulture);
        object result = SimpleJson.DeserializeObject(json);

        Assert.IsType<decimal>(result);
    }

    [Fact]
    public void GivenNumberWithoutDecimalTooLargeForDecimalTypeIsDouble()
    {
        decimal veryVeryLargeInteger = decimal.MaxValue;
        var json = veryVeryLargeInteger + "0"; // Tack a zero onto the end -- multiply by 10
        object result = SimpleJson.DeserializeObject(json);

        Assert.IsType<double>(result);
    }
}