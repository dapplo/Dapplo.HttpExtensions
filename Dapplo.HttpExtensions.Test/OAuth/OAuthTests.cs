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
using Dapplo.HttpExtensions.OAuth;
using System.Threading.Tasks;
using Dapplo.LogFacade;
using Dapplo.LogFacade.Loggers;

namespace Dapplo.HttpExtensions.Test.OAuth
{
	/// <summary>
	/// This test is more an integration test, SHOULD NOT RUN on a headless server, as it opens a browser where a user should do something
	/// </summary>
	//[TestClass]
	public class OAuthTests
	{
		[TestInitialize]
		public void InitLogger()
		{
			// Make sure we get some logging from the internals of our library
			LogSettings.Logger = new TraceLogger();
		}

		/// <summary>
		/// This will test Oauth with a LocalServer "code" receiver against a demo oauth server provided by brentertainment.com
		/// </summary>
		/// <returns>Task</returns>
		[TestMethod]
		public async Task TestOAuthHttpMessageHandler()
		{
			// Create OAuth2Setting for a demo server, which expects a token url like:
			// http://brentertainment.com/oauth2/lockdin/authorize?response_type=code&client_id=demoapp&redirect_uri=http%3A%2F%2Fbrentertainment.com%2Foauth2%2Fclient%2Freceive_authcode%3Fshow_refresh_token%3D1&state=120b347034ef48c18caee7214f12bdcd
			var oAuth2Settings = new OAuth2Settings
			{
				ClientId = "demoapp",
				ClientSecret = "demopass",
				CloudServiceName = "brentertainment",
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
			var oauthHttpBehaviour = OAuth2HttpBehaviourFactory.Create(oAuth2Settings);
			// Special need, as http://brentertainment.com/oauth2/lockdin/resource returns text/html instead of json.
			oauthHttpBehaviour.ValidateResponseContentType = false;

			var response = await new Uri("http://brentertainment.com/oauth2/lockdin/resource").GetAsAsync<dynamic>(oauthHttpBehaviour);
			Assert.IsTrue(response.ContainsKey("friends"));
			Assert.IsTrue(response["friends"].Count > 0);
		}
	}
}
