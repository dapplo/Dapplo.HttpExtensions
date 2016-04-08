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

namespace Dapplo.HttpExtensions.Support
{
	/// <summary>
	///     This defines the default way how Json is de-/serialized.
	/// </summary>
	public class SimpleJsonSerializer : IJsonSerializer
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetType">Type to deserialize from a json string</param>
		/// <param name="jsonString">json</param>
		/// <returns>Deserialized json as targetType</returns>
		public object DeserializeJson(Type targetType, string jsonString)
		{
			return SimpleJson.DeserializeObject(jsonString, targetType);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TResult">Type to deserialize from a json string</typeparam>
		/// <param name="jsonString">json</param>
		/// <returns>Deserialized json as TResult</returns>
		public TResult DeserializeJson<TResult>(string jsonString) where TResult : class
		{
			return SimpleJson.DeserializeObject<TResult>(jsonString);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TContent">Type to serialize to json string</typeparam>
		/// <param name="jsonObject">The actual object</param>
		/// <returns>string with json</returns>
		public string SerializeJson<TContent>(TContent jsonObject)
		{
			return SimpleJson.SerializeObject(jsonObject);
		}
	}
}