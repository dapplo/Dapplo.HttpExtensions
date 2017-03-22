using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Dapplo.HttpExtensions.WinHttp
{
	/// <summary>
	/// A WinHttp helper
	/// </summary>
	public static class WinHttp
	{
		// Session for all WinHttp requests
		private static readonly IntPtr WinHttpSession = GetWinHttpSession();

		/// <summary>
		/// This function implements the Web Proxy Auto-Discovery (WPAD) protocol for automatically configuring the proxy settings for an HTTP request. The WPAD protocol downloads a Proxy Auto-Configuration (PAC) file, which is a script that identifies the proxy server to use for a given target URL. PAC files are typically deployed by the IT department within a corporate network environment. The URL of the PAC file can either be specified explicitly or WinHttpGetProxyForUrl can be instructed to automatically discover the location of the PAC file on the local network.
		/// </summary>
		/// <param name="hSession">The WinHTTP session handle returned by the WinHttpOpen function</param>
		/// <param name="lpcwszUrl">A pointer to a null-terminated Unicode string that contains the URL of the HTTP request that the application is preparing to send.</param>
		/// <param name="pAutoProxyOptions">A pointer to a WINHTTP_AUTOPROXY_OPTIONS structure that specifies the auto-proxy options to use.</param>
		/// <param name="pProxyInfo">A pointer to a WINHTTP_PROXY_INFO structure that receives the proxy setting. This structure is then applied to the request handle using the WINHTTP_OPTION_PROXY option.</param>
		/// <returns>If the function succeeds, the function returns <c>true</c>. If the function fails, it returns <c>false</c>. For extended error data, call <see cref="System.Runtime.InteropServices.Marshal.GetLastWin32Error"/>.</returns>
		[DllImport("winhttp", SetLastError = true, CharSet = CharSet.Unicode)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool WinHttpGetProxyForUrl(IntPtr hSession, [MarshalAs(UnmanagedType.LPWStr)]string lpcwszUrl, [In] ref AutoProxyOptions pAutoProxyOptions, out ProxyInfo pProxyInfo);

		/// <summary>
		/// The WinHttpOpen function initializes, for an application, the use of WinHTTP functions and returns a WinHTTP-session handle.
		/// </summary>
		/// <param name="userAgent">string with the user agent</param>
		/// <param name="accessType">AccessTypes with the type of access required</param>
		/// <param name="proxyName">A pointer to a string variable that contains the name of the proxy server to use when proxy access is specified by setting dwAccessType to WINHTTP_ACCESS_TYPE_NAMED_PROXY.</param>
		/// <param name="proxyBypass">a string variable that contains an optional semicolon delimited list of host names or IP addresses, or both, that should not be routed through the proxy when dwAccessType is set to WINHTTP_ACCESS_TYPE_NAMED_PROXY.</param>
		/// <param name="dwFlags">Unsigned long integer value that contains the flags that indicate various options affecting the behavior of this function. </param>
		/// <returns></returns>
		[DllImport("winhttp", SetLastError = true, CharSet = CharSet.Unicode)]
		private static extern IntPtr WinHttpOpen([MarshalAs(UnmanagedType.LPWStr)]string userAgent, AccessTypes accessType, [MarshalAs(UnmanagedType.LPWStr)]string proxyName, [MarshalAs(UnmanagedType.LPWStr)]string proxyBypass, uint dwFlags);

		/// <summary>
		/// Get a session from WinHttp
		/// </summary>
		/// <returns>IntPtr with a WinHttp session handle</returns>
		private static IntPtr GetWinHttpSession()
		{
			// TODO: Use AccessTypes.AutomaticProxy on Windows 8 or later!
			var winHttpSessionHandle = WinHttpOpen(HttpExtensionsGlobals.HttpSettings.DefaultUserAgent, AccessTypes.NamedProxy, "http://proxy-url:8080", null, 0);
			if (winHttpSessionHandle == IntPtr.Zero)
			{
				var lastError = Marshal.GetLastWin32Error();
				throw new Win32Exception(lastError);
			}
			return winHttpSessionHandle;
		}

		/// <summary>
		/// Retrieves the proxy Uri for the specified Uri
		/// If the system is configured to use PAC, this is processed
		/// </summary>
		/// <param name="uri">Uri to find the proxy for</param>
		/// <returns>Uri with the proxy</returns>
		public static Uri GetProxyFor(this Uri uri)
		{
			var autoProxyOptions = AutoProxyOptions.Create();
			ProxyInfo proxyInfo = ProxyInfo.Create();
			try
			{
				if (WinHttpGetProxyForUrl(WinHttpSession, uri.AbsoluteUri, ref autoProxyOptions, out proxyInfo))
				{
					if (proxyInfo.AccessType != AccessTypes.NoProxy)
					{
						return new Uri(Marshal.PtrToStringUni(proxyInfo.Proxy));
					}
				}
				else
				{
					var lastError = Marshal.GetLastWin32Error();
					if (lastError >= (int)WinHttpErrors.OutOfHandles && lastError <= (int)WinHttpErrors.ClientCertNoAccessPrivateKey)
					{
						throw new Exception(((WinHttpErrors)lastError).ToString());
					}
					throw new Win32Exception(lastError);
				}
			}
			finally
			{
				proxyInfo.Dispose();
			}
			return null;
		}

	}
}
