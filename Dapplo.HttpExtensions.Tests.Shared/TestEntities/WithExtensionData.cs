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

		[ExtensionData]
		public IDictionary<string, string> ExtensionData { get; set; } = new Dictionary<string, string>();
	}
}
