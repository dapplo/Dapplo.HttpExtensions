using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Dapplo.HttpExtensions.OAuth
{
	public enum GrantTypes
	{
		[EnumMember(Value = "password")]
		Password,
		[EnumMember(Value = "refresh_token")]
		RefreshToken,
		[EnumMember(Value = "authorization_code")]
		AuthorizationCode
	}
}
