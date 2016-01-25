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
	/// This is the default implementation of the IHttpBehaviour, see IHttpBehaviour for details
	/// Most values are initialized via the HttpExtensionsGlobals
	/// </summary>
	public class HttpBehaviour : IHttpBehaviour
	{
		public IHttpSettings HttpSettings { get; set; } = HttpExtensionsGlobals.HttpSettings;

		public IJsonSerializer JsonSerializer { get; set; } = HttpExtensionsGlobals.JsonSerializer;

		public IList<IHttpContentConverter> HttpContentConverters { get; set; } = HttpExtensionsGlobals.HttpContentConverters;

		public Action<HttpRequestMessage> OnHttpRequestMessageCreated { get; set; }

		public Action<HttpClient> OnHttpClientCreated { get; set; }

		public Action<HttpMessageHandler> OnHttpMessageHandlerCreated { get; set; }

		public Action<HttpContent> OnHttpContentCreated { get; set; }

		public bool ThrowOnError { get; set; } = HttpExtensionsGlobals.ThrowOnError;

		public HttpCompletionOption HttpCompletionOption { get; set; } = HttpCompletionOption.ResponseContentRead;

		public bool ValidateResponseContentType { get; set; } = HttpExtensionsGlobals.ValidateResponseContentType;

		public Encoding DefaultEncoding { get; set; } = HttpExtensionsGlobals.DefaultEncoding;

		public int ReadBufferSize { get; set; } = HttpExtensionsGlobals.ReadBufferSize;

		public object Clone()
		{
			return MemberwiseClone();
		}
	}
}
