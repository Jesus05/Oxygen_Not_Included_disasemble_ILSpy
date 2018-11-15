using System;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization.NodeTypeResolvers
{
	public sealed class YamlConvertibleTypeResolver : INodeTypeResolver
	{
		public bool Resolve(NodeEvent nodeEvent, ref Type currentType)
		{
			return typeof(IYamlConvertible).IsAssignableFrom(currentType);
		}
	}
}
