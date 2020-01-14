// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Dapplo.HttpExtensions
{
    /// <summary>
    ///     Use this to configure the HttpRequestMessage, created in the HttpRequestMessageFactory
    /// </summary>
    public class HttpRequestMessageConfiguration : IHttpRequestConfiguration
    {
        /// <summary>
        ///     Name of the configuration, this should be unique and usually is the type of the object
        /// </summary>
        public string Name { get; } = nameof(HttpRequestMessageConfiguration);

        /// <summary>
        ///     A set of properties for the HTTP request.
        /// </summary>
        public IDictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

        /// <summary>
        ///     The HTTP Message version, default is 1.1
        /// </summary>
        public Version HttpMessageVersion { get; set; } = new Version(1, 1);
    }
}