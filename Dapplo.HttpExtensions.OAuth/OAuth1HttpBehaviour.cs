//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2015-2017 Dapplo
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
using System.Net;
using System.Net.Http;
using System.Text;

#endregion

namespace Dapplo.HttpExtensions.OAuth
{
	/// <summary>
	///     Implementation of the IHttpBehaviour which bases upon the HttpBehaviour and extends it with special OAuth 1
	///     functionality
	/// </summary>
	public class OAuth1HttpBehaviour : IChangeableHttpBehaviour
	{
		private readonly IChangeableHttpBehaviour _wrapped;

		/// <summary>
		///     Create a OAuthHttpBehaviour
		/// </summary>
		/// <param name="httpBehaviour">IHttpBehaviour to wrap</param>
		public OAuth1HttpBehaviour(IHttpBehaviour httpBehaviour = null)
		{
			_wrapped = (httpBehaviour ?? HttpBehaviour.Current).ShallowClone();
		}

		/// <summary>
		///     Set this function if you want to modify the request message that is send to the service
		/// </summary>
		public Action<HttpRequestMessage> BeforeSend { get; set; }

		/// <summary>
		///     Set this function if you want to process any additional access token values
		/// </summary>
		public Action<IDictionary<string, string>> OnAccessTokenValues { get; set; }

		/// <inheritdoc />
		public IDictionary<string, IHttpRequestConfiguration> RequestConfigurations
		{ 
			get { return _wrapped.RequestConfigurations; }
			set { _wrapped.RequestConfigurations = value;}
		}

		/// <inheritdoc />
		public Encoding DefaultEncoding
		{
			get { return _wrapped.DefaultEncoding; }
			set { _wrapped.DefaultEncoding = value; }
		}

		/// <inheritdoc />
		public Action<float> DownloadProgress
		{
			get { return _wrapped.DownloadProgress; }
			set { _wrapped.DownloadProgress = value; }
		}

		/// <inheritdoc />
		public HttpCompletionOption HttpCompletionOption
		{
			get { return _wrapped.HttpCompletionOption; }
			set { _wrapped.HttpCompletionOption = value; }
		}

		/// <inheritdoc />
		public IList<IHttpContentConverter> HttpContentConverters
		{
			get { return _wrapped.HttpContentConverters; }
			set { _wrapped.HttpContentConverters = value; }
		}

		/// <inheritdoc />
		public IHttpSettings HttpSettings
		{
			get { return _wrapped.HttpSettings; }
			set { _wrapped.HttpSettings = value; }
		}

		/// <inheritdoc />
		public IJsonSerializer JsonSerializer
		{
			get { return _wrapped.JsonSerializer; }
			set { _wrapped.JsonSerializer = value; }
		}

		/// <inheritdoc />
		public Action<HttpClient> OnHttpClientCreated
		{
			get { return _wrapped.OnHttpClientCreated; }
			set { _wrapped.OnHttpClientCreated = value; }
		}

		/// <inheritdoc />
		public Func<HttpContent, HttpContent> OnHttpContentCreated
		{
			get { return _wrapped.OnHttpContentCreated; }
			set { _wrapped.OnHttpContentCreated = value; }
		}

		/// <inheritdoc />
		public Func<HttpMessageHandler, HttpMessageHandler> OnHttpMessageHandlerCreated
		{
			get { return _wrapped.OnHttpMessageHandlerCreated; }
			set { _wrapped.OnHttpMessageHandlerCreated = value; }
		}

		/// <inheritdoc />
		public Func<HttpRequestMessage, HttpRequestMessage> OnHttpRequestMessageCreated
		{
			get { return _wrapped.OnHttpRequestMessageCreated; }
			set { _wrapped.OnHttpRequestMessageCreated = value; }
		}

		/// <inheritdoc />
		public int ReadBufferSize
		{
			get { return _wrapped.ReadBufferSize; }
			set { _wrapped.ReadBufferSize = value; }
		}

		/// <inheritdoc />
		public bool ThrowOnError
		{
			get { return _wrapped.ThrowOnError; }
			set { _wrapped.ThrowOnError = value; }
		}

		/// <inheritdoc />
		public Action<float> UploadProgress
		{
			get { return _wrapped.UploadProgress; }
			set { _wrapped.UploadProgress = value; }
		}

		/// <inheritdoc />
		public bool UseProgressStream
		{
			get { return _wrapped.UseProgressStream; }
			set { _wrapped.UseProgressStream = value; }
		}

		/// <inheritdoc />
		public bool ValidateResponseContentType
		{
			get { return _wrapped.ValidateResponseContentType; }
			set { _wrapped.ValidateResponseContentType = value; }
		}

		/// <inheritdoc />
		public CookieContainer CookieContainer
		{
			get { return _wrapped.CookieContainer; }
			set { _wrapped.CookieContainer = value; }
		}

		/// <inheritdoc />
		public IChangeableHttpBehaviour ShallowClone()
		{
			// the wrapper object will be clone when creating the OAuth1HttpBehaviour
			var result = new OAuth1HttpBehaviour(_wrapped)
			{
				OnAccessTokenValues = OnAccessTokenValues,
				BeforeSend = BeforeSend
			};
			return result;
		}

		/// <inheritdoc />
		public void MakeCurrent()
		{
			_wrapped.MakeCurrent();
		}
	}
}