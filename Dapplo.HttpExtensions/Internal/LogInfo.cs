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
using System.Diagnostics;

namespace Dapplo.HttpExtensions.Internal
{
	/// <summary>
	/// This solve the problem of the need to specify the type
	/// </summary>
	internal class LogContext
	{
		private LogContext() { }

		/// <summary>
		/// This creates a log context which contains the type of the calling class
		/// This is slow, only use this in a static variable!
		/// </summary>
		/// <returns>LogContext</returns>
		public static LogContext Create()
		{
			// Get the stacktrace, first frame, method and it's declaring type.
			var souceType = new StackTrace().GetFrame(1).GetMethod().DeclaringType;
			return new LogContext
			{
				Source = souceType
			};
		}
		public Type Source { get; private set; }
	}

	/// <summary>
	/// A simple wrapper for some information which is passed to the logger
	/// </summary>
	internal class LogInfo : ILogInfo
	{
		public Type Caller { get; set; }

		public string Method { get; set; }

		public int Line { get; set; }

		public ILogger Logger { get; set; }

		public LogLevel Level { get; set; }

		public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;

		public override string ToString()
		{
			return $"{Timestamp.ToString("yyyy-MM-dd HH:mm:sss")} {Level} {Caller.FullName}:{Method}({Line})";
        }
	}
}
