﻿/*
	Dapplo - building blocks for desktop applications
	Copyright (C) 2015-2016 Dapplo

	For more information see: http://dapplo.net/
	Dapplo repositories are hosted on GitHub: https://github.com/dapplo

	This file is part of Dapplo.HttpExtensions.

	Dapplo.HttpExtensions is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	Dapplo.HttpExtensions is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.ComponentModel;
using System.Net;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Security.Principal;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// Interface for the global settings, as the DefaultValue attributes are set this CAN be used with Dapplo.Config.
	/// Best example would be:
	/// [IniSection("Network"), Description("Network / HTPP Settings")]
	/// public interface IHttpConfig : ISettings, IIniSection {
	/// }
	/// (Yes, it's can be empty, the settings are in the ISettings interface) and than assign the generated instance to Settings.Instance
	/// </summary>
	public interface IHttpSettings
	{
		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.httpclienthandler.allowautoredirect.aspx
		/// </summary>
		[DefaultValue(true), Description("When true a connection would automatically redirect, if the server says so"), DataMember(EmitDefaultValue = true)]
		bool AllowAutoRedirect { get; set; }

		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.webrequesthandler.allowpipelining.aspx
		/// </summary>
		[DefaultValue(true), Description("When true, pipelined connections are allowed")]
		bool AllowPipelining { get; set; }

		/// <summary>
		/// In mutual authentication, both the client and server present credentials to establish their identity. The MutualAuthRequired and MutualAuthRequested values are relevant for Kerberos authentication. Kerberos authentication can be supported directly, or can be used if the Negotiate security protocol is used to select the actual security protocol.
		/// For more information about authentication protocols, see Internet Authentication: https://msdn.microsoft.com/en-us/library/47zhdx9d.aspx
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.webrequesthandler.authenticationlevel.aspx
		/// </summary>
		[DefaultValue(AuthenticationLevel.MutualAuthRequested),
		 Description("The level of authentication and impersonation used for every request")]
		AuthenticationLevel AuthenticationLevel { get; set; }

		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.httpclienthandler.credentials.aspx
		/// </summary>
		[Description("The credentials for the request, only used when UseDefaultCredentials is set to false")]
		ICredentials Credentials { get; set; }

		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.webrequesthandler.continuetimeout.aspx
		/// </summary>
		[DefaultValue("0:0:0.350"), DataMember(EmitDefaultValue = true),
		 Description("The amount of time (with milliseconds) the application will wait for 100-continue from the server before uploading data")]
		TimeSpan ContinueTimeout { get; set; }

		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.httpclienthandler.automaticdecompression.aspx
		/// </summary>
		[DefaultValue(DecompressionMethods.Deflate | DecompressionMethods.GZip), Description("Decompression methods used")]
		DecompressionMethods DefaultDecompressionMethods { get; set; }

		[Description("The default User-Agent value to use, a lot of services don't like it when this is empty or the behaviour depends on the value")]
		string DefaultUserAgent { get; set; }

		/// <summary>
		/// The impersonation level determines how the server can use the client's credentials.
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.webrequesthandler.impersonationlevel(v=vs.110).aspx
		/// </summary>
		[DefaultValue(TokenImpersonationLevel.Delegation),
		 Description("The impersonation level determines how the server can use the client's credentials")]
		TokenImpersonationLevel ImpersonationLevel { get; set; }

		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.httpclienthandler.maxautomaticredirections.aspx
		/// And: https://msdn.microsoft.com/en-us/library/system.net.httpwebrequest.allowautoredirect.aspx
		/// </summary>
		[DefaultValue(50), Description("The maximum amount of redirections that are followed")]
		int MaxAutomaticRedirections { get; set; }

		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.httpclienthandler.maxrequestcontentbuffersize.aspx
		/// </summary>
		[DefaultValue(2 * 1024 * 1024 * 1024L - 1L), Description("Max request content buffer size")]
		long MaxRequestContentBufferSize { get; set; }

		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.httpclient.maxresponsecontentbuffersize.aspx
		/// </summary>
		[DefaultValue(2 * 1024 * 1024 * 1024L - 1L), Description("Max response content buffer size")]
		long MaxResponseContentBufferSize { get; set; }

		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.webrequesthandler.maxresponseheaderslength.aspx
		/// Default would have been 64, this is increased to 256
		/// </summary>
		[DefaultValue(256), Description("The max length, in kilobytes (1024 bytes), of the response headers")]
		int MaxResponseHeadersLength { get; set; }

		/// <summary>
		/// The Uri for the proxy to use, when the UseDefaultProxy is set to false
		/// </summary>
		[Description("When true the configured proxy will used the default user credentials")]
		Uri ProxyUri { get; set; }

		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.webproxy.credentials.aspx
		/// </summary>
		[Description("The credentials for the proxy, only used when UseDefaultCredentialsForProy is set to false")]
		ICredentials ProxyCredentials { get; set; }

		/// <summary>
		/// see: https://msdn.microsoft.com/en-us/library/system.net.webproxy.bypassproxyonlocal.aspx
		/// </summary>
		[DefaultValue(true), Description("When set to true, all local addresses will bypass the proxy")]
		bool ProxyBypassOnLocal { get; set; }

		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.webproxy.bypassarraylist.aspx
		/// </summary>
		[Description("The bypass list for the proxy, only used when UseDefaultProxy is set to false (and UseProxy is true)")]
        string[] ProxyBypassList { get; set; }

		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.httpclienthandler.preauthenticate.aspx
		/// And: http://weblog.west-wind.com/posts/2010/Feb/18/NET-WebRequestPreAuthenticate-not-quite-what-it-sounds-like
		/// </summary>
		[DefaultValue(false), Description("When true the request is directly send with a HTTP Authorization header.")]
		bool PreAuthenticate { get; set; }

		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.httpclient.timeout.aspx
		/// </summary>
		[DefaultValue("0:01:40"), Description("Request timeout"), DataMember(EmitDefaultValue = true)]
        TimeSpan RequestTimeout { get; set; }
		
		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.webrequesthandler.readwritetimeout.aspx
		/// </summary>
		[DefaultValue(300000), Description("The number of milliseconds before the writing or reading times out"), DataMember(EmitDefaultValue = true)]
        int ReadWriteTimeout { get; set; }

		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.httpclienthandler.usecookies.aspx
		/// And: https://msdn.microsoft.com/en-us/library/system.net.http.httpclienthandler.cookiecontainer.aspx
		/// </summary>
		[DefaultValue(true), Description("Should requests store & resend cookies?"), DataMember(EmitDefaultValue = true)]
		bool UseCookies { get; set; }

		[DefaultValue(true), Description("When true every http request will supply the default user credentials when the server asks for them"), DataMember(EmitDefaultValue = true)]
		bool UseDefaultCredentials { get; set; }

		[DefaultValue(true), Description("If true, every request is made via the configured or default proxy"), DataMember(EmitDefaultValue = true)]
		bool UseProxy { get; set; }

		[DefaultValue(true), Description("When true the default system proxy is used"), DataMember(EmitDefaultValue = true)]
		bool UseDefaultProxy { get; set; }

		[DefaultValue(true), Description("When true the configured proxy will used the default user credentials"), DataMember(EmitDefaultValue = true)]
		bool UseDefaultCredentialsForProy { get; set; }
	}
}
