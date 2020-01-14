// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.HttpExtensions.Support
{
    /// <summary>
    ///     This class contains all the information on the content that will be used to build a request
    /// </summary>
    internal class ContentItem
    {
        public object Content { get; set; }
        public string ContentFileName { get; set; }
        public string ContentName { get; set; }
        public string ContentType { get; set; }
        public int Order { get; set; }
    }
}