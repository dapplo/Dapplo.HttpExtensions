using System.Runtime.Serialization;

namespace Dapplo.HttpExtensions.Tests.TestEntities
{
    /// <summary>
    /// Possible types of releases
    /// </summary>
    public enum ReleaseTypes
    {
        /// <summary>
        /// The release is public
        /// </summary>
        [EnumMember(Value = "public")] Public,
        /// <summary>
        /// The release is private
        /// </summary>
        [EnumMember(Value = "private")] Private
    }
}
