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
using System.Collections.Generic;

namespace Dapplo.HttpExtensions.Test
{
	[TestClass]
	public class UriModifyExtensionsTests
	{
		[TestMethod]
		public void TestAppendSegments()
		{
			var uri = new Uri("http://jira/name?value1=1234");
			uri = uri.AppendSegments("joost");
			Assert.AreEqual("http://jira/name/joost?value1=1234", uri.AbsoluteUri);
		}

		[TestMethod]
		public void TestExtendQuery()
		{
			var uri = new Uri("http://jira/issue?value1=4321");
			const int key = 1234;
			uri = uri.ExtendQuery(new Dictionary<string, object>
			{
				{
					"key", key
				}
			});

			Assert.AreEqual("http://jira/issue?value1=4321&key=1234", uri.AbsoluteUri);
		}

		[TestMethod]
		public void TestExtendQuery2()
		{
			var uri = new Uri("http://jira/issue?value1=4321&value1=43211");
			const int key = 1234;
			uri = uri.ExtendQuery(new Dictionary<string, object>
			{
				{
					"key", key
				}
			});

			Assert.AreEqual("http://jira/issue?value1=4321&value1=43211&key=1234", uri.AbsoluteUri);
		}

		[TestMethod]
		public void TestExtendQuery3()
		{
			var uri = new Uri("http://jira/issue?value1=4321&value1=43211");
			const int key = 1234;
			uri = uri.ExtendQuery("key", key);

			Assert.AreEqual("http://jira/issue?value1=4321&value1=43211&key=1234", uri.AbsoluteUri);
		}

		[TestMethod]
		public void TestExtendQueryEncoding()
		{
			var uriValue = new Uri("http://jira/issue?otherval1=10&othervar2=20");
			var encodedUri = Uri.EscapeDataString(uriValue.ToString());
			var uri = new Uri("http://jira/issue?value1=4321&value1=43211");
			uri = uri.ExtendQuery("url", uriValue);

			Assert.AreEqual("http://jira/issue?value1=4321&value1=43211&url="+ encodedUri, uri.AbsoluteUri);
		}
	}
}
