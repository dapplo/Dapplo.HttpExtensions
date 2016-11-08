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
using System.Collections.Generic;
using System.Threading;

#endregion

namespace Dapplo.HttpExtensions.OAuth
{
	/// <summary>
	///     Common properties for the OauthXSettings
	/// </summary>
	public abstract class BaseOAuthSettings : ICodeReceiverSettings
	{
		/// <summary>
		///     Put anything in here which is needed for the OAuth implementation of this specific service but isn't generic, e.g.
		///     for Google there is a "scope"
		/// </summary>
		public IDictionary<string, string> AdditionalAttributes { get; set; } = new Dictionary<string, string>();

		/// <summary>
		///     The AuthorizeMode for this OAuth settings
		/// </summary>
		public AuthorizeModes AuthorizeMode { get; set; } = AuthorizeModes.Unknown;

		/// <summary>
		///     This makes sure than the OAuth request that needs to authenticate blocks all others until it's ready.
		/// </summary>
		public SemaphoreSlim Lock { get; } = new SemaphoreSlim(1, 1);

		/// <summary>
		///     The URL to get a (request) token
		/// </summary>
		public Uri TokenUrl { get; set; }

		#region ICodeReceiverSettings

		/// <summary>
		///     The autorization Uri where the values of this class will be "injected"
		///     Example how this can be created:
		///     <code>
		/// new Uri("http://server").AppendSegments("auth").Query("client_id", "{ClientId}");
		/// </code>
		/// </summary>
		public Uri AuthorizationUri { get; set; }

		/// <summary>
		///     This is the redirect URL, in some implementations this is automatically set (LocalServerCodeReceiver)
		///     In some implementations this could be e.g. urn:ietf:wg:oauth:2.0:oob or urn:ietf:wg:oauth:2.0:oob:auto
		/// </summary>
		public string RedirectUrl { get; set; }

		/// <summary>
		///     Specify the name of the cloud service, so it can be used in window titles, logs etc
		/// </summary>
		public string CloudServiceName { get; set; } = "the remote server";

		/// <summary>
		///     The OAuth client id / consumer key
		/// </summary>
		public string ClientId { get; set; }

		/// <summary>
		///     The OAuth client/consumer secret
		///     For OAuth1SignatureTypes.RsaSha1 use RsaSha1Provider instead!
		/// </summary>
		public string ClientSecret { get; set; }

		/// <summary>
		///     The OAuth state, this is something that is passed to the server, is not processed but returned back to the client.
		///     e.g. a correlation ID
		///     Default this is filled with a new Guid
		/// </summary>
		public string State { get; set; } = Guid.NewGuid().ToString();

		/// <summary>
		///     This can be used to specify the width of the embedded browser window
		/// </summary>
		public int EmbeddedBrowserWidth { get; set; } = 600;

		/// <summary>
		///     This can be used to specify the height of the embedded browser window
		/// </summary>
		public int EmbeddedBrowserHeight { get; set; } = 400;

		#endregion
	}
}