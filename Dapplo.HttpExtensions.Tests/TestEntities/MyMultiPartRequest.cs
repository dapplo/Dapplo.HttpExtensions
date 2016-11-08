//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016 Dapplo
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

using System.Collections.Generic;
using Dapplo.HttpExtensions.Support;

#endregion

namespace Dapplo.HttpExtensions.Tests.TestEntities
{
	/// <summary>
	///     Example class wich is posted & filled automatically from the response information
	/// </summary>
	[HttpRequest]
	public class MyMultiPartRequest<TBitmap>
	{
		[HttpPart(HttpParts.RequestMultipartName, Order = 1)]
		public string BitmapContentName { get; set; } = "File";

		[HttpPart(HttpParts.RequestContentType, Order = 1)]
		public string BitmapContentType { get; set; } = "image/png";

		[HttpPart(HttpParts.RequestMultipartFilename, Order = 1)]
		public string BitmapFileName { get; set; } = "empty.png";

		[HttpPart(HttpParts.RequestContentType, Order = 0)]
		public string ContentType { get; set; } = "application/json";

		[HttpPart(HttpParts.RequestHeaders)]
		public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>();

		[HttpPart(HttpParts.RequestContent, Order = 0)]
		public object JsonInformation { get; set; }

		[HttpPart(HttpParts.RequestContent, Order = 1)]
		public TBitmap OurBitmap { get; set; }
	}
}