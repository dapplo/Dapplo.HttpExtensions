//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016 Dapplo
// 
//  For more information see: http://dapplo.net/
//  Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
//  This file is part of Dapplo.HttpExtensions
// 
//  Dapplo.HttpExtensions is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  Dapplo.HttpExtensions is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have a copy of the GNU Lesser General Public License
//  along with Dapplo.HttpExtensions. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

#region using

using System;
using System.Collections.Generic;
using System.Linq;
using Dapplo.Log.XUnit;
using Dapplo.Log.Facade;
using Xunit;
using Xunit.Abstractions;

#endregion

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
			Assert.True(keyValuePairs.Any(x => x.Key == TestKey && x.Value == TestValue));
		}

		[Fact]
		public void TestQueryToLookup()
		{
			var loopkup = TestUriSimple.QueryToLookup();
			Assert.NotNull(loopkup);
			Assert.True(loopkup.Any(x => x.Key == TestKey && x.Contains(TestValue)));
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