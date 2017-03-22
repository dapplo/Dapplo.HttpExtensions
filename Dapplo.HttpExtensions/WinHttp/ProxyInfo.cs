using System;
using System.Runtime.InteropServices;

namespace Dapplo.HttpExtensions.WinHttp
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct ProxyInfo
	{
		/// <summary>
		/// Unsigned long integer value that contains the access type. This can be one of the following values:
		/// </summary>
		public AccessTypes AccessType;

		public IntPtr Proxy;

		public IntPtr ProxyBypass;

		/// <summary>
		/// Factory to create
		/// </summary>
		/// <returns></returns>
		internal static ProxyInfo Create()
		{
			return new ProxyInfo
			{
				AccessType = AccessTypes.NamedProxy,
				Proxy =  IntPtr.Zero,
				ProxyBypass = IntPtr.Zero
			};
		}

		public void Dispose()
		{
			if (Proxy != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(Proxy);
				Proxy = IntPtr.Zero;
			}
			if (ProxyBypass == IntPtr.Zero)
			{
				return;
			}
			Marshal.FreeHGlobal(ProxyBypass);
			ProxyBypass = IntPtr.Zero;
		}
	}
}
