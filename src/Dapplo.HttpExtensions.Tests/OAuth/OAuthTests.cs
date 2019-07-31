//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2019 Dapplo
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
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.OAuth;
using Dapplo.Log;
using Dapplo.Log.XUnit;
using Xunit;
using Xunit.Abstractions;

#endregion

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
        ///     This will test Oauth with a EmbeddedBrowser "code" receiver against an oauth server provided by Photo bucket
        /// </summary>
        /// <returns>Task</returns>
        [Fact]
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