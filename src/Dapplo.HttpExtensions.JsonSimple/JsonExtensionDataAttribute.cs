// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dapplo.HttpExtensions.JsonSimple
{
    /// <summary>
    ///     Use this attribute on a IDictionary to be able to receive all data which cannot be matched in the target type
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class JsonExtensionDataAttribute : Attribute
    {
        /// <summary>
        ///     Specify a regex to match the property names of the Json content
        /// </summary>
        public string Pattern { get; set; }
    }
}