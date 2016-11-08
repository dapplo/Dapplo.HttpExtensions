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

namespace Dapplo.HttpExtensions.Support
{
	/// <summary>
	///     Marker for the response
	/// </summary>
	public enum HttpParts
	{
		/// <summary>
		///     Default value.
		/// </summary>
		None,

		/// <summary>
		///     The property specifies the boundary of a multi-part
		/// </summary>
		MultipartBoundary,

		/// <summary>
		///     The property specifies the name of the content in a multi-part post
		/// </summary>
		RequestMultipartName,

		/// <summary>
		///     The property specifies the filename of the content in a multi-part post
		/// </summary>
		RequestMultipartFilename,

		/// <summary>
		///     Specifies the content for uploading
		/// </summary>
		RequestContent,

		/// <summary>
		///     Specifies the content-type for uploading
		/// </summary>
		RequestContentType,

		/// <summary>
		///     Specifies the request headers to send on the request, this should be of type IDictionary where key is string and
		///     value is string
		/// </summary>
		RequestHeaders,

		/// <summary>
		///     The property will get the response content, HttpResponseMessage can also be used
		/// </summary>
		ResponseContent,

		/// <summary>
		///     The property will get the response content, when an error occured
		/// </summary>
		ResponseErrorContent,

		/// <summary>
		///     Specifies the content-type, either for uploading or for the response
		/// </summary>
		ResponseContentType,

		/// <summary>
		///     The Http-Status code, should be of type HttpStatusCode
		/// </summary>
		ResponseStatuscode,

		/// <summary>
		///     Marks HttpResponseHeaders,
		/// </summary>
		ResponseHeaders
	}
}