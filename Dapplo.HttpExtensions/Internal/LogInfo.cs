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

namespace Dapplo.HttpExtensions.Internal
{
	internal class LogContext
	{
		private LogContext()
		{

		}
		public static LogContext Create<TSource>()
		{
			return new LogContext
			{
				Source = typeof(TSource)
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

		public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;

		public override string ToString()
		{
			return $"{Timestamp.ToString("yyyy-MM-dd HH:mm:sss")} {Caller.FullName}:{Method}({Line})";
        }
	}
}
