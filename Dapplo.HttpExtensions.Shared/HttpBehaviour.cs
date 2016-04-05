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
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;

#endregion

#if DOTNET45
using Nito.AsyncEx.AsyncLocal;
#endif

namespace Dapplo.HttpExtensions
{
	/// <summary>
	///     This is the default implementation of the IHttpBehaviour, see IHttpBehaviour for details
	///     Most values are initialized via the HttpExtensionsGlobals
	/// </summary>
	public class HttpBehaviour : IChangeableHttpBehaviour
	{
		private static readonly AsyncLocal<IHttpBehaviour> AsyncLocalBehavior = new AsyncLocal<IHttpBehaviour>();

		/// <summary>
		///     Retrieve the current IHttpBehaviour from the CallContext, if there is nothing available, create and make it current
		///     This never returns null
		/// </summary>
		public static IHttpBehaviour Current
		{
			get
			{
				var httpBehaviour = AsyncLocalBehavior.Value;
				if (httpBehaviour == null)
				{
					httpBehaviour = new HttpBehaviour();
					httpBehaviour.MakeCurrent();
				}
				return httpBehaviour;
			}
		}

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
		///     The ResponseHeadersRead forces a pause between the initial response and reading the content, this is needed for
		///     better error handling and progress
		///     Turning this to ResponseContentRead might change the behaviour
		/// </summary>
		public HttpCompletionOption HttpCompletionOption { get; set; } = HttpCompletionOption.ResponseHeadersRead;

		public bool ValidateResponseContentType { get; set; } = HttpExtensionsGlobals.ValidateResponseContentType;

		public Encoding DefaultEncoding { get; set; } = HttpExtensionsGlobals.DefaultEncoding;

		public int ReadBufferSize { get; set; } = HttpExtensionsGlobals.ReadBufferSize;

		public IChangeableHttpBehaviour Clone()
		{
			return (HttpBehaviour) MemberwiseClone();
		}

		public void MakeCurrent()
		{
			AsyncLocalBehavior.Value = this;
		}
	}
}