using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Dapplo.HttpExtensions.Desktop
{
	[ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ComVisible(true), Guid("B722BCCB-4E68-101B-A2BC-00AA00404770")]
	public interface IOleCommandTarget
	{
		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int QueryStatus([In, MarshalAs(UnmanagedType.LPStruct)] Guid pguidCmdGroup, int cCmds, IntPtr prgCmds, IntPtr pCmdText);

		[return: MarshalAs(UnmanagedType.I4)]
		[PreserveSig]
		int Exec([In, MarshalAs(UnmanagedType.LPStruct)] Guid pguidCmdGroup, int nCmdId, int nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut);
	}

	public class ExtendedWebBrowserForms : WebBrowser
	{
		protected class ExtendedWebBrowserSite : WebBrowserSite, IOleCommandTarget
		{
			private const int OleCmdIdShowScriptError = 40;

			private static readonly Guid GuidDocHostCommandHandler = new Guid("F38BC242-B950-11D1-8918-00C04FC2C836");

			private const int Ok = 0;
			private const int OleCmdErrorNotsupported = (-2147221248);

			public ExtendedWebBrowserSite(WebBrowser wb) : base(wb)
			{
			}

			#region IOleCommandTarget Members

			public int QueryStatus(Guid pguidCmdGroup, int cCmds, IntPtr prgCmds, IntPtr pCmdText)
			{
				return OleCmdErrorNotsupported;
			}

			public int Exec(Guid pguidCmdGroup, int nCmdId, int nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
			{
				if (pguidCmdGroup == GuidDocHostCommandHandler)
				{
					if (nCmdId == OleCmdIdShowScriptError)
					{
						// do not need to alter pvaOut as the docs says, enough to return S_OK here
						return Ok;
					}
				}

				return OleCmdErrorNotsupported;
			}

			#endregion
		}

		protected override WebBrowserSiteBase CreateWebBrowserSiteBase()
		{
			return new ExtendedWebBrowserSite(this);
		}

	}
}
