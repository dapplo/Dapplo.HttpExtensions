﻿/*
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
using Dapplo.LogFacade.Loggers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dapplo.HttpExtensions.Test
{
	/// <summary>
	/// This initializes the logger for all tests
	/// </summary>
	[TestClass]
	public class ConfigureLogging4Tests
	{
		[AssemblyInitialize]
		public static void ConfigureLogging(TestContext context)
		{
			LogSettings.Logger = new TraceLogger { Level = LogLevel.Verbose };
		}
	}
}
