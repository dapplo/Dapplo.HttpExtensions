//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2015-2016 Dapplo
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

#region using

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Cache;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Security.Principal;

#endregion

namespace Dapplo.HttpExtensions
{
	/// <summary>
	///     Interface for the global settings, as the DefaultValue attributes are set this CAN be used with Dapplo.Config.
	///     <br />
	///     Best example would be:
	///     <br />
	///     <code>
	/// [IniSection("Network"), Display(Description = "Network / HTTP Settings")]<br />
	/// public interface IHttpConfig : IHttpSettings, IIniSection {<br />
	/// }
	/// </code>
	///     <br />
	///     (Yes, it's can be empty, the settings are in the IHttpSettings interface) and than assign the generated instance to
	///     <see cref="HttpExtensionsGlobals" />
	/// </summary>
	public partial interface IHttpSettings
	{
		/// <summary>
		///     For more details, click
		///     <a href="https://msdn.microsoft.com/en-us/library/system.net.http.webrequesthandler.allowpipelining.aspx">here</a>
		/// </summary>
		[DefaultValue(true), Display(Description = "When true, pipelined connections are allowed")]
		bool AllowPipelining { get; set; }

		/// <summary>
		///     In mutual authentication, both the client and server present credentials to establish their identity. The
		///     MutualAuthRequired and MutualAuthRequested values are relevant for Kerberos authentication. Kerberos authentication
		///     can be supported directly, or can be used if the Negotiate security protocol is used to select the actual security
		///     protocol.
		///     For more information about authentication protocols, see
		///     <a href="https://msdn.microsoft.com/en-us/library/47zhdx9d.aspx">Internet Authentication</a>
		///     For more details, click
		///     <a href="https://msdn.microsoft.com/en-us/library/system.net.http.webrequesthandler.authenticationlevel.aspx">here</a>
		/// </summary>
		[DefaultValue(AuthenticationLevel.MutualAuthRequested), Display(Description = "The level of authentication and impersonation used for every request")]
		AuthenticationLevel AuthenticationLevel { get; set; }

		/// <summary>
		///     For more details, click
		///     <a href="https://msdn.microsoft.com/en-us/library/system.net.http.webrequesthandler.continuetimeout.aspx">here</a>
		/// </summary>
		[DefaultValue("0:0:0.350"), DataMember(EmitDefaultValue = true),
		Display(Description = "The amount of time (with milliseconds) the application will wait for 100-continue from the server before uploading data")]
		TimeSpan ContinueTimeout { get; set; }

		/// <summary>
		///     The impersonation level determines how the server can use the client's credentials.
		///     For more details, click
		///     <a href="https://msdn.microsoft.com/en-us/library/system.net.http.webrequesthandler.impersonationlevel.aspx">here</a>
		/// </summary>
		[DefaultValue(TokenImpersonationLevel.Delegation),
		Display(Description = "The impersonation level determines how the server can use the client's credentials")]
		TokenImpersonationLevel ImpersonationLevel { get; set; }

		/// <summary>
		///     For more details, click
		///     <a
		///         href="https://msdn.microsoft.com/en-us/library/system.net.http.httpclienthandler.maxrequestcontentbuffersize.aspx">
		///         here
		///     </a>
		/// </summary>
		[DefaultValue(2*1024*1024*1024L - 1L), Display(Description = "Max request content buffer size")]
		long MaxRequestContentBufferSize { get; set; }

		/// <summary>
		///     For more details, click
		///     <a href="https://msdn.microsoft.com/en-us/library/system.net.http.webrequesthandler.maxresponseheaderslength.aspx">here</a>
		///     Default would have been 64, this is increased to 256
		/// </summary>
		[DefaultValue(256), Display(Description = "The max length, in kilobytes (1024 bytes), of the response headers")]
		int MaxResponseHeadersLength { get; set; }

		/// <summary>
		///     For more details, click
		///     <a href="https://msdn.microsoft.com/en-us/library/system.net.http.webrequesthandler.readwritetimeout.aspx">here</a>
		/// </summary>
		[DefaultValue(300000), Display(Description = "The number of milliseconds before the writing or reading times out"), DataMember(EmitDefaultValue = true)]
		int ReadWriteTimeout { get; set; }

		/// <summary>
		///     For more details, click
		///     <a href="https://msdn.microsoft.com/en-us/library/system.net.cache.httprequestcachelevel.aspx">here</a>
		///     Default is RequestCacheLevel.Default
		/// </summary>
		[DefaultValue(RequestCacheLevel.Default), Display(Description = "The cache level for the request")]
		RequestCacheLevel RequestCacheLevel { get; set; }

		[DefaultValue(true), Display(Description = "If true, every request is made via the configured or default proxy"), DataMember(EmitDefaultValue = true)]
		bool UseProxy { get; set; }
	}
}