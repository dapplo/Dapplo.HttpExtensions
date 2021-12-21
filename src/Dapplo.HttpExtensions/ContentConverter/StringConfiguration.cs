// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dapplo.HttpExtensions.Extensions;
using Dapplo.HttpExtensions.Support;

namespace Dapplo.HttpExtensions.ContentConverter;

/// <summary>
///     Configuration for the StringHttpContentConverter
/// </summary>
public class StringConfiguration : IHttpRequestConfiguration
{
    /// <summary>
    ///     Specify the supported content types
    /// </summary>
    public IList<string> SupportedContentTypes { get; } = new List<string>
    {
        MediaTypes.Txt.EnumValueOf(),
        MediaTypes.Html.EnumValueOf(),
        MediaTypes.Xml.EnumValueOf(),
        MediaTypes.TxtJson.EnumValueOf(),
        MediaTypes.Json.EnumValueOf(),
        MediaTypes.XmlReadable.EnumValueOf()
    };

    /// <summary>
    ///     Name of the configuration, this should be unique and usually is the type of the object
    /// </summary>
    public string Name { get; } = nameof(StringConfiguration);
}