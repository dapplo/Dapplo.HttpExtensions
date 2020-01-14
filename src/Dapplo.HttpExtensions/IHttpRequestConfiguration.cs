// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.HttpExtensions
{
    /// <summary>
    ///     This interface is the base interface for configuration information.
    ///     It makes it possible to supply configuration to different parts of the library during a request, where as a caller
    ///     you normally don't interact with directly.
    ///     The interface only specifies the name of the configuration, specific implementations should be used.
    ///     Instances of this interface are added to the HttpBehaviour, so they are available throughout a request.
    /// </summary>
    public interface IHttpRequestConfiguration
    {
        /// <summary>
        ///     Name of the configuration, this should be unique
        /// </summary>
        string Name { get; }
    }
}