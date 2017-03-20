//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2017 Dapplo
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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.Extensions;
using Dapplo.Log;

#endregion

namespace Dapplo.HttpExtensions.OAuth
{
    /// <summary>
    ///     This DelegatingHandler handles the OAuth2 specific stuff and delegates the "final" SendAsync to the InnerHandler
    /// </summary>
    public class OAuth2HttpMessageHandler : DelegatingHandler
    {
        private static readonly LogSource Log = new LogSource();
        private readonly IHttpBehaviour _httpBehaviour;
        private readonly OAuth2Settings _oAuth2Settings;

        /// <summary>
        ///     Add the local server handler.
        /// </summary>
        static OAuth2HttpMessageHandler()
        {
#if NET45 || NET46
			CodeReceivers.Add(
				AuthorizeModes.LocalhostServer,
				new LocalhostCodeReceiver()
				);
			CodeReceivers.Add(
				AuthorizeModes.OutOfBound,
				new OutOfBoundCodeReceiver()
				);
			CodeReceivers.Add(
				AuthorizeModes.EmbeddedBrowser,
				new EmbeddedBrowserCodeReceiver()
				);
#endif
        }

        /// <summary>
        ///     Create a HttpMessageHandler which handles the OAuth 2 communication for you
        /// </summary>
        /// <param name="oAuth2Settings">OAuth2Settings</param>
        /// <param name="httpBehaviour">IHttpBehaviour</param>
        /// <param name="innerHandler">HttpMessageHandler</param>
        public OAuth2HttpMessageHandler(OAuth2Settings oAuth2Settings, IHttpBehaviour httpBehaviour, HttpMessageHandler innerHandler) : base(innerHandler)
        {
            if (oAuth2Settings.ClientId == null)
            {
                throw new ArgumentNullException(nameof(oAuth2Settings.ClientId));
            }
            if (oAuth2Settings.ClientSecret == null)
            {
                throw new ArgumentNullException(nameof(oAuth2Settings.ClientSecret));
            }
            if (oAuth2Settings.TokenUrl == null)
            {
                throw new ArgumentNullException(nameof(oAuth2Settings.TokenUrl));
            }

            _oAuth2Settings = oAuth2Settings;
            var newHttpBehaviour = httpBehaviour.ShallowClone();
            // Remove the OnHttpMessageHandlerCreated
            newHttpBehaviour.OnHttpMessageHandlerCreated = null;
            // Use it for internal communication
            _httpBehaviour = newHttpBehaviour;
        }

        /// <summary>
        ///     Register your special OAuth handler for the AuthorizeMode here
        ///     Default the AuthorizeModes.LocalServer is registered.
        ///     Your implementation is a function which returns a Task with a IDictionary string,string.
        ///     It receives the OAuth2Settings and a CancellationToken.
        ///     The return value should be that which the OAuth server gives as return values, no processing.
        /// </summary>
        public static IDictionary<AuthorizeModes, IOAuthCodeReceiver> CodeReceivers { get; } = new Dictionary<AuthorizeModes, IOAuthCodeReceiver>();

        /// <summary>
        ///     Authenticate by using the mode specified in the settings
        /// </summary>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>false if it was canceled, true if it worked, exception if not</returns>
        private async Task<bool> AuthenticateAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            IOAuthCodeReceiver codeReceiver;

            if (!CodeReceivers.TryGetValue(_oAuth2Settings.AuthorizeMode, out codeReceiver))
            {
                throw new NotImplementedException($"Authorize mode '{_oAuth2Settings.AuthorizeMode}' is not implemented/registered.");
            }

            var result = await codeReceiver.ReceiveCodeAsync(_oAuth2Settings.AuthorizeMode, _oAuth2Settings, cancellationToken).ConfigureAwait(false);
            if (result == null)
            {
                return false;
            }

            string error;
            if (result.TryGetValue(OAuth2Fields.Error.EnumValueOf(), out error))
            {
                string errorDescription;
                if (result.TryGetValue(OAuth2Fields.ErrorDescription.EnumValueOf(), out errorDescription))
                {
                    throw new Exception(errorDescription);
                }
                if ("access_denied" == error)
                {
                    throw new UnauthorizedAccessException("Access denied");
                }
                throw new Exception(error);
            }
            string code;
            if (result.TryGetValue(OAuth2Fields.Code.EnumValueOf(), out code) && !string.IsNullOrEmpty(code))
            {
                _oAuth2Settings.Code = code;
                Log.Debug().WriteLine("Exchangeing code for an access or refresh token.");
                await GenerateRefreshTokenAsync(cancellationToken).ConfigureAwait(false);
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Check and authenticate or refresh tokens
        /// </summary>
        /// <param name="cancellationToken">CancellationToken</param>
        private async Task CheckAndAuthenticateOrRefreshAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            _httpBehaviour.MakeCurrent();

            Log.Debug().WriteLine("Checking authentication.");
            // Get Refresh / Access token
            if (string.IsNullOrEmpty(_oAuth2Settings.Token.OAuth2AccessToken))
            {
                Log.Debug().WriteLine("No token, performing an Authentication");
                if (!await AuthenticateAsync(cancellationToken).ConfigureAwait(false))
                {
                    throw new Exception("Authentication cancelled");
                }
            }
            if (_oAuth2Settings.IsAccessTokenExpired)
            {
                Log.Debug().WriteLine("Access-token expired, refreshing token");
                await GenerateAccessTokenAsync(cancellationToken).ConfigureAwait(false);
                // Get Refresh / Access token
                if (string.IsNullOrEmpty(_oAuth2Settings.Token.OAuth2RefreshToken))
                {
                    if (!await AuthenticateAsync(cancellationToken).ConfigureAwait(false))
                    {
                        throw new Exception("Authentication cancelled");
                    }
                    await GenerateAccessTokenAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            if (_oAuth2Settings.IsAccessTokenExpired)
            {
                throw new Exception("Authentication failed");
            }
        }

        /// <summary>
        ///     Step 3:
        ///     Go out and retrieve a new access token via refresh-token
        ///     Will upate the access token, refresh token, expire date
        /// </summary>
        /// <param name="cancellationToken">CancellationToken</param>
        private async Task GenerateAccessTokenAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            Log.Debug().WriteLine("Generating a access token.");
            var data = new Dictionary<string, string>
            {
                {OAuth2Fields.RefreshToken.EnumValueOf(), _oAuth2Settings.Token.OAuth2RefreshToken},
                {OAuth2Fields.ClientId.EnumValueOf(), _oAuth2Settings.ClientId},
                {OAuth2Fields.ClientSecret.EnumValueOf(), _oAuth2Settings.ClientSecret},
                {OAuth2Fields.GrantType.EnumValueOf(), GrantTypes.RefreshToken.EnumValueOf()}
            };
            foreach (var key in _oAuth2Settings.AdditionalAttributes.Keys)
            {
                if (data.ContainsKey(key))
                {
                    data[key] = _oAuth2Settings.AdditionalAttributes[key];
                }
                else
                {
                    data.Add(key, _oAuth2Settings.AdditionalAttributes[key]);
                }
            }

            var accessTokenResult = await _oAuth2Settings.TokenUrl.PostAsync<OAuth2TokenResponse>(data, cancellationToken).ConfigureAwait(false);

            if (accessTokenResult.HasError)
            {
                if (accessTokenResult.IsInvalidGrant)
                {
                    // Refresh token has also expired, we need a new one!
                    _oAuth2Settings.Token.OAuth2RefreshToken = null;
                    _oAuth2Settings.Token.OAuth2AccessToken = null;
                    _oAuth2Settings.Token.OAuth2AccessTokenExpires = DateTimeOffset.MinValue;
                    _oAuth2Settings.Code = null;
                }
                else
                {
                    if (!string.IsNullOrEmpty(accessTokenResult.ErrorDescription))
                    {
                        throw new Exception($"{accessTokenResult.Error} - {accessTokenResult.ErrorDescription}");
                    }
                    throw new Exception(accessTokenResult.Error);
                }
            }
            else
            {
                // gives as described here: https://developers.google.com/identity/protocols/OAuth2InstalledApp
                //  "access_token":"1/fFAGRNJru1FTz70BzhT3Zg",
                //	"expires_in":3920,
                //	"token_type":"Bearer"
                _oAuth2Settings.Token.OAuth2AccessToken = accessTokenResult.AccessToken;
                if (!string.IsNullOrEmpty(accessTokenResult.RefreshToken))
                {
                    // Refresh the refresh token :)
                    _oAuth2Settings.Token.OAuth2RefreshToken = accessTokenResult.RefreshToken;
                }
                if (accessTokenResult.ExpiresInSeconds > 0)
                {
                    _oAuth2Settings.Token.OAuth2AccessTokenExpires = accessTokenResult.Expires;
                }
            }
        }

        /// <summary>
        ///     Step 2: Generate an OAuth 2 AccessToken / RefreshToken
        /// </summary>
        /// <param name="cancellationToken">CancellationToken</param>
        private async Task GenerateRefreshTokenAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            Log.Debug().WriteLine("Generating a refresh token.");
            var data = new Dictionary<string, string>
            {
                {OAuth2Fields.Code.EnumValueOf(), _oAuth2Settings.Code},
                {OAuth2Fields.ClientId.EnumValueOf(), _oAuth2Settings.ClientId},
                {OAuth2Fields.RedirectUri.EnumValueOf(), _oAuth2Settings.RedirectUrl},
                {OAuth2Fields.ClientSecret.EnumValueOf(), _oAuth2Settings.ClientSecret},
                {OAuth2Fields.GrantType.EnumValueOf(), GrantTypes.AuthorizationCode.EnumValueOf()}
            };
            foreach (var key in _oAuth2Settings.AdditionalAttributes.Keys)
            {
                if (data.ContainsKey(key))
                {
                    data[key] = _oAuth2Settings.AdditionalAttributes[key];
                }
                else
                {
                    data.Add(key, _oAuth2Settings.AdditionalAttributes[key]);
                }
            }

            var normalHttpBehaviour = _httpBehaviour.ShallowClone();
            normalHttpBehaviour.OnHttpMessageHandlerCreated = null;
            normalHttpBehaviour.MakeCurrent();

            var refreshTokenResult = await _oAuth2Settings.TokenUrl.PostAsync<OAuth2TokenResponse>(data, cancellationToken).ConfigureAwait(false);
            if (refreshTokenResult.HasError)
            {
                if (!string.IsNullOrEmpty(refreshTokenResult.ErrorDescription))
                {
                    throw new Exception($"{refreshTokenResult.Error} - {refreshTokenResult.ErrorDescription}");
                }
                throw new Exception(refreshTokenResult.Error);
            }
            // gives as described here: https://developers.google.com/identity/protocols/OAuth2InstalledApp
            //  "access_token":"1/fFAGRNJru1FTz70BzhT3Zg",
            //	"expires_in":3920,
            //	"token_type":"Bearer",
            //	"refresh_token":"1/xEoDL4iW3cxlI7yDbSRFYNG01kVKM2C-259HOF2aQbI"
            _oAuth2Settings.Token.OAuth2AccessToken = refreshTokenResult.AccessToken;
            _oAuth2Settings.Token.OAuth2RefreshToken = refreshTokenResult.RefreshToken;

            if (refreshTokenResult.ExpiresInSeconds > 0)
            {
                _oAuth2Settings.Token.OAuth2AccessTokenExpires = refreshTokenResult.Expires;
            }
            _oAuth2Settings.Code = null;
        }

        /// <summary>
        ///     Check the HttpRequestMessage if all OAuth setting are there, if not make this available.
        /// </summary>
        /// <param name="httpRequestMessage">HttpRequestMessage</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>HttpResponseMessage</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
        {
            // TODO: The caller of this DelegatingHandler override is passing a CancellationToken which cancels after a certain time, what to do with this?

            // Make sure the first call does the authorization, and all others wait for it.
            await _oAuth2Settings.Lock.WaitAsync().ConfigureAwait(false);
            try
            {
                await CheckAndAuthenticateOrRefreshAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _oAuth2Settings.Lock.Release();
            }
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _oAuth2Settings.Token.OAuth2AccessToken);
            Log.Verbose().WriteLine("Continueing original request to {0}", httpRequestMessage.RequestUri);
            var result = await base.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);
            return result;
        }
    }
}