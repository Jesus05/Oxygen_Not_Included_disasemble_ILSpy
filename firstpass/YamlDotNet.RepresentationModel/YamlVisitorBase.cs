using System.Collections.Generic;

namespace YamlDotNet.RepresentationModel
{
	public abstract class YamlVisitorBase : IYamlVisitor
	{
		public virtual void Visit(YamlStream stream)
		{
			VisitChildren(stream);
		}

		public virtual void Visit(YamlDocument document)
		{
			VisitChildren(document);
		}

		public virtual void Visit(YamlScalarNode scalar)
		{
		}

		public virtual void Visit(YamlSequenceNode sequence)
		{
			VisitChildren(sequence);
		}

		public virtual void Visit(YamlMappingNode mapping)
		{
			VisitChildren(mapping);
		}

		protected virtual void VisitPair(YamlNode key, YamlNode value)
		{
			key.Accept(this);
			value.Accept(this);
		}

		protected virtual void VisitChildren(YamlStream stream)
		{
			foreach (YamlDocument document in stream.Documents)
			{
				document.Accept(this);
			}
		}

		protected virtual void VisitChildren(YamlDocument document)
		{
			if (document.RootNode != null)
			{
				document.RootNode.Accept(this);
			}
		}

		protected virtual void VisitChildren(YamlSequenceNode sequence)
		{
			foreach (YamlNode child in sequence.Children)
			{
				child.Accept(this);
			}
		}

		protected virtual void VisitChildren(YamlMappingNode mapping)
		{
			foreach (KeyValuePair<YamlNode, YamlNode> child in mapping.Children)
			{
				VisitPair(child.Key, child.Value);
			}
		}
	}
}
