// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.OAuth;
using Dapplo.Log;
using Dapplo.Log.XUnit;
using Xunit;
using Xunit.Abstractions;

namespace Dapplo.HttpExtensions.Tests.OAuth
{
    /// <summary>
    ///     This tests some of the basic oauth 1 logic, together with a server at: http://term.ie/oauth/example/
    /// </summary>
    public class OAuthTests
    {
        private static readonly Uri OAuthTestServerUri = new Uri("http://term.ie/oauth/example/");
        private readonly IHttpBehaviour _oAuthHttpBehaviour;

        public OAuthTests(ITestOutputHelper testOutputHelper)
        {
            LogSettings.RegisterDefaultLogger<XUnitLogger>(LogLevels.Verbose, testOutputHelper);
            var oAuthSettings = new OAuth1Settings
            {
                ClientId = "key",
                ClientSecret = "secret",
                AuthorizeMode = AuthorizeModes.TestPassThrough,
                TokenUrl = OAuthTestServerUri.AppendSegments("request_token.php"),
                TokenMethod = HttpMethod.Post,
                AccessTokenUrl = OAuthTestServerUri.AppendSegments("access_token.php"),
                AccessTokenMethod = HttpMethod.Post,
                CheckVerifier = false
            };
            _oAuthHttpBehaviour = OAuth1HttpBehaviourFactory.Create(oAuthSettings);
        }

        [Fact]
        public void TestHmacSha1Hash()
        {
            // See: http://oauth.net/core/1.0a/#RFC2104
            var hmacsha1 = new HMACSHA1 {Key = Encoding.UTF8.GetBytes("kd94hf93k423kf44&pfkkdhi9sl3r4s00")};
            var digest = OAuth1HttpMessageHandler.ComputeHash(hmacsha1,
                "GET&http%3A%2F%2Fphotos.example.net%2Fphotos&file%3Dvacation.jpg%26oauth_consumer_key%3Ddpf43f3p2l4k3l03%26oauth_nonce%3Dkllo9940pd9333jh%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1191242096%26oauth_token%3Dnnch734d00sl2jdk%26oauth_version%3D1.0%26size%3Doriginal");

            Assert.Equal("tR3+Ty81lMeYAr/Fid0kMTYa/WM=", digest);
        }

        /// <summary>
        ///     This will test Oauth with http://term.ie/oauth/example/
        /// </summary>
        /// <returns>Task</returns>
        //[Fact] // Disabled as term.ie is no longer available
        public async Task TestOAuthHttpMessageHandler_Get()
        {
            var userInformationUri = OAuthTestServerUri.AppendSegments("echo_api.php").ExtendQuery("name", "dapplo");

            // Make sure you use your special IHttpBehaviour for the OAuth requests!
            _oAuthHttpBehaviour.MakeCurrent();
            var response = await userInformationUri.GetAsAsync<string>();

            Assert.Contains("dapplo", response);
        }
    }
}