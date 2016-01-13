using System.Runtime.Serialization;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// Use this enum for the creating the accept header or checking the content-type
	/// </summary>
	public enum MediaTypes
	{
		[EnumMember(Value = "application/json")]
		Json
	}
}
