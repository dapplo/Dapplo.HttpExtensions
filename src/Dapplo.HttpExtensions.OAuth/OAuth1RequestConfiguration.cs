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

using System.Collections.Generic;

namespace Dapplo.HttpExtensions.OAuth
{
    /// <summary>
    ///     Specify OAuth 1 request configuration
    /// </summary>
    public class OAuth1RequestConfiguration : IHttpRequestConfiguration
    {
        /// <summary>
        ///     The OAuth parameters for this request
        /// </summary>
        public IDictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();

        /// <summary>
        ///     Name of the configuration, this should be unique and usually is the type of the object
        /// </summary>
        public string Name { get; } = nameof(OAuth1RequestConfiguration);
    }
}