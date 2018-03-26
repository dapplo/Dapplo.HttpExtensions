﻿//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2018 Dapplo
// 
//  For more information see: http://dapplo.net/
//  Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
//  This file is part of Dapplo.HttpExtensions
// 
//  Dapplo.HttpExtensions is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  Dapplo.HttpExtensions is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have a copy of the GNU Lesser General Public License
//  along with Dapplo.HttpExtensions. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

#if NET45 || NET46

#region Usings

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#endregion

namespace Dapplo.HttpExtensions.OAuth.Desktop
{
    /// <summary>
    ///     Used to show an extended embedded web-browser
    ///     See <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms683797.aspx">here</a> for more information
    ///     on this interface
    /// </summary>
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComVisible(true)]
    [Guid("B722BCCB-4E68-101B-A2BC-00AA00404770")]
    public interface IOleCommandTarget
    {
        /// <summary>
        ///     Queries the object for the status of one or more commands generated by user interface events.
        /// </summary>
        /// <param name="pguidCmdGroup"></param>
        /// <param name="cCmds"></param>
        /// <param name="prgCmds"></param>
        /// <param name="pCmdText"></param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int QueryStatus([In] [MarshalAs(UnmanagedType.LPStruct)] Guid pguidCmdGroup, int cCmds, IntPtr prgCmds, IntPtr pCmdText);

        /// <summary>
        ///     Executes the specified command or displays help for the command.
        /// </summary>
        /// <param name="pguidCmdGroup"></param>
        /// <param name="nCmdId"></param>
        /// <param name="nCmdexecopt"></param>
        /// <param name="pvaIn"></param>
        /// <param name="pvaOut"></param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Exec([In] [MarshalAs(UnmanagedType.LPStruct)] Guid pguidCmdGroup, int nCmdId, int nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut);
    }

    /// <summary>
    ///     This is the "extended" version of the WebBrowser
    /// </summary>
    public class ExtendedWebBrowserForm : WebBrowser
    {
        /// <summary>
        ///     Overrides the CreateWebBrowserSiteBase to return ExtendedWebBrowserSite instead of WebBrowserSiteBase
        /// </summary>
        /// <returns>ExtendedWebBrowserSite</returns>
        protected override WebBrowserSiteBase CreateWebBrowserSiteBase()
        {
            return new ExtendedWebBrowserSite(this);
        }

        /// <summary>
        ///     an internal class, which enables some additional information for the embedded WebBrowser
        /// </summary>
        protected class ExtendedWebBrowserSite : WebBrowserSite, IOleCommandTarget
        {
            private const int OleCmdIdShowScriptError = 40;

            private const int Ok = 0;
            private const int OleCmdErrorNotsupported = -2147221248;

            private static readonly Guid GuidDocHostCommandHandler = new Guid("F38BC242-B950-11D1-8918-00C04FC2C836");

            /// <summary>
            ///     Constructor
            /// </summary>
            /// <param name="webBrowser">WebBrowser</param>
            public ExtendedWebBrowserSite(WebBrowser webBrowser) : base(webBrowser)
            {
            }

            #region IOleCommandTarget Members

            /// <inheritdoc />
            public int QueryStatus(Guid pguidCmdGroup, int cCmds, IntPtr prgCmds, IntPtr pCmdText)
            {
                return OleCmdErrorNotsupported;
            }

            /// <inheritdoc />
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
    }
}

#endif