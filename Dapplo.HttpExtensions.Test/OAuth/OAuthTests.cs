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

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dapplo.HttpExtensions.Support;
using Dapplo.HttpExtensions.OAuth;
using System.Threading.Tasks;

namespace Dapplo.HttpExtensions.Test.OAuth
{
	//[TestClass]
	public class OAuthTests
	{
		[TestInitialize]
		public void InitLogger()
		{
			HttpExtensionsGlobals.Logger = new TraceLogger();
		}

		[TestMethod]
		public async Task TestOAuthHttpMessageHandler()
		{
			var oauthHttpBehaviour = new HttpBehaviour();
			var oAuth2Settings = new OAuth2Settings
			{
				ClientId = "demoapp",
				ClientSecret = "demopass",
				AuthorizeMode = AuthorizeModes.LocalServer,
				TokenUrl = new Uri("http://brentertainment.com/oauth2/lockdin/token"),
				AuthorizationUri = new Uri("http://brentertainment.com").
					AppendSegments("oauth2","lockdin","authorize").
					ExtendQuery(new Dictionary<string, string>{
						{ "response_type", "code"},
						{ "client_id", "{ClientId}" },
						{ "redirect_uri", "{RedirectUrl}" },
						{ "state", "{State}"}
					})
			};
			oauthHttpBehaviour.OnHttpMessageHandlerCreated = httpMessageHandler => new OAuthHttpMessageHandler(oAuth2Settings, oauthHttpBehaviour, httpMessageHandler);
			var response = await new Uri("http://brentertainment.com/oauth2/lockdin/resource").GetAsAsync<string>(oauthHttpBehaviour);
			Assert.IsTrue(response.Contains("friends"));
		}
	}
}
