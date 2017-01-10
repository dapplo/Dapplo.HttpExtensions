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

#region using

using System;

#endregion

namespace Dapplo.HttpExtensions
{
	/// <summary>
	///     This interface makes it possible to change the Json serializer which is used for de- serializing JSON.
	///     The default implementation for this, SimpleJson, is included in this project
	/// </summary>
	public interface IJsonSerializer
	{
		/// <summary>
		/// Test if the specified type can be serialized to JSON
		/// </summary>
		/// <param name="sourceType">Type to check</param>
		/// <returns>bool</returns>
		bool CanSerializeTo(Type sourceType);

		/// <summary>
		/// Test if the specified type can be deserialized
		/// </summary>
		/// <param name="targetType">Type to check</param>
		/// <returns>bool</returns>
		bool CanDeserializeFrom(Type targetType);

		/// <summary>
		///     Deserialize a string with Json to the specified type
		/// </summary>
		/// <param name="targetType">Type to deserialize to</param>
		/// <param name="jsonString">string with json content</param>
		/// <returns>an object of type targetType or null</returns>
		object Deserialize(Type targetType, string jsonString);

		/// <summary>
		///     Serialize the generic object into a string with Json content
		/// </summary>
		/// <typeparam name="T">Type to serialize</typeparam>
		/// <param name="jsonObject">Object to serialize</param>
		/// <returns>string with Json content</returns>
		string Serialize<T>(T jsonObject);
	}
}