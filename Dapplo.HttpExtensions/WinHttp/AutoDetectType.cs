using System;

namespace Dapplo.HttpExtensions.WinHttp
{
	/// <summary>
	/// Flags for the 
	/// </summary>
	[Flags]
	internal enum AutoDetectTypes
	{
		None = 0x0,
		Dhcp = 0x1, // WINHTTP_AUTO_DETECT_TYPE_DHCP
		DnsA = 0x2, // WINHTTP_AUTO_DETECT_TYPE_DNS_A 
	}
}
