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

using System.Net;
using System.Net.Http.Headers;
using Dapplo.HttpExtensions.Support;

namespace Dapplo.HttpExtensions
{
    /// <summary>
    ///     This container can be used to get the details of a response.
    ///     You can specify your own container, by using the HttpAttribute.
    /// </summary>
    [HttpResponse]
    public class HttpResponse
    {
        /// <summary>
        ///     The Content-Type of the response
        ///     Will be filled due to the annotation
        /// </summary>
        [HttpPart(HttpParts.ResponseContentType)]
        public string ContentType { get; set; }

        /// <summary>
        ///     The reponse headers
        ///     Will be filled due to the annotation
        /// </summary>
        [HttpPart(HttpParts.ResponseHeaders)]
        public HttpResponseHeaders Headers { get; set; }

        /// <summary>
        ///     The response http status code
        ///     Will be filled due to the annotation
        /// </summary>
        [HttpPart(HttpParts.ResponseStatuscode)]
        public HttpStatusCode StatusCode { get; set; }
    }

    /// <summary>
    ///     This container can be used to get the details of a response without content but with a potential error
    /// </summary>
    /// <typeparam name="TErrorResponse">Type for the error response</typeparam>
    [HttpResponse]
    public class HttpResponseWithError<TErrorResponse> : HttpResponse
        where TErrorResponse : class
    {
        /// <summary>
        ///     The response if there was an error
        ///     Will be filled due to the annotation
        /// </summary>
        [HttpPart(HttpParts.ResponseErrorContent)]
        public TErrorResponse ErrorResponse { get; set; }

        /// <summary>
        ///     Was there an error?
        /// </summary>
        public bool HasError => ErrorResponse != null;
    }

    /// <summary>
    ///     This container can be used to get the details of a response.
    /// </summary>
    /// <typeparam name="TResponse">Type for the normal response</typeparam>
    [HttpResponse]
    public class HttpResponse<TResponse> : HttpResponse
        where TResponse : class
    {
        /// <summary>
        ///     The response, if there was no error
        ///     Will be filled due to the annotation
        /// </summary>
        [HttpPart(HttpParts.ResponseContent)]
        public TResponse Response { get; set; }
    }

    /// <summary>
    ///     This container can be used to get the details of a response.
    ///     It also makes it possible to process the error information, and eventually do something different.
    ///     You can specify your own container, by using the HttpAttribute.
    /// </summary>
    /// <typeparam name="TResponse">Type for the normal response</typeparam>
    /// <typeparam name="TErrorResponse">Type for the error response</typeparam>
    [HttpResponse]
    public class HttpResponse<TResponse, TErrorResponse> : HttpResponseWithError<TErrorResponse>
        where TResponse : class
        where TErrorResponse : class
    {
        /// <summary>
        ///     The response, if there was no error
        ///     Will be filled due to the annotation
        /// </summary>
        [HttpPart(HttpParts.ResponseContent)]
        public TResponse Response { get; set; }
    }
}