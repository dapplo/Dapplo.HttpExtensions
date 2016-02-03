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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.LogFacade;
using Dapplo.HttpExtensions.Support;
using System.Collections.Generic;

namespace Dapplo.HttpExtensions.ContentConverter
{
	/// <summary>
	/// This can convert HttpContent from/to Json
	/// TODO: add JsonObject from SimpleJson for more clear generic code..
	/// </summary>
	public class JsonHttpContentConverter : IHttpContentConverter
	{
		private static readonly LogSource Log = new LogSource();
		public static readonly JsonHttpContentConverter Instance = new JsonHttpContentConverter();
		private static readonly IList<string> SupportedContentTypes = new List<string>();

		static JsonHttpContentConverter()
		{
			SupportedContentTypes.Add(MediaTypes.Json.EnumValueOf());
		}

		/// <summary>
		/// Sort order
		/// </summary>
		public int Order => int.MaxValue;

		/// <summary>
		/// If the json content is any longer than LogThreshold AppendedWhenCut is appended to the cut string
		/// </summary>
		public string AppendedWhenCut { get; set; } = "...";

		/// <summary>
		/// This is the amount of characters that are written to the log, if the json content is any longer that it will be cut (and AppendedWhenCut is appended)
		/// </summary>
		public int LogThreshold { get; set; } = 512;



		public bool CanConvertFromHttpContent<TResult>(HttpContent httpContent, IHttpBehaviour httpBehaviour = null)
			where TResult : class
		{
			return CanConvertFromHttpContent(typeof (TResult), httpContent, httpBehaviour);
		}

		public bool CanConvertFromHttpContent(Type typeToConvertTo, HttpContent httpContent, IHttpBehaviour httpBehaviour = null)
		{
			httpBehaviour = httpBehaviour ?? new HttpBehaviour();
			return !httpBehaviour.ValidateResponseContentType || SupportedContentTypes.Contains(httpContent.GetContentType());
		}

		public async Task<TResult> ConvertFromHttpContentAsync<TResult>(HttpContent httpContent, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken))
			where TResult : class
		{
			return await ConvertFromHttpContentAsync(typeof (TResult), httpContent, httpBehaviour, token).ConfigureAwait(false) as TResult;
		}

		public async Task<object> ConvertFromHttpContentAsync(Type resultType, HttpContent httpContent, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken))
		{
			httpBehaviour = httpBehaviour ?? new HttpBehaviour();
			if (!CanConvertFromHttpContent(resultType, httpContent, httpBehaviour))
			{
				throw new NotSupportedException("CanConvertFromHttpContent resulted in false, this is not supposed to be called.");
			}

			var jsonString = await httpContent.ReadAsStringAsync().ConfigureAwait(false);
			// Check if verbose is enabled, if so log but only up to a certain size
			if (Log.IsVerboseEnabled())
			{
				Log.Verbose().WriteLine("Read Json content: {0}{1}", jsonString.Substring(0, Math.Min(jsonString.Length, LogThreshold)), jsonString.Length > LogThreshold ? AppendedWhenCut : string.Empty);
			}
			return httpBehaviour.JsonSerializer.DeserializeJson(resultType == typeof(object) ? null : resultType, jsonString);
		}

		public bool CanConvertToHttpContent(Type typeToConvert, object content, IHttpBehaviour httpBehaviour = null)
		{
			return typeToConvert != typeof (string);
		}

		public bool CanConvertToHttpContent<TInput>(TInput content, IHttpBehaviour httpBehaviour = null)
			where TInput : class
		{
			return CanConvertToHttpContent(typeof (TInput), content, httpBehaviour);
		}

		public HttpContent ConvertToHttpContent(Type typeToConvert, object content, IHttpBehaviour httpBehaviour = null)
		{
			httpBehaviour = httpBehaviour ?? new HttpBehaviour();
			var jsonString = httpBehaviour.JsonSerializer.SerializeJson(content);
			Log.Debug().WriteLine("Created HttpContent for Json: {0}", jsonString);
			return new StringContent(jsonString, httpBehaviour.DefaultEncoding, MediaTypes.Json.EnumValueOf());
		}

		public HttpContent ConvertToHttpContent<TInput>(TInput content, IHttpBehaviour httpBehaviour = null)
			where TInput : class
		{
			return ConvertToHttpContent(typeof (TInput), content, httpBehaviour);
		}

		public void AddAcceptHeadersForType(Type resultType, HttpRequestMessage httpRequestMessage, IHttpBehaviour httpBehaviour = null)
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
