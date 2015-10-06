/*
 * dapplo - building blocks for desktop applications
 * Copyright (C) 2015 Robin Krom
 * 
 * For more information see: http://dapplo.net/
 * dapplo repositories are hosted on GitHub: https://github.com/dapplo
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 1 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
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

		[DefaultValue(true), Description("If true, every request is made via the configured or default proxy.")]
		bool UseProxy { get; set; }

		[DefaultValue(true), Description("When true the default system proxy is used")]
		bool UseDefaultProxy { get; set; }

		[DefaultValue(true), Description("When true the configured proxy will used the default user credentials")]
		bool UseDefaultCredentialsForProy { get; set; }

		/// <summary>
		/// The Uri for the proxy to use, when the UseDefaultProxy is set to false
		/// </summary>
		[Description("When true the configured proxy will used the default user credentials"), DataMember(EmitDefaultValue = true)]
		Uri ProxyUri { get; set; }

		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.webproxy.credentials.aspx
		/// </summary>
		[Description("The credentials for the proxy, only used when UseDefaultCredentialsForProy is set to false"), DataMember(EmitDefaultValue = true)]
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
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.httpclienthandler.usecookies.aspx
		/// And: https://msdn.microsoft.com/en-us/library/system.net.http.httpclienthandler.cookiecontainer.aspx
		/// </summary>
		[DefaultValue(true), Description("Should all requests via one Httpclient store & resend cookies?")]
        bool UseCookies { get; set; }

		[DefaultValue(true), Description("When true every http request will supply the default user credentials when the server asks for them")]
		bool UseDefaultCredentials { get; set; }

		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.httpclient.timeout.aspx
		/// </summary>
		[DefaultValue("0:01:40"), Description("Request timeout")]
        TimeSpan RequestTimeout { get; set; }

		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.httpclienthandler.allowautoredirect.aspx
		/// </summary>
		[DefaultValue(true), Description("When true a connection would automatically redirect, if the server says so.")]
		bool AllowAutoRedirect { get; set; }

		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.httpclienthandler.automaticdecompression.aspx
		/// </summary>
		[DefaultValue(DecompressionMethods.Deflate | DecompressionMethods.GZip), Description("Decompression methods used"), DataMember(EmitDefaultValue = true)]
        DecompressionMethods DefaultDecompressionMethods { get; set; }

		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.httpclienthandler.preauthenticate.aspx
		/// And: http://weblog.west-wind.com/posts/2010/Feb/18/NET-WebRequestPreAuthenticate-not-quite-what-it-sounds-like
		/// </summary>
		[DefaultValue(false), Description("When true the request is directly send with a HTTP Authorization header."), DataMember(EmitDefaultValue = true)]
        bool PreAuthenticate { get; set; }

		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.httpclienthandler.maxautomaticredirections.aspx
		/// And: https://msdn.microsoft.com/en-us/library/system.net.httpwebrequest.allowautoredirect.aspx
		/// </summary>
		[DefaultValue(50), Description("The maximum amount of redirections that are followed"), DataMember(EmitDefaultValue = true)]
        int MaxAutomaticRedirections { get; set; }

		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.httpclienthandler.maxrequestcontentbuffersize.aspx
		/// </summary>
		[DefaultValue(2 * 1024 * 1024 * 1024L), Description("Max request content buffer size"), DataMember(EmitDefaultValue = true)]
        long MaxRequestContentBufferSize { get; set; }

		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.httpclient.maxresponsecontentbuffersize.aspx
		/// </summary>
		[DefaultValue(2 * 1024 * 1024 * 1024L), Description("Max response content buffer size"), DataMember(EmitDefaultValue = true)]
		long MaxResponseContentBufferSize { get; set; }

		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.webrequesthandler.readwritetimeout.aspx
		/// </summary>
		[DefaultValue(300000), Description("The number of milliseconds before the writing or reading times out.")]
        int ReadWriteTimeout { get; set; }

		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.webrequesthandler.allowpipelining.aspx
		/// </summary>
		[DefaultValue(true), Description("When true, pipelined connections are allowed."), DataMember(EmitDefaultValue = true)]
        bool AllowPipelining { get; set; }

		/// <summary>
		/// In mutual authentication, both the client and server present credentials to establish their identity. The MutualAuthRequired and MutualAuthRequested values are relevant for Kerberos authentication. Kerberos authentication can be supported directly, or can be used if the Negotiate security protocol is used to select the actual security protocol.
		/// For more information about authentication protocols, see Internet Authentication: https://msdn.microsoft.com/en-us/library/47zhdx9d.aspx
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.webrequesthandler.authenticationlevel.aspx
		/// </summary>
		[DefaultValue(AuthenticationLevel.MutualAuthRequested), DataMember(EmitDefaultValue = true),
		 Description("The level of authentication and impersonation used for every request")]
        AuthenticationLevel AuthenticationLevel { get; set; }

		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.webrequesthandler.continuetimeout.aspx
		/// </summary>
		[DefaultValue("0.350"), DataMember(EmitDefaultValue = true),
		 Description("The amount of time, in milliseconds, the application will wait for 100-continue from the server before uploading data.")]
		TimeSpan ContinueTimeout { get; set; }

		/// <summary>
		/// The impersonation level determines how the server can use the client's credentials.
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.webrequesthandler.impersonationlevel(v=vs.110).aspx
		/// </summary>
		[DefaultValue(TokenImpersonationLevel.Delegation), DataMember(EmitDefaultValue = true),
         Description("The impersonation level determines how the server can use the client's credentials.")]
		TokenImpersonationLevel ImpersonationLevel { get; set; }

		/// <summary>
		/// See: https://msdn.microsoft.com/en-us/library/system.net.http.webrequesthandler.maxresponseheaderslength.aspx
		/// Default would have been 64, this is increased to 256
		/// </summary>
		[DefaultValue(256), Description("The max length, in kilobytes (1024 bytes), of the response headers."), DataMember(EmitDefaultValue = true)]
		int MaxResponseHeadersLength { get; set; }

	}
}
