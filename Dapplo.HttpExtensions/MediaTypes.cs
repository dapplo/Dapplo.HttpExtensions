using System.Runtime.Serialization;

namespace Dapplo.HttpExtensions
{
	/// <summary>
	/// Use this enum for the creating the accept header or checking the content-type
	/// </summary>
	public enum MediaTypes
	{
		[EnumMember(Value = "application/json")]
		Json,
		[EnumMember(Value = "application/xml")]
		Xml,
		[EnumMember(Value = "text/xml")]
		XmlReadable,
		[EnumMember(Value = "text/html")]
		Html,
		[EnumMember(Value = "text/plain")]
		Txt,
		[EnumMember(Value = "application/x-www-form-urlencoded")]
		WwwFormUrlEncoded,
		[EnumMember(Value = "image/gif")]
		Gif,
		[EnumMember(Value = "image/jpeg")]
		Jpeg,
		[EnumMember(Value = "image/png")]
		Png,
		[EnumMember(Value = "image/bmp")]
		Bmp,
		[EnumMember(Value = "image/tiff")]
		Tiff,
		[EnumMember(Value = "image/svg+xml")]
		Svg
	}
}
