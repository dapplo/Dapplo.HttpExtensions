//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2017 Dapplo
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;

#endregion

namespace Dapplo.HttpExtensions.JsonSimple
{
    [GeneratedCode("simple-json", "1.0.0")]
#if SIMPLE_JSON_INTERNAL
    internal
#else
    public
#endif
        class DataContractJsonSerializerStrategy : PocoJsonSerializerStrategy
    {
        public DataContractJsonSerializerStrategy()
        {
            GetCache = new ReflectionUtils.ThreadSafeDictionary<Type, IDictionary<string, ReflectionUtils.GetDelegate>>(GetterValueFactory);
            SetCache = new ReflectionUtils.ThreadSafeDictionary<Type, IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>>(SetterValueFactory);
            EmitPredicateCache = new ReflectionUtils.ThreadSafeDictionary<Type, IDictionary<string, Func<object, bool>>>(EmitPredicateFactory);
        }

        /// <summary>
        ///     Helper method to supply the name of the json key, either from the DataMemberAttribute or from the MemberInfo
        /// </summary>
        /// <param name="dataMemberAttribute">DataMemberAttribute</param>
        /// <param name="memberInfo"></param>
        /// <returns>string with the name in the Json</returns>
        private string JsonKey(DataMemberAttribute dataMemberAttribute, MemberInfo memberInfo)
        {
            return string.IsNullOrEmpty(dataMemberAttribute.Name) ? memberInfo.Name : dataMemberAttribute.Name;
        }

        /// <summary>
        ///     Create a default value for a type, this usually is "null" for reference type, but for other, e.g. bool it's false
        ///     or for int it's 0
        /// </summary>
        /// <param name="type">Type to create a default for</param>
        /// <returns>Default for type</returns>
        private static object Default(Type type)
        {
            return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
        }

        /// <summary>
        ///     Generate a cache with predicates which decides if the value needs to be emitted
        ///     Would have been nicer to integrate it into the getter, but this would mean more changes
        /// </summary>
        /// <param name="type"></param>
        internal override IDictionary<string, Func<object, bool>> EmitPredicateFactory(Type type)
        {
            var result = new Dictionary<string, Func<object, bool>>();
            var dataContractAttribute = (DataContractAttribute) ReflectionUtils.GetAttribute(type, typeof(DataContractAttribute));
            if (dataContractAttribute == null)
            {
                return result;
            }
            DataMemberAttribute dataMemberAttribute;
            foreach (var propertyInfo in ReflectionUtils.GetProperties(type))
            {
                if (!CanWrite(propertyInfo, out dataMemberAttribute))
                {
                    continue;
                }
                if (dataMemberAttribute?.EmitDefaultValue != false)
                {
                    continue;
                }
                var jsonKey = JsonKey(dataMemberAttribute, propertyInfo);
                var def = Default(propertyInfo.PropertyType);
                result[jsonKey] = value => !Equals(def, value);
            }
            foreach (var fieldInfo in ReflectionUtils.GetFields(type))
            {
                if (fieldInfo.IsStatic || !CanWrite(fieldInfo, out dataMemberAttribute))
                {
                    continue;
                }
                if (dataMemberAttribute?.EmitDefaultValue != false)
                {
                    continue;
                }
                var jsonKey = JsonKey(dataMemberAttribute, fieldInfo);
                var def = Default(fieldInfo.FieldType);
                result[jsonKey] = value => !Equals(def, value);
            }
            return result;
        }

        internal override IDictionary<string, ReflectionUtils.GetDelegate> GetterValueFactory(Type type)
        {
            var dataContractAttribute = (DataContractAttribute) ReflectionUtils.GetAttribute(type, typeof(DataContractAttribute));
            if (dataContractAttribute == null)
            {
                return base.GetterValueFactory(type);
            }

            string jsonKey;
            DataMemberAttribute dataMemberAttribute;
            IDictionary<string, ReflectionUtils.GetDelegate> result = new Dictionary<string, ReflectionUtils.GetDelegate>();
            foreach (var propertyInfo in ReflectionUtils.GetProperties(type))
            {
                if (!propertyInfo.CanRead)
                {
                    continue;
                }
                var getMethod = ReflectionUtils.GetGetterMethodInfo(propertyInfo);
                if (getMethod.IsStatic || !CanWrite(propertyInfo, out dataMemberAttribute))
                {
                    continue;
                }
                jsonKey = string.IsNullOrEmpty(dataMemberAttribute.Name) ? propertyInfo.Name : dataMemberAttribute.Name;
                result[jsonKey] = ReflectionUtils.GetGetMethod(propertyInfo);
            }
            foreach (var fieldInfo in ReflectionUtils.GetFields(type))
            {
                if (fieldInfo.IsStatic || !CanWrite(fieldInfo, out dataMemberAttribute))
                {
                    continue;
                }
                jsonKey = string.IsNullOrEmpty(dataMemberAttribute.Name) ? fieldInfo.Name : dataMemberAttribute.Name;
                result[jsonKey] = ReflectionUtils.GetGetMethod(fieldInfo);
            }
            return result;
        }

        internal override IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>> SetterValueFactory(Type type)
        {
            var hasDataContract = ReflectionUtils.GetAttribute(type, typeof(DataContractAttribute)) != null;
            if (!hasDataContract)
            {
                return base.SetterValueFactory(type);
            }
            string jsonKey;
            DataMemberAttribute dataMemberAttribute;
            IDictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>> result = new Dictionary<string, KeyValuePair<Type, ReflectionUtils.SetDelegate>>();
            foreach (var propertyInfo in ReflectionUtils.GetProperties(type))
            {
                if (!propertyInfo.CanWrite)
                {
                    continue;
                }
                var setMethod = ReflectionUtils.GetSetterMethodInfo(propertyInfo);
                if (setMethod.IsStatic || !CanRead(propertyInfo, out dataMemberAttribute))
                {
                    continue;
                }
                jsonKey = string.IsNullOrEmpty(dataMemberAttribute.Name) ? propertyInfo.Name : dataMemberAttribute.Name;
                result[jsonKey] = new KeyValuePair<Type, ReflectionUtils.SetDelegate>(propertyInfo.PropertyType, ReflectionUtils.GetSetMethod(propertyInfo));
            }
            foreach (var fieldInfo in ReflectionUtils.GetFields(type))
            {
                if (fieldInfo.IsInitOnly || fieldInfo.IsStatic || !CanRead(fieldInfo, out dataMemberAttribute))
                {
                    continue;
                }
                jsonKey = string.IsNullOrEmpty(dataMemberAttribute.Name) ? fieldInfo.Name : dataMemberAttribute.Name;
                result[jsonKey] = new KeyValuePair<Type, ReflectionUtils.SetDelegate>(fieldInfo.FieldType, ReflectionUtils.GetSetMethod(fieldInfo));
            }
            // TODO implement sorting for DATACONTRACT.
            return result;
        }

        /// <summary>
        ///     Check if we should read the member
        /// </summary>
        /// <param name="info"></param>
        /// <param name="dataMemberAttribute"></param>
        /// <returns></returns>
        private static bool CanRead(MemberInfo info, out DataMemberAttribute dataMemberAttribute)
        {
            dataMemberAttribute = null;

            if (ReflectionUtils.GetAttribute(info, typeof(IgnoreDataMemberAttribute)) != null)
            {
                return false;
            }
            dataMemberAttribute = (DataMemberAttribute) ReflectionUtils.GetAttribute(info, typeof(DataMemberAttribute));
            return dataMemberAttribute != null;
        }

        /// <summary>
        ///     Check if we should write the member
        /// </summary>
        /// <param name="info"></param>
        /// <param name="dataMemberAttribute"></param>
        /// <returns>bool</returns>
        private static bool CanWrite(MemberInfo info, out DataMemberAttribute dataMemberAttribute)
        {
            dataMemberAttribute = null;

            if (ReflectionUtils.GetAttribute(info, typeof(IgnoreDataMemberAttribute)) != null)
            {
                return false;
            }
            // check if the member has a ReadOnlyAttribute set to true, this means we wont emit
            var readOnlyAttribute = (ReadOnlyAttribute) ReflectionUtils.GetAttribute(info, typeof(ReadOnlyAttribute));
            if (readOnlyAttribute != null && readOnlyAttribute.IsReadOnly)
            {
                return false;
            }
            dataMemberAttribute = (DataMemberAttribute) ReflectionUtils.GetAttribute(info, typeof(DataMemberAttribute));
            if (dataMemberAttribute == null)
            {
                return false;
            }
            return true;
        }
    }
}