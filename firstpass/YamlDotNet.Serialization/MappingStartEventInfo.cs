using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization
{
	public sealed class MappingStartEventInfo : ObjectEventInfo
	{
		public bool IsImplicit
		{
			get;
			set;
		}

		public MappingStyle Style
		{
			get;
			set;
		}

		public MappingStartEventInfo(IObjectDescriptor source)
			: base(source)
		{
		}
	}
}
