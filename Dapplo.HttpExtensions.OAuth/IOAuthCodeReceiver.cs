//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2015-2017 Dapplo
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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#endregion

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
            CancellationToken cancellationToken = default(CancellationToken));
    }
}