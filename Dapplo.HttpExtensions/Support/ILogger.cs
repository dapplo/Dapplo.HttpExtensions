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

namespace Dapplo.HttpExtensions.Support
{
	public enum LogLevel
	{
		Debug,
		Info,
		Warn,
		Error,
		Fatal
	}

	/// <summary>
	/// Used to pass 
	/// </summary>
	public interface ILogInfo
	{
		/// <summary>
		/// Class from where the log statement came
		/// </summary>
		Type Caller { get; set; }

		/// <summary>
		/// Method in the Caller (class) from where the log statement came
		/// </summary>
		string Method { get; set; }

		/// <summary>
		/// The LogLevel for this log info
		/// </summary>
		LogLevel Level { get; set; }

		/// <summary>
		/// Timestamp of the log
		/// </summary>
		DateTimeOffset Timestamp { get; set; }
	}

	/// <summary>
	/// This is the interface used for internal logging.
	/// The idea is that you can implement a small wrapper for you favorite logger which implements this interface.
	/// Assign it to the HttpExtensionsGlobals.Logger and Dapplo.HttpExtensions will start logger with your class.
	/// A TraceLogger implementation is supplied, so you can see some output while your project is in development.
	/// </summary>
	public interface ILogger
	{
		void Write(ILogInfo logInfo, string messageTemplate, params object[] propertyValues);
		void Write(ILogInfo logInfo, Exception exception, string messageTemplate, params object[] propertyValues);
	}
}
