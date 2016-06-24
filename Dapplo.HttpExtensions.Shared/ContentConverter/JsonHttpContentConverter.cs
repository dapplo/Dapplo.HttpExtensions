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
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.Support;
using Dapplo.Log.Facade;
using Dapplo.Utils.Extensions;

#endregion

namespace Dapplo.HttpExtensions.ContentConverter
{
	/// <summary>
	///     This can convert HttpContent from/to Json
	///     TODO: add JsonObject from SimpleJson for more clear generic code..
	/// </summary>
	public class JsonHttpContentConverter : IHttpContentConverter
	{
		private static readonly LogSource Log = new LogSource();

		/// <summary>
		///     Singleton instance for reuse
		/// </summary>
		public static readonly JsonHttpContentConverter Instance = new JsonHttpContentConverter();

		private static readonly IList<string> SupportedContentTypes = new List<string>();

		static JsonHttpContentConverter()
		{
			SupportedContentTypes.Add(MediaTypes.Json.EnumValueOf());
		}

		/// <summary>
		///     If the json content is any longer than LogThreshold AppendedWhenCut is appended to the cut string
		/// </summary>
		public string AppendedWhenCut { get; set; } = "...";

		/// <summary>
		///     This is the amount of characters that are written to the log, if the json content is any longer that it will be cut
		///     (and AppendedWhenCut is appended)
		/// </summary>
		public int LogThreshold { get; set; } = 256;

		/// <inheritdoc />
		public int Order => int.MaxValue;

		/// <inheritdoc />
		public bool CanConvertFromHttpContent(Type typeToConvertTo, HttpContent httpContent)
		{
			if (!typeToConvertTo.GetTypeInfo().IsClass && !typeToConvertTo.GetTypeInfo().IsInterface)
			{
				return false;
			}
			var httpBehaviour = HttpBehaviour.Current;
			return !httpBehaviour.ValidateResponseContentType || SupportedContentTypes.Contains(httpContent.GetContentType());
		}

		/// <inheritdoc />
		public async Task<object> ConvertFromHttpContentAsync(Type resultType, HttpContent httpContent, CancellationToken token = default(CancellationToken))
		{
			if (!CanConvertFromHttpContent(resultType, httpContent))
			{
				throw new NotSupportedException("CanConvertFromHttpContent resulted in false, this is not supposed to be called.");
			}

			var jsonString = await httpContent.ReadAsStringAsync().ConfigureAwait(false);
			// Check if verbose is enabled, if so log but only up to a certain size
			if (Log.IsVerboseEnabled())
			{
				if (LogThreshold > 0)
				{
					Log.Verbose().WriteLine("Read Json content: {0}{1}", jsonString.Substring(0, Math.Min(jsonString.Length, LogThreshold)), jsonString.Length > LogThreshold ? AppendedWhenCut : string.Empty);
				}
				else
				{
					Log.Verbose().WriteLine("Read Json content: {0}", jsonString);
				}
			}
			// Check if we can just pass it back, as the target is string
			if (resultType == typeof (string))
			{
				return jsonString;
			}
			// empty json should return the default of the resultType
			if (string.IsNullOrEmpty(jsonString))
			{
				return resultType.Default();
			}
			var httpBehaviour = HttpBehaviour.Current;
			return httpBehaviour.JsonSerializer.DeserializeJson(resultType == typeof (object) ? null : resultType, jsonString);
		}

		/// <inheritdoc />
		public bool CanConvertToHttpContent(Type typeToConvert, object content)
		{
			return typeToConvert != typeof (string);
		}

		/// <inheritdoc />
		public HttpContent ConvertToHttpContent(Type typeToConvert, object content)
		{
			var httpBehaviour = HttpBehaviour.Current;
			var jsonString = httpBehaviour.JsonSerializer.SerializeJson(content);
			Log.Debug().WriteLine("Created HttpContent for Json: {0}", jsonString);
			return new StringContent(jsonString, httpBehaviour.DefaultEncoding, MediaTypes.Json.EnumValueOf());
		}

		/// <summary>
		///     Add Accept-Headers to the HttpRequestMessage, depending on the passt resultType.
		///     This tries to hint the Http server what we can accept, which depends on the type of the return value
		/// </summary>
		/// <param name="resultType">Result type, this where to a conversion from HttpContent is made</param>
		/// <param name="httpRequestMessage">HttpRequestMessage</param>
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
			// TODO: How to prevent the adding if this type is really not something we can de-serialize, like Bitmap?
			httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypes.Json.EnumValueOf()));
			Log.Debug().WriteLine("Modified the header(s) of the HttpRequestMessage: Accept: {0}", httpRequestMessage.Headers.Accept);
		}
	}
}