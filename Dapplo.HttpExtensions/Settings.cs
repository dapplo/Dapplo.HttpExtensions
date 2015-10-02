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

using System.Net;

namespace Dapplo.HttpExtensions
{
	public static class Settings
	{
		/// <summary>
		/// Configuration for deciding if a Proxy is used when creating the HttpClient in the extensions.
		/// </summary>
		public static bool UseProxy { get; set; } = true;

		/// <summary>
		/// Configuration for deciding if a cookie-container is used when creating the HttpClient in the extensions.
		/// </summary>
		public static bool UseCookies { get; set; } = true;

		/// <summary>
		/// Configuration for deciding if the default credentials are used when creating the HttpClient in the extensions.
		/// </summary>
		public static bool UseDefaultCredentials { get; set; } = true;

		/// <summary>
		/// Configuration for the connection timeout for each HttpClient used by the extensions.
		/// </summary>
		public static int ConnectionTimeout { get; set; } = 60;

		/// <summary>
		/// Configuration for setting the allow auto redirect when creating the HttpClient in the extensions.
		/// </summary>
		public static bool AllowAutoRedirect { get; set; } = true;

		/// <summary>
		/// Configure the decompression methods for the connection
		/// </summary>
		public static DecompressionMethods DefaultDecompressionMethods { get; set; } = DecompressionMethods.Deflate | DecompressionMethods.GZip;

	}
}
