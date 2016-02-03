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

using Dapplo.HttpExtensions.OAuth;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dapplo.HttpExtensions.Test.OAuth
{
	/// <summary>
	/// This test is more an integration test, SHOULD NOT RUN on a headless server, as it opens a browser where a user should do something
	/// </summary>
	//[TestClass]
	public class OAuthTests
	{
		private static readonly Uri _googleApiUri = new Uri("https://www.googleapis.com");
		private static IHttpBehaviour _oAuthHttpBehaviour;

		[ClassInitialize]
		public static void SetupOAuth(TestContext context)
		{
			var oAuth2Settings = new OAuth2Settings
			{
				ClientId = "<client id from google developer console>",
				ClientSecret = "<client id from google developer console>",
				CloudServiceName = "Google",
				AuthorizeMode = AuthorizeModes.LocalServer,
				TokenUrl = _googleApiUri.AppendSegments("oauth2","v4","token"),
				AuthorizationUri = new Uri("https://accounts.google.com").AppendSegments("o", "oauth2", "v2", "auth"). ExtendQuery(new Dictionary<string, string>{
						{ "response_type", "code"},
						{ "client_id", "{ClientId}" },
						{ "redirect_uri", "{RedirectUrl}" },
						{ "state", "{State}"},
						{ "scope" , _googleApiUri.AppendSegments("auth","calendar").AbsoluteUri}
				})
			};
			_oAuthHttpBehaviour = OAuth2HttpBehaviourFactory.Create(oAuth2Settings);
		}
		/// <summary>
		/// This will test Oauth with a LocalServer "code" receiver against a demo oauth server provided by brentertainment.com
		/// </summary>
		/// <returns>Task</returns>
		[TestMethod]
		public async Task TestOAuthHttpMessageHandler()
		{
			var calendarApiUri = _googleApiUri.AppendSegments("calendar", "v3");
			var response = await calendarApiUri.AppendSegments("users","me","calendarList").GetAsAsync<dynamic>(_oAuthHttpBehaviour);
			Assert.IsTrue(response.ContainsKey("items"));
			Assert.IsTrue(response["items"].Count > 0);
		}
	}
}
