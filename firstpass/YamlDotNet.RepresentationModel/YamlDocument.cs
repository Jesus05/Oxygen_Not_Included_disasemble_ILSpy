using System;
using System.Collections.Generic;
using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.RepresentationModel
{
	[Serializable]
	public class YamlDocument
	{
		private class AnchorAssigningVisitor : YamlVisitorBase
		{
			private readonly HashSet<string> existingAnchors = new HashSet<string>();

			private readonly Dictionary<YamlNode, bool> visitedNodes = new Dictionary<YamlNode, bool>();

			public void AssignAnchors(YamlDocument document)
			{
				existingAnchors.Clear();
				visitedNodes.Clear();
				document.Accept(this);
				Random random = new Random();
				foreach (KeyValuePair<YamlNode, bool> visitedNode in visitedNodes)
				{
					if (visitedNode.Value)
					{
						string text;
						if (string.IsNullOrEmpty(visitedNode.Key.Anchor) || existingAnchors.Contains(visitedNode.Key.Anchor))
						{
							do
							{
								text = random.Next().ToString(CultureInfo.InvariantCulture);
							}
							while (existingAnchors.Contains(text));
						}
						else
						{
							text = visitedNode.Key.Anchor;
						}
						existingAnchors.Add(text);
						visitedNode.Key.Anchor = text;
					}
				}
			}

			private bool VisitNodeAndFindDuplicates(YamlNode node)
			{
				if (!visitedNodes.TryGetValue(node, out bool value))
				{
					visitedNodes.Add(node, false);
					return false;
				}
				if (!value)
				{
					visitedNodes[node] = true;
				}
				return !value;
			}

			public override void Visit(YamlScalarNode scalar)
			{
				VisitNodeAndFindDuplicates(scalar);
			}

			public override void Visit(YamlMappingNode mapping)
			{
				if (!VisitNodeAndFindDuplicates(mapping))
				{
					base.Visit(mapping);
				}
			}

			public override void Visit(YamlSequenceNode sequence)
			{
				if (!VisitNodeAndFindDuplicates(sequence))
				{
					base.Visit(sequence);
				}
			}
		}

		public YamlNode RootNode
		{
			get;
			private set;
		}

		public IEnumerable<YamlNode> AllNodes => RootNode.AllNodes;

		public YamlDocument(YamlNode rootNode)
		{
			RootNode = rootNode;
		}

		public YamlDocument(string rootNode)
		{
			RootNode = new YamlScalarNode(rootNode);
		}

		internal YamlDocument(IParser parser)
		{
			DocumentLoadingState documentLoadingState = new DocumentLoadingState();
			parser.Expect<DocumentStart>();
			while (!parser.Accept<DocumentEnd>())
			{
				Debug.Assert(RootNode == null);
				RootNode = YamlNode.ParseNode(parser, documentLoadingState);
				if (RootNode is YamlAliasNode)
				{
					throw new YamlException();
				}
			}
			documentLoadingState.ResolveAliases();
			parser.Expect<DocumentEnd>();
		}

		private void AssignAnchors()
		{
			AnchorAssigningVisitor anchorAssigningVisitor = new AnchorAssigningVisitor();
			anchorAssigningVisitor.AssignAnchors(this);
		}

		internal void Save(IEmitter emitter, bool assignAnchors = true)
		{
			if (assignAnchors)
			{
				AssignAnchors();
			}
			emitter.Emit(new DocumentStart());
			RootNode.Save(emitter, new EmitterState());
			emitter.Emit(new DocumentEnd(false));
		}

		public void Accept(IYamlVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}
