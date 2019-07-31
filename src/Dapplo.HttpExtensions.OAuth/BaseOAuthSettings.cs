//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2019 Dapplo
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

#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

        /// <inheritdoc />
        public Uri AuthorizationUri { get; set; }

        /// <inheritdoc />
        public string RedirectUrl { get; set; }

        /// <inheritdoc />
        public string CloudServiceName { get; set; } = "the remote server";

        /// <inheritdoc />
        public string ClientId { get; set; }

        /// <inheritdoc />
        public string ClientSecret { get; set; }

        /// <inheritdoc />
        public string State { get; set; } = Guid.NewGuid().ToString();

        /// <inheritdoc />
        public int EmbeddedBrowserWidth { get; set; } = 600;

        /// <inheritdoc />
        public int EmbeddedBrowserHeight { get; set; } = 400;

        /// <inheritdoc />
        public TaskScheduler UiTaskScheduler { get; set; }
        #endregion
    }
}