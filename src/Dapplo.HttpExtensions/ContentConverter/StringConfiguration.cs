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
using Dapplo.HttpExtensions.Extensions;
using Dapplo.HttpExtensions.Support;

namespace Dapplo.HttpExtensions.ContentConverter
{
    /// <summary>
    ///     Configuration for the StringHttpContentConverter
    /// </summary>
    public class StringConfiguration : IHttpRequestConfiguration
    {
        /// <summary>
        ///     Specify the supported content types
        /// </summary>
        public IList<string> SupportedContentTypes { get; } = new List<string>
        {
            MediaTypes.Txt.EnumValueOf(),
            MediaTypes.Html.EnumValueOf(),
            MediaTypes.Xml.EnumValueOf(),
            MediaTypes.Json.EnumValueOf(),
            MediaTypes.XmlReadable.EnumValueOf()
        };


        /// <summary>
        ///     Name of the configuration, this should be unique and usually is the type of the object
        /// </summary>
        public string Name { get; } = nameof(StringConfiguration);
    }
}