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
    ///     These are the tests for the UriModifyExtensions
    /// </summary>
    public class UriModifyExtensionsTests
    {
        private const string Key = "key";
        private const int Value = 1234;
        private const string TestUriSingleValue = "http://jira/issue?value1=4321";
        private const string TestUriDuplicateValues = "http://jira/issue?value1=4321&value1=1234";

        public UriModifyExtensionsTests(ITestOutputHelper testOutputHelper)
        {
            LogSettings.RegisterDefaultLogger<XUnitLogger>(LogLevels.Verbose, testOutputHelper);
        }

        [Fact]
        public void TestAppendSegments()
        {
            var uri = new Uri("http://jira/name?value1=1234");
            uri = uri.AppendSegments("joost");
            Assert.Equal("http://jira/name/joost?value1=1234", uri.AbsoluteUri);
        }

        [Fact]
        public void TestAppendSegments_SlashAlreadyThere1()
        {
            var uri = new Uri("http://jira/name/?value1=1234");
            uri = uri.AppendSegments("joost");
            Assert.Equal("http://jira/name/joost?value1=1234", uri.AbsoluteUri);
        }

        [Fact]
        public void TestAppendSegments_SlashAlreadyThere2()
        {
            var uri = new Uri("http://jira/name/?value1=1234");
            uri = uri.AppendSegments("/joost");
            Assert.Equal("http://jira/name/joost?value1=1234", uri.AbsoluteUri);
        }

        [Fact]
        public void TestAppendSegments_SlashAlreadyThere3()
        {
            var uri = new Uri("http://jira/name/?value1=1234");
            uri = uri.AppendSegments("joost/");
            Assert.Equal("http://jira/name/joost?value1=1234", uri.AbsoluteUri);
        }

        [Fact]
        public void TestAppendSegments_SquareBrackets()
        {
            var uri = new Uri("http://jira/name/?value1=1234");
            uri = uri.AppendSegments("blub[1]/");
            Assert.Equal("http://jira/name/blub%5B1%5D?value1=1234", uri.AbsoluteUri);
        }

        [Fact]
        public void TestExtendQuery_WithDictionary()
        {
            var uri = new Uri(TestUriSingleValue);
            uri = uri.ExtendQuery(new Dictionary<string, object>
            {
                {
                    Key, Value
                }
            });

            Assert.Equal($"{TestUriSingleValue}&{Key}={Value}", uri.AbsoluteUri);
        }

        [Fact]
        public void TestExtendQuery_WithDictionary_MultipleValuesInSource()
        {
            var uri = new Uri(TestUriDuplicateValues);
            uri = uri.ExtendQuery(new Dictionary<string, object>
            {
                {
                    Key, Value
                }
            });

            Assert.Equal($"{TestUriDuplicateValues}&{Key}={Value}", uri.AbsoluteUri);
        }

        [Fact]
        public void TestExtendQuery_WithLookup_MultipleValuesInSource()
        {
            var uri = new Uri(TestUriDuplicateValues);
            var testValues = new List<KeyValuePair<string, int>>
            {
                new KeyValuePair<string, int>(Key, Value),
                new KeyValuePair<string, int>(Key, Value)
            };
            var lookup = testValues.ToLookup(x => x.Key, x => x.Value);
            // Make sure we have one Key, which has multiple values
            Assert.True(lookup.Count == 1);

            uri = uri.ExtendQuery(lookup);
            Assert.Equal($"{TestUriDuplicateValues}&{Key}={Value}&{Key}={Value}", uri.AbsoluteUri);
        }

        [Fact]
        public void TestExtendQuery_WithNameValue()
        {
            var uri = new Uri(TestUriDuplicateValues);
            uri = uri.ExtendQuery(Key, Value);

            Assert.Equal($"{TestUriDuplicateValues}&{Key}={Value}", uri.AbsoluteUri);
        }

        [Fact]
        public void TestExtendQuery_WithNameValue_EncodingNeeded()
        {
            var uri = new Uri(TestUriDuplicateValues);

            var uriValue = new Uri("http://jira/issue?otherval1=10&othervar2=20");
            var encodedUri = Uri.EscapeDataString(uriValue.AbsoluteUri);

            uri = uri.ExtendQuery("url", uriValue);

            Assert.Equal($"{TestUriDuplicateValues}&url={encodedUri}", uri.AbsoluteUri);
        }

        [Fact]
        public void TestSetCredentials()
        {
            const string username = "myusername";
            const string password = "mypassword";
            var uri = new Uri(TestUriDuplicateValues);
            uri = uri.SetCredentials(username, password);

            Assert.Equal($"http://{username}:{password}@{TestUriDuplicateValues.Replace("http://", "")}", uri.AbsoluteUri);
        }
    }
}