//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2015-2016 Dapplo
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
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.Factory;
using Dapplo.LogFacade;
using Dapplo.Utils.Extensions;

#endregion

namespace Dapplo.HttpExtensions.OAuth
{
	/// <summary>
	///     This DelegatingHandler handles the OAuth specific stuff and delegates the "final" SendAsync to the InnerHandler
	/// </summary>
	public class OAuth1HttpMessageHandler : DelegatingHandler
	{
		private static readonly LogSource Log = new LogSource();
		private static readonly Random RandomForNonce = new Random();
		private readonly OAuth1HttpBehaviour _oAuth1HttpBehaviour;
		private readonly OAuth1Settings _oAuth1Settings;

		/// <summary>
		///     Add the local server handler.
		/// </summary>
		static OAuth1HttpMessageHandler()
		{
			CodeReceivers.Add(
				AuthorizeModes.LocalhostServer,
				new LocalhostCodeReceiver()
				);
			CodeReceivers.Add(
				AuthorizeModes.TestPassThrough,
				new PassThroughCodeReceiver()
				);
#if !_PCL_
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
		///     Create a HttpMessageHandler which handles the OAuth communication for you
		/// </summary>
		/// <param name="oAuth1Settings">OAuth1Settings</param>
		/// <param name="oAuth1HttpBehaviour">OAuth1HttpBehaviour</param>
		/// <param name="innerHandler">HttpMessageHandler</param>
		public OAuth1HttpMessageHandler(OAuth1Settings oAuth1Settings, OAuth1HttpBehaviour oAuth1HttpBehaviour, HttpMessageHandler innerHandler) : base(innerHandler)
		{
			if (oAuth1Settings.ClientId == null)
			{
				throw new ArgumentNullException(nameof(oAuth1Settings.ClientId));
			}
			if (oAuth1Settings.ClientSecret == null)
			{
				throw new ArgumentNullException(nameof(oAuth1Settings.ClientSecret));
			}
			if (oAuth1Settings.TokenUrl == null)
			{
				throw new ArgumentNullException(nameof(oAuth1Settings.TokenUrl));
			}

			_oAuth1Settings = oAuth1Settings;

			var newHttpBehaviour = oAuth1HttpBehaviour.ShallowClone();
			// Remove the OnHttpMessageHandlerCreated
			newHttpBehaviour.OnHttpMessageHandlerCreated = null;
			// Use it for internal communication
			_oAuth1HttpBehaviour = (OAuth1HttpBehaviour)newHttpBehaviour;
		}

		/// <summary>
		///     Register your special OAuth handler for the AuthorizeMode here
		///     Default the AuthorizeModes.LocalServer is registered.
		///     Your implementation is a function which returns a Task with a IDictionary string,string.
		///     It receives the OAuthSettings and a CancellationToken.
		///     The return value should be that which the OAuth server gives as return values, no processing.
		/// </summary>
		public static IDictionary<AuthorizeModes, IOAuthCodeReceiver> CodeReceivers { get; } = new Dictionary<AuthorizeModes, IOAuthCodeReceiver>();

		/// <summary>
		///     Helper function to compute a hash value
		/// </summary>
		/// <param name="hashAlgorithm">
		///     The hashing algorithm used. If that algorithm needs some initialization, like HMAC and its
		///     derivatives, they should be initialized prior to passing it to this function
		/// </param>
		/// <param name="data">The data to hash</param>
		/// <returns>a Base64 string of the hash value</returns>
		public static string ComputeHash(HashAlgorithm hashAlgorithm, string data)
		{
			if (hashAlgorithm == null)
			{
				throw new ArgumentNullException(nameof(hashAlgorithm));
			}

			if (string.IsNullOrEmpty(data))
			{
				throw new ArgumentNullException(nameof(data));
			}

			var dataBuffer = Encoding.UTF8.GetBytes(data);
			var hashBytes = hashAlgorithm.ComputeHash(dataBuffer);

			return Convert.ToBase64String(hashBytes);
		}

		/// <summary>
		///     Generate a nonce
		/// </summary>
		/// <returns></returns>
		public static string GenerateNonce()
		{
			// Just a simple implementation of a random number between 123400 and 9999999
			return RandomForNonce.Next(123400, 9999999).ToString();
		}

		/// <summary>
		///     Generate the normalized paramter string
		/// </summary>
		/// <param name="queryParameters">the list of query parameters</param>
		/// <returns>a string with the normalized query parameters</returns>
		private static string GenerateNormalizedParametersString<T>(IDictionary<string, T> queryParameters)
		{
			if (queryParameters == null || queryParameters.Count == 0)
			{
				return string.Empty;
			}

			queryParameters = new SortedDictionary<string, T>(queryParameters);

			var sb = new StringBuilder();
			foreach (var key in queryParameters.Keys)
			{
				if (queryParameters[key] is string)
				{
					sb.AppendFormat(CultureInfo.InvariantCulture, "{0}={1}&", key, Uri.EscapeDataString($"{queryParameters[key]}"));
				}
			}
			sb.Remove(sb.Length - 1, 1);

			return sb.ToString();
		}

		/// <summary>
		///     Generate the timestamp for the signature
		/// </summary>
		/// <returns></returns>
		public static string GenerateTimeStamp()
		{
			// Default implementation of UNIX time of the current UTC time
			var timespan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return Convert.ToInt64(timespan.TotalSeconds).ToString();
		}

		/// <summary>
		///     Get the access token
		/// </summary>
		/// <param name="cancellationToken">CancellationToken</param>
		/// <returns>The access token.</returns>
		private async Task GetAccessTokenAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(_oAuth1Settings.AuthorizeToken))
			{
				throw new ArgumentNullException(nameof(_oAuth1Settings.AuthorizeToken), "The authorize token is not set");
			}
			if (_oAuth1Settings.CheckVerifier && string.IsNullOrEmpty(_oAuth1Settings.Token.OAuthTokenVerifier))
			{
				throw new ArgumentNullException(nameof(_oAuth1Settings.Token.OAuthTokenVerifier), "The verifier is not set");
			}

			_oAuth1HttpBehaviour.MakeCurrent();
			// Create a HttpRequestMessage for the Token-Url
			var httpRequestMessage = HttpRequestMessageFactory.Create(_oAuth1Settings.AccessTokenMethod, _oAuth1Settings.AccessTokenUrl);
			// sign it
			Sign(httpRequestMessage);

			// Use it to get the response
			var response = await httpRequestMessage.SendAsync<string>(cancellationToken).ConfigureAwait(false);

			if (!string.IsNullOrEmpty(response))
			{
				Log.Verbose().WriteLine("Access token response: {0}", response);
				var resultParameters = UriParseExtensions.QueryStringToDictionary(response);
				string tokenValue;
				if (resultParameters.TryGetValue(OAuth1Parameters.Token.EnumValueOf(), out tokenValue))
				{
					_oAuth1Settings.Token.OAuthToken = tokenValue;
					resultParameters.Remove(OAuth1Parameters.Token.EnumValueOf());
				}
				string secretValue;
				if (resultParameters.TryGetValue(OAuth1Parameters.TokenSecret.EnumValueOf(), out secretValue))
				{
					_oAuth1Settings.Token.OAuthTokenSecret = secretValue;
					resultParameters.Remove(OAuth1Parameters.TokenSecret.EnumValueOf());
				}
				// Process the rest, if someone registed, some services return more values
				_oAuth1HttpBehaviour?.OnAccessTokenValues?.Invoke(resultParameters);
			}
		}

		/// <summary>
		///     Authorize the token by showing the authorization uri of the oauth service
		/// </summary>
		/// <param name="cancellationToken">CancellationToken</param>
		private async Task GetAuthorizeTokenAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(_oAuth1Settings.RequestToken))
			{
				throw new ArgumentNullException(nameof(_oAuth1Settings.RequestToken), "The request token is not set");
			}
			IOAuthCodeReceiver codeReceiver;

			if (!CodeReceivers.TryGetValue(_oAuth1Settings.AuthorizeMode, out codeReceiver))
			{
				throw new NotImplementedException($"Authorize mode '{_oAuth1Settings.AuthorizeMode}' is not implemented/registered.");
			}
			Log.Debug().WriteLine("Calling code receiver : {0}", _oAuth1Settings.AuthorizeMode);
			var result = await codeReceiver.ReceiveCodeAsync(_oAuth1Settings.AuthorizeMode, _oAuth1Settings, cancellationToken).ConfigureAwait(false);

			if (result != null)
			{
				string tokenValue;
				if (result.TryGetValue(OAuth1Parameters.Token.EnumValueOf(), out tokenValue))
				{
					_oAuth1Settings.AuthorizeToken = tokenValue;
				}
				string verifierValue;
				if (result.TryGetValue(OAuth1Parameters.Verifier.EnumValueOf(), out verifierValue))
				{
					_oAuth1Settings.Token.OAuthTokenVerifier = verifierValue;
				}
			}
			if (_oAuth1Settings.CheckVerifier)
			{
				if (!string.IsNullOrEmpty(_oAuth1Settings.Token.OAuthTokenVerifier))
				{
					throw new Exception("Token verifier is not set, while CheckVerifier is set to true");
				}
			}
		}

		/// <summary>
		///     Get the request token using the consumer id and secret.  Also initializes token secret
		/// </summary>
		/// <param name="cancellationToken">CancellationToken</param>
		private async Task GetRequestTokenAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			_oAuth1HttpBehaviour.MakeCurrent();
			// Create a HttpRequestMessage for the Token-Url
			var httpRequestMessage = HttpRequestMessageFactory.Create(_oAuth1Settings.TokenMethod, _oAuth1Settings.TokenUrl);
			// sign it
			Sign(httpRequestMessage);
			// Use it to get the response
			var response = await httpRequestMessage.SendAsync<string>(cancellationToken).ConfigureAwait(false);

			if (!string.IsNullOrEmpty(response))
			{
				Log.Verbose().WriteLine("Request token response: {0}", response);
				var resultParameters = UriParseExtensions.QueryStringToDictionary(response);
				string tokenValue;
				if (resultParameters.TryGetValue(OAuth1Parameters.Token.EnumValueOf(), out tokenValue))
				{
					Log.Verbose().WriteLine("Storing token {0}", tokenValue);
					_oAuth1Settings.RequestToken = tokenValue;
				}
				string tokenSecretValue;
				if (resultParameters.TryGetValue(OAuth1Parameters.TokenSecret.EnumValueOf(), out tokenSecretValue))
				{
					Log.Verbose().WriteLine("Storing token secret {0}", tokenSecretValue);
					_oAuth1Settings.RequestTokenSecret = tokenSecretValue;
				}
			}
		}

		/// <summary>
		///     Check the HttpRequestMessage if all OAuth setting are there, if not make this available.
		/// </summary>
		/// <param name="httpRequestMessage">HttpRequestMessage</param>
		/// <param name="cancellationToken">CancellationToken</param>
		/// <returns>HttpResponseMessage</returns>
		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
		{
			// Make sure the first call does the authorization, and all others wait for it.
			using (await _oAuth1Settings.Lock.LockAsync().ConfigureAwait(false))
			{
				if (string.IsNullOrEmpty(_oAuth1Settings.Token.OAuthToken))
				{
					await GetRequestTokenAsync(cancellationToken).ConfigureAwait(false);
					await GetAuthorizeTokenAsync(cancellationToken).ConfigureAwait(false);
					await GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
				}
				else
				{
					Log.Verbose().WriteLine("Continueing, already a token available.");
				}
			}

			if (string.IsNullOrEmpty(_oAuth1Settings.Token.OAuthToken))
			{
				throw new AuthenticationException("Not possible to authenticate the OAuth request.");
			}

			// sign the HttpRequestMessage
			Sign(httpRequestMessage);

			_oAuth1HttpBehaviour?.BeforeSend?.Invoke(httpRequestMessage);

			return await base.SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		///     Create a signature for the supplied HttpRequestMessage
		///     And additionally a signature is added to the parameters
		/// </summary>
		/// <param name="httpRequestMessage">HttpRequestMessage with method and Uri</param>
		/// <returns>HttpRequestMessage for fluent usage</returns>
		private void Sign(HttpRequestMessage httpRequestMessage)
		{
			// TODO: Add _oAuth1Settings.AdditionalAttributes!
			var parameters = new Dictionary<string, object>(httpRequestMessage.Properties);
			// Build the signature base
			var signatureBase = new StringBuilder();

			// Add Method to signature base
			signatureBase.Append(httpRequestMessage.Method).Append("&");

			// Add normalized URL, most of it is already normalized by using AbsoluteUri, but we need the Uri without Query and Fragment
			var normalizedUri = new UriBuilder(httpRequestMessage.RequestUri)
			{
				Query = "",
				Fragment = ""
			};
			signatureBase.Append(Uri.EscapeDataString(normalizedUri.Uri.AbsoluteUri)).Append("&");

			// Add normalized parameters
			parameters.Add(OAuth1Parameters.Version.EnumValueOf(), "1.0");
			parameters.Add(OAuth1Parameters.Nonce.EnumValueOf(), GenerateNonce());
			parameters.Add(OAuth1Parameters.Timestamp.EnumValueOf(), GenerateTimeStamp());

			parameters.Add(OAuth1Parameters.SignatureMethod.EnumValueOf(), _oAuth1Settings.SignatureType.EnumValueOf());

			parameters.Add(OAuth1Parameters.ConsumerKey.EnumValueOf(), _oAuth1Settings.ClientId);

			if (_oAuth1Settings.RedirectUrl != null && _oAuth1Settings.TokenUrl != null && httpRequestMessage.RequestUri.Equals(_oAuth1Settings.TokenUrl))
			{
				parameters.Add(OAuth1Parameters.Callback.EnumValueOf(), _oAuth1Settings.RedirectUrl);
			}
			if (!string.IsNullOrEmpty(_oAuth1Settings.Token.OAuthTokenVerifier))
			{
				parameters.Add(OAuth1Parameters.Verifier.EnumValueOf(), _oAuth1Settings.Token.OAuthTokenVerifier);
			}
			// ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
			if (!string.IsNullOrEmpty(_oAuth1Settings.Token.OAuthToken))
			{
				parameters.Add(OAuth1Parameters.Token.EnumValueOf(), _oAuth1Settings.Token.OAuthToken);
			}
			else
			{
				parameters.Add(OAuth1Parameters.Token.EnumValueOf(), _oAuth1Settings.AuthorizeToken);
			}
			// Create a copy of the parameters, so we add the query parameters as signle parameters to the signature base
			var signatureParameters = new Dictionary<string, object>(parameters);
			foreach (var valuePair in httpRequestMessage.RequestUri.QueryToKeyValuePairs())
			{
				if (!signatureParameters.ContainsKey(valuePair.Key))
				{
					signatureParameters.Add(valuePair.Key, valuePair.Value);
				}
			}
			signatureBase.Append(Uri.EscapeDataString(GenerateNormalizedParametersString(signatureParameters)));
			Log.Verbose().WriteLine("Signature base: {0}", signatureBase);
			var secret = string.IsNullOrEmpty(_oAuth1Settings.Token.OAuthTokenSecret)
				? string.IsNullOrEmpty(_oAuth1Settings.RequestTokenSecret) ? string.Empty : _oAuth1Settings.RequestTokenSecret
				: _oAuth1Settings.Token.OAuthTokenSecret;
			var key = string.Format(CultureInfo.InvariantCulture, "{0}&{1}", Uri.EscapeDataString(_oAuth1Settings.ClientSecret), Uri.EscapeDataString(secret));
			Log.Verbose().WriteLine("Signing with key {0}", key);
			switch (_oAuth1Settings.SignatureType)
			{
				case OAuth1SignatureTypes.RsaSha1:
					// Code comes from here: http://www.dotnetfunda.com/articles/article1932-rest-service-call-using-oauth-10-authorization-with-rsa-sha1.aspx
					// Read the .P12 file to read Private/Public key Certificate
					var certFilePath = _oAuth1Settings.ClientId; // The .P12 certificate file path Example: "C:/mycertificate/MCOpenAPI.p12
					var password = _oAuth1Settings.ClientSecret; // password to read certificate .p12 file
					// Read the Certification from .P12 file.
					var cert = new X509Certificate2(certFilePath, password);
					// Retrieve the Private key from Certificate.
					var rsaCrypt = (RSACryptoServiceProvider) cert.PrivateKey;
					// Create a RSA-SHA1 Hash object
					using (var shaHashObject = new SHA1Managed())
					{
						// Create Byte Array of Signature base string
						var data = Encoding.ASCII.GetBytes(signatureBase.ToString());
						// Create Hashmap of Signature base string
						var hash = shaHashObject.ComputeHash(data);
						// Create Sign Hash of base string
						// NOTE - 'SignHash' gives correct data. Don't use SignData method
						var rsaSignature = rsaCrypt.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
						// Convert to Base64 string
						var base64String = Convert.ToBase64String(rsaSignature);
						// Return the Encoded UTF8 string
						parameters.Add(OAuth1Parameters.Signature.EnumValueOf(), Uri.EscapeDataString(base64String));
					}
					break;
				case OAuth1SignatureTypes.PlainText:
					parameters.Add(OAuth1Parameters.Signature.EnumValueOf(), key);
					break;
				default:
					// Generate Signature and add it to the parameters
					var hmacsha1 = new HMACSHA1 {Key = Encoding.UTF8.GetBytes(key)};
					var signature = ComputeHash(hmacsha1, signatureBase.ToString());
					parameters.Add(OAuth1Parameters.Signature.EnumValueOf(), signature);
					break;
			}

			var authorizationHeaderValues = string.Join(", ",
				parameters.Where(x => x.Key.StartsWith("oauth_") && x.Value is string)
					.OrderBy(x => x.Key)
					.Select(x => $"{x.Key}=\"{Uri.EscapeDataString((string) x.Value)}\""));
			// Add the OAuth to the headers
			httpRequestMessage.SetAuthorization("OAuth", authorizationHeaderValues);

			if (httpRequestMessage.Method == HttpMethod.Post && httpRequestMessage.Properties.Count > 0)
			{
				var multipartFormDataContent = new MultipartFormDataContent();
				foreach (var propertyName in httpRequestMessage.Properties.Keys)
				{
					var requestObject = httpRequestMessage.Properties[propertyName];
					var formattedKey = $"\"{propertyName}\"";
					if (requestObject is HttpContent)
					{
						multipartFormDataContent.Add(requestObject as HttpContent, formattedKey);
					}
					else
					{
						multipartFormDataContent.Add(new StringContent(requestObject as string), formattedKey);
					}
				}
				// It's possible that there was no content posted, so check before adding.
				if (httpRequestMessage.Content != null)
				{
					multipartFormDataContent.Add(httpRequestMessage.Content);
				}
				httpRequestMessage.Content = multipartFormDataContent;
			}
		}
	}
}