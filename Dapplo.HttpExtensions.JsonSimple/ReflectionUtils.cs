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

#if NETSTANDARD1_3
#define SIMPLE_JSON_TYPEINFO
#endif

#region using

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

#endregion

namespace Dapplo.HttpExtensions.JsonSimple
{
    [GeneratedCode("reflection-utils", "1.0.0")]
#if SIMPLE_JSON_REFLECTION_UTILS_PUBLIC
		public
#else
    internal
#endif
        static class ReflectionUtils
    {
#if SIMPLE_JSON_NO_LINQ_EXPRESSION
			private static readonly object[] EmptyObjects = { };
#endif

        public delegate object GetDelegate(object source);

        public delegate void SetDelegate(object source, object value);

        public delegate object ConstructorDelegate(params object[] args);

        public delegate TValue ThreadSafeDictionaryValueFactory<in TKey, out TValue>(TKey key);

#if SIMPLE_JSON_TYPEINFO

        public static TypeInfo GetTypeInfo(Type type)
        {
            return type.GetTypeInfo();
        }

#else
		public static Type GetTypeInfo(Type type)
		{
			return type;
		}
#endif

        public static Attribute GetAttribute(MemberInfo info, Type type)
        {
#if SIMPLE_JSON_TYPEINFO
            if (info == null || type == null || !info.IsDefined(type))
            {
                return null;
            }
            return info.GetCustomAttribute(type);
#else
			if (info == null || type == null || !Attribute.IsDefined(info, type))
				return null;
			return Attribute.GetCustomAttribute(info, type);
#endif
        }

        public static Type GetGenericListElementType(Type type)
        {
#if SIMPLE_JSON_TYPEINFO
            IEnumerable<Type> interfaces = type.GetTypeInfo().ImplementedInterfaces;
#else
			IEnumerable<Type> interfaces = type.GetInterfaces();
#endif
            foreach (var implementedInterface in interfaces)
            {
                if (IsTypeGeneric(implementedInterface) &&
                    implementedInterface.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    return GetGenericTypeArguments(implementedInterface)[0];
                }
            }
            return GetGenericTypeArguments(type)[0];
        }

        public static Attribute GetAttribute(Type objectType, Type attributeType)
        {
#if SIMPLE_JSON_TYPEINFO
            if (objectType == null || attributeType == null || !objectType.GetTypeInfo().IsDefined(attributeType))
            {
                return null;
            }
            return objectType.GetTypeInfo().GetCustomAttribute(attributeType);
#else
			if (objectType == null || attributeType == null || !Attribute.IsDefined(objectType, attributeType))
				return null;
			return Attribute.GetCustomAttribute(objectType, attributeType);
#endif
        }

        public static Type[] GetGenericTypeArguments(Type type)
        {
#if SIMPLE_JSON_TYPEINFO
            return type.GetTypeInfo().GenericTypeArguments;
#else
			return type.GetGenericArguments();
#endif
        }

        public static bool IsTypeGeneric(Type type)
        {
            return GetTypeInfo(type).IsGenericType;
        }

        public static bool IsTypeGenericeCollectionInterface(Type type)
        {
            if (!IsTypeGeneric(type))
            {
                return false;
            }

            var genericDefinition = type.GetGenericTypeDefinition();

            return genericDefinition == typeof(IList<>)
                   || genericDefinition == typeof(ICollection<>)
                   || genericDefinition == typeof(IEnumerable<>)
#if SIMPLE_JSON_READONLY_COLLECTIONS
			       || genericDefinition == typeof(IReadOnlyCollection<>)
			       || genericDefinition == typeof(IReadOnlyList<>)
#endif
                ;
        }

        public static bool IsAssignableFrom(Type type1, Type type2)
        {
            return GetTypeInfo(type1).IsAssignableFrom(GetTypeInfo(type2));
        }

        public static bool IsTypeDictionary(Type type)
        {
#if SIMPLE_JSON_TYPEINFO
            if (typeof(IDictionary<,>).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
            {
                return true;
            }
#else
			if (typeof(IDictionary).IsAssignableFrom(type))
				return true;
#endif
            if (!GetTypeInfo(type).IsGenericType)
            {
                return false;
            }

            var genericDefinition = type.GetGenericTypeDefinition();
            return genericDefinition == typeof(IDictionary<,>);
        }

        public static bool IsNullableType(Type type)
        {
            return GetTypeInfo(type).IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static object ToNullableType(object obj, Type nullableType)
        {
            return obj == null ? null : Convert.ChangeType(obj, Nullable.GetUnderlyingType(nullableType), CultureInfo.InvariantCulture);
        }

        public static bool IsValueType(Type type)
        {
            return GetTypeInfo(type).IsValueType;
        }

        public static IEnumerable<ConstructorInfo> GetConstructors(Type type)
        {
#if SIMPLE_JSON_TYPEINFO
            return type.GetTypeInfo().DeclaredConstructors;
#else
			return type.GetConstructors();
#endif
        }

        public static ConstructorInfo GetConstructorInfo(Type type, params Type[] argsType)
        {
            var constructorInfos = GetConstructors(type);
            foreach (var constructorInfo in constructorInfos)
            {
                var parameters = constructorInfo.GetParameters();
                if (argsType.Length != parameters.Length)
                {
                    continue;
                }

                var i = 0;
                var matches = true;
                foreach (var parameterInfo in constructorInfo.GetParameters())
                {
                    if (parameterInfo.ParameterType != argsType[i])
                    {
                        matches = false;
                        break;
                    }
                }

                if (matches)
                {
                    return constructorInfo;
                }
            }

            return null;
        }

        public static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
#if SIMPLE_JSON_TYPEINFO
            return type.GetRuntimeProperties();
#else
			return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
#endif
        }

        public static IEnumerable<FieldInfo> GetFields(Type type)
        {
#if SIMPLE_JSON_TYPEINFO
            return type.GetRuntimeFields();
#else
			return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
#endif
        }

        public static MethodInfo GetGetterMethodInfo(PropertyInfo propertyInfo)
        {
#if SIMPLE_JSON_TYPEINFO
            return propertyInfo.GetMethod;
#else
			return propertyInfo.GetGetMethod(true);
#endif
        }

        public static MethodInfo GetSetterMethodInfo(PropertyInfo propertyInfo)
        {
#if SIMPLE_JSON_TYPEINFO
            return propertyInfo.SetMethod;
#else
			return propertyInfo.GetSetMethod(true);
#endif
        }

        public static ConstructorDelegate GetContructor(ConstructorInfo constructorInfo)
        {
#if SIMPLE_JSON_NO_LINQ_EXPRESSION
				return GetConstructorByReflection(constructorInfo);
#else
            return GetConstructorByExpression(constructorInfo);
#endif
        }

        public static ConstructorDelegate GetContructor(Type type, params Type[] argsType)
        {
#if SIMPLE_JSON_NO_LINQ_EXPRESSION
				return GetConstructorByReflection(type, argsType);
#else
            return GetConstructorByExpression(type, argsType);
#endif
        }

#if SIMPLE_JSON_NO_LINQ_EXPRESSION
			public static ConstructorDelegate GetConstructorByReflection(Type type, params Type[] argsType)
			{
				ConstructorInfo constructorInfo = GetConstructorInfo(type, argsType);
				return constructorInfo == null ? null : GetConstructorByReflection(constructorInfo);
			}
			public static ConstructorDelegate GetConstructorByReflection(ConstructorInfo constructorInfo)
			{
				return constructorInfo.Invoke;
			}
#else

        public static ConstructorDelegate GetConstructorByExpression(ConstructorInfo constructorInfo)
        {
            var paramsInfo = constructorInfo.GetParameters();
            var param = Expression.Parameter(typeof(object[]), "args");
            var argsExp = new Expression[paramsInfo.Length];
            for (var i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                var paramType = paramsInfo[i].ParameterType;
                Expression paramAccessorExp = Expression.ArrayIndex(param, index);
                Expression paramCastExp = Expression.Convert(paramAccessorExp, paramType);
                argsExp[i] = paramCastExp;
            }
            var newExp = Expression.New(constructorInfo, argsExp);
            var lambda = Expression.Lambda<Func<object[], object>>(newExp, param);
            var compiledLambda = lambda.Compile();
            return args => compiledLambda(args);
        }

        public static ConstructorDelegate GetConstructorByExpression(Type type, params Type[] argsType)
        {
            var constructorInfo = GetConstructorInfo(type, argsType);
            return constructorInfo == null ? null : GetConstructorByExpression(constructorInfo);
        }

#endif

        public static GetDelegate GetGetMethod(PropertyInfo propertyInfo)
        {
#if SIMPLE_JSON_NO_LINQ_EXPRESSION
				return GetGetMethodByReflection(propertyInfo);
#else
            return GetGetMethodByExpression(propertyInfo);
#endif
        }

        public static GetDelegate GetGetMethod(FieldInfo fieldInfo)
        {
#if SIMPLE_JSON_NO_LINQ_EXPRESSION
				return GetGetMethodByReflection(fieldInfo);
#else
            return GetGetMethodByExpression(fieldInfo);
#endif
        }

#if SIMPLE_JSON_NO_LINQ_EXPRESSION
			public static GetDelegate GetGetMethodByReflection(PropertyInfo propertyInfo)
			{
				MethodInfo methodInfo = GetGetterMethodInfo(propertyInfo);
				return source => methodInfo.Invoke(source, EmptyObjects);
			}

			public static GetDelegate GetGetMethodByReflection(FieldInfo fieldInfo)
			{
				return fieldInfo.GetValue;
			}

#else

        public static GetDelegate GetGetMethodByExpression(PropertyInfo propertyInfo)
        {
            var getMethodInfo = GetGetterMethodInfo(propertyInfo);
            var instance = Expression.Parameter(typeof(object), "instance");
            var instanceCast = !IsValueType(propertyInfo.DeclaringType) ? Expression.TypeAs(instance, propertyInfo.DeclaringType) : Expression.Convert(instance, propertyInfo.DeclaringType);
            var compiled = Expression.Lambda<Func<object, object>>(Expression.TypeAs(Expression.Call(instanceCast, getMethodInfo), typeof(object)), instance).Compile();
            return source => compiled(source);
        }

        public static GetDelegate GetGetMethodByExpression(FieldInfo fieldInfo)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var unaryExpression = Expression.Convert(instance, fieldInfo.DeclaringType);
            var member = Expression.Field(unaryExpression, fieldInfo);
            var compiled = Expression.Lambda<GetDelegate>(Expression.Convert(member, typeof(object)), instance).Compile();
            return source => compiled(source);
        }

#endif

        public static SetDelegate GetSetMethod(PropertyInfo propertyInfo)
        {
#if SIMPLE_JSON_NO_LINQ_EXPRESSION
				return GetSetMethodByReflection(propertyInfo);
#else
            return GetSetMethodByExpression(propertyInfo);
#endif
        }

        public static SetDelegate GetSetMethod(FieldInfo fieldInfo)
        {
#if SIMPLE_JSON_NO_LINQ_EXPRESSION
				return GetSetMethodByReflection(fieldInfo);
#else
            return GetSetMethodByExpression(fieldInfo);
#endif
        }

#if SIMPLE_JSON_NO_LINQ_EXPRESSION
			public static SetDelegate GetSetMethodByReflection(PropertyInfo propertyInfo)
			{
				MethodInfo methodInfo = GetSetterMethodInfo(propertyInfo);
				return delegate(object source, object value) { methodInfo.Invoke(source, new object[] { value }); };
			}

			public static SetDelegate GetSetMethodByReflection(FieldInfo fieldInfo)
			{
				return fieldInfo.SetValue;
			}

#else

        public static SetDelegate GetSetMethodByExpression(PropertyInfo propertyInfo)
        {
            var setMethodInfo = GetSetterMethodInfo(propertyInfo);
            var instance = Expression.Parameter(typeof(object), "instance");
            var value = Expression.Parameter(typeof(object), "value");
            var instanceCast = !IsValueType(propertyInfo.DeclaringType) ? Expression.TypeAs(instance, propertyInfo.DeclaringType) : Expression.Convert(instance, propertyInfo.DeclaringType);
            var valueCast = !IsValueType(propertyInfo.PropertyType) ? Expression.TypeAs(value, propertyInfo.PropertyType) : Expression.Convert(value, propertyInfo.PropertyType);
            var compiled = Expression.Lambda<Action<object, object>>(Expression.Call(instanceCast, setMethodInfo, valueCast), instance, value).Compile();
            return delegate(object source, object val) { compiled(source, val); };
        }

        public static SetDelegate GetSetMethodByExpression(FieldInfo fieldInfo)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var value = Expression.Parameter(typeof(object), "value");
            var compiled = Expression.Lambda<Action<object, object>>(Assign(Expression.Field(Expression.Convert(instance, fieldInfo.DeclaringType), fieldInfo), Expression.Convert(value, fieldInfo.FieldType)), instance, value).Compile();
            return delegate(object source, object val) { compiled(source, val); };
        }

        public static BinaryExpression Assign(Expression left, Expression right)
        {
#if SIMPLE_JSON_TYPEINFO
            return Expression.Assign(left, right);
#else
			var assign = typeof(Assigner<>).MakeGenericType(left.Type).GetMethod("Assign");
			var assignExpr = Expression.Add(left, right, assign);
			return assignExpr;
#endif
        }

#if !SIMPLE_JSON_TYPEINFO
		private static class Assigner<T>
		{
			public static T Assign(out T left, T right)
			{
				return left = right;
			}
		}
#endif

#endif

        public sealed class ThreadSafeDictionary<TKey, TValue> : IDictionary<TKey, TValue>
        {
            private readonly object _lock = new object();
            private readonly ThreadSafeDictionaryValueFactory<TKey, TValue> _valueFactory;
            private Dictionary<TKey, TValue> _dictionary;

            public ThreadSafeDictionary(ThreadSafeDictionaryValueFactory<TKey, TValue> valueFactory)
            {
                _valueFactory = valueFactory;
            }

            public void Add(TKey key, TValue value)
            {
                throw new NotImplementedException();
            }

            public bool ContainsKey(TKey key)
            {
                return _dictionary.ContainsKey(key);
            }

            public ICollection<TKey> Keys => _dictionary.Keys;

            public bool Remove(TKey key)
            {
                throw new NotImplementedException();
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                value = this[key];
                return true;
            }

            public ICollection<TValue> Values => _dictionary.Values;

            public TValue this[TKey key]
            {
                get { return Get(key); }
                set { throw new NotImplementedException(); }
            }

            public void Add(KeyValuePair<TKey, TValue> item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(KeyValuePair<TKey, TValue> item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public int Count => _dictionary.Count;

            public bool IsReadOnly
            {
                get { throw new NotImplementedException(); }
            }

            public bool Remove(KeyValuePair<TKey, TValue> item)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                return _dictionary.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _dictionary.GetEnumerator();
            }

            private TValue AddValue(TKey key)
            {
                var value = _valueFactory(key);
                lock (_lock)
                {
                    if (_dictionary == null)
                    {
                        _dictionary = new Dictionary<TKey, TValue> {[key] = value};
                    }
                    else
                    {
                        TValue val;
                        if (_dictionary.TryGetValue(key, out val))
                        {
                            return val;
                        }
                        var dict = new Dictionary<TKey, TValue>(_dictionary) {[key] = value};
                        _dictionary = dict;
                    }
                }
                return value;
            }

            private TValue Get(TKey key)
            {
                if (_dictionary == null)
                {
                    return AddValue(key);
                }
                TValue value;
                if (!_dictionary.TryGetValue(key, out value))
                {
                    return AddValue(key);
                }
                return value;
            }
        }
    }
}