// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
    }
}