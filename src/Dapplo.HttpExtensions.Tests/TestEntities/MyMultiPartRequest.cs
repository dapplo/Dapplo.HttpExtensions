// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dapplo.HttpExtensions.Support;

namespace Dapplo.HttpExtensions.Tests.TestEntities;

/// <summary>
///     Example class wich is posted and filled automatically from the response information
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