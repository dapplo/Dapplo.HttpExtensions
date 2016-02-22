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
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.ContentConverter;
using Dapplo.HttpExtensions.Support;
using Dapplo.LogFacade;

#if DESKTOP
using Dapplo.HttpExtensions.Desktop;
#endif

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// These are the globals for some of the important configurable settings
	/// When a HttpBehaviour is created, some of the values from here will be copied. (unless diffently specified)
	/// </summary>
	public static class HttpExtensionsGlobals
	{
		private static readonly LogSource Log = new LogSource();

		// Used for UI stuff, e.g. EmbeddedBrowserCodeReceiver
		private static TaskScheduler _uiTaskScheduler;

		static HttpExtensionsGlobals()
		{
			// Try to store the current SynchronizationContext
			try
			{
				_uiTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
			}
			catch (Exception ex)
			{
				Log.Warn().WriteLine(ex, "Can't capture the UI TaskScheduler, this might cause issues when an EmbeddedBrowserCodeReceiver is used.");
			}
		}

		/// <summary>
		/// The global IHttpSettings
		/// </summary>
		public static IHttpSettings HttpSettings { get; set; } = new HttpSettings();

		/// <summary>
		/// The global JsonSerializer
		/// </summary>
		public static IJsonSerializer JsonSerializer { get; set; } = new SimpleJsonSerializer();

		/// <summary>
		/// The global list of HttpContent converters
		/// </summary>
		public static IList<IHttpContentConverter> HttpContentConverters { get; set; } = new List<IHttpContentConverter>
		{
#if DESKTOP
			BitmapHttpContentConverter.Instance,
			BitmapSourceHttpContentConverter.Instance,
#endif
			ByteArrayHttpContentConverter.Instance,
			FormUriEncodedContentConverter.Instance,
			SyndicationFeedHttpContentConverter.Instance,
			XDocumentHttpContentConverter.Instance,
			JsonHttpContentConverter.Instance,
			StreamHttpContentConverter.Instance,
			StringHttpContentConverter.Instance
		};

		/// <summary>
		/// Global value for UseProgressStream, see IHttpBehaviour
		/// </summary>
		public static bool UseProgressStreamContent { get; set; } = true;

		/// <summary>
		/// Global value for ThrowOnError, see IHttpBehaviour
		/// </summary>
		public static bool ThrowOnError { get; set; } = true;

		/// <summary>
		/// Global validate response content-type
		/// </summary>
		public static bool ValidateResponseContentType { get; set; } = true;

		/// <summary>
		/// The global default encoding
		/// </summary>
		public static Encoding DefaultEncoding { get; set; } = Encoding.UTF8;

		/// <summary>
		/// The global read buffer-size
		/// </summary>
		public static int ReadBufferSize { get; set; } = 4096;

		/// <summary>
		/// This offset is used in the OAuth2Setting.IsAccessTokenExpired to check the OAuth2AccessTokenExpires
		/// Now + this > OAuth2AccessTokenExpires
		/// </summary>
		public static int OAuth2ExpireOffset { get; set; } = 5;

		/// <summary>
		/// This value is used when a Task needs to run on the UI Thread, e.g. the EmbeddedBrowserCodeReceiver
		/// </summary>
		public static TaskScheduler UITaskScheduler
		{
			get
			{
				if (_uiTaskScheduler == null)
				{
					try {
						_uiTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
					}
					catch (Exception ex)
					{
						throw new InvalidOperationException("The framework needed a TaskScheduler for the UI, maybe you are not running ", ex);
					}
				}
				return _uiTaskScheduler;
			}
			set
			{
				_uiTaskScheduler = value;
			}
		}
	}
}
