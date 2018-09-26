//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2018 Dapplo
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
using Dapplo.Log;
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
        private static readonly LogSource Log = new LogSource();
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
                if (httpBehaviour is null)
                {
                    httpBehaviour = new HttpBehaviour();
                    httpBehaviour.MakeCurrent();
                }
                return httpBehaviour;
            }
        }

        /// <inheritdoc cref="IHttpBehaviour" />
        public IHttpSettings HttpSettings { get; set; } = HttpExtensionsGlobals.HttpSettings;

        /// <inheritdoc cref="IHttpBehaviour" />
        public IJsonSerializer JsonSerializer { get; set; } = HttpExtensionsGlobals.JsonSerializer;

        /// <inheritdoc cref="IHttpBehaviour" />
        public IList<IHttpContentConverter> HttpContentConverters { get; set; } = HttpExtensionsGlobals.HttpContentConverters;

        /// <inheritdoc cref="IHttpBehaviour" />
        public Func<HttpRequestMessage, HttpRequestMessage> OnHttpRequestMessageCreated { get; set; }

        /// <inheritdoc cref="IHttpBehaviour" />
        public Action<HttpClient> OnHttpClientCreated { get; set; }

        /// <inheritdoc cref="IHttpBehaviour" />
        public Func<HttpMessageHandler, HttpMessageHandler> OnHttpMessageHandlerCreated { get; set; }

        /// <inheritdoc cref="IHttpBehaviour" />
        public Func<HttpContent, HttpContent> OnHttpContentCreated { get; set; }

        /// <inheritdoc cref="IHttpBehaviour" />
        public Action<float> UploadProgress { get; set; }

        /// <inheritdoc cref="IHttpBehaviour" />
        public Action<float> DownloadProgress { get; set; }

        /// <inheritdoc cref="IHttpBehaviour" />
        public bool UseProgressStream { get; set; } = HttpExtensionsGlobals.UseProgressStream;

        /// <inheritdoc cref="IHttpBehaviour" />
        public bool ThrowOnError { get; set; } = HttpExtensionsGlobals.ThrowOnError;

        /// <summary>
        ///     The ResponseHeadersRead forces a pause between the initial response and reading the content, this is needed for
        ///     better error handling and progress
        ///     Turning this to ResponseContentRead might change the behaviour
        /// </summary>
        public HttpCompletionOption HttpCompletionOption { get; set; } = HttpCompletionOption.ResponseHeadersRead;

        /// <inheritdoc cref="IHttpBehaviour" />
        public bool ValidateResponseContentType { get; set; } = HttpExtensionsGlobals.ValidateResponseContentType;

        /// <summary>
        ///     Configuration for different parts of the library, or your own implementations, which can be set on a thread/request
        ///     base
        /// </summary>
        public IDictionary<string, IHttpRequestConfiguration> RequestConfigurations { get; set; } = new Dictionary<string, IHttpRequestConfiguration>();

        /// <inheritdoc cref="IHttpBehaviour" />
        public Encoding DefaultEncoding { get; set; } = HttpExtensionsGlobals.DefaultEncoding;

        /// <inheritdoc cref="IHttpBehaviour" />
        public int ReadBufferSize { get; set; } = HttpExtensionsGlobals.ReadBufferSize;

        /// <inheritdoc cref="IHttpBehaviour" />
        public CookieContainer CookieContainer { get; set; } = new CookieContainer();

        /// <inheritdoc cref="IHttpBehaviour" />
        public IChangeableHttpBehaviour ShallowClone()
        {
            var result = (HttpBehaviour) MemberwiseClone();
            // Sometimes we can't ShallowClone, use the original in this case
            try
            {
                result.HttpSettings = HttpSettings.ShallowClone();
            }
            catch (NotImplementedException ex)
            {
                Log.Warn().WriteLine(ex, "Couldn't ShallowClone {0}, using original", GetType());
                result.HttpSettings = HttpSettings;
            }
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

        /// <inheritdoc cref="IHttpBehaviour" />
        public void MakeCurrent()
        {
            AsyncLocalBehavior.Value = this;
        }
    }
}