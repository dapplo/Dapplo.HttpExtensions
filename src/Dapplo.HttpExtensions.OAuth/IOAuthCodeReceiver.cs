// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dapplo.HttpExtensions.OAuth
{
    /// <summary>
    ///     This is the interface for the OAuth code receiver
    /// </summary>
    public interface IOAuthCodeReceiver
    {
        /// <summary>
        ///     The actual code receiving code
        /// </summary>
        /// <param name="authorizeMode">AuthorizeModes will tell you for what mode you were called</param>
        /// <param name="codeReceiverSettings">ICodeReceiverSettings with the settings for the code receiver</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>Dictionary with the returned key-values</returns>
        Task<IDictionary<string, string>> ReceiveCodeAsync(AuthorizeModes authorizeMode, ICodeReceiverSettings codeReceiverSettings,
            CancellationToken cancellationToken = default);
    }
}