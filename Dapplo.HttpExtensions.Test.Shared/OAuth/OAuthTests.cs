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
using Dapplo.LogFacade;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Dapplo.HttpExtensions.Test.OAuth
{
	/// <summary>
	/// This test is more an integration test, SHOULD NOT RUN on a headless server, as it opens a browser where a user should do something
	/// </summary>
	public class OAuthTests
	{
		private static readonly LogSource Log = new LogSource();

		private static readonly Uri PhotobucketApiUri = new Uri("http://api.photobucket.com");
		private readonly IHttpBehaviour _oAuthHttpBehaviour;
		private string _subdomain;
		private string _username;

		public OAuthTests(ITestOutputHelper testOutputHelper)
		{
			XUnitLogger.RegisterLogger(testOutputHelper, LogLevel.Verbose);
			var oAuthSettings = new OAuth1Settings
			{
				ClientId = "<photobucket consumer key>",
				ClientSecret = "<photobucket consumer secret>",
				CloudServiceName = "Photo bucket",
				EmbeddedBrowserWidth = 1010,
				EmbeddedBrowserHeight = 400,
				AuthorizeMode = AuthorizeModes.EmbeddedBrowser,
				TokenUrl = PhotobucketApiUri.AppendSegments("login", "request"),
				TokenMethod = HttpMethod.Post,
				AccessTokenUrl = new Uri("http://api.photobucket.com/login/access"),
				AccessTokenMethod = HttpMethod.Post,
				AuthorizationUri = PhotobucketApiUri.AppendSegments("apilogin", "login")
				 .ExtendQuery(new Dictionary<string, string>{
						{ OAuth1Parameters.Token.EnumValueOf(), "{RequestToken}"},
						{ OAuth1Parameters.Callback.EnumValueOf(), "{RedirectUrl}"}
				 }),
				RedirectUrl = "http://getgreenshot.org",
				CheckVerifier = false,
			};
			var oAuthHttpBehaviour = OAuth1HttpBehaviourFactory.Create(oAuthSettings);
			// Store the leftover values
			oAuthHttpBehaviour.OnAccessToken = values =>
			{
				if (values.ContainsKey("subdomain"))
				{
					_subdomain = values["subdomain"];
				}
				if (values.ContainsKey("username"))
				{
					_username = values["username"];
				}
			};
			// Process the leftover values
			oAuthHttpBehaviour.BeforeSend = httpRequestMessage =>
			{
				if (_subdomain != null)
				{
					var uriBuilder = new UriBuilder(httpRequestMessage.RequestUri)
					{
						Host = _subdomain
					};
					httpRequestMessage.RequestUri = uriBuilder.Uri;
				}
			};
			_oAuthHttpBehaviour = oAuthHttpBehaviour;
		}

		/// <summary>
		/// This will test Oauth with a EmbeddedBrowser "code" receiver against an oauth server provided by Photo bucket
		/// </summary>
		/// <returns>Task</returns>
		//[WinFormsFact]
		public async Task TestOAuthHttpMessageHandler_Get()
		{
			var userInformationUri = PhotobucketApiUri.AppendSegments("user").ExtendQuery("format", "json");

			// Make sure you use your special IHttpBehaviour for the OAuth requests!
			_oAuthHttpBehaviour.MakeCurrent();
			var response = await userInformationUri.OAuth1GetAsAsync<dynamic>();


			Assert.True(response.status == "OK");
		}

		//[WinFormsFact]
		public async Task TestOAuthHttpMessageHandler_PostImage()
		{
			_oAuthHttpBehaviour.MakeCurrent();

			// This request is important, as the username is not available without having an access token.
			await PhotobucketApiUri.AppendSegments("users").OAuth1GetAsAsync<XDocument>();

			var uploadUri = PhotobucketApiUri.AppendSegments("album", _username, "upload");

			var filename = "d.png";
			var signedParameters = new Dictionary<string, object>
			{
				{"type", "image"},
				{"title", "Dapplo logo"},
				{"filename", filename}
			};
			// Add image
			using (var fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
			{
				using (var streamContent = new StreamContent(fileStream))
				{
					streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
					streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
					{
						Name = "\"uploadfile\"",
						FileName = "\"" + filename + "\"",
					};

					try
					{
						var responseString = await uploadUri.OAuth1PostAsync<string>(streamContent, signedParameters);
						Log.Info().WriteLine(responseString);
					}
					catch (Exception ex)
					{
						Log.Error().WriteLine(ex, "Error uploading to Photobucket.");
						throw;
					}
				}
			}
		}

		[Fact]
		public void TestHmacSha1Hash()
		{
			// See: http://oauth.net/core/1.0a/#RFC2104
			var hmacsha1 = new HMACSHA1 { Key = Encoding.UTF8.GetBytes("kd94hf93k423kf44&pfkkdhi9sl3r4s00") };
			var digest = OAuth1HttpMessageHandler.ComputeHash(hmacsha1, "GET&http%3A%2F%2Fphotos.example.net%2Fphotos&file%3Dvacation.jpg%26oauth_consumer_key%3Ddpf43f3p2l4k3l03%26oauth_nonce%3Dkllo9940pd9333jh%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1191242096%26oauth_token%3Dnnch734d00sl2jdk%26oauth_version%3D1.0%26size%3Doriginal");

			Assert.Equal("tR3+Ty81lMeYAr/Fid0kMTYa/WM=", digest);
		}
	}
}
