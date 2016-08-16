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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace Dapplo.HttpExtensions
{
	/// <summary>
	///     This interface is used for all
	/// </summary>
	public interface IHttpContentConverter
	{
		/// <summary>
		///     Specify the order in that this IHttpContentConverter is used
		/// </summary>
		int Order { get; }

		/// <summary>
		///     This will add accept headers depending on the result type
		/// </summary>
		/// <param name="resultType">Type to read into</param>
		/// <param name="httpRequestMessage">HttpClient for the response headers</param>
		void AddAcceptHeadersForType(Type resultType, HttpRequestMessage httpRequestMessage);

		/// <summary>
		///     Check if this IHttpContentProcessor can convert the HttpContent into the specified type
		/// </summary>
		/// <param name="typeToConvertTo">Type from which a conversion should be made</param>
		/// <param name="httpContent">HttpContent object to process</param>
		/// <returns>true if this processor can do the conversion</returns>
		bool CanConvertFromHttpContent(Type typeToConvertTo, HttpContent httpContent);

		/// <summary>
		///     Check if this IHttpContentProcessor can convert the specified type to a HttpContent
		/// </summary>
		/// <param name="typeToConvertFrom">Type to convert</param>
		/// <param name="content">Content to place into a HttpContent</param>
		/// <returns>true if this processor can do the conversion</returns>
		bool CanConvertToHttpContent(Type typeToConvertFrom, object content);

		/// <summary>
		///     Create the target object from the supplied HttpContent
		/// </summary>
		/// <param name="resultType">Typ to process the HttpContent to</param>
		/// <param name="httpContent">HttpContent</param>
		/// <param name="cancellationToken">CancellationToken</param>
		/// <returns>object of type resultType</returns>
		Task<object> ConvertFromHttpContentAsync(Type resultType, HttpContent httpContent, CancellationToken cancellationToken = default(CancellationToken));

		/// <summary>
		///     Create HttpContent for the supplied object/type
		/// </summary>
		/// <param name="typeToConvert">Type of the content to convert</param>
		/// <param name="content">Content to place into a HttpContent</param>
		/// <returns>HttpContent</returns>
		HttpContent ConvertToHttpContent(Type typeToConvert, object content);
	}
}