using System;

namespace Dapplo.HttpExtensions.WinHttp
{

	/// <summary>
	/// Flags for the Auto proxy configuration
	/// </summary>
	[Flags]
	internal enum AutoProxyFlags
	{
		AutoDetect = 0x00000001, // WINHTTP_AUTOPROXY_AUTO_DETECT 
		AutoProxyConfigUrl = 0x00000002, // WINHTTP_AUTOPROXY_CONFIG_URL
		RunInProcess = 0x00010000, // WINHTTP_AUTOPROXY_RUN_INPROCESS
		RunOutProcessOnly = 0x00020000 // WINHTTP_AUTOPROXY_RUN_OUTPROCESS_ONLY
	}
}
