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
	along with Dapplo.HttpExtensions. If not, see <http://www.gnu.org/licenses/>.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dapplo.HttpExtensions.Test
{
	/// <summary>
	/// These are the tests for the UriModifyExtensions
	/// </summary>
	[TestClass]
	public class UriModifyExtensionsTests
	{
		private const string Key = "key";
		private const int Value = 1234;
		private const string TestUriSingleValue = "http://jira/issue?value1=4321";
		private const string TestUriDuplicateValues = "http://jira/issue?value1=4321&value1=1234";

		[TestMethod]
		public void TestAppendSegments()
		{
			var uri = new Uri("http://jira/name?value1=1234");
			uri = uri.AppendSegments("joost");
			Assert.AreEqual("http://jira/name/joost?value1=1234", uri.AbsoluteUri);
		}

		[TestMethod]
		public void TestExtendQuery_WithDictionary()
		{
			var uri = new Uri(TestUriSingleValue);
			uri = uri.ExtendQuery(new Dictionary<string, object>
			{
				{
					Key, Value
				}
			});

			Assert.AreEqual($"{TestUriSingleValue}&{Key}={Value}", uri.AbsoluteUri);
		}

		[TestMethod]
		public void TestExtendQuery_WithDictionary_MultipleValuesInSource()
		{
			var uri = new Uri(TestUriDuplicateValues);
			uri = uri.ExtendQuery(new Dictionary<string, object>
			{
				{
					Key, Value
				}
			});

			Assert.AreEqual($"{TestUriDuplicateValues}&{Key}={Value}", uri.AbsoluteUri);
		}

		[TestMethod]
		public void TestExtendQuery_WithLookup_MultipleValuesInSource()
		{
			var uri = new Uri(TestUriDuplicateValues);
			var testValues = new List<KeyValuePair<string, int>>
			{
				new KeyValuePair<string, int>(Key,Value),
				new KeyValuePair<string, int>(Key,Value),
			};
			var lookup = testValues.ToLookup(x => x.Key, x => x.Value);
			// Make sure we have one Key, which has multiple values
			Assert.IsTrue(lookup.Count() == 1);

			uri = uri.ExtendQuery(lookup);
			Assert.AreEqual($"{TestUriDuplicateValues}&{Key}={Value}&{Key}={Value}", uri.AbsoluteUri);
		}

		[TestMethod]
		public void TestExtendQuery_WithNameValue()
		{
			var uri = new Uri(TestUriDuplicateValues);
			uri = uri.ExtendQuery(Key, Value);

			Assert.AreEqual($"{TestUriDuplicateValues}&{Key}={Value}", uri.AbsoluteUri);
		}

		[TestMethod]
		public void TestExtendQuery_WithNameValue_EncodingNeeded()
		{
			var uri = new Uri(TestUriDuplicateValues);

			var uriValue = new Uri("http://jira/issue?otherval1=10&othervar2=20");
			var encodedUri = Uri.EscapeDataString(uriValue.AbsoluteUri);

			uri = uri.ExtendQuery("url", uriValue);

			Assert.AreEqual($"{TestUriDuplicateValues}&url={encodedUri}", uri.AbsoluteUri);
		}

		[TestMethod]
		public void TestSetCredentials()
		{
			const string username = "myusername";
			const string password = "mypassword";
			var uri = new Uri(TestUriDuplicateValues);
			uri = uri.SetCredentials(username, password);

			Assert.AreEqual($"http://{username}:{password}@{TestUriDuplicateValues.Replace("http://","")}", uri.AbsoluteUri);
		}
	}
}
