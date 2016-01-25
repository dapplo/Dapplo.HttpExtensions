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

using System.Net;
using System.Net.Http.Headers;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// This interface returns the information of a HTTP request
	/// Makes it possible to process the error information too
	/// </summary>
	/// <typeparam name="TResponse">Type for the normal response</typeparam>
	/// <typeparam name="TErrorResponse">Type for the error response</typeparam>
	public interface IHttpResponse<TResponse, TErrorResponse>
		where TResponse : class
		where TErrorResponse : class
	{
		/// <summary>
		/// The response when an "OK" status was returned
		/// </summary>
		TResponse Response { get; set; }

		/// <summary>
		/// The response when an "OK" status was returned
		/// </summary>
		TErrorResponse ErrorResponse { get; set; }

		/// <summary>
		/// true if the reponse has an error
		/// </summary>
		bool HasError { get; }

		/// <summary>
		/// Headers of the response
		/// </summary>
		HttpResponseHeaders Headers { get; set; }

		/// <summary>
		/// The response HTTP status code
		/// </summary>
		HttpStatusCode StatusCode { get; set; }
	}
}