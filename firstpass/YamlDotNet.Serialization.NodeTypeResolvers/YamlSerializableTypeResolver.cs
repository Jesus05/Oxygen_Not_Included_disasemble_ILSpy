using System;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization.NodeTypeResolvers
{
	public sealed class YamlSerializableTypeResolver : INodeTypeResolver
	{
		public bool Resolve(NodeEvent nodeEvent, ref Type currentType)
		{
			return typeof(IYamlSerializable).IsAssignableFrom(currentType);
		}
	}
}
