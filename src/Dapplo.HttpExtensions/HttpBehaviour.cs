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

#region Usings

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
#if !NET45
using System.Threading;
#else
using Nito.AsyncEx.AsyncLocal;
#endif

#endregion


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

        /// <inheritdoc />
        public IHttpSettings HttpSettings { get; set; } = HttpExtensionsGlobals.HttpSettings;

        /// <inheritdoc />
        public IJsonSerializer JsonSerializer { get; set; } = HttpExtensionsGlobals.JsonSerializer;

        /// <inheritdoc />
        public IList<IHttpContentConverter> HttpContentConverters { get; set; } = HttpExtensionsGlobals.HttpContentConverters;

        /// <inheritdoc />
        public Func<HttpRequestMessage, HttpRequestMessage> OnHttpRequestMessageCreated { get; set; }

        /// <inheritdoc />
        public Action<HttpClient> OnHttpClientCreated { get; set; }

        /// <inheritdoc />
        public Func<HttpMessageHandler, HttpMessageHandler> OnHttpMessageHandlerCreated { get; set; }

        /// <inheritdoc />
        public Func<HttpContent, HttpContent> OnHttpContentCreated { get; set; }

        /// <inheritdoc />
        public Action<float> UploadProgress { get; set; }

        /// <inheritdoc />
        public Action<float> DownloadProgress { get; set; }

        /// <inheritdoc />
        public bool CallProgressOnUiContext { get; set; } = HttpExtensionsGlobals.CallProgressOnUiContext;

        /// <inheritdoc />
        public bool UseProgressStream { get; set; } = HttpExtensionsGlobals.UseProgressStream;

        /// <inheritdoc />
        public bool ThrowOnError { get; set; } = HttpExtensionsGlobals.ThrowOnError;

        /// <summary>
        ///     The ResponseHeadersRead forces a pause between the initial response and reading the content, this is needed for
        ///     better error handling and progress
        ///     Turning this to ResponseContentRead might change the behaviour
        /// </summary>
        public HttpCompletionOption HttpCompletionOption { get; set; } = HttpCompletionOption.ResponseHeadersRead;

        /// <inheritdoc />
        public bool ValidateResponseContentType { get; set; } = HttpExtensionsGlobals.ValidateResponseContentType;

        /// <summary>
        ///     Configuration for different parts of the library, or your own implementations, which can be set on a thread/request
        ///     base
        /// </summary>
        public IDictionary<string, IHttpRequestConfiguration> RequestConfigurations { get; set; } = new Dictionary<string, IHttpRequestConfiguration>();

        /// <inheritdoc />
        public Encoding DefaultEncoding { get; set; } = HttpExtensionsGlobals.DefaultEncoding;

        /// <inheritdoc />
        public int ReadBufferSize { get; set; } = HttpExtensionsGlobals.ReadBufferSize;

        /// <inheritdoc />
        public CookieContainer CookieContainer { get; set; } = new CookieContainer();

        /// <inheritdoc />
        public IChangeableHttpBehaviour ShallowClone()
        {
            var result = (HttpBehaviour) MemberwiseClone();
            result.HttpSettings = HttpSettings.ShallowClone();
            // Make sure the RequestConfigurations are copied but changeable
            if (RequestConfigurations != null)
            {
                result.RequestConfigurations = new Dictionary<string, IHttpRequestConfiguration>();
                foreach (var key in RequestConfigurations.Keys)
                {
                    result.RequestConfigurations[key] = RequestConfigurations[key];
                }
            }
            // Make sure the RequestConfigurations are copied but changeable
            if (HttpContentConverters != null)
            {
                result.HttpContentConverters = new List<IHttpContentConverter>(HttpContentConverters);
            }
            return result;
        }

        /// <inheritdoc />
        public void MakeCurrent()
        {
            AsyncLocalBehavior.Value = this;
        }
    }
}