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
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// This is the default implementation of the IHttpBehaviour, see IHttpBehaviour for details
	/// Most values are initialized via the HttpExtensionsGlobals
	/// </summary>
	public class HttpBehaviour : IChangeableHttpBehaviour
	{
		public IHttpSettings HttpSettings { get; set; } = HttpExtensionsGlobals.HttpSettings;

		public IJsonSerializer JsonSerializer { get; set; } = HttpExtensionsGlobals.JsonSerializer;

		public IList<IHttpContentConverter> HttpContentConverters { get; set; } = HttpExtensionsGlobals.HttpContentConverters;

		public Func<HttpRequestMessage, HttpRequestMessage> OnHttpRequestMessageCreated { get; set; }

		public Action<HttpClient> OnHttpClientCreated { get; set; }

		public Func<HttpMessageHandler, HttpMessageHandler> OnHttpMessageHandlerCreated { get; set; }

		public Func<HttpContent, HttpContent> OnHttpContentCreated { get; set; }

		public IProgress<float> UploadProgress { get; set; }

		public bool UseProgressStreamContent { get; set; } = HttpExtensionsGlobals.UseProgressStreamContent;

		public bool ThrowOnError { get; set; } = HttpExtensionsGlobals.ThrowOnError;

		/// <summary>
		/// The ResponseHeadersRead forces a pause between the initial response and reading the content, this is needed for better error handling and progress
		/// Turning this to ResponseContentRead might change the behaviour
		/// </summary>
		public HttpCompletionOption HttpCompletionOption { get; set; } = HttpCompletionOption.ResponseHeadersRead;

		public bool ValidateResponseContentType { get; set; } = HttpExtensionsGlobals.ValidateResponseContentType;

		public Encoding DefaultEncoding { get; set; } = HttpExtensionsGlobals.DefaultEncoding;

		public int ReadBufferSize { get; set; } = HttpExtensionsGlobals.ReadBufferSize;

		public IChangeableHttpBehaviour Clone()
		{
			return (HttpBehaviour)MemberwiseClone();
		}

		/// <summary>
		/// Explicit IClonable interface implementation
		/// </summary>
		/// <returns></returns>
		object ICloneable.Clone()
		{
			return Clone();
		}

		public void MakeCurrent()
		{
			CallContext.LogicalSetData(typeof(IHttpBehaviour).Name, this);
		}

		/// <summary>
		/// Retrieve the current IHttpBehaviour from the CallContext, if there is nothing available, create and make it current
		/// This never returns null
		/// </summary>
		public static IHttpBehaviour Current
		{
			get
			{
				var httpBehaviour = CallContext.LogicalGetData(typeof(IHttpBehaviour).Name) as IHttpBehaviour;
				if (httpBehaviour == null)
				{
					httpBehaviour = new HttpBehaviour();
					httpBehaviour.MakeCurrent();
				}
				return httpBehaviour;
			}
		}
	}
}
