//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2018 Dapplo
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

#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.Factory;
using Dapplo.HttpExtensions.JsonNet;
using Dapplo.HttpExtensions.OAuth;
using Dapplo.Log;
using Dapplo.Log.XUnit;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace Dapplo.HttpExtensions.Tests.OAuth
{
    /// <summary>
    ///     This test is more an integration test, SHOULD NOT RUN on a headless server, as it opens a browser where a user
    ///     should do something
    /// </summary>
    public class ImgurOAuth2Tests
    {
        private static readonly LogSource Log = new LogSource();
        private static readonly Uri GoogleApiUri = new Uri("https://www.googleapis.com");
        private readonly IHttpBehaviour _oAuthHttpBehaviour;
	private static readonly string ClientId = Environment.GetEnvironmentVariable("imgur_clientid");
	private static readonly string ClientSecret = Environment.GetEnvironmentVariable("imgur_clientsecret");
        private static readonly Uri ApiUri = new Uri("https://api.imgur.com/");
        private readonly HttpBehaviour _behavior;
        public ImgurOAuth2Tests(ITestOutputHelper testOutputHelper)
        {
            LogSettings.RegisterDefaultLogger<XUnitLogger>(LogLevels.Verbose, testOutputHelper);
            var oAuth2Settings = new OAuth2Settings
            {
                AuthorizationUri = ApiUri.AppendSegments("oauth2", "authorize").
                    ExtendQuery(new Dictionary<string, string>
                    {
                        {"response_type", "code"},
                        {"client_id", "{ClientId}"},
                        {"redirect_uri", "{RedirectUrl}"},
                        // TODO: Add version?
                        {"state", "{State}"}
                    }),
                TokenUrl = ApiUri.AppendSegments("oauth2", "token"),
                CloudServiceName = "Imgur",
                ClientId = ClientId,
                ClientSecret = ClientSecret,
                RedirectUrl = "https://getgreenshot.org/oauth/imgur",
                AuthorizeMode = AuthorizeModes.OutOfBoundAuto
            };

            _behavior = new HttpBehaviour
            {
                JsonSerializer = new JsonNetJsonSerializer(),
                OnHttpClientCreated = httpClient =>
                {
                    httpClient.SetAuthorization("Client-ID", ClientId);
                    httpClient.DefaultRequestHeaders.ExpectContinue = false;
                }
            };
            _oAuthHttpBehaviour = OAuth2HttpBehaviourFactory.Create(oAuth2Settings, _behavior);
        }

        /// <summary>
        ///     Retrieve the thumbnail of an imgur image
        /// </summary>
        /// <param name="token"></param>
        public async Task<int> RetrieveImgurCredits(CancellationToken token = default)
        {
            var creditsUri = ApiUri.AppendSegments("3", "credits.json");
            _oAuthHttpBehaviour.MakeCurrent();
            using (var client = HttpClientFactory.Create(creditsUri))
            {
                var response = await client.GetAsync(creditsUri, token).ConfigureAwait(false);
                await response.HandleErrorAsync().ConfigureAwait(false);
                var creditsJson = await response.GetAsAsync<dynamic>(token).ConfigureAwait(false);
                if ((creditsJson != null) && creditsJson.ContainsKey("data"))
                {
                    dynamic data = creditsJson.data;
                    if (data.ContainsKey("ClientRemaining"))
                    {
                        Log.Debug().WriteLine("{0}={1}", "ClientRemaining", (int)data.ClientRemaining);
                        return (int)data.ClientRemaining;
                    }
                    if (data.ContainsKey("UserRemaining"))
                    {
                        Log.Debug().WriteLine("{0}={1}", "UserRemaining", (int)data.UserRemaining);
                        return (int) data.UserRemaining;
                    }
                }

                return -1;
            }
        }

        /// <summary>
        ///     This will test Oauth with a LocalServer "code" receiver against a demo oauth server provided by brentertainment.com
        /// </summary>
        /// <returns>Task</returns>
        //[WpfFact]
        public async Task TestOAuth2HttpMessageHandler()
        {
            var credits = await RetrieveImgurCredits();
            Assert.True(credits > 0);
        }
    }
}