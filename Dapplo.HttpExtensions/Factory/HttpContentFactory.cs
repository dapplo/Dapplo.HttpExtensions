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
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Dapplo.HttpExtensions.Factory
{
	/// <summary>
	/// Factory methods to create HttpContent
	/// </summary>
	public static class HttpContentFactory
	{
		/// <summary>
		/// Create a HttpContent object from the supplied content
		/// </summary>
		/// <typeparam name="TInput">type for the content</typeparam>
		/// <param name="content">content</param>
		/// <param name="httpBehaviour">IHttpBehaviour</param>
		/// <returns>HttpContent</returns>
		public static HttpContent Create<TInput>(TInput content, IHttpBehaviour httpBehaviour = null) where TInput : class
		{
			HttpContent result = null;
			if (content != null)
			{
				if (typeof(HttpContent).IsAssignableFrom(typeof(TInput)))
				{
					return content as HttpContent;
				}
				httpBehaviour = httpBehaviour ?? new HttpBehaviour();
				var httpContentConverter = httpBehaviour.HttpContentConverters.OrderBy(x => x.Order).FirstOrDefault(x => x.CanConvertToHttpContent(content, httpBehaviour));
				if (httpContentConverter != null)
				{
					result = httpContentConverter.ConvertToHttpContent(content, httpBehaviour);
					httpBehaviour.OnHttpContentCreated?.Invoke(result);
				}
			}
			return result;
		}

		/// <summary>
		/// This method processes the HttpContent
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="httpContent"></param>
		/// <param name="httpBehaviour"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public static async Task<TResult> ProcessAsync<TResult>(HttpContent httpContent, IHttpBehaviour httpBehaviour = null, CancellationToken token = default(CancellationToken)) where TResult : class
		{
			var resultType = typeof(TResult);
			if (typeof(HttpContent).IsAssignableFrom(resultType))
			{
				return httpContent as TResult;
			}
			httpBehaviour = httpBehaviour ?? new HttpBehaviour();
			var converter = httpBehaviour.HttpContentConverters.OrderBy(x => x.Order).FirstOrDefault(x => x.CanConvertFromHttpContent(resultType, httpContent, httpBehaviour));
			if (converter != null)
			{
				return await converter.ConvertFromHttpContentAsync(resultType, httpContent, httpBehaviour, token).ConfigureAwait(false) as TResult;
			}
			if (resultType == typeof(string))
			{
				return await httpContent.ReadAsStringAsync().ConfigureAwait(false) as TResult;
			}

			throw new NotSupportedException($"Unsupported result type {resultType}");
		}
	}
}
