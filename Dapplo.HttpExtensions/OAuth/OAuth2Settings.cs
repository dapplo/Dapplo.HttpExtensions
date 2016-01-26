using System;
using System.Collections.Generic;

namespace Dapplo.HttpExtensions.OAuth
{
	/// <summary>
	/// Settings for the OAuth 2 protocol
	/// </summary>
	public class OAuth2Settings
	{
		public AuthorizeModes AuthorizeMode
		{
			get;
			set;
		} = AuthorizeModes.Unknown;

		/// <summary>
		/// Specify the name of the cloud service, so it can be used in window titles, logs etc
		/// </summary>
		public string CloudServiceName
		{
			get;
			set;
		}

		/// <summary>
		/// Specify the width of the embedded Browser
		/// </summary>
		public int BrowserWidth
		{
			get;
			set;
		}
		/// <summary>
		/// Specify the Height of the embedded Browser
		/// </summary>
		public int BrowserHeight
		{
			get;
			set;
		}

		/// <summary>
		/// The OAuth 2 client id
		/// </summary>
		public string ClientId
		{
			get;
			set;
		}

		/// <summary>
		/// The OAuth 2 client secret
		/// </summary>
		public string ClientSecret
		{
			get;
			set;
		}

		/// <summary>
		/// The OAuth 2 state, this is something that is passed to the server, is not processed but returned back to the client.
		/// e.g. a correlation ID
		/// Default this is filled with a new Guid
		/// </summary>
		public string State
		{
			get;
			set;
		} = Guid.NewGuid().ToString();

		/// <summary>
		/// The autorization URL where the values of this class can be "injected"
		/// </summary>
		public string AuthUrlPattern
		{
			get;
			set;
		}

		/// <summary>
		/// The URL to get a Token
		/// </summary>
		public Uri TokenUrl
		{
			get;
			set;
		}

		/// <summary>
		/// This is the redirect URL, in some implementations this is automatically set (LocalServerCodeReceiver)
		/// In some implementations this could be e.g. urn:ietf:wg:oauth:2.0:oob or urn:ietf:wg:oauth:2.0:oob:auto
		/// </summary>
		public string RedirectUrl { get; set; } = "http://getgreenshot.org";

		/// <summary>
		/// Bearer token for accessing OAuth 2 services
		/// </summary>
		public string AccessToken
		{
			get;
			set;
		}

		/// <summary>
		/// Expire time for the AccessToken, this this time (-60 seconds) is passed a new AccessToken needs to be generated with the RefreshToken
		/// </summary>
		public DateTimeOffset AccessTokenExpires
		{
			get;
			set;
		}

		/// <summary>
		/// Return true if the access token is expired.
		/// Important "side-effect": if true is returned the AccessToken will be set to null!
		/// </summary>
		public bool IsAccessTokenExpired
		{
			get
			{
				bool expired = true;
				if (!string.IsNullOrEmpty(AccessToken) && AccessTokenExpires != default(DateTimeOffset))
				{
					expired = DateTimeOffset.Now.AddSeconds(60) > AccessTokenExpires;
				}
				// Make sure the token is not usable
				if (expired)
				{
					AccessToken = null;
				}
				return expired;
			}
		}

		/// <summary>
		/// Token used to get a new Access Token
		/// </summary>
		public string RefreshToken
		{
			get;
			set;
		}

		/// <summary>
		/// Put anything in here which is needed for the OAuth 2 implementation of this specific service but isn't generic, e.g. for Google there is a "scope"
		/// </summary>
		public IDictionary<string, string> AdditionalAttributes
		{
			get;
			set;
		} = new Dictionary<string, string>();


		/// <summary>
		/// This contains the code returned from the authorization, but only shortly after it was received.
		/// It will be cleared as soon as it was used.
		/// </summary>
		public string Code
		{
			get;
			set;
		}
	}
}
