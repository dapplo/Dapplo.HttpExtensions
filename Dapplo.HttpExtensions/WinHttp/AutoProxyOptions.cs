using System;
using System.Runtime.InteropServices;

namespace Dapplo.HttpExtensions.WinHttp
{
	/// <summary>
	/// 
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct AutoProxyOptions
	{
		/// <summary>
		/// Mechanisms should be used to obtain the PAC file.
		/// </summary>
		public AutoProxyFlags Flags;

		/// <summary>
		/// If dwFlags includes the WINHTTP_AUTOPROXY_AUTO_DETECT flag, then dwAutoDetectFlags specifies what protocols are to be used to locate the PAC file. If both the DHCP and DNS auto detect flags are specified, then DHCP is used first; if no PAC URL is discovered using DHCP, then DNS is used.
		/// If dwFlags does not include the WINHTTP_AUTOPROXY_AUTO_DETECT flag, then dwAutoDetectFlags must be zero.
		/// </summary>
		public AutoDetectTypes AutoDetectFlags;

		/// <summary>
		/// If dwFlags includes the WINHTTP_AUTOPROXY_CONFIG_URL flag, the lpszAutoConfigUrl must point to a null-terminated Unicode string that contains the URL of the proxy auto-configuration (PAC) file.
		/// If dwFlags does not include the WINHTTP_AUTOPROXY_CONFIG_URL flag, then lpszAutoConfigUrl must be NULL.
		/// </summary>
		[MarshalAs(UnmanagedType.LPWStr)]
		public string AutoConfigUrl;

		/// <summary>
		/// Reserved for future use; must be NULL.
		/// </summary>
		private IntPtr lpvReserved;

		/// <summary>
		/// Reserved for future use; must be zero.
		/// </summary>
		private int dwReserved;

		/// <summary>
		/// Specifies whether the client's domain credentials should be automatically sent in response to an NTLM or Negotiate Authentication challenge when WinHTTP requests the PAC file.
		///If this flag is TRUE, credentials should automatically be sent in response to an authentication challenge. If this flag is FALSE and authentication is required to download the PAC file, the WinHttpGetProxyForUrl function fails.
		/// </summary>
		public bool AutoLogonIfChallenged;

		/// <summary>
		/// Factory to create a default AutoProxyOptions
		/// </summary>
		/// <returns>AutoProxyOptions</returns>
		public static AutoProxyOptions Create()
		{
			return new AutoProxyOptions
			{
				AutoDetectFlags= AutoDetectTypes.DnsA | AutoDetectTypes.Dhcp,
				AutoConfigUrl = "http://ppacnew.corp.int/proxy.pac",
				Flags = AutoProxyFlags.AutoProxyConfigUrl,
				lpvReserved = IntPtr.Zero,
				dwReserved = 0,
				AutoLogonIfChallenged = true
			};
		}
	}

}
