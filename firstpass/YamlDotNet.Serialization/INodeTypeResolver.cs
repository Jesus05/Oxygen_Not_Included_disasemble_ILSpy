using System;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization
{
	public interface INodeTypeResolver
	{
		bool Resolve(NodeEvent nodeEvent, ref Type currentType);
	}
}
