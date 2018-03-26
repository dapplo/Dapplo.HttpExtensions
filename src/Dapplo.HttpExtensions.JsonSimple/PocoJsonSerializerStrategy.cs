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

#region Usings

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

#endregion

namespace Dapplo.HttpExtensions.JsonSimple
{
    [GeneratedCode("simple-json", "1.0.0")]
#if SIMPLE_JSON_INTERNAL
	internal
#else
    public
#endif
        class PocoJsonSerializerStrategy : IJsonSerializerStrategy
    {
        internal readonly IDictionary<Type, ReflectionUtils.ConstructorDelegate> ConstructorCache;
        internal IDictionary<Type, IDictionary<string, ReflectionUtils.GetDelegate>> GetCache;
        internal IDictionary<Type, IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>> SetCache;
        internal IDictionary<Type, IDictionary<string, Func<object, bool>>> EmitPredicateCache;

        internal static readonly Type[] EmptyTypes = new Type[0];
        internal static readonly Type[] ArrayConstructorParameterTypes = {typeof(int)};

        private static readonly string[] Iso8601Format =
        {
            @"yyyy-MM-dd\THH:mm:ss.FFFFFFFK",
            @"yyyy-MM-dd\THH:mm:ss.FFFFFFF\Z",
            @"yyyy-MM-dd\THH:mm:ss\Z",
            @"yyyy-MM-dd\THH:mm:ssK",
            @"yyyy-MM-dd\THH:mm:ss.FFFFFFF",
            @"yyyy-MM-dd"
        };

        public PocoJsonSerializerStrategy()
        {
            ConstructorCache = new ReflectionUtils.ThreadSafeDictionary<Type, ReflectionUtils.ConstructorDelegate>(ContructorDelegateFactory);
            GetCache = new ReflectionUtils.ThreadSafeDictionary<Type, IDictionary<string, ReflectionUtils.GetDelegate>>(GetterValueFactory);
            SetCache = new ReflectionUtils.ThreadSafeDictionary<Type, IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>>(SetterValueFactory);
        }

        protected virtual string MapClrMemberNameToJsonFieldName(string clrPropertyName)
        {
            return clrPropertyName;
        }

        internal virtual IDictionary<string, Func<object, bool>> EmitPredicateFactory(Type type)
        {
            return null;
        }

        internal virtual ReflectionUtils.ConstructorDelegate ContructorDelegateFactory(Type key)
        {
            return ReflectionUtils.GetContructor(key, key.IsArray ? ArrayConstructorParameterTypes : EmptyTypes);
        }

        /// <summary>
        ///     Get all the PropertyInfo objects for the type which have a JsonExtensionDataAttribute
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>IEnumerable with PropertyInfo</returns>
        internal IEnumerable<PropertyInfo> JsonExtensionDataPropertyInfos(Type type)
        {
            foreach (var propertyInfo in ReflectionUtils.GetProperties(type))
            {
                var getMethod = ReflectionUtils.GetGetterMethodInfo(propertyInfo);
                if (getMethod.IsStatic || !getMethod.IsPublic)
                {
                    continue;
                }
                var jsonExtensionDataAtrribute = propertyInfo.GetCustomAttribute<JsonExtensionDataAttribute>();
                if (jsonExtensionDataAtrribute != null)
                {
                    yield return propertyInfo;
                }
            }
        }

        /// <summary>
        ///     Get all the FieldInfo objects for the type which have a JsonExtensionDataAttribute
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>IEnumerable with FieldInfo</returns>
        internal IEnumerable<FieldInfo> JsonExtensionDataFieldInfo(Type type)
        {
            foreach (var fieldInfo in ReflectionUtils.GetFields(type))
            {
                if (fieldInfo.IsStatic || !fieldInfo.IsPublic)
                {
                    continue;
                }
                var jsonExtensionDataAtrribute = fieldInfo.GetCustomAttribute<JsonExtensionDataAttribute>();
                if (jsonExtensionDataAtrribute != null)
                {
                    yield return fieldInfo;
                }
            }
        }

        internal virtual IDictionary<string, ReflectionUtils.GetDelegate> GetterValueFactory(Type type)
        {
            IDictionary<string, ReflectionUtils.GetDelegate> result = new Dictionary<string, ReflectionUtils.GetDelegate>();
            foreach (var propertyInfo in ReflectionUtils.GetProperties(type))
            {
                if (!propertyInfo.CanRead)
                {
                    continue;
                }
                var getMethod = ReflectionUtils.GetGetterMethodInfo(propertyInfo);
                if (getMethod.IsStatic || !getMethod.IsPublic)
                {
                    continue;
                }
                result[MapClrMemberNameToJsonFieldName(propertyInfo.Name)] = ReflectionUtils.GetGetMethod(propertyInfo);
            }
            foreach (var fieldInfo in ReflectionUtils.GetFields(type))
            {
                if (fieldInfo.IsStatic || !fieldInfo.IsPublic)
                {
                    continue;
                }
                result[MapClrMemberNameToJsonFieldName(fieldInfo.Name)] = ReflectionUtils.GetGetMethod(fieldInfo);
            }
            return result;
        }

        internal virtual IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>> SetterValueFactory(Type type)
        {
            IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>> result = new Dictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>();
            foreach (var propertyInfo in ReflectionUtils.GetProperties(type))
            {
                if (!propertyInfo.CanWrite)
                {
                    continue;
                }
                var setMethod = ReflectionUtils.GetSetterMethodInfo(propertyInfo);
                if (setMethod.IsStatic || !setMethod.IsPublic)
                {
                    continue;
                }
                result[MapClrMemberNameToJsonFieldName(propertyInfo.Name)] = new KeyValuePair<Type, ReflectionUtils.SetDelegate>(propertyInfo.PropertyType,
                    ReflectionUtils.GetSetMethod(propertyInfo));
            }
            foreach (var fieldInfo in ReflectionUtils.GetFields(type))
            {
                if (fieldInfo.IsInitOnly || fieldInfo.IsStatic || !fieldInfo.IsPublic)
                {
                    continue;
                }
                result[MapClrMemberNameToJsonFieldName(fieldInfo.Name)] = new KeyValuePair<Type, ReflectionUtils.SetDelegate>(fieldInfo.FieldType,
                    ReflectionUtils.GetSetMethod(fieldInfo));
            }
            return result;
        }

        public virtual bool TrySerializeNonPrimitiveObject(object input, out object output)
        {
            return TrySerializeKnownTypes(input, out output) || TrySerializeUnknownTypes(input, out output);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public virtual object DeserializeObject(object value, Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            var str = value as string;

            if (type == typeof(Guid) && string.IsNullOrEmpty(str))
            {
                return default(Guid);
            }

            if (value == null)
            {
                return null;
            }

            object obj = null;

            if (str != null)
            {
                if (str.Length != 0) // We know it can't be null now.
                {
                    if (type == typeof(DateTime) || ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(DateTime))
                    {
                        return DateTime.ParseExact(str, Iso8601Format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                    }
                    if (type == typeof(DateTimeOffset) || ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(DateTimeOffset))
                    {
                        return DateTimeOffset.ParseExact(str, Iso8601Format, CultureInfo.InvariantCulture,
                            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                    }
                    if (type == typeof(Guid) || ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(Guid))
                    {
                        return new Guid(str);
                    }
                    if (type == typeof(Uri))
                    {
                        var isValid = Uri.IsWellFormedUriString(str, UriKind.RelativeOrAbsolute);

                        if (isValid && Uri.TryCreate(str, UriKind.RelativeOrAbsolute, out var result))
                        {
                            return result;
                        }

                        return null;
                    }

                    if (type.GetTypeInfo().IsEnum || ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type).GetTypeInfo().IsEnum)
                    {
                        return Enum.Parse(ReflectionUtils.IsNullableType(type) ? Nullable.GetUnderlyingType(type) : type, str, true);
                    }

                    if (type == typeof(string))
                    {
                        return str;
                    }

                    return Convert.ChangeType(str, type, CultureInfo.InvariantCulture);
                }
                if (type == typeof(Guid))
                {
                    obj = default(Guid);
                }
                else
                {
                    obj = str;
                }
                // Empty string case
                if (!ReflectionUtils.IsNullableType(type) && Nullable.GetUnderlyingType(type) == typeof(Guid))
                {
                    return str;
                }
            }
            else if (value is bool)
            {
                return value;
            }

            var valueIsLong = value is long;
            var valueIsDouble = value is double;
            if (valueIsLong && type == typeof(long) || valueIsDouble && type == typeof(double))
            {
                return value;
            }

            var objects = value as IDictionary<string, object>;
            if (valueIsDouble && type != typeof(double) || valueIsLong && type != typeof(long))
            {
                obj = type == typeof(int) || type == typeof(long) || type == typeof(double) || type == typeof(float) || type == typeof(bool) || type == typeof(decimal) ||
                      type == typeof(byte) || type == typeof(short)
                    ? Convert.ChangeType(value, type, CultureInfo.InvariantCulture)
                    : value;
            }
            else
            {
                if (objects != null)
                {
                    var jsonObject = objects;

                    if (ReflectionUtils.IsTypeDictionary(type))
                    {
                        // if dictionary then
                        var types = ReflectionUtils.GetGenericTypeArguments(type);
                        var keyType = types[0];
                        var valueType = types[1];

                        var genericType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);

                        var dict = (IDictionary) ConstructorCache[genericType]();

                        foreach (var kvp in jsonObject)
                        {
                            dict.Add(kvp.Key, DeserializeObject(kvp.Value, valueType));
                        }

                        obj = dict;
                    }
                    else
                    {
                        if (type == typeof(object))
                        {
                            obj = value;
                        }
                        else
                        {
                            obj = ConstructorCache[type]();
                            foreach (var setter in SetCache[type])
                            {
                                if (jsonObject.TryGetValue(setter.Key, out var jsonValue))
                                {
                                    try
                                    {
                                        jsonValue = DeserializeObject(jsonValue, setter.Value.Key);
                                        if (jsonValue != null)
                                        {
                                            setter.Value.Value(obj, jsonValue);
                                            // Value was processed, remove it from the Dictionary
                                            jsonObject.Remove(setter.Key);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        if (ex is ArgumentException)
                                        {
                                            throw;
                                        }
                                        // Added additiona information for easier debugging
                                        throw new ArgumentException(
                                            $"Json field \"{setter.Key}\" with Value \"{jsonValue}\" cannot be converted to Type {setter.Value.Key}", ex);
                                    }
                                }
                            }
                            // Check if there is any unprocessed data, if so check if there is a JsonExtensionDataAttribute inside the target object
                            if (jsonObject.Count <= 0)
                            {
                                return obj;
                            }
                            // The attribute should be set on a value which has a IDictionary instance (not null)
                            foreach (var extensionPropertyInfo in type.GetProperties())
                            {
                                var jsonExtensionDataAtrribute = extensionPropertyInfo.GetCustomAttribute<JsonExtensionDataAttribute>();
                                if (jsonExtensionDataAtrribute == null)
                                {
                                    continue;
                                }
                                var matchRegex = new Regex(jsonExtensionDataAtrribute.Pattern ?? ".*");
                                if (extensionPropertyInfo?.GetValue(obj) is IDictionary extensionData)
                                {
                                    // Get the type, if possible, so we can convert
                                    var valueType = typeof(string);
                                    var genericArguments = extensionData.GetType().GetGenericArguments();
                                    if (genericArguments.Length == 2)
                                    {
                                        valueType = genericArguments[1];
                                    }
                                    foreach (var key in jsonObject.Keys.ToList())
                                    {
                                        // Only keys which match
                                        if (!matchRegex.IsMatch(key))
                                        {
                                            continue;
                                        }
                                        var jsonValue = jsonObject[key];
                                        if (jsonValue == null && valueType.GetTypeInfo().IsValueType)
                                        {
                                            // no value, but we need to add it... create instance
                                            jsonValue = Activator.CreateInstance(valueType);
                                        }
                                        else if (jsonValue != null && !valueType.IsInstanceOfType(jsonValue))
                                        {
                                            if (jsonValue is IConvertible)
                                            {
                                                jsonValue = Convert.ChangeType(jsonValue, valueType);
                                            }
                                            else
                                            {
                                                // Prevent errors, do not convert or add the value
                                                continue;
                                            }
                                        }
                                        extensionData.Add(key, jsonValue);
                                        // Remove it, at it was matched
                                        jsonObject.Remove(key);
                                    }
                                }
                                // Nothing more to process
                                if (jsonObject.Count == 0)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (value is IList<object> valueAsList)
                    {
                        var jsonObject = valueAsList;
                        IList list = null;

                        if (type.IsArray)
                        {
                            list = (IList) ConstructorCache[type](jsonObject.Count);
                            var i = 0;
                            foreach (var o in jsonObject)
                            {
                                list[i++] = DeserializeObject(o, type.GetElementType());
                            }
                        }
                        else if (ReflectionUtils.IsTypeGenericeCollectionInterface(type) || ReflectionUtils.IsAssignableFrom(typeof(IList), type))
                        {
                            var innerType = ReflectionUtils.GetGenericListElementType(type);
                            list = (IList) (ConstructorCache[type] ?? ConstructorCache[typeof(List<>).MakeGenericType(innerType)])(jsonObject.Count);
                            foreach (var o in jsonObject)
                            {
                                list.Add(DeserializeObject(o, innerType));
                            }
                        }
                        obj = list;
                    }
                }
                return obj;
            }
            if (ReflectionUtils.IsNullableType(type))
            {
                return ReflectionUtils.ToNullableType(obj, type);
            }
            return obj;
        }

        protected virtual object SerializeEnum(Enum enumerationItem)
        {
            var attributes = (EnumMemberAttribute[])enumerationItem.GetType().GetRuntimeField(enumerationItem.ToString()).GetCustomAttributes(typeof(EnumMemberAttribute), false);
            return attributes.Length > 0 ? attributes[0].Value : enumerationItem.ToString();
        }

        [SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "Need to support .NET 2")]
        protected virtual bool TrySerializeKnownTypes(object input, out object output)
        {
            var returnValue = true;
            if (input is DateTime time)
            {
                output = time.ToUniversalTime().ToString(Iso8601Format[0], CultureInfo.InvariantCulture);
            }
            else if (input is DateTimeOffset)
            {
                var dateTime = ((DateTimeOffset) input).ToUniversalTime();
                string sign = dateTime.Offset < TimeSpan.Zero ? "-" : "+";
                int offsetHours = Math.Abs(dateTime.Offset.Hours);
                int offsetMinutes = Math.Abs(dateTime.Offset.Minutes);
                output = $"{dateTime.ToUniversalTime().ToString(Iso8601Format[4], CultureInfo.InvariantCulture)}{sign}{offsetHours:00}{offsetMinutes:00}";
            }
            else if (input is Guid)
            {
                output = ((Guid) input).ToString("D");
            }
            else if (input is Uri)
            {
                output = input.ToString();
            }
            else
            {
                if (input is Enum inputEnum)
                {
                    output = SerializeEnum(inputEnum);
                }
                else
                {
                    returnValue = false;
                    output = null;
                }
            }
            return returnValue;
        }

        private void AddExtensionData(Type type, object input, IDictionary<string, object> output)
        {
            foreach (var jsonExtensionDataMember in JsonExtensionDataPropertyInfos(type))
            {
                var value = (IDictionary) jsonExtensionDataMember.GetValue(input);
                if (value == null)
                {
                    continue;
                }
                foreach (var keyObject in value.Keys)
                {
                    var key = keyObject as string;
                    output.Add(key, value[keyObject]);
                }
            }

            foreach (var jsonExtensionDataMember in JsonExtensionDataFieldInfo(type))
            {
                if (!(jsonExtensionDataMember.GetValue(input) is IDictionary value))
                {
                    continue;
                }
                foreach (var keyObject in value.Keys)
                {
                    var key = keyObject as string;
                    output.Add(key, value[keyObject]);
                }
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1007:UseGenericsWhereAppropriate", Justification = "Need to support .NET 2")]
        protected virtual bool TrySerializeUnknownTypes(object input, out object output)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            output = null;
            var type = input.GetType();
            if (type.FullName == null)
            {
                return false;
            }
            IDictionary<string, object> obj = new JsonObject();

            // Add the extension data to the output
            AddExtensionData(type, input, obj);

            var getters = GetCache[type];
            var emitPredicate = EmitPredicateCache?[type];
            foreach (var getter in getters)
            {
                if (getter.Value != null)
                {
                    var value = getter.Value(input);
                    if (emitPredicate?.ContainsKey(getter.Key) == true)
                    {
                        if (!emitPredicate[getter.Key](value))
                        {
                            continue;
                        }
                    }
                    obj.Add(MapClrMemberNameToJsonFieldName(getter.Key), value);
                }
            }
            output = obj;
            return true;
        }
    }
}