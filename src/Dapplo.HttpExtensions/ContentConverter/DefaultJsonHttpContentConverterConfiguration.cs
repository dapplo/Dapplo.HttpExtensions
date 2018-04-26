//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2018 Dapplo
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

namespace Dapplo.HttpExtensions.ContentConverter
{
    /// <summary>
    ///     This is a configuration class for the DefaultJsonHttpContentConverter
    /// </summary>
    public class DefaultJsonHttpContentConverterConfiguration : IHttpRequestConfiguration
    {
        /// <summary>
        ///     Name of the configuration, this should be unique and usually is the type of the object
        /// </summary>
        public string Name { get; } = nameof(DefaultJsonHttpContentConverterConfiguration);

        /// <summary>
        ///     If the json content is any longer than LogThreshold AppendedWhenCut is appended to the cut string
        /// </summary>
        public string AppendedWhenCut { get; set; } = "...";

        /// <summary>
        ///     This is the amount of characters that are written to the log, if the json content is any longer that it will be cut
        ///     (and AppendedWhenCut is appended)
        /// </summary>
        public int LogThreshold { get; set; } = 256;
    }
}