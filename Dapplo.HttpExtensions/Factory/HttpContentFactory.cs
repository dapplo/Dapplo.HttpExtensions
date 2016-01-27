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

using System.Net.Http;
using System.Linq;
using Dapplo.HttpExtensions.Internal;

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
			if (content == null) return null;

			if (typeof(HttpContent).IsAssignableFrom(typeof(TInput)))
			{
				return content as HttpContent;
			}
			httpBehaviour = httpBehaviour ?? new HttpBehaviour();
			var httpContentConverter = httpBehaviour.HttpContentConverters.OrderBy(x => x.Order).FirstOrDefault(x => x.CanConvertToHttpContent(content, httpBehaviour));
			if (httpContentConverter == null) return null;

			var result = httpContentConverter.ConvertToHttpContent(content, httpBehaviour);
			httpBehaviour.OnHttpContentCreated?.Invoke(result);
			return result;
		}
	}
}
