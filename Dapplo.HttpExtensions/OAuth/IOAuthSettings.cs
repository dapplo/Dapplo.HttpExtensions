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
	along with Dapplo.HttpExtensions. If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;

namespace Dapplo.HttpExtensions.OAuth
{
	/// <summary>
	/// Settings interface the OAuth (2) protocol
	/// </summary>
	public interface IOAuthSettings
	{
		/// <summary>
		/// Specify the name of the cloud service, so it can be used in window titles, logs etc
		/// </summary>
		string CloudServiceName { get; set; }

		/// <summary>
		/// The OAuth (2) client id
		/// </summary>
		string ClientId { get; set; }

		/// <summary>
		/// The OAuth (2) client secret
		/// </summary>
		string ClientSecret { get; set; }

		/// <summary>
		/// The OAuth (2) state, this is something that is passed to the server, is not processed but returned back to the client.
		/// e.g. a correlation ID
		/// Default this is filled with a new Guid
		/// </summary>
		string State { get; set; }

		/// <summary>
		/// The autorization Uri where the values of this class will be "injected"
		/// Example how this can be created:
		/// <code>
		/// new Uri("http://server").AppendSegments("auth").Query("client_id", "{ClientId}");
		/// </code>
		/// </summary>
		Uri AuthorizationUri
		{
			get;
			set;
		}

		/// <summary>
		/// The URL to get a Token
		/// </summary>
		Uri TokenUrl
		{
			get;
			set;
		}

		/// <summary>
		/// This is the redirect URL, in some implementations this is automatically set (LocalServerCodeReceiver)
		/// In some implementations this could be e.g. urn:ietf:wg:oauth:2.0:oob or urn:ietf:wg:oauth:2.0:oob:auto
		/// </summary>
		string RedirectUrl { get; set; }

		/// <summary>
		/// Put anything in here which is needed for the OAuth (2) implementation of this specific service but isn't generic, e.g. for Google there is a "scope"
		/// </summary>
		IDictionary<string, string> AdditionalAttributes { get; set; }
	}
}
