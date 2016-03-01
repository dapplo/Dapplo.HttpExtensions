/*
	Dapplo - building blocks for desktop applications
	Copyright (C) 2015-2016 Dapplo

	For more information see: http://dapplo.net/
	Dapplo repositories are hosted on GitHub: https://github.com/dapplo

	This file is part of Dapplo.HttpExtensions.

	Dapplo.HttpExtensions is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	Dapplo.HttpExtensions is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with Dapplo.HttpExtensions. If not, see <http://www.gnu.org/licenses/>.
 */

using System;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// This interface makes it possible to change the Json serializer which is used for de- serializing JSON.
	/// As a default implementation is used, using SimpleJson.
	/// </summary>
	public interface IJsonSerializer
	{
		/// <summary>
		/// Deserialize a string with Json to the generic type
		/// </summary>
		/// <typeparam name="TResult">a class which is used to deserialize to</typeparam>
		/// <param name="jsonString">string with json content</param>
		/// <returns>TResult</returns>
		TResult DeserializeJson<TResult>(string jsonString) where TResult : class;

		/// <summary>
		/// Deserialize a string with Json to the specified type
		/// </summary>
		/// <param name="targetType">Type to deserialize to</param>
		/// <param name="jsonString">string with json content</param>
		/// <returns>an object of type targetType or null</returns>
		object DeserializeJson(Type targetType, string jsonString);

		/// <summary>
		/// Serialize the generic object into a string with Json content
		/// </summary>
		/// <typeparam name="T">Type to serialize</typeparam>
		/// <param name="jsonObject">Object to serialize</param>
		/// <returns>string with Json content</returns>
		string SerializeJson<T>(T jsonObject);
	}
}
