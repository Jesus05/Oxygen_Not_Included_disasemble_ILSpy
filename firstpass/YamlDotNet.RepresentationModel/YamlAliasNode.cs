using System;
using System.Collections.Generic;
using YamlDotNet.Core;

namespace YamlDotNet.RepresentationModel
{
	[Serializable]
	internal class YamlAliasNode : YamlNode
	{
		public override YamlNodeType NodeType => YamlNodeType.Alias;

		internal YamlAliasNode(string anchor)
		{
			base.Anchor = anchor;
		}

		internal override void ResolveAliases(DocumentLoadingState state)
		{
			throw new NotSupportedException("Resolving an alias on an alias node does not make sense");
		}

		internal override void Emit(IEmitter emitter, EmitterState state)
		{
			throw new NotSupportedException("A YamlAliasNode is an implementation detail and should never be saved.");
		}

		public override void Accept(IYamlVisitor visitor)
		{
			throw new NotSupportedException("A YamlAliasNode is an implementation detail and should never be visited.");
		}

		public override bool Equals(object obj)
		{
			YamlAliasNode yamlAliasNode = obj as YamlAliasNode;
			return yamlAliasNode != null && Equals(yamlAliasNode) && YamlNode.SafeEquals(base.Anchor, yamlAliasNode.Anchor);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		internal override string ToString(RecursionLevel level)
		{
			return "*" + base.Anchor;
		}

		internal override IEnumerable<YamlNode> SafeAllNodes(RecursionLevel level)
		{
			yield return (YamlNode)this;
			/*Error: Unable to find new state assignment for yield return*/;
		}
	}
}
