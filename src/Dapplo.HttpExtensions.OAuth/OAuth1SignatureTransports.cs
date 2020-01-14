// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.HttpExtensions.OAuth
{
    /// <summary>
    ///     Used to define the transport which the signature takes, in the headers or as query parameters
    /// </summary>
    public enum OAuth1SignatureTransports
    {
        /// <summary>
        ///     Place the signature information in the headers of the request, this is the default
        /// </summary>
        Headers,

        /// <summary>
        ///     Place the signature information in the query parameters of the request
        /// </summary>
        QueryParameters
    }
}