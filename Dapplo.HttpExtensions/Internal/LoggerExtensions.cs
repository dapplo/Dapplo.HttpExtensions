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

using Dapplo.HttpExtensions.Support;
using System;
using System.Runtime.CompilerServices;

namespace Dapplo.HttpExtensions.Internal
{
	/// <summary>
	/// The extensions for making logging easy and flexible
	/// </summary>
	internal static class LoggerExtensions
	{
		/// <summary>
		/// Wrapper for the Debug call
		/// </summary>
		/// <param name="logInfo">ILogInfo</param>
		/// <param name="messageTemplate">string with formatting</param>
		/// <param name="propertyValues">parameters for the formatting</param>
		public static void Debug(this ILogInfo logInfo, string messageTemplate, params object[] propertyValues)
		{
			if (logInfo == null)
			{
				return;
			}
			HttpExtensionsGlobals.Logger?.Debug(logInfo, messageTemplate, propertyValues);
        }
		/// <summary>
		/// Wrapper for the Debug call
		/// </summary>
		/// <param name="logInfo">ILogInfo</param>
		/// <param name="exception">Exception to log</param>
		/// <param name="messageTemplate">string with formatting</param>
		/// <param name="propertyValues">parameters for the formatting</param>
		public static void Debug(this ILogInfo logInfo, Exception exception, string messageTemplate, params object[] propertyValues)
		{
			if (logInfo == null)
			{
				return;
			}
			HttpExtensionsGlobals.Logger?.Debug(logInfo, exception, messageTemplate, propertyValues);
		}

		/// <summary>
		/// Wrapper for the Info call
		/// </summary>
		/// <param name="logInfo">ILogInfo</param>
		/// <param name="messageTemplate">string with formatting</param>
		/// <param name="propertyValues">parameters for the formatting</param>
		public static void Info(this ILogInfo logInfo, string messageTemplate, params object[] propertyValues)
		{
			if (logInfo == null)
			{
				return;
			}
			HttpExtensionsGlobals.Logger?.Info(logInfo, messageTemplate, propertyValues);
		}
		/// <summary>
		/// Wrapper for the Info call
		/// </summary>
		/// <param name="logInfo">ILogInfo</param>
		/// <param name="exception">Exception to log</param>
		/// <param name="messageTemplate">string with formatting</param>
		/// <param name="propertyValues">parameters for the formatting</param>
		public static void Info(this ILogInfo logInfo, Exception exception, string messageTemplate, params object[] propertyValues)
		{
			if (logInfo == null)
			{
				return;
			}
			HttpExtensionsGlobals.Logger?.Info(logInfo, exception, messageTemplate, propertyValues);
		}

		/// <summary>
		/// Wrapper for the Warn call
		/// </summary>
		/// <param name="logInfo">ILogInfo</param>
		/// <param name="messageTemplate">string with formatting</param>
		/// <param name="propertyValues">parameters for the formatting</param>
		public static void Warn(this ILogInfo logInfo, string messageTemplate, params object[] propertyValues)
		{
			if (logInfo == null)
			{
				return;
			}
			HttpExtensionsGlobals.Logger?.Warn(logInfo, messageTemplate, propertyValues);
		}
		/// <summary>
		/// Wrapper for the Warn call
		/// </summary>
		/// <param name="logInfo">ILogInfo</param>
		/// <param name="exception">Exception to log</param>
		/// <param name="messageTemplate">string with formatting</param>
		/// <param name="propertyValues">parameters for the formatting</param>
		public static void Warn(this ILogInfo logInfo, Exception exception, string messageTemplate, params object[] propertyValues)
		{
			if (logInfo == null)
			{
				return;
			}
			HttpExtensionsGlobals.Logger?.Warn(logInfo, exception, messageTemplate, propertyValues);
		}
		/// <summary>
		/// Wrapper for the Error call
		/// </summary>
		/// <param name="logInfo">ILogInfo</param>
		/// <param name="messageTemplate">string with formatting</param>
		/// <param name="propertyValues">parameters for the formatting</param>
		public static void Error(this ILogInfo logInfo, string messageTemplate, params object[] propertyValues)
		{
			if (logInfo == null)
			{
				return;
			}
			HttpExtensionsGlobals.Logger?.Error(logInfo, messageTemplate, propertyValues);
		}

		/// <summary>
		/// Wrapper for the Error call
		/// </summary>
		/// <param name="logInfo">ILogInfo</param>
		/// <param name="exception">Exception to log</param>
		/// <param name="messageTemplate">string with formatting</param>
		/// <param name="propertyValues">parameters for the formatting</param>
		public static void Error(this ILogInfo logInfo, Exception exception, string messageTemplate, params object[] propertyValues)
		{
			if (logInfo == null)
			{
				return;
			}
			HttpExtensionsGlobals.Logger?.Error(logInfo, exception, messageTemplate, propertyValues);
		}

		/// <summary>
		/// This extension will create ILogInfo, if a logger is passed
		/// </summary>
		/// <param name="logContext">LogContext is the context (source) from where the log entry came</param>
		/// <param name="memberName">Should be set by the compiler, is the calling method</param>
		/// <param name="lineNumber">int lineNumber of the log statement</param>
		/// <returns>ILogInfo</returns>
		public static ILogInfo Prepare(this LogContext logContext, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
		{
			if (logContext == null)
			{
				return null;
			}

			return new LogInfo
			{
				Caller = logContext.Source,
				Method = memberName,
				Line = lineNumber
			};
		}
	}
}
