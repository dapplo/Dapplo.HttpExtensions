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

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// This interface is the base interface for configuration information.
	/// It makes it possible to supply configuration to different parts of the library during a request, where as a caller you normally don't interact with directly.
	/// The interface only specifies the name of the configuration, specific implementations should be used.
	/// Instances of this interface are added to the HttpBehaviour, so they are available throughout a request.
	/// </summary>
	public interface IHttpRequestConfiguration
	{
		/// <summary>
		/// Name of the configuration, this should be unique
		/// </summary>
		string Name { get; }
	}
}
