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
using System.Runtime.Remoting.Messaging;

#endregion

namespace Nito.AsyncEx.AsyncLocal
{
	/// <summary>
	///     AsyncLocal implementation of Stephen Cleary
	///     See <a href="https://github.com/StephenCleary/AsyncLocal">here</a>
	///     License:
	///     The MIT License (MIT) here: https://raw.githubusercontent.com/StephenCleary/AsyncLocal/master/LICENSE
	///     Copyright(c) 2014 StephenCleary
	///     Permission is hereby granted, free of charge, to any person obtaining a copy
	///     of this software and associated documentation files (the "Software"), to deal
	///     in the Software without restriction, including without limitation the rights
	///     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	///     copies of the Software, and to permit persons to whom the Software is
	///     furnished to do so, subject to the following conditions:
	///     The above copyright notice and this permission notice shall be included in all
	///     copies or substantial portions of the Software.
	///     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	///     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	///     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	///     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	///     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	///     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
	///     SOFTWARE.
	///     Data that is "local" to the current async method. This is the async near-equivalent of <c>ThreadLocal&lt;T&gt;</c>.
	/// </summary>
	/// <typeparam name="TImmutableType">The type of the data. This must be an immutable type.</typeparam>
	public sealed class AsyncLocal<TImmutableType>
	{
		/// <summary>
		///     The default value when none has been set.
		/// </summary>
		private readonly TImmutableType _defaultValue;

		/// <summary>
		///     Our unique slot name.
		/// </summary>
		private readonly string _slotName = Guid.NewGuid().ToString("N");

		/// <summary>
		///     Creates a new async-local variable with the default value of <typeparamref name="TImmutableType" />.
		/// </summary>
		public AsyncLocal()
			: this(default(TImmutableType))
		{
		}

		/// <summary>
		///     Creates a new async-local variable with the specified default value.
		/// </summary>
		public AsyncLocal(TImmutableType defaultValue)
		{
			_defaultValue = defaultValue;
		}

		/// <summary>
		///     Returns a value indicating whether the value of this async-local variable has been set for the local context.
		/// </summary>
		public bool IsValueSet
		{
			get { return CallContext.LogicalGetData(_slotName) != null; }
		}

		/// <summary>
		///     Gets or sets the value of this async-local variable for the local context.
		/// </summary>
		public TImmutableType Value
		{
			get
			{
				var ret = CallContext.LogicalGetData(_slotName);
				if (ret == null)
					return _defaultValue;
				return (TImmutableType) ret;
			}

			set { CallContext.LogicalSetData(_slotName, value); }
		}

		/// <summary>
		///     Clears the value of this async-local variable for the local context.
		/// </summary>
		public void ClearValue()
		{
			CallContext.FreeNamedDataSlot(_slotName);
		}
	}
}