using Dapplo.HttpExtensions.Support;
using System;
using System.ComponentModel;

namespace Dapplo.HttpExtensions.OAuth
{
	/// <summary>
	/// The credentials which should be stored.
	/// This can be used to extend your Dapplo.Config.IIniSection extending interface.
	/// </summary>
	public interface IOAuth2Token
	{
		/// <summary>
		/// Bearer token for accessing OAuth 2 services
		/// </summary>
		[Description("Contains a OAuth 2 AccessToken"), TypeConverter(typeof(DelegatingStringEncryptionTypeConverter))]
		string AccessToken
		{
			get;
			set;
		}

		/// <summary>
		/// Expire time for the AccessToken, this this time (-60 seconds) is passed a new AccessToken needs to be generated with the RefreshToken
		/// </summary>
		[Description("This describes when the OAuth 2 AccessToken expires"), TypeConverter(typeof(DelegatingStringEncryptionTypeConverter))]
		DateTimeOffset AccessTokenExpires
		{
			get;
			set;
		}

		/// <summary>
		/// Token used to get a new Access Token
		/// </summary>
		[Description("OAuth 2 authorization refresh Token"), TypeConverter(typeof (DelegatingStringEncryptionTypeConverter))]
		string RefreshToken
		{
			get;
			set;
		}
	}
}
