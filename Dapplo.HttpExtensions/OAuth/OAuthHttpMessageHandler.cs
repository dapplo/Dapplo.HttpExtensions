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

using Dapplo.HttpExtensions.Internal;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Dapplo.HttpExtensions.OAuth
{
	/// <summary>
	/// This DelegatingHandler handles the OAuth2 specific stuff and delegates the "final" SendAsync to the InnerHandler
	/// </summary>
	public class OAuthHttpMessageHandler : DelegatingHandler
	{
		private static readonly LogContext Log = new LogContext();

		private const string RefreshToken = "refresh_token";
		//private const string AccessToken = "access_token";
		private const string Code = "code";
		private const string ClientId = "client_id";
		private const string ClientSecret = "client_secret";
		private const string GrantType = "grant_type";
		private const string AuthorizationCode = "authorization_code";
		private const string RedirectUri = "redirect_uri";
		private const string ExpiresIn = "expires_in";

		/// <summary>
		/// Register your special OAuth handler for the AuthorizeMode here
		/// </summary>
		public static IDictionary<AuthorizeModes, Func<OAuth2Settings, CancellationToken, Task<IDictionary<string,string>>>> AuthorizeHandlers
		{
			get;
		} = new Dictionary<AuthorizeModes, Func<OAuth2Settings, CancellationToken, Task<IDictionary<string,string>>>>();

		static OAuthHttpMessageHandler()
		{
			AuthorizeHandlers.Add(AuthorizeModes.LocalServer, async(oAuth2Settings, cancellationToken) =>
			{
				return await new LocalServerCodeReceiver().ReceiveCodeAsync(oAuth2Settings, cancellationToken);
			});
		}

		private readonly OAuth2Settings _oAuth2Settings;
		private readonly IHttpBehaviour _httpBehaviour;

		public OAuthHttpMessageHandler(OAuth2Settings oAuth2Settings, IHttpBehaviour httpBehaviour, HttpMessageHandler innerHandler) : base(innerHandler)
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
			if (oAuth2Settings.TokenUrl.Scheme != "https")
			{
				throw new ArgumentException("Only https is allowed.", nameof(oAuth2Settings.TokenUrl));
			}

			_oAuth2Settings = oAuth2Settings;
			_httpBehaviour = httpBehaviour.Clone() as IHttpBehaviour;
			// Remove the 
			_httpBehaviour.OnHttpMessageHandlerCreated = null;
		}

		/// <summary>
		/// Authenticate by using the mode specified in the settings
		/// </summary>
		/// <param name="settings">OAuth2Settings</param>
		/// <param name="cancellationToken">CancellationToken</param>
		/// <returns>false if it was canceled, true if it worked, exception if not</returns>
		private async Task<bool> AuthenticateAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			Func<OAuth2Settings, CancellationToken, Task<IDictionary<string, string>>> authorizeFunc;
			if (!AuthorizeHandlers.TryGetValue(_oAuth2Settings.AuthorizeMode, out authorizeFunc)) {
				throw new NotImplementedException($"Authorize mode '{_oAuth2Settings.AuthorizeMode}' is not specified.");
			}
			var result = await authorizeFunc(_oAuth2Settings, cancellationToken);

			string error;
			if (result.TryGetValue("error", out error))
			{
				string errorDescription;
				if (result.TryGetValue("error_description", out errorDescription))
				{
					throw new ApplicationException(errorDescription);
				}
				if ("access_denied" == error)
				{
					throw new UnauthorizedAccessException("Access denied");
				}
				throw new ApplicationException(error);
			}
			string code;
			if (result.TryGetValue(Code, out code) && !string.IsNullOrEmpty(code))
			{
				_oAuth2Settings.Code = code;
				Log.Debug().Write("Retrieving a first time refresh token.");
				await GenerateRefreshTokenAsync(cancellationToken);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Step 3:
		/// Go out and retrieve a new access token via refresh-token
		/// Will upate the access token, refresh token, expire date
		/// </summary>
		/// <param name="cancellationToken">CancellationToken</param>
		private async Task GenerateAccessTokenAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			Log.Debug().Write("Generating a access token.");
			var data = new Dictionary<string, string>();
			data.Add(RefreshToken, _oAuth2Settings.RefreshToken);
			data.Add(ClientId, _oAuth2Settings.ClientId);
			data.Add(ClientSecret, _oAuth2Settings.ClientSecret);
			data.Add(GrantType, RefreshToken);
			foreach (string key in _oAuth2Settings.AdditionalAttributes.Keys)
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

			var accessTokenResult = await _oAuth2Settings.TokenUrl.PostAsync<OAuthTokenResponse, IDictionary<string, string>>(data, null, cancellationToken).ConfigureAwait(false);

			if (accessTokenResult.HasError)
			{
				if (accessTokenResult.IsInvalidGrant)
				{
					// Refresh token has also expired, we need a new one!
					_oAuth2Settings.RefreshToken = null;
					_oAuth2Settings.AccessToken = null;
					_oAuth2Settings.AccessTokenExpires = DateTimeOffset.MinValue;
					_oAuth2Settings.Code = null;
				}
				else
				{
					if (!string.IsNullOrEmpty(accessTokenResult.ErrorDescription))
					{
						throw new ApplicationException($"{accessTokenResult.Error} - {accessTokenResult.ErrorDescription}");
					}
					throw new ApplicationException(accessTokenResult.Error);
				}
			}
			else
			{
				// gives as described here: https://developers.google.com/identity/protocols/OAuth2InstalledApp
				//  "access_token":"1/fFAGRNJru1FTz70BzhT3Zg",
				//	"expires_in":3920,
				//	"token_type":"Bearer"
				_oAuth2Settings.AccessToken = accessTokenResult.AccessToken;
				if (!string.IsNullOrEmpty(accessTokenResult.RefreshToken))
				{
					// Refresh the refresh token :)
					_oAuth2Settings.RefreshToken = accessTokenResult.RefreshToken;
				}
				if (accessTokenResult.ExpiresInSeconds > 0)
				{
					_oAuth2Settings.AccessTokenExpires = accessTokenResult.Expires;
				}
			}
		}

		/// <summary>
		/// Step 2: Generate an OAuth 2 AccessToken / RefreshToken
		/// </summary>
		/// <param name="cancellationToken">CancellationToken</param>
		private async Task GenerateRefreshTokenAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			Log.Debug().Write("Generating a refresh token.");
			var data = new Dictionary<string, string>();
			// Use the returned code to get a refresh code
			data.Add(Code, _oAuth2Settings.Code);
			data.Add(ClientId, _oAuth2Settings.ClientId);
			data.Add(RedirectUri, _oAuth2Settings.RedirectUrl);
			data.Add(ClientSecret, _oAuth2Settings.ClientSecret);
			data.Add(GrantType, AuthorizationCode);
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
			var normalHttpBehaviour = _httpBehaviour.Clone() as IHttpBehaviour;
			normalHttpBehaviour.OnHttpMessageHandlerCreated = null;

			var refreshTokenResult = await _oAuth2Settings.TokenUrl.PostAsync<OAuthTokenResponse, IDictionary<string, string>>(data, normalHttpBehaviour, cancellationToken).ConfigureAwait(false);
			if (refreshTokenResult.HasError)
			{
				if (!string.IsNullOrEmpty(refreshTokenResult.ErrorDescription))
				{
					throw new ApplicationException($"{refreshTokenResult.Error} - {refreshTokenResult.ErrorDescription}");
				}
				throw new ApplicationException(refreshTokenResult.Error);
			}
			// gives as described here: https://developers.google.com/identity/protocols/OAuth2InstalledApp
			//  "access_token":"1/fFAGRNJru1FTz70BzhT3Zg",
			//	"expires_in":3920,
			//	"token_type":"Bearer",
			//	"refresh_token":"1/xEoDL4iW3cxlI7yDbSRFYNG01kVKM2C-259HOF2aQbI"
			_oAuth2Settings.AccessToken = refreshTokenResult.AccessToken;
			_oAuth2Settings.RefreshToken = refreshTokenResult.RefreshToken;

			if (refreshTokenResult.ExpiresInSeconds > 0)
			{
				_oAuth2Settings.AccessTokenExpires = refreshTokenResult.Expires;
			}
			_oAuth2Settings.Code = null;
		}

		/// <summary>
		/// Check and authenticate or refresh tokens 
		/// </summary>
		/// <param name="settings">OAuth2Settings</param>
		/// <param name="token"></param>
		private async Task CheckAndAuthenticateOrRefreshAsync(CancellationToken token = default(CancellationToken))
		{
			Log.Debug().Write("Checking authentication.");
			// Get Refresh / Access token
			if (string.IsNullOrEmpty(_oAuth2Settings.RefreshToken))
			{
				Log.Debug().Write("No refresh-token, performing an Authentication");
				if (!await AuthenticateAsync(token).ConfigureAwait(false))
				{
					throw new ApplicationException("Authentication cancelled");
				}
			}
			if (_oAuth2Settings.IsAccessTokenExpired)
			{
				Log.Debug().Write("No access-token expired, generating an access token");
				await GenerateAccessTokenAsync(token).ConfigureAwait(false);
				// Get Refresh / Access token
				if (string.IsNullOrEmpty(_oAuth2Settings.RefreshToken))
				{
					if (!await AuthenticateAsync(token).ConfigureAwait(false))
					{
						throw new ApplicationException("Authentication cancelled");
					}
					await GenerateAccessTokenAsync(token).ConfigureAwait(false);
				}
			}
			if (_oAuth2Settings.IsAccessTokenExpired)
			{
				throw new ApplicationException("Authentication failed");
			}
		}

		/// <summary>
		/// Check the HttpRequestMessage if all OAuth setting are there, if not make this available.
		/// </summary>
		/// <param name="httpRequestMessage">HttpRequestMessage</param>
		/// <param name="cancellationToken">CancellationToken</param>
		/// <returns></returns>
		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken = default(CancellationToken))
		{
			Log.Debug().Write("Before call to {0}", httpRequestMessage.RequestUri);
			await CheckAndAuthenticateOrRefreshAsync().ConfigureAwait(false);
			httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _oAuth2Settings.AccessToken);
			var result = await base.SendAsync(httpRequestMessage, cancellationToken);
			Log.Debug().Write("After call to {0}", httpRequestMessage.RequestUri);
			return result;
		}
	}
}