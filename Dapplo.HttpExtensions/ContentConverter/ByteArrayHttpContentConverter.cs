//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2017 Dapplo
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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.Log;

#endregion

namespace Dapplo.HttpExtensions.ContentConverter
{
    /// <summary>
    ///     This can convert HttpContent from/to a byte[]
    /// </summary>
    public class ByteArrayHttpContentConverter : IHttpContentConverter
    {
        private static readonly LogSource Log = new LogSource();


        /// <summary>
        ///     Instance of this IHttpContentConverter for reusing
        /// </summary>
        public static Lazy<ByteArrayHttpContentConverter> Instance { get; } = new Lazy<ByteArrayHttpContentConverter>(() => new ByteArrayHttpContentConverter());

        /// <summary>
        ///     Order or priority of the IHttpContentConverter
        /// </summary>
        public int Order => 0;

        /// <summary>
        ///     Check if we can convert from the HttpContent to a byte array
        /// </summary>
        /// <param name="typeToConvertTo">To what type will the result be assigned</param>
        /// <param name="httpContent">HttpContent</param>
        /// <returns>true if we can convert the HttpContent to a ByteArray</returns>
        public bool CanConvertFromHttpContent(Type typeToConvertTo, HttpContent httpContent)
        {
            return typeToConvertTo == typeof(byte[]);
        }

        /// <inheritdoc />
        public async Task<object> ConvertFromHttpContentAsync(Type resultType, HttpContent httpContent, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!CanConvertFromHttpContent(resultType, httpContent))
            {
                throw new NotSupportedException("CanConvertFromHttpContent resulted in false, this is not supposed to be called.");
            }
            Log.Debug().WriteLine("Retrieving the content as byte[], Content-Type: {0}", httpContent.Headers.ContentType);

            return await httpContent.ReadAsByteArrayAsync().ConfigureAwait(false);
        }

        /// <inheritdoc />
        public bool CanConvertToHttpContent(Type typeToConvert, object content)
        {
            return typeToConvert == typeof(byte[]);
        }

        /// <inheritdoc />
        public HttpContent ConvertToHttpContent(Type typeToConvert, object content)
        {
            var byteArray = content as byte[];
            return new ByteArrayContent(byteArray);
        }

        /// <inheritdoc />
        public void AddAcceptHeadersForType(Type resultType, HttpRequestMessage httpRequestMessage)
        {
            if (resultType == null)
            {
                throw new ArgumentNullException(nameof(resultType));
            }
            if (httpRequestMessage == null)
            {
                throw new ArgumentNullException(nameof(httpRequestMessage));
            }
        }
    }
}