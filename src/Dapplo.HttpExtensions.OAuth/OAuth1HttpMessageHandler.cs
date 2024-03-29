﻿// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Security.Authentication;
using System.Security.Cryptography;
using Dapplo.HttpExtensions.OAuth.CodeReceivers;

namespace Dapplo.HttpExtensions.OAuth;

/// <summary>
///     This DelegatingHandler handles the OAuth specific stuff and delegates the "final" SendAsync to the InnerHandler
/// </summary>
public class OAuth1HttpMessageHandler : DelegatingHandler
{
#pragma warning disable IDE0090 // Use 'new(...)'
    private static readonly LogSource Log = new LogSource();
#pragma warning restore IDE0090 // Use 'new(...)'
    private static readonly Random RandomForNonce = new();
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

        CodeReceivers.Add(
            AuthorizeModes.OutOfBound,
            new OutOfBoundCodeReceiver()
        );
        CodeReceivers.Add(
            AuthorizeModes.OutOfBoundAuto,
            new OutOfBoundCodeReceiver()
        );
        CodeReceivers.Add(
            AuthorizeModes.EmbeddedBrowser,
            new EmbeddedBrowserCodeReceiver()
        );
    }

    /// <summary>
    ///     Create a HttpMessageHandler which handles the OAuth communication for you
    /// </summary>
    /// <param name="oAuth1Settings">OAuth1Settings</param>
    /// <param name="oAuth1HttpBehaviour">OAuth1HttpBehaviour</param>
    /// <param name="innerHandler">HttpMessageHandler</param>
    public OAuth1HttpMessageHandler(OAuth1Settings oAuth1Settings, OAuth1HttpBehaviour oAuth1HttpBehaviour, HttpMessageHandler innerHandler) : base(innerHandler)
    {
        if (oAuth1Settings.ClientId is null)
        {
            throw new ArgumentNullException(nameof(oAuth1Settings.ClientId));
        }
        if (oAuth1Settings.SignatureType == OAuth1SignatureTypes.RsaSha1)
        {
            if (oAuth1Settings.RsaSha1Provider is null)
            {
                throw new ArgumentNullException(nameof(oAuth1Settings.RsaSha1Provider));
            }
        }
        else if (oAuth1Settings.ClientSecret is null)
        {
            throw new ArgumentNullException(nameof(oAuth1Settings.ClientSecret));
        }
        if (oAuth1Settings.TokenUrl is null)
        {
            throw new ArgumentNullException(nameof(oAuth1Settings.TokenUrl));
        }

        _oAuth1Settings = oAuth1Settings;

        var newHttpBehaviour = oAuth1HttpBehaviour.ShallowClone();
        // Remove the OnHttpMessageHandlerCreated
        newHttpBehaviour.OnHttpMessageHandlerCreated = null;
        // Use it for internal communication
        _oAuth1HttpBehaviour = (OAuth1HttpBehaviour) newHttpBehaviour;
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
        if (hashAlgorithm is null)
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
        if (queryParameters is null || queryParameters.Count == 0)
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
    private async Task GetAccessTokenAsync(CancellationToken cancellationToken = default)
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
            if (resultParameters.TryGetValue(OAuth1Parameters.Token.EnumValueOf(), out var tokenValue))
            {
                _oAuth1Settings.Token.OAuthToken = tokenValue;
                resultParameters.Remove(OAuth1Parameters.Token.EnumValueOf());
            }

            if (resultParameters.TryGetValue(OAuth1Parameters.TokenSecret.EnumValueOf(), out var secretValue))
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
    private async Task GetAuthorizeTokenAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_oAuth1Settings.RequestToken))
        {
            throw new ArgumentNullException(nameof(_oAuth1Settings.RequestToken), "The request token is not set");
        }

        if (!CodeReceivers.TryGetValue(_oAuth1Settings.AuthorizeMode, out var codeReceiver))
        {
            throw new NotImplementedException($"Authorize mode '{_oAuth1Settings.AuthorizeMode}' is not implemented/registered.");
        }
        Log.Debug().WriteLine("Calling code receiver : {0}", _oAuth1Settings.AuthorizeMode);
        var result = await codeReceiver.ReceiveCodeAsync(_oAuth1Settings.AuthorizeMode, _oAuth1Settings, cancellationToken).ConfigureAwait(false);

        if (result != null)
        {
            if (result.TryGetValue(OAuth1Parameters.Token.EnumValueOf(), out var tokenValue))
            {
                _oAuth1Settings.AuthorizeToken = tokenValue;
            }

            if (result.TryGetValue(OAuth1Parameters.Verifier.EnumValueOf(), out var verifierValue))
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
    private async Task GetRequestTokenAsync(CancellationToken cancellationToken = default)
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
            if (resultParameters.TryGetValue(OAuth1Parameters.Token.EnumValueOf(), out var tokenValue))
            {
                Log.Verbose().WriteLine("Storing token {0}", tokenValue);
                _oAuth1Settings.RequestToken = tokenValue;
            }

            if (resultParameters.TryGetValue(OAuth1Parameters.TokenSecret.EnumValueOf(), out var tokenSecretValue))
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
        await _oAuth1Settings.Lock.WaitAsync(CancellationToken.None).ConfigureAwait(false);
        try
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
        finally
        {
            _oAuth1Settings.Lock.Release();
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
        // TODO: Use IHttpRequestConfiguration here

#if NET6_0
            var parameters = new Dictionary<string, object>(httpRequestMessage.Options);
#else
        var parameters = new Dictionary<string, object>(httpRequestMessage.Properties);
#endif
        // Build the signature base
        var signatureBase = new StringBuilder();

        // Add Method to signature base
        signatureBase.Append(httpRequestMessage.Method).Append('&');

        // Add normalized URL, most of it is already normalized by using AbsoluteUri, but we need the Uri without Query and Fragment
        var normalizedUri = new UriBuilder(httpRequestMessage.RequestUri)
        {
            Query = "",
            Fragment = ""
        };
        signatureBase.Append(Uri.EscapeDataString(normalizedUri.Uri.AbsoluteUri)).Append('&');

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
        // Create a copy of the parameters, so we add the query parameters as single parameters to the signature base
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
        switch (_oAuth1Settings.SignatureType)
        {
            case OAuth1SignatureTypes.RsaSha1:
                byte[] dataBuffer = Encoding.UTF8.GetBytes(signatureBase.ToString());
                byte[] signatureBytes = _oAuth1Settings.RsaSha1Provider.SignData(dataBuffer, "SHA1");
                var base64Signature = Convert.ToBase64String(signatureBytes);
                // Return the Encoded UTF8 string
                parameters.Add(OAuth1Parameters.Signature.EnumValueOf(), base64Signature);
                break;
            case OAuth1SignatureTypes.PlainText:
                var keyPlain = string.Format(CultureInfo.InvariantCulture, "{0}&{1}", Uri.EscapeDataString(_oAuth1Settings.ClientSecret), Uri.EscapeDataString(secret));
                Log.Verbose().WriteLine("Signing with key {0}", keyPlain);
                parameters.Add(OAuth1Parameters.Signature.EnumValueOf(), keyPlain);
                break;
            case OAuth1SignatureTypes.HMacSha1:
                // Generate Signature and add it to the parameters
                var keySha1 = string.Format(CultureInfo.InvariantCulture, "{0}&{1}", Uri.EscapeDataString(_oAuth1Settings.ClientSecret), Uri.EscapeDataString(secret));
                Log.Verbose().WriteLine("Signing with key {0}", keySha1);
                string signature;
                using (var hashAlgorithm = new HMACSHA1 {Key = Encoding.UTF8.GetBytes(keySha1)})
                {
                    signature = ComputeHash(hashAlgorithm, signatureBase.ToString());
                }
                parameters.Add(OAuth1Parameters.Signature.EnumValueOf(), signature);
                break;
            default:
                throw new ArgumentException("Unknown SignatureType", nameof(_oAuth1Settings.SignatureType));
        }

        if (_oAuth1Settings.SignatureTransport == OAuth1SignatureTransports.QueryParameters)
        {
            // Add the OAuth values to the query parameters
            httpRequestMessage.RequestUri.ExtendQuery(parameters.Where(x => x.Key.StartsWith("oauth_") && x.Value is string).OrderBy(x => x.Key));
        }
        else
        {
            var authorizationHeaderValues = string.Join(", ",
                parameters.Where(x => x.Key.StartsWith("oauth_") && x.Value is string)
                    .OrderBy(x => x.Key)
                    .Select(x => $"{x.Key}=\"{Uri.EscapeDataString((string)x.Value)}\""));
            // Add the OAuth values to the headers
            httpRequestMessage.SetAuthorization("OAuth", authorizationHeaderValues);

        }

#if NET6_0
            if (httpRequestMessage.Method == HttpMethod.Post && httpRequestMessage.Options.Any())
#else
        if (httpRequestMessage.Method == HttpMethod.Post && httpRequestMessage.Properties.Count > 0)
#endif
        {
            var multipartFormDataContent = new MultipartFormDataContent();
#if NET6_0
                foreach (var option in httpRequestMessage.Options)
                {
                    var propertyName = option.Key;
                    var requestObject = option.Value;

#else
            foreach (var propertyName in httpRequestMessage.Properties.Keys)
            {
                var requestObject = httpRequestMessage.Properties[propertyName];
#endif

                var formattedKey = $"\"{propertyName}\"";
                if (requestObject is HttpContent httpContent)
                {
                    multipartFormDataContent.Add(httpContent, formattedKey);
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