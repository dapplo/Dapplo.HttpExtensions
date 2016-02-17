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

using Dapplo.HttpExtensions.Factory;
using Dapplo.LogFacade;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Dapplo.HttpExtensions.OAuth
{
	/// <summary>
	/// This DelegatingHandler handles the OAuth specific stuff and delegates the "final" SendAsync to the InnerHandler
	/// </summary>
	public class OAuthHttpMessageHandler : DelegatingHandler
	{
		private static readonly LogSource Log = new LogSource();
		private static readonly Random RandomForNonce = new Random();

		/// <summary>
		/// Register your special OAuth handler for the AuthorizeMode here
		/// Default the AuthorizeModes.LocalServer is registered.
		/// Your implementation is a function which returns a Task with a IDictionary string,string.
		/// It receives the OAuthSettings and a CancellationToken.
		/// The return value should be that which the OAuth server gives as return values, no processing.
		/// </summary>
		public static IDictionary<AuthorizeModes, IOAuthCodeReceiver> CodeReceivers
		{
			get;
		} = new Dictionary<AuthorizeModes, IOAuthCodeReceiver>();

		/// <summary>
		/// Add the local server handler.
		/// </summary>
		static OAuthHttpMessageHandler()
		{
			CodeReceivers.Add(
				AuthorizeModes.LocalhostServer,
				new LocalhostCodeReceiver()
			);
#if DESKTOP
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

		private readonly OAuthSettings _oAuthSettings;
		private readonly IHttpBehaviour _httpBehaviour;

		/// <summary>
		/// Create a HttpMessageHandler which handles the OAuth communication for you
		/// </summary>
		/// <param name="oAuthSettings">OAuthSettings</param>
		/// <param name="httpBehaviour">IHttpBehaviour</param>
		/// <param name="innerHandler">HttpMessageHandler</param>
		public OAuthHttpMessageHandler(OAuthSettings oAuthSettings, IHttpBehaviour httpBehaviour, HttpMessageHandler innerHandler) : base(innerHandler)
		{
			if (oAuthSettings.ClientId == null)
			{
				throw new ArgumentNullException(nameof(oAuthSettings.ClientId));
			}
			if (oAuthSettings.ClientSecret == null)
			{
				throw new ArgumentNullException(nameof(oAuthSettings.ClientSecret));
			}
			if (oAuthSettings.TokenUrl == null)
			{
				throw new ArgumentNullException(nameof(oAuthSettings.TokenUrl));
			}

			_oAuthSettings = oAuthSettings;
			_httpBehaviour = (IHttpBehaviour) httpBehaviour.Clone();
			// Remove the OnHttpMessageHandlerCreated
			_httpBehaviour.OnHttpMessageHandlerCreated = null;
		}

		/// <summary>
		/// Get the request token using the consumer id and secret.  Also initializes token secret
		/// </summary>
		/// <param name="cancellationToken">CancellationToken</param>
		private async Task GetRequestTokenAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var parameters = new Dictionary<string, string>();
			Sign(_oAuthSettings.TokenMethod, _oAuthSettings.TokenUrl, parameters);
			string response;
			var httpClient = HttpClientFactory.Create(_httpBehaviour, _oAuthSettings.TokenUrl);
			// Generate OAuth authorization header
			httpClient.SetAuthorization("OAuth", string.Join(", ", parameters.Where(x => x.Key.StartsWith("oauth_")).Select(x => $"{x.Key}=\"{ Uri.EscapeDataString(x.Value)}\"")));
			if (_oAuthSettings.TokenMethod == HttpMethod.Get)
			{
			   response = await httpClient.GetAsAsync<string>(_oAuthSettings.TokenUrl, _httpBehaviour, cancellationToken).ConfigureAwait(false);
			}
			else
			{
				// Post with no content (don't know why some services want this...)
				response = await httpClient.PostAsync<string, object>(_oAuthSettings.TokenUrl, null, _httpBehaviour, cancellationToken).ConfigureAwait(false);
			}
			if (!string.IsNullOrEmpty(response))
			{
				Log.Verbose().WriteLine("Request token response: {0}", response);
				var resultParameters = UriParseExtensions.QueryStringToDictionary(response);
				string value;
				if (resultParameters.TryGetValue(OAuthParameters.OauthTokenKey.EnumValueOf(), out value))
				{
					_oAuthSettings.Token.OAuthToken = value;
					_oAuthSettings.Token.OAuthTokenSecret = resultParameters[OAuthParameters.OauthTokenSecretKey.EnumValueOf()];
				}
			}
		}

		/// <summary>
		/// Authorize the token by showing the authorization uri of the oauth service
		/// </summary>
		/// <param name="cancellationToken">CancellationToken</param>
		private async Task GetAuthorizeTokenAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (string.IsNullOrEmpty(_oAuthSettings.Token.OAuthToken))
			{
				throw new ArgumentNullException("The request token is not set");
			}
			_oAuthSettings.AuthorizeFormattingParameters.Clear();
			_oAuthSettings.AuthorizeFormattingParameters.Add(_oAuthSettings.Token);
			IOAuthCodeReceiver codeReceiver;

			if (!CodeReceivers.TryGetValue(_oAuthSettings.AuthorizeMode, out codeReceiver))
			{
				throw new NotImplementedException($"Authorize mode '{_oAuthSettings.AuthorizeMode}' is not implemented/registered.");
			}
			Log.Debug().WriteLine("Calling code receiver : {0}", _oAuthSettings.AuthorizeMode);
			var result = await codeReceiver.ReceiveCodeAsync(_oAuthSettings.AuthorizeMode, _oAuthSettings, cancellationToken);

			if (result != null)
			{
				string tokenValue;
				if (result.TryGetValue(OAuthParameters.OauthTokenKey.EnumValueOf(), out tokenValue))
				{
					_oAuthSettings.Token.OAuthToken = tokenValue;
				}
				string verifierValue;
				if (result.TryGetValue(OAuthParameters.OauthVerifierKey.EnumValueOf(), out verifierValue))
				{
					_oAuthSettings.Token.OAuthTokenVerifier = verifierValue;
				}
			}
			if (_oAuthSettings.CheckVerifier)
			{
				if (!string.IsNullOrEmpty(_oAuthSettings.Token.OAuthTokenVerifier))
				{
					throw new ApplicationException("Token verifier is not set, while CheckVerifier is set to true");
				}
			}
		}

		/// <summary>
		/// OAuth sign the parameters, meaning all oauth parameters are added to the supplied dictionary.
		/// And additionally a signature is added to the parameters
		/// </summary>
		/// <param name="method">Method (POST,PUT,GET)</param>
		/// <param name="requestUri">Url to call</param>
		/// <param name="parameters">IDictionar&lt;string, string&gt;</param>
		private void Sign(HttpMethod method, Uri requestUri, IDictionary<string, string> parameters)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}
			// Build the signature base
			var signatureBase = new StringBuilder();

			// Add Method to signature base
			signatureBase.Append(method).Append("&");

			// Add normalized URL, most of it is already normalized by using AbsoluteUri, but we need the Uri without Query and Fragment
			var normalizedUri = new UriBuilder(requestUri)
			{
				Query = null,
				Fragment = null
			};
			signatureBase.Append(Uri.EscapeDataString(normalizedUri.Uri.AbsoluteUri)).Append("&");

			// Add normalized parameters
			parameters.Add(OAuthParameters.OauthVersionKey.EnumValueOf(), "1.0");
			parameters.Add(OAuthParameters.OauthNonceKey.EnumValueOf(), GenerateNonce());
			parameters.Add(OAuthParameters.OauthTimestampKey.EnumValueOf(), GenerateTimeStamp());

			parameters.Add(OAuthParameters.OauthSignatureMethodKey.EnumValueOf(), _oAuthSettings.SignatureType.EnumValueOf());

			parameters.Add(OAuthParameters.OauthConsumerKeyKey.EnumValueOf(), _oAuthSettings.ClientId);

			if (_oAuthSettings.RedirectUrl != null && _oAuthSettings.TokenUrl != null && requestUri.Equals(_oAuthSettings.TokenUrl))
			{
				parameters.Add(OAuthParameters.OauthCallbackKey.EnumValueOf(), _oAuthSettings.RedirectUrl);
			}
			if (!string.IsNullOrEmpty(_oAuthSettings.Token.OAuthTokenVerifier))
			{
				parameters.Add(OAuthParameters.OauthVerifierKey.EnumValueOf(), _oAuthSettings.Token.OAuthTokenVerifier);
			}
			if (!string.IsNullOrEmpty(_oAuthSettings.Token.OAuthToken))
			{
				parameters.Add(OAuthParameters.OauthTokenKey.EnumValueOf(), _oAuthSettings.Token.OAuthToken);
			}
			// Create a copy of the parameters, so we add the query parameters as signle parameters to the signature base
			var signatureParameters = new Dictionary<string, string>(parameters);
			foreach (var valuePair in requestUri.QueryToKeyValuePairs())
			{
				if (!signatureParameters.ContainsKey(valuePair.Key))
				{
					signatureParameters.Add(valuePair.Key, valuePair.Value);
				}
			}
			signatureBase.Append(Uri.EscapeDataString(GenerateNormalizedParametersString(signatureParameters)));
			Log.Verbose().WriteLine("Signature base: {0}", signatureBase);

			string key = string.Format(CultureInfo.InvariantCulture, "{0}&{1}", Uri.EscapeDataString(_oAuthSettings.ClientSecret), string.IsNullOrEmpty(_oAuthSettings.Token.OAuthTokenSecret) ? string.Empty : Uri.EscapeDataString(_oAuthSettings.Token.OAuthTokenSecret));
			switch (_oAuthSettings.SignatureType)
			{
				case OAuthSignatureTypes.RsaSha1:
					// Code comes from here: http://www.dotnetfunda.com/articles/article1932-rest-service-call-using-oauth-10-authorization-with-rsa-sha1.aspx
					// Read the .P12 file to read Private/Public key Certificate
					string certFilePath = _oAuthSettings.ClientId; // The .P12 certificate file path Example: "C:/mycertificate/MCOpenAPI.p12
					string password = _oAuthSettings.ClientSecret; // password to read certificate .p12 file
													   // Read the Certification from .P12 file.
					var cert = new X509Certificate2(certFilePath, password);
					// Retrieve the Private key from Certificate.
					var rsaCrypt = (RSACryptoServiceProvider)cert.PrivateKey;
					// Create a RSA-SHA1 Hash object
					using (var shaHashObject = new SHA1Managed())
					{
						// Create Byte Array of Signature base string
						byte[] data = Encoding.ASCII.GetBytes(signatureBase.ToString());
						// Create Hashmap of Signature base string
						byte[] hash = shaHashObject.ComputeHash(data);
						// Create Sign Hash of base string
						// NOTE - 'SignHash' gives correct data. Don't use SignData method
						byte[] rsaSignature = rsaCrypt.SignHash(hash, CryptoConfig.MapNameToOID("SHA1"));
						// Convert to Base64 string
						string base64String = Convert.ToBase64String(rsaSignature);
						// Return the Encoded UTF8 string
						parameters.Add(OAuthParameters.OauthSignatureKey.EnumValueOf(), Uri.EscapeDataString(base64String));
					}
					break;
				case OAuthSignatureTypes.PlainText:
					parameters.Add(OAuthParameters.OauthSignatureKey.EnumValueOf(), key);
					break;
				default:
					// Generate Signature and add it to the parameters
					var hmacsha1 = new HMACSHA1 { Key = Encoding.UTF8.GetBytes(key) };
					string signature = ComputeHash(hmacsha1, signatureBase.ToString());
					parameters.Add(OAuthParameters.OauthSignatureKey.EnumValueOf(), signature);
					break;
			}
		}

		/// <summary>
		/// Generate the normalized paramter string
		/// </summary>
		/// <param name="queryParameters">the list of query parameters</param>
		/// <returns>a string with the normalized query parameters</returns>
		private static string GenerateNormalizedParametersString(IDictionary<string, string> queryParameters)
		{
			if (queryParameters == null || queryParameters.Count == 0)
			{
				return string.Empty;
			}

			queryParameters = new SortedDictionary<string, string>(queryParameters);

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
		/// Generate a nonce
		/// </summary>
		/// <returns></returns>
		public static string GenerateNonce()
		{
			// Just a simple implementation of a random number between 123400 and 9999999
			return RandomForNonce.Next(123400, 9999999).ToString();
		}

		/// <summary>
		/// Generate the timestamp for the signature		
		/// </summary>
		/// <returns></returns>
		public static string GenerateTimeStamp()
		{
			// Default implementation of UNIX time of the current UTC time
			TimeSpan timespan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return Convert.ToInt64(timespan.TotalSeconds).ToString();
		}

		/// <summary>
		/// Helper function to compute a hash value
		/// </summary>
		/// <param name="hashAlgorithm">The hashing algorithm used. If that algorithm needs some initialization, like HMAC and its derivatives, they should be initialized prior to passing it to this function</param>
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

			byte[] dataBuffer = Encoding.UTF8.GetBytes(data);
			byte[] hashBytes = hashAlgorithm.ComputeHash(dataBuffer);

			return Convert.ToBase64String(hashBytes);
		}

		/// <summary>
		/// Check the HttpRequestMessage if all OAuth setting are there, if not make this available.
		/// </summary>
		/// <param name="httpRequestMessage">HttpRequestMessage</param>
		/// <param name="cancellationToken">CancellationToken</param>
		/// <returns>HttpResponseMessage</returns>
		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
		{
			if (string.IsNullOrEmpty(_oAuthSettings.Token.OAuthToken))
			{
				await GetRequestTokenAsync(cancellationToken);
				await GetAuthorizeTokenAsync(cancellationToken);
			}
			var result = await base.SendAsync(httpRequestMessage, cancellationToken);
			return result;
		}
	}
}