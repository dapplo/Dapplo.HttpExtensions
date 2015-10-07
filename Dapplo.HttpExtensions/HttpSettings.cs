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
using System.Net;
using System.Net.Security;
using System.Security.Principal;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// This class contains the default settings for the proxy / httpclient
	/// These can be modified, are on a global "application" scale.
	/// Most have their normal defaults, which would also normally be used, some have special settings
	/// The default values and the property descriptions are in the ISettings (which can be used by Dapplo.Config)
	/// </summary>
	public class HttpSettings : IHttpSettings
	{
		const int Kb = 1024;
		const int Mb = Kb * 1024;
		const long Gb = Mb * 1024;

		public static IHttpSettings Instance
		{
			get;
			set;
		} = new HttpSettings();

		public bool UseProxy { get; set; } = true;

		public bool UseDefaultProxy { get; set; } = true;

		public bool UseDefaultCredentialsForProy { get; set; } = true;

		public Uri ProxyUri { get; set; }

		public ICredentials ProxyCredentials{ get; set; }

		public bool ProxyBypassOnLocal { get; set; } = true;

		public string[] ProxyBypassList { get; set; }

		public bool UseCookies { get; set; } = true;

		public bool UseDefaultCredentials { get; set; } = true;

		public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(60);

		public bool AllowAutoRedirect { get; set; } = true;

		public DecompressionMethods DefaultDecompressionMethods { get; set; } = DecompressionMethods.Deflate | DecompressionMethods.GZip;

		public bool PreAuthenticate { get; set; } = true;

		public int MaxAutomaticRedirections { get; set; } = 50;

		public long MaxRequestContentBufferSize { get; set; } = (2 * Gb) - 1;

		public long MaxResponseContentBufferSize { get; set; } = (2 * Gb) - 1;
		
		public int ReadWriteTimeout { get; set; } = 300000;

		public bool AllowPipelining { get; set; } = true;

		public AuthenticationLevel AuthenticationLevel { get; set; } = AuthenticationLevel.MutualAuthRequested;

		public TimeSpan ContinueTimeout { get; set; } = TimeSpan.FromMilliseconds(350);

		public TokenImpersonationLevel ImpersonationLevel { get; set; } = TokenImpersonationLevel.Delegation;

		public int MaxResponseHeadersLength { get; set; } = 256;
	}
}
