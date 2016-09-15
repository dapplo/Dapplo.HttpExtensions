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
using Dapplo.HttpExtensions.Tests.Shared.TestEntities;
using Dapplo.Log.XUnit;
using Dapplo.HttpExtensions.Tests.TestEntities;
using Dapplo.Log.Facade;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace Dapplo.HttpExtensions.Tests
{
	/// <summary>
	///     Tests for (de)serializing Json
	/// </summary>
	public class SimpleJsonTests
	{
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
				PublishedAt = DateTime.Now
			};
			var jsonString = SimpleJson.SerializeObject(gitHubRelease);
			Assert.Contains("html_url", jsonString);
			Assert.DoesNotContain("prerelease", jsonString);

			gitHubRelease.Prerelease = true;
			gitHubRelease.HtmlUrl = null;
			jsonString = SimpleJson.SerializeObject(gitHubRelease);
			Assert.DoesNotContain("html_url", jsonString);
			Assert.Contains("prerelease", jsonString);
		}

		[Fact]
		public void TestSimpleJson_DeserializeObject()
		{
			var json = "{\"customstringfield_array\": [],\"customstringfield_1\": \"testvalue1\",\"customstringfield_2\": \"testvalue2\",\"customintfield_1\": \"10\",\"customintfield_2\": 20,\"another_value\": \"testvalue3\",\"name\": \"Robin\"}";

			var jsonObject = SimpleJson.DeserializeObject<WithExtensionData>(json);
			Assert.True(jsonObject.StringExtensionData.Count == 2);
			Assert.True(jsonObject.IntExtensionData.Count == 2);
		}
	}
}