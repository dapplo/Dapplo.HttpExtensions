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


#endregion

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