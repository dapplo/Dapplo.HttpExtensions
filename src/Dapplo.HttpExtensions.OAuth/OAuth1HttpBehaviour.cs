//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2019 Dapplo
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

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

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

        /// <inheritdoc cref="IHttpBehaviour" />
        public IDictionary<string, IHttpRequestConfiguration> RequestConfigurations
        {
            get => _wrapped.RequestConfigurations;
            set => _wrapped.RequestConfigurations = value;
        }

        /// <inheritdoc cref="IHttpBehaviour" />
        public Encoding DefaultEncoding
        {
            get => _wrapped.DefaultEncoding;
            set => _wrapped.DefaultEncoding = value;
        }

        /// <inheritdoc cref="IHttpBehaviour" />
        public Action<float> DownloadProgress
        {
            get => _wrapped.DownloadProgress;
            set => _wrapped.DownloadProgress = value;
        }

        /// <inheritdoc cref="IHttpBehaviour" />
        public HttpCompletionOption HttpCompletionOption
        {
            get => _wrapped.HttpCompletionOption;
            set => _wrapped.HttpCompletionOption = value;
        }

        /// <inheritdoc cref="IHttpBehaviour" />
        public IList<IHttpContentConverter> HttpContentConverters
        {
            get => _wrapped.HttpContentConverters;
            set => _wrapped.HttpContentConverters = value;
        }

        /// <inheritdoc cref="IHttpBehaviour" />
        public IHttpSettings HttpSettings
        {
            get => _wrapped.HttpSettings;
            set => _wrapped.HttpSettings = value;
        }

        /// <inheritdoc cref="IHttpBehaviour" />
        public IJsonSerializer JsonSerializer
        {
            get => _wrapped.JsonSerializer;
            set => _wrapped.JsonSerializer = value;
        }

        /// <inheritdoc cref="IHttpBehaviour" />
        public Action<HttpClient> OnHttpClientCreated
        {
            get => _wrapped.OnHttpClientCreated;
            set => _wrapped.OnHttpClientCreated = value;
        }

        /// <inheritdoc cref="IHttpBehaviour" />
        public Func<HttpContent, HttpContent> OnHttpContentCreated
        {
            get => _wrapped.OnHttpContentCreated;
            set => _wrapped.OnHttpContentCreated = value;
        }

        /// <inheritdoc cref="IHttpBehaviour" />
        public Func<HttpMessageHandler, HttpMessageHandler> OnHttpMessageHandlerCreated
        {
            get => _wrapped.OnHttpMessageHandlerCreated;
            set => _wrapped.OnHttpMessageHandlerCreated = value;
        }

        /// <inheritdoc cref="IHttpBehaviour" />
        public Func<HttpRequestMessage, HttpRequestMessage> OnHttpRequestMessageCreated
        {
            get => _wrapped.OnHttpRequestMessageCreated;
            set => _wrapped.OnHttpRequestMessageCreated = value;
        }

        /// <inheritdoc cref="IHttpBehaviour" />
        public int ReadBufferSize
        {
            get => _wrapped.ReadBufferSize;
            set => _wrapped.ReadBufferSize = value;
        }

        /// <inheritdoc cref="IHttpBehaviour" />
        public bool ThrowOnError
        {
            get => _wrapped.ThrowOnError;
            set => _wrapped.ThrowOnError = value;
        }

        /// <inheritdoc cref="IHttpBehaviour" />
        public Action<float> UploadProgress
        {
            get => _wrapped.UploadProgress;
            set => _wrapped.UploadProgress = value;
        }

        /// <inheritdoc cref="IHttpBehaviour" />
        public bool UseProgressStream
        {
            get => _wrapped.UseProgressStream;
            set => _wrapped.UseProgressStream = value;
        }

        /// <inheritdoc cref="IHttpBehaviour" />
        public bool ValidateResponseContentType
        {
            get => _wrapped.ValidateResponseContentType;
            set => _wrapped.ValidateResponseContentType = value;
        }

        /// <inheritdoc cref="IHttpBehaviour" />
        public CookieContainer CookieContainer
        {
            get => _wrapped.CookieContainer;
            set => _wrapped.CookieContainer = value;
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