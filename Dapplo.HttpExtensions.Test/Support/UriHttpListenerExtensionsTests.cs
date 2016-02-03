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

using Dapplo.HttpExtensions.Listener;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Dapplo.HttpExtensions.Test.Support
{
	[TestClass]
	public class UriHttpListenerExtensionsTests
	{
		[TestMethod]
		public async Task TestListenAsync()
		{
			// Try listening on 8080, if this doesn't work take the first free port
			var listenUri = new[] { 8080, 0 }.CreateLocalHostUri().AppendSegments("AsyncHttpListenerTests");
			var listenTask = listenUri.ListenAsync(async httpListenerContext =>
			{
				// Process the request
				var httpListenerRequest = httpListenerContext.Request;
				var result = httpListenerRequest.Url.QueryToDictionary();
				await httpListenerContext.RespondAsync("OK");
				return result;
			});
			// Do we need a delay for the listener to be ready?
			//await Task.Delay(100);
			var testUri = listenUri.ExtendQuery("name", "dapplo");
			var okResponse = await testUri.GetAsAsync<string>();
			Assert.AreEqual("OK", okResponse);
			var actionResult = await listenTask;
			Assert.IsTrue(actionResult.ContainsKey("name"));
			Assert.IsTrue(actionResult["name"] == "dapplo");
		}
	}
}
