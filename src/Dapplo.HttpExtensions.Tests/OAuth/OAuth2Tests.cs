// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.OAuth;
using Dapplo.Log;
using Dapplo.Log.XUnit;
using Xunit;
using Xunit.Abstractions;

namespace Dapplo.HttpExtensions.Tests.OAuth
{
    /// <summary>
    ///     This test is more an integration test, SHOULD NOT RUN on a headless server, as it opens a browser where a user
    ///     should do something
    /// </summary>
    public class OAuth2Tests
    {
        private static readonly Uri GoogleApiUri = new("https://www.googleapis.com");
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
                AuthorizationUri = new Uri("https://accounts.google.com").AppendSegments("o", "oauth2", "v2", "auth")
                    .ExtendQuery(new Dictionary<string, string>
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
#pragma warning disable xUnit1013 // Public method should be marked as test
        public async Task TestOAuth2HttpMessageHandler()
#pragma warning restore xUnit1013 // Public method should be marked as test
        {
            var calendarApiUri = GoogleApiUri.AppendSegments("calendar", "v3");
            // Make sure you use your special IHttpBehaviour before the requests which need OAuth
            _oAuthHttpBehaviour.MakeCurrent();
            var response = await calendarApiUri.AppendSegments("users", "me", "calendarList").GetAsAsync<dynamic>();
            Assert.True(response.ContainsKey("items"));
            Assert.True(response["items"].Count > 0);
        }

        [Fact]
        public void TestOutOfBoundCodeReceiverParseTitle()
        {
            var queryPartOfTitleRegEx = new Regex(@".*\|\|(?<query>.*)\|\|.*", RegexOptions.IgnoreCase);
            var state = Guid.NewGuid().ToString();
            const string code = "2497hf29ruh234zruif390ugo34t23jfg23";
            var query = $"state={state}&code={code}";
            var testString = $"Greenshot authenticated with Imgur||{query}|| - Google Chrome";
            Assert.False(string.IsNullOrEmpty(testString));
            var match = queryPartOfTitleRegEx.Match(testString);
            Assert.True(match.Success);
            var queryParameters = match.Groups["query"]?.Value;
            Assert.NotNull(queryParameters);
            Assert.NotEmpty(queryParameters);
            Assert.Equal(query, queryParameters);
            var dict = UriParseExtensions.QueryStringToDictionary(queryParameters);
            Assert.Equal(code, dict["code"]);
            Assert.Equal(state, dict["state"]);

        }
    }
}