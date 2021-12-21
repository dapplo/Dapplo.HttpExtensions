// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.HttpExtensions.ContentConverter;

/// <summary>
///     This is a configuration class for the DefaultJsonHttpContentConverter
/// </summary>
public class DefaultJsonHttpContentConverterConfiguration : IHttpRequestConfiguration
{
    /// <summary>
    ///     Name of the configuration, this should be unique and usually is the type of the object
    /// </summary>
    public string Name { get; } = nameof(DefaultJsonHttpContentConverterConfiguration);

    /// <summary>
    ///     If the json content is any longer than LogThreshold AppendedWhenCut is appended to the cut string
    /// </summary>
    public string AppendedWhenCut { get; set; } = "...";

    /// <summary>
    ///     This is the amount of characters that are written to the log, if the json content is any longer that it will be cut
    ///     (and AppendedWhenCut is appended)
    /// </summary>
    public int LogThreshold { get; set; } = 256;
}