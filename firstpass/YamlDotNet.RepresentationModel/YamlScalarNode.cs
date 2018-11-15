using System;
using System.Collections.Generic;
using System.Diagnostics;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace YamlDotNet.RepresentationModel
{
	[Serializable]
	[DebuggerDisplay("{Value}")]
	public sealed class YamlScalarNode : YamlNode, IYamlConvertible
	{
		public string Value
		{
			get;
			set;
		}

		public ScalarStyle Style
		{
			get;
			set;
		}

		public override YamlNodeType NodeType => YamlNodeType.Scalar;

		internal YamlScalarNode(IParser parser, DocumentLoadingState state)
		{
			Load(parser, state);
		}

		public YamlScalarNode()
		{
		}

		public YamlScalarNode(string value)
		{
			Value = value;
		}

		private void Load(IParser parser, DocumentLoadingState state)
		{
			Scalar scalar = parser.Expect<Scalar>();
			Load(scalar, state);
			Value = scalar.Value;
			Style = scalar.Style;
		}

		internal override void ResolveAliases(DocumentLoadingState state)
		{
			throw new NotSupportedException("Resolving an alias on a scalar node does not make sense");
		}

		internal override void Emit(IEmitter emitter, EmitterState state)
		{
			emitter.Emit(new Scalar(base.Anchor, base.Tag, Value, Style, base.Tag == null, false));
		}

		public override void Accept(IYamlVisitor visitor)
		{
			visitor.Visit(this);
		}

		public override bool Equals(object obj)
		{
			YamlScalarNode yamlScalarNode = obj as YamlScalarNode;
			return yamlScalarNode != null && Equals(yamlScalarNode) && YamlNode.SafeEquals(Value, yamlScalarNode.Value);
		}

		public override int GetHashCode()
		{
			return YamlNode.CombineHashCodes(base.GetHashCode(), YamlNode.GetHashCode(Value));
		}

		public static explicit operator string(YamlScalarNode value)
		{
			return value.Value;
		}

		internal override string ToString(RecursionLevel level)
		{
			return Value;
		}

		internal override IEnumerable<YamlNode> SafeAllNodes(RecursionLevel level)
		{
			yield return (YamlNode)this;
			/*Error: Unable to find new state assignment for yield return*/;
		}

		void IYamlConvertible.Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
		{
			Load(parser, new DocumentLoadingState());
		}

		void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
		{
			Emit(emitter, new EmitterState());
		}
	}
}
