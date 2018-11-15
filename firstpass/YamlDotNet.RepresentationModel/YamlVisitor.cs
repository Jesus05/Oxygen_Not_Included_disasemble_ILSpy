using System;
using System.Collections.Generic;

namespace YamlDotNet.RepresentationModel
{
	[Obsolete("Use YamlVisitorBase")]
	public abstract class YamlVisitor : IYamlVisitor
	{
		protected virtual void Visit(YamlStream stream)
		{
		}

		protected virtual void Visited(YamlStream stream)
		{
		}

		protected virtual void Visit(YamlDocument document)
		{
		}

		protected virtual void Visited(YamlDocument document)
		{
		}

		protected virtual void Visit(YamlScalarNode scalar)
		{
		}

		protected virtual void Visited(YamlScalarNode scalar)
		{
		}

		protected virtual void Visit(YamlSequenceNode sequence)
		{
		}

		protected virtual void Visited(YamlSequenceNode sequence)
		{
		}

		protected virtual void Visit(YamlMappingNode mapping)
		{
		}

		protected virtual void Visited(YamlMappingNode mapping)
		{
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
				child.Key.Accept(this);
				child.Value.Accept(this);
			}
		}

		void IYamlVisitor.Visit(YamlStream stream)
		{
			Visit(stream);
			VisitChildren(stream);
			Visited(stream);
		}

		void IYamlVisitor.Visit(YamlDocument document)
		{
			Visit(document);
			VisitChildren(document);
			Visited(document);
		}

		void IYamlVisitor.Visit(YamlScalarNode scalar)
		{
			Visit(scalar);
			Visited(scalar);
		}

		void IYamlVisitor.Visit(YamlSequenceNode sequence)
		{
			Visit(sequence);
			VisitChildren(sequence);
			Visited(sequence);
		}

		void IYamlVisitor.Visit(YamlMappingNode mapping)
		{
			Visit(mapping);
			VisitChildren(mapping);
			Visited(mapping);
		}
	}
}
