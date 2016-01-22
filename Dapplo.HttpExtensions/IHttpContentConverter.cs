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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// This interface is used for all 
	/// </summary>
	public interface IHttpContentConverter
	{
		/// <summary>
		/// Specify the order in that this IHttpContentConverter is used
		/// </summary>
		int Order { get; }

		/// <summary>
		/// Check if this IHttpContentProcessor can convert the specified object to a HttpContent
		/// </summary>
		/// <typeparam name="TInput">content type to place into a HttpContent</typeparam>
		/// <param name="content">Content to place into a HttpContent</param>
		/// <param name="httpBehaviour">IHttpBehaviour which controls the validation</param>
		/// <returns>true if this processor can do the conversion</returns>
		bool CanConvertToHttpContent<TInput>(TInput content, IHttpBehaviour httpBehaviour = null) where TInput : class;

		/// <summary>
		/// Check if this IHttpContentProcessor can convert the specified type to a HttpContent
		/// </summary>
		/// <param name="typeToConvertFrom">Type to convert</param>
		/// <param name="content">Content to place into a HttpContent</param>
		/// <param name="httpBehaviour">IHttpBehaviour which controls the validation</param>
		/// <returns>true if this processor can do the conversion</returns>
		bool CanConvertToHttpContent(Type typeToConvertFrom, object content, IHttpBehaviour httpBehaviour = null);

		/// <summary>
		/// Create HttpContent for the supplied object/type
		/// </summary>
		/// <typeparam name="TInput">Type of the content</typeparam>
		/// <param name="content">Content to place into a HttpContent</param>
		/// <param name="httpBehaviour">IHttpBehaviour which controls the validation</param>
		/// <returns>HttpContent</returns>
		HttpContent ConvertToHttpContent<TInput>(TInput content, IHttpBehaviour httpBehaviour = null) where TInput : class;

		/// <summary>
		/// Create HttpContent for the supplied object/type
		/// </summary>
		/// <param name="typeToConvert">Type of the content to convert</param>
		/// <param name="content">Content to place into a HttpContent</param>
		/// <param name="httpBehaviour">IHttpBehaviour which controls the validation</param>
		/// <returns>HttpContent</returns>
		HttpContent ConvertToHttpContent(Type typeToConvert, object content, IHttpBehaviour httpBehaviour = null);

		/// <summary>
		/// Check if this IHttpContentProcessor can convert the HttpContent into the specified type
		/// </summary>
		/// <typeparam name="TResult">Type to which a convertion should be made</typeparam>
		/// <param name="httpContent">HttpContent object to process</param>
		/// <param name="httpBehaviour">IHttpBehaviour which controls the validation</param>
		/// <returns>true if this processor can do the conversion</returns>
		bool CanConvertFromHttpContent<TResult>(HttpContent httpContent, IHttpBehaviour httpBehaviour = null) where TResult : class;

		/// <summary>
		/// Check if this IHttpContentProcessor can convert the HttpContent into the specified type
		/// </summary>
		/// <param name="typeToConvertTo">Type from which a conversion should be made</param>
		/// <param name="httpContent">HttpContent object to process</param>
		/// <param name="httpBehaviour">IHttpBehaviour which controls the validation</param>
		/// <returns>true if this processor can do the conversion</returns>
		bool CanConvertFromHttpContent(Type typeToConvertTo, HttpContent httpContent, IHttpBehaviour httpBehaviour = null);

		/// <summary>
		/// Create the target object from the supplied HttpContent
		/// </summary>
		/// <typeparam name="TResult">Typ to process the HttpContent to</typeparam>
		/// <param name="httpContent">HttpContent</param>
		/// <param name="httpBehaviour">IHttpBehaviour which controls the validation</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>TResult</returns>
		Task<TResult> ConvertFromHttpContentAsync<TResult>(HttpContent httpContent, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken)) where TResult : class;

		/// <summary>
		/// Create the target object from the supplied HttpContent
		/// </summary>
		/// <param name="resultType">Typ to process the HttpContent to</param>
		/// <param name="httpContent">HttpContent</param>
		/// <param name="httpBehaviour">IHttpBehaviour which controls the validation</param>
		/// <param name="token">CancellationToken</param>
		/// <returns>object of type resultType</returns>
		Task<object> ConvertFromHttpContentAsync(Type resultType, HttpContent httpContent, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken));
	}
}