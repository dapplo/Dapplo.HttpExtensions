using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Dapplo.HttpExtensions.Tests.Shared.TestEntities
{
	/// <summary>
	///     Container for a test with Extension data
	/// </summary>
	[DataContract]
	class WithExtensionData
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }

		[ExtensionData(Pattern = "customstringfield_.*")]
		public IDictionary<string, string> StringExtensionData { get; set; } = new Dictionary<string, string>();

		[ExtensionData(Pattern = "customintfield_.*")]
		public IDictionary<string, int> IntExtensionData { get; set; } = new Dictionary<string, int>();

		[ExtensionData]
		public IDictionary<string, object> RestExtensionData { get; set; } = new Dictionary<string, object>();

	}
}
