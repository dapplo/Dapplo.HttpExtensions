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

#if NET45 || NET46
#region using

using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

#endregion

namespace Dapplo.HttpExtensions.Support
{
	/// <summary>
	///     Use this to delegate to an not yet existing TypeConverter
	///     Example: DelegatingStringEncryptionTypeConverter
	/// </summary>
	public class DelegatingTypeConverter : TypeConverter
	{
		private readonly TypeConverter _innerTypeConverter;

		/// <summary>
		///     Create the inner type, which we wrap, so we can "delegate" to it.
		/// </summary>
		/// <param name="assemblyName">string</param>
		/// <param name="typeName">string</param>
		public DelegatingTypeConverter(string assemblyName, string typeName)
		{
			var handle = Activator.CreateInstance(assemblyName, typeName);
			_innerTypeConverter = handle.Unwrap() as TypeConverter;
		}

		/// <inheritdoc />
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return _innerTypeConverter.CanConvertFrom(context, sourceType);
		}

		/// <inheritdoc />
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return _innerTypeConverter.CanConvertTo(context, destinationType);
		}

		/// <inheritdoc />
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return _innerTypeConverter.ConvertFrom(context, culture, value);
		}

		/// <inheritdoc />
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			return _innerTypeConverter.ConvertTo(context, culture, value, destinationType);
		}

		/// <inheritdoc />
		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			return _innerTypeConverter.CreateInstance(context, propertyValues);
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			return _innerTypeConverter.Equals(obj);
		}

		/// <inheritdoc />
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return _innerTypeConverter.GetCreateInstanceSupported(context);
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			return _innerTypeConverter.GetHashCode();
		}

		/// <inheritdoc />
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			return _innerTypeConverter.GetProperties(context, value, attributes);
		}

		/// <inheritdoc />
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return _innerTypeConverter.GetPropertiesSupported(context);
		}

		/// <inheritdoc />
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return _innerTypeConverter.GetStandardValues(context);
		}

		/// <inheritdoc />
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return _innerTypeConverter.GetStandardValuesExclusive(context);
		}

		/// <inheritdoc />
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return _innerTypeConverter.GetStandardValuesSupported(context);
		}

		/// <inheritdoc />
		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			return _innerTypeConverter.IsValid(context, value);
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return _innerTypeConverter.ToString();
		}
	}
}
#endif