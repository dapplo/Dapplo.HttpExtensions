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

using Dapplo.Utils;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;

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
#if _PCL_
	public interface IHttpSettings : IShallowCloneable<IHttpSettings>
#else
	public partial interface IHttpSettings : IShallowCloneable<IHttpSettings>
#endif
	{
		/// <summary>
		///     For more details, click
		///     <a href="https://msdn.microsoft.com/en-us/library/system.net.http.httpclienthandler.allowautoredirect.aspx">here</a>
		/// </summary>
		[DefaultValue(true), Display(Description = "When true a connection would automatically redirect, if the server says so"), DataMember(EmitDefaultValue = true)]
		bool AllowAutoRedirect { get; set; }

		/// <summary>
		///     For more details, click
		///     <a href="https://msdn.microsoft.com/en-us/library/system.net.http.httpclienthandler.credentials.aspx">here</a>
		/// </summary>
		[Display(Description = "The credentials for the request, only used when UseDefaultCredentials is set to false")]
		ICredentials Credentials { get; set; }

		/// <summary>
		/// For more details, click
		///     <a href="https://msdn.microsoft.com/en-us/library/system.net.http.httpclienthandler.clientcertificateoptions.aspx">here</a>
		/// </summary>
		[DefaultValue(ClientCertificateOption.Automatic), Display(Description = "A value that indicates if the certificate is automatically picked from the certificate store or if the caller is allowed to pass in a specific client certificate.")]
		ClientCertificateOption ClientCertificateOptions { get; set; }

		/// <summary>
		///     For more details, click
		///     <a href="https://msdn.microsoft.com/en-us/library/system.net.http.httpclienthandler.automaticdecompression.aspx">here</a>
		/// </summary>
		[DefaultValue(DecompressionMethods.Deflate | DecompressionMethods.GZip), Display(Description = "Decompression methods used")]
		DecompressionMethods DefaultDecompressionMethods { get; set; }

		/// <summary>
		/// The default User-Agent value to use, a lot of services don't like it when this is empty or the behaviour depends on the value
		/// </summary>
		[Display(Description = "The default User-Agent value to use, a lot of services don't like it when this is empty or the behaviour depends on the value")]
		string DefaultUserAgent { get; set; }

		/// <summary>
		/// When true the configured proxy will used the default user credentials
		/// </summary>
		[DefaultValue(false), Display(Description = "When true the configured proxy will used the default user credentials"), DataMember(EmitDefaultValue = true)]
		bool Expect100Continue { get; set; }

		/// <summary>
		///     For more details, click
		///     <a href="https://msdn.microsoft.com/en-us/library/system.net.http.httpclienthandler.maxautomaticredirections.aspx">here</a>
		///     and <a href="https://msdn.microsoft.com/en-us/library/system.net.httpwebrequest.allowautoredirect.aspx">here</a>
		/// </summary>
		[DefaultValue(50), Display(Description = "The maximum amount of redirections that are followed")]
		int MaxAutomaticRedirections { get; set; }

		/// <summary>
		///     For more details, click
		///     <a href="https://msdn.microsoft.com/en-us/library/system.net.http.httpclient.maxresponsecontentbuffersize.aspx">here</a>
		/// </summary>
		[DefaultValue(2*1024*1024*1024L - 1L), Display(Description = "Max response content buffer size")]
		long MaxResponseContentBufferSize { get; set; }

		/// <summary>
		///     For more details, click
		///     <a href="https://msdn.microsoft.com/en-us/library/system.net.http.httpclienthandler.preauthenticate.aspx">here</a>
		///     And
		///     <a href="http://weblog.west-wind.com/posts/2010/Feb/18/NET-WebRequestPreAuthenticate-not-quite-what-it-sounds-like">here</a>
		/// </summary>
		[DefaultValue(false), Display(Description = "When true the request is directly send with a HTTP Authorization header."), DataMember(EmitDefaultValue = true)]
		bool PreAuthenticate { get; set; }

		/// <summary>
		///     For more details, click
		///     <a href="https://msdn.microsoft.com/en-us/library/system.net.http.httpclient.timeout.aspx">here</a>
		/// </summary>
		[DefaultValue("0:01:40"), Display(Description = "Request timeout"), DataMember(EmitDefaultValue = true)]
		TimeSpan RequestTimeout { get; set; }

		/// <summary>
		///     For more details, click
		///     <a href="https://msdn.microsoft.com/en-us/library/system.net.http.httpclienthandler.usecookies.aspx">here</a>
		///     And
		///     <a href="https://msdn.microsoft.com/en-us/library/system.net.http.httpclienthandler.cookiecontainer.aspx">here</a>
		/// </summary>
		[DefaultValue(true), Display(Description = "Should requests store & resend cookies?"), DataMember(EmitDefaultValue = true)]
		bool UseCookies { get; set; }

		/// <summary>
		/// When true every http request will supply the default user credentials when the server asks for them
		/// </summary>
		[DefaultValue(true), Display(Description = "When true every http request will supply the default user credentials when the server asks for them"), DataMember(EmitDefaultValue = true)]
		bool UseDefaultCredentials { get; set; }
	}
}