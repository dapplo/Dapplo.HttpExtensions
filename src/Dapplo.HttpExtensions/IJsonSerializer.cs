// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.HttpExtensions;

/// <summary>
///     This interface makes it possible to change the Json serializer which is used for de- serializing JSON.
///     The default implementation for this, SimpleJson, is included in this project
/// </summary>
public interface IJsonSerializer
{
    /// <summary>
    ///     Test if the specified type can be serialized to JSON
    /// </summary>
    /// <param name="sourceType">Type to check</param>
    /// <returns>bool</returns>
    bool CanSerializeTo(Type sourceType);

    /// <summary>
    ///     Test if the specified type can be deserialized
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