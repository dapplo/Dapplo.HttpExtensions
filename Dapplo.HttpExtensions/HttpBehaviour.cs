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
	along with Foobar.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Net.Http;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// This can be used to modify the behaviour of the HttpExtensions operations.
	/// Most of the times the default is okay but sometimes you just want to have a way to fix something
	/// </summary>
	public class HttpBehaviour
	{
		public static HttpBehaviour GlobalHttpBehaviour
		{
			get;
			set;
		} = new HttpBehaviour();

		/// <summary>
		/// Pass your HttpSettings here, which will be used to create the HttpClient
		/// If not specified, the HttpSettings.GlobalSettings will be used
		/// </summary>
		public IHttpSettings HttpSettings { get; set; }

		/// <summary>
		/// An action which can modify the HttpClient which is generated in the HttpClientFactory.
		/// Use cases for this, might be adding a header or other settings for specific cases
		/// </summary>
		public Action<HttpClient> OnCreateHttpClient { get; set; }

		/// <summary>
		/// An action which can modify the HttpMessageHandler which is generated in the HttpMessageHandlerFactory.
		/// Use cases for this, might be if you have very specify settings which can't be set via the IHttpSettings
		/// </summary>
		public Action<HttpMessageHandler> OnCreateHttpMessageHandler { get; set; }

		/// <summary>
		/// If a request gets a response which has a HTTP status code which is not 200, it would normally throw an exception.
		/// Sometimes you would still want the response, settings this to false would allow this.
		/// </summary>
		public bool ThrowErrorOnNonSuccess { get; set; } = true;

		/// <summary>
		/// This can be used to change the behaviour of Http operation, default is to read the complete response.
		/// </summary>
		public HttpCompletionOption HttpCompletionOption { get; set; } = HttpCompletionOption.ResponseContentRead;

		/// <summary>
		/// Check if the response has the expected content-type, when servers are used that are not following specifications this should be set to false
		/// </summary>
		public bool ValidateResponseContentType { get; set; } = true;

		/// <summary>
		/// Clone this HttpBehaviour
		/// Remember not to modify the values of any reference objects like HttpSettings
		/// </summary>
		/// <returns>HttpBehaviour</returns>
		public HttpBehaviour Clone()
		{
			return (HttpBehaviour)MemberwiseClone();
		}
	}
}
