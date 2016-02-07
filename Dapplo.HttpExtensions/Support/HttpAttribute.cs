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

namespace Dapplo.HttpExtensions.Support
{
	/// <summary>
	/// This attribute marks a property as "http content"
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
	public class HttpAttribute : Attribute
	{
		public HttpAttribute(object part)
		{
			if (part == null)
			{
				throw new ArgumentNullException(nameof(part));
			}
			if (typeof(HttpParts) != part.GetType())
			{
				throw new ArgumentException(nameof(part));
			}
			Part = (HttpParts)part;
		}

		/// <summary>
		/// Order of the content when using multi-part content
		/// </summary>
		public int Order
		{
			get;
			set;
		}

		/// <summary>
		/// Use this to specify what the property is representing
		/// </summary>
		public HttpParts Part
		{
			get;
			set;
		}
	}
}
