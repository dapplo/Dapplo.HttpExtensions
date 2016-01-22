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
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// The IHttpBehaviour is used to control the behaviour of all operations in the HttpExtensions library.
	/// </summary>
	public interface IHttpBehaviour
	{
		/// <summary>
		/// Pass your HttpSettings here, which will be used to create the HttpClient
		/// If not specified, the HttpSettings.GlobalSettings will be used
		/// </summary>
		IHttpSettings HttpSettings { get; set; }

		/// <summary>
		/// This is used to de- serialize Json, can be overwritten by your own implementation.
		/// By default, also when empty, the SimpleJsonSerializer is used.
		/// </summary>
		IJsonSerializer JsonSerializer { get; set; }

		/// <summary>
		/// This is the list of IHttpContentConverters which is used when converting from/to HttpContent
		/// </summary>
		IList<IHttpContentConverter> HttpContentConverters { get; set; }

		/// <summary>
		/// An action which can modify the HttpClient which is generated in the HttpClientFactory.
		/// Use cases for this, might be adding a header or other settings for specific cases
		/// </summary>
		Action<HttpClient> OnCreateHttpClient { get; set; }

		/// <summary>
		/// An action which can modify the HttpMessageHandler which is generated in the HttpMessageHandlerFactory.
		/// Use cases for this, might be if you have very specify settings which can't be set via the IHttpSettings
		/// </summary>
		Action<HttpMessageHandler> OnCreateHttpMessageHandler { get; set; }

		/// <summary>
		/// If a request gets a response which has a HTTP status code which is not 200, it would normally throw an exception.
		/// Sometimes you would still want the response, settings this to false would allow this.
		/// </summary>
		bool ThrowErrorOnNonSuccess { get; set; }

		/// <summary>
		/// This can be used to change the behaviour of Http operation, default is to read the complete response.
		/// </summary>
		HttpCompletionOption HttpCompletionOption { get; set; }

		/// <summary>
		/// Check if the response has the expected content-type, when servers are used that are not following specifications this should be set to false
		/// </summary>
		bool ValidateResponseContentType { get; set; }

		/// <summary>
		/// The default encoding which is used wherever an encoding is specified.
		/// The default is set to Encoding.UTF8
		/// </summary>
		Encoding DefaultEncoding { get; set; }

		/// <summary>
		/// Specify the buffer for reading operations
		/// </summary>
		int ReadBufferSize { get; set; }

		/// <summary>
		/// Clone this IHttpBehaviour
		/// Remember not to modify the values of any reference objects like IHttpSettings
		/// </summary>
		/// <returns>HttpBehaviour</returns>
		IHttpBehaviour Clone();
	}
}