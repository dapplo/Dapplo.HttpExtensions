using System.Collections.Generic;

namespace Dapplo.HttpExtensions.SharedDesktop.OAuth
{
	/// <summary>
	/// Specify OAuth 1 request configuration
	/// </summary>
	public class OAuth1RequestConfiguration : IHttpRequestConfiguration
	{
		/// <summary>
		/// Name of the configuration, this should be unique and usually is the type of the object
		/// </summary>
		public string Name { get; } = nameof(OAuth1RequestConfiguration);

		/// <summary>
		/// The OAuth parameters for this request
		/// </summary>
		public IDictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
	}
}
