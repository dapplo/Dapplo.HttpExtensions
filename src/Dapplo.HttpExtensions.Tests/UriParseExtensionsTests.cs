// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dapplo.Log;
using Dapplo.Log.XUnit;
using Xunit;
using Xunit.Abstractions;

namespace Dapplo.HttpExtensions.Tests
{
    /// <summary>
    ///     These are the tests for the UriParseExtensions
    /// </summary>
    public class UriParseExtensionsTests
    {
        private const string TestKey = "value1";
        private const string TestValue = "1234";
        private static readonly Uri TestUriSimple = new Uri("http://jira/name?somevalue=42").ExtendQuery(TestKey, TestValue);
        private readonly Uri _testUriComplex = TestUriSimple.ExtendQuery(TestKey, TestValue);

        public UriParseExtensionsTests(ITestOutputHelper testOutputHelper)
        {
            LogSettings.RegisterDefaultLogger<XUnitLogger>(LogLevels.Verbose, testOutputHelper);
        }

        [Fact]
        public void TestExtendQuery_empty_dictionary()
        {
            var simpleUri = new Uri("http://dapplo.net");
            var newUri = simpleUri.ExtendQuery(new Dictionary<string, string>());
            Assert.NotNull(newUri);
            Assert.Equal(simpleUri, newUri);
        }

        [Fact]
        public void TestQueryToDictionary()
        {
            var dictionary = _testUriComplex.QueryToDictionary();
            Assert.NotNull(dictionary);
            Assert.True(dictionary.ContainsKey(TestKey));
            Assert.Equal(TestValue, dictionary[TestKey]);
        }

        [Fact]
        public void TestQueryToKeyValuePairs()
        {
            var keyValuePairs = _testUriComplex.QueryToKeyValuePairs();
            Assert.NotNull(keyValuePairs);
            Assert.Contains(keyValuePairs, x => x.Key == TestKey && x.Value == TestValue);
        }

        [Fact]
        public void TestQueryToLookup()
        {
            var loopkup = TestUriSimple.QueryToLookup();
            Assert.NotNull(loopkup);
            Assert.Contains(loopkup, x => x.Key == TestKey && x.Contains(TestValue));
        }

        [Fact]
        public void TestQueryToLookup_WithDuplicates()
        {
            var loopkup = _testUriComplex.QueryToLookup();
            Assert.NotNull(loopkup);
            Assert.Equal(2, loopkup[TestKey].Count(x => x == TestValue));
        }
    }
}