using System;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization.NodeTypeResolvers
{
	[Obsolete("The mechanism that this class uses to specify type names is non-standard. Register the tags explicitly instead of using this convention.")]
	public sealed class TypeNameInTagNodeTypeResolver : INodeTypeResolver
	{
		bool INodeTypeResolver.Resolve(NodeEvent nodeEvent, ref Type currentType)
		{
			if (!string.IsNullOrEmpty(nodeEvent.Tag))
			{
				currentType = Type.GetType(nodeEvent.Tag.Substring(1), false);
				return currentType != null;
			}
			return false;
		}
	}
}
