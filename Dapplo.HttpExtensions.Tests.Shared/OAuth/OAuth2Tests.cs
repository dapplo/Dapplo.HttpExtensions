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
using System.Threading.Tasks;
using Dapplo.HttpExtensions.OAuth;
using Dapplo.Log.XUnit;
using Dapplo.Log.Facade;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace Dapplo.HttpExtensions.Tests.OAuth
{
	/// <summary>
	///     This test is more an integration test, SHOULD NOT RUN on a headless server, as it opens a browser where a user
	///     should do something
	/// </summary>
	public class OAuth2Tests
	{
		private static readonly Uri GoogleApiUri = new Uri("https://www.googleapis.com");
		private readonly IHttpBehaviour _oAuthHttpBehaviour;

		public OAuth2Tests(ITestOutputHelper testOutputHelper)
		{
			LogSettings.RegisterDefaultLogger<XUnitLogger>(LogLevels.Verbose, testOutputHelper);
			var oAuth2Settings = new OAuth2Settings
			{
				ClientId = "<client id from google developer console>",
				ClientSecret = "<client secret from google developer console>",
				CloudServiceName = "Google",
				AuthorizeMode = AuthorizeModes.LocalhostServer,
				TokenUrl = GoogleApiUri.AppendSegments("oauth2", "v4", "token"),
				AuthorizationUri = new Uri("https://accounts.google.com").AppendSegments("o", "oauth2", "v2", "auth").ExtendQuery(new Dictionary<string, string>
				{
					{"response_type", "code"},
					{"client_id", "{ClientId}"},
					{"redirect_uri", "{RedirectUrl}"},
					{"state", "{State}"},
					{"scope", GoogleApiUri.AppendSegments("auth", "calendar").AbsoluteUri}
				})
			};
			_oAuthHttpBehaviour = OAuth2HttpBehaviourFactory.Create(oAuth2Settings);
		}

		/// <summary>
		///     This will test Oauth with a LocalServer "code" receiver against a demo oauth server provided by brentertainment.com
		/// </summary>
		/// <returns>Task</returns>
		//[Fact]
		public async Task TestOAuth2HttpMessageHandler()
		{
			var calendarApiUri = GoogleApiUri.AppendSegments("calendar", "v3");
			// Make sure you use your special IHttpBehaviour before the requests which need OAuth
			_oAuthHttpBehaviour.MakeCurrent();
			var response = await calendarApiUri.AppendSegments("users", "me", "calendarList").GetAsAsync<dynamic>();
			Assert.True(response.ContainsKey("items"));
			Assert.True(response["items"].Count > 0);
		}
	}
}