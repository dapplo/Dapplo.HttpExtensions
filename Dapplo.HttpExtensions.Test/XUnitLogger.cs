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

using Dapplo.LogFacade;
using System;
using System.Runtime.Remoting.Messaging;
using Xunit.Abstractions;

namespace Dapplo.HttpExtensions.Test
{
	/// <summary>
	/// xUnit will have tests run parallel, and due to this it won't capture trace output correctly.
	/// This is where their ITestOutputHelper comes around, but Dapplo.LogFacade can only have one logger.
	/// This class solves the problem by registering the ITestOutputHelper in the CallContext.
	/// Every log statement will retrieve the ITestOutputHelper from the context and use it to log.
	/// </summary>
	public class XUnitLogger : ILogger
	{
		/// <summary>
		/// Register the XUnitLogger,  as the global LogFacade logger
		/// This also places the ITestOutputHelper in the CallContext, so the output is mapped to the xUnit test
		/// </summary>
		/// <param name="testOutputHelper">ITestOutputHelper</param>
		/// <param name="level">LogLevel, when none is given the LogSettings.DefaultLevel is used</param>
		public static void RegisterLogger(ITestOutputHelper testOutputHelper, LogLevel level = default(LogLevel))
		{
			CallContext.LogicalSetData(typeof(ITestOutputHelper).Name, testOutputHelper);
			if (level != LogLevel.None)
			{
				CallContext.LogicalSetData(typeof(LogLevel).Name, level);
			}
			if (!(LogSettings.Logger is XUnitLogger))
			{
				LogSettings.Logger = new XUnitLogger();
			}
		}

		/// <summary>
		/// Prevent the constructor from being use elsewhere
		/// </summary>
		private XUnitLogger()
		{

		}

		/// <summary>
		/// LogLevel, this can give a different result pro xUnit test...
		/// It will depend on the RegisterLogger value which was used in the current xUnit test
		/// </summary>
		public LogLevel Level
		{
			get
			{
				var level = CallContext.LogicalGetData(typeof(LogLevel).Name);
				if (level != null)
				{
					var logLevel = (LogLevel)level;
					if (logLevel != LogLevel.None)
					{
						return logLevel;
					}
				}
				return LogSettings.DefaultLevel;
			}
			set
			{
				CallContext.LogicalSetData(typeof(LogLevel).Name, value);
			}
		}

		/// <summary>
		/// If the level is enabled, this returns true
		/// The level depends on what the xUnit test used in the RegisterLogger
		/// </summary>
		/// <param name="level">LogLevel</param>
		/// <returns>true if the level is enabled</returns>
		public bool IsLogLevelEnabled(LogLevel level)
		{
			return level != LogLevel.None && level >= Level;
		}

		public void Write(LogInfo logInfo, string messageTemplate, params object[] logParameters)
		{
			var testOutputHelper = CallContext.LogicalGetData(typeof(ITestOutputHelper).Name) as ITestOutputHelper;
			if (testOutputHelper == null)
			{
				throw new ArgumentNullException("Couldn't find a ITestOutputHelper in the CallContext", nameof(testOutputHelper));
			}
			testOutputHelper.WriteLine($"{logInfo} - {messageTemplate}", logParameters);
		}

		public void Write(LogInfo logInfo, Exception exception, string messageTemplate, params object[] logParameters)
		{
			var testOutputHelper = CallContext.LogicalGetData(typeof(ITestOutputHelper).Name) as ITestOutputHelper;
			if (testOutputHelper == null)
			{
				throw new ArgumentNullException("Couldn't find a ITestOutputHelper in the CallContext", nameof(testOutputHelper));
			}
			testOutputHelper.WriteLine($"{logInfo} - {messageTemplate}", logParameters);
			testOutputHelper.WriteLine(exception.ToString());
		}
	}
}
