/*
	Dapplo - building blocks for desktop applications
	Copyright (C) 2015-2016 Dapplo

	For more information see: http://dapplo.net/
	Dapplo repositories are hosted on GitHub: https://github.com/dapplo

	This file is part of Dapplo.HttpExtensions.

	Dapplo.HttpExtensions is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	Dapplo.HttpExtensions is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Dapplo.HttpExtensions.Test
{
	/// <summary>
	/// These are the tests for the UriParseExtensions
	/// </summary>
	[TestClass]
	public class UriParseExtensionsTests
	{
		private const string TestKey = "value1";
		private const string TestValue = "1234";
		private static readonly Uri TestUriSimple = new Uri("http://jira/name?somevalue=42").ExtendQuery(TestKey, TestValue);
		private Uri TestUriComplex = TestUriSimple.ExtendQuery(TestKey, TestValue);

		[TestMethod]
		public void TestQueryToDictionary()
		{
			var dictionary = TestUriComplex.QueryToDictionary();
			Assert.IsNotNull(dictionary);
			Assert.IsTrue(dictionary.ContainsKey(TestKey));
			Assert.AreEqual(TestValue, dictionary[TestKey]);
		}

		[TestMethod]
		public void TestQueryToKeyValuePairs()
		{
			var keyValuePairs = TestUriComplex.QueryToKeyValuePairs();
			Assert.IsNotNull(keyValuePairs);
			Assert.IsTrue(keyValuePairs.Any(x => x.Key == TestKey && x.Value == TestValue));
		}

		[TestMethod]
		public void TestQueryToLookup()
		{
			var loopkup = TestUriSimple.QueryToLookup();
			Assert.IsNotNull(loopkup);
			Assert.IsTrue(loopkup.Any(x => x.Key == TestKey && x.Contains(TestValue)));
		}

		[TestMethod]
		public void TestQueryToLookup_WithDuplicates()
		{
			var loopkup = TestUriComplex.QueryToLookup();
			Assert.IsNotNull(loopkup);
			Assert.AreEqual(2, loopkup[TestKey].Count(x => x == TestValue));
		}
	}
}
