namespace Dapplo.HttpExtensions.WinHttp
{
	/// <summary>
	/// 
	/// </summary>
	internal enum AccessTypes : uint
	{
		/// <summary>
		/// Important  Use of this option is deprecated on Windows 8.1 and newer. Use WINHTTP_ACCESS_TYPE_AUTOMATIC_PROXY instead.
		/// 
		/// Retrieves the static proxy or direct configuration from the registry. WINHTTP_ACCESS_TYPE_DEFAULT_PROXY does not inherit browser proxy settings.
		/// The WinHTTP proxy configuration is set by one of these mechanisms.
		/// The proxycfg.exe utility on Windows XP and Windows Server 2003 or earlier.
		/// The netsh.exe utility on Windows Vista and Windows Server 2008 or later.
		/// WinHttpSetDefaultProxyConfiguration on all platforms.
		/// </summary>
		DefaultProxy = 0,
		/// <summary>
		/// Resolves all host names directly without a proxy.
		/// </summary>
		NoProxy = 1,
		/// <summary>
		/// Passes requests to the proxy unless a proxy bypass list is supplied and the name to be resolved bypasses the proxy.
		/// In this case, this function uses the values passed for pwszProxyName and pwszProxyBypass.
		/// </summary>
		NamedProxy = 3,
		/// <summary>
		/// Uses system and per-user proxy settings (including the Internet Explorer proxy configuration) to determine which proxy/proxies to use. Automatically attempts to handle failover between multiple proxies, different proxy configurations per interface, and authentication. Supported in Windows 8.1 and newer.
		/// </summary>
		AutomaticProxy = 4
	}
}
