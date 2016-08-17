//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2015-2016 Dapplo
// 
//  For more information see: http://dapplo.net/
//  Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
//  This file is part of Dapplo.HttpExtensions
// 
//  Dapplo.HttpExtensions is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  Dapplo.HttpExtensions is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have a copy of the GNU Lesser General Public License
//  along with Dapplo.HttpExtensions. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

#region using

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapplo.HttpExtensions.Extensions;

#endregion

namespace Dapplo.HttpExtensions.OAuth
{
	/// <summary>
	///     Simply pass the Request token as the authentication
	/// </summary>
	internal class PassThroughCodeReceiver : IOAuthCodeReceiver
	{
		/// <summary>
		///     The OAuth code receiver implementation
		/// </summary>
		/// <param name="authorizeMode">which of the AuthorizeModes was used to call the method</param>
		/// <param name="codeReceiverSettings"></param>
		/// <param name="cancellationToken">CancellationToken</param>
		/// <returns>Dictionary with values</returns>
		public Task<IDictionary<string, string>> ReceiveCodeAsync(AuthorizeModes authorizeMode, ICodeReceiverSettings codeReceiverSettings,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			var result = new Dictionary<string, string>();
			var oauth1Settings = codeReceiverSettings as OAuth1Settings;

			if (oauth1Settings != null)
			{
				result.Add(OAuth1Parameters.Token.EnumValueOf(), oauth1Settings.RequestToken);
			}
			return Task.FromResult<IDictionary<string, string>>(result);
		}
	}
}