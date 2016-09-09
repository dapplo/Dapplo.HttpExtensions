//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2015-2016 Dapplo
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

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// Use this to configure the HttpRequestMessage, created in the HttpRequestMessageFactory
	/// </summary>
	public class HttpRequestMessageConfiguration : IHttpRequestConfiguration
	{
		/// <summary>
		/// Name of the configuration, this should be unique and usually is the type of the object
		/// </summary>
		public string Name { get; } = nameof(HttpRequestMessageConfiguration);

		/// <summary>
		/// A set of properties for the HTTP request.
		/// </summary>
		public IDictionary<string, object> Properties { get; }= new Dictionary<string, object>();
	}
}
