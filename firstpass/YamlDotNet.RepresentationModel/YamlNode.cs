using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.RepresentationModel
{
	[Serializable]
	public abstract class YamlNode
	{
		private const int MaximumRecursionLevel = 1000;

		internal const string MaximumRecursionLevelReachedToStringValue = "WARNING! INFINITE RECURSION!";

		public string Anchor
		{
			get;
			set;
		}

		public string Tag
		{
			get;
			set;
		}

		public Mark Start
		{
			get;
			private set;
		}

		public Mark End
		{
			get;
			private set;
		}

		public IEnumerable<YamlNode> AllNodes
		{
			get
			{
				RecursionLevel level = new RecursionLevel(1000);
				return SafeAllNodes(level);
			}
		}

		public abstract YamlNodeType NodeType
		{
			get;
		}

		public YamlNode this[int index]
		{
			get
			{
				return ((YamlSequenceNode)this).Children[index];
			}
		}

		public YamlNode this[YamlNode key]
		{
			get
			{
				return ((YamlMappingNode)this).Children[key];
			}
		}

		internal void Load(NodeEvent yamlEvent, DocumentLoadingState state)
		{
			Tag = yamlEvent.Tag;
			if (yamlEvent.Anchor != null)
			{
				Anchor = yamlEvent.Anchor;
				state.AddAnchor(this);
			}
			Start = yamlEvent.Start;
			End = yamlEvent.End;
		}

		internal static YamlNode ParseNode(IParser parser, DocumentLoadingState state)
		{
			if (parser.Accept<Scalar>())
			{
				return new YamlScalarNode(parser, state);
			}
			if (parser.Accept<SequenceStart>())
			{
				return new YamlSequenceNode(parser, state);
			}
			if (parser.Accept<MappingStart>())
			{
				return new YamlMappingNode(parser, state);
			}
			if (parser.Accept<AnchorAlias>())
			{
				AnchorAlias anchorAlias = parser.Expect<AnchorAlias>();
				return state.GetNode(anchorAlias.Value, false, anchorAlias.Start, anchorAlias.End) ?? new YamlAliasNode(anchorAlias.Value);
			}
			throw new ArgumentException("The current event is of an unsupported type.", "events");
		}

		internal abstract void ResolveAliases(DocumentLoadingState state);

		internal void Save(IEmitter emitter, EmitterState state)
		{
			if (!string.IsNullOrEmpty(Anchor) && !state.EmittedAnchors.Add(Anchor))
			{
				emitter.Emit(new AnchorAlias(Anchor));
			}
			else
			{
				Emit(emitter, state);
			}
		}

		internal abstract void Emit(IEmitter emitter, EmitterState state);

		public abstract void Accept(IYamlVisitor visitor);

		protected bool Equals(YamlNode other)
		{
			return SafeEquals(Tag, other.Tag);
		}

		protected static bool SafeEquals(object first, object second)
		{
			return first?.Equals(second) ?? second?.Equals(first) ?? true;
		}

		public override int GetHashCode()
		{
			return GetHashCode(Tag);
		}

		protected static int GetHashCode(object value)
		{
			return value?.GetHashCode() ?? 0;
		}

		protected static int CombineHashCodes(int h1, int h2)
		{
			return ((h1 << 5) + h1) ^ h2;
		}

		public override string ToString()
		{
			RecursionLevel level = new RecursionLevel(1000);
			return ToString(level);
		}

		internal abstract string ToString(RecursionLevel level);

		internal abstract IEnumerable<YamlNode> SafeAllNodes(RecursionLevel level);

		public static implicit operator YamlNode(string value)
		{
			return new YamlScalarNode(value);
		}

		public static implicit operator YamlNode(string[] sequence)
		{
			return new YamlSequenceNode(from i in (IEnumerable<string>)sequence
			select (i));
		}

		public static explicit operator string(YamlNode scalar)
		{
			return ((YamlScalarNode)scalar).Value;
		}
	}
}
