using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace YamlDotNet.RepresentationModel
{
	[Serializable]
	public sealed class YamlMappingNode : YamlNode, IEnumerable<KeyValuePair<YamlNode, YamlNode>>, IYamlConvertible, IEnumerable
	{
		private readonly IDictionary<YamlNode, YamlNode> children = new Dictionary<YamlNode, YamlNode>();

		public IDictionary<YamlNode, YamlNode> Children => children;

		public MappingStyle Style
		{
			get;
			set;
		}

		public override YamlNodeType NodeType => YamlNodeType.Mapping;

		internal YamlMappingNode(IParser parser, DocumentLoadingState state)
		{
			Load(parser, state);
		}

		public YamlMappingNode()
		{
		}

		public YamlMappingNode(params KeyValuePair<YamlNode, YamlNode>[] children)
			: this((IEnumerable<KeyValuePair<YamlNode, YamlNode>>)children)
		{
		}

		public YamlMappingNode(IEnumerable<KeyValuePair<YamlNode, YamlNode>> children)
		{
			foreach (KeyValuePair<YamlNode, YamlNode> child in children)
			{
				this.children.Add(child);
			}
		}

		public YamlMappingNode(params YamlNode[] children)
			: this((IEnumerable<YamlNode>)children)
		{
		}

		public YamlMappingNode(IEnumerable<YamlNode> children)
		{
			using (IEnumerator<YamlNode> enumerator = children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					YamlNode current = enumerator.Current;
					if (!enumerator.MoveNext())
					{
						throw new ArgumentException("When constructing a mapping node with a sequence, the number of elements of the sequence must be even.");
					}
					Add(current, enumerator.Current);
				}
			}
		}

		private void Load(IParser parser, DocumentLoadingState state)
		{
			MappingStart mappingStart = parser.Expect<MappingStart>();
			Load(mappingStart, state);
			Style = mappingStart.Style;
			bool flag = false;
			while (!parser.Accept<MappingEnd>())
			{
				YamlNode yamlNode = YamlNode.ParseNode(parser, state);
				YamlNode yamlNode2 = YamlNode.ParseNode(parser, state);
				try
				{
					children.Add(yamlNode, yamlNode2);
				}
				catch (ArgumentException innerException)
				{
					throw new YamlException(yamlNode.Start, yamlNode.End, "Duplicate key", innerException);
				}
				flag |= (yamlNode is YamlAliasNode || yamlNode2 is YamlAliasNode);
			}
			if (flag)
			{
				state.AddNodeWithUnresolvedAliases(this);
			}
			parser.Expect<MappingEnd>();
		}

		public void Add(YamlNode key, YamlNode value)
		{
			children.Add(key, value);
		}

		public void Add(string key, YamlNode value)
		{
			children.Add(new YamlScalarNode(key), value);
		}

		public void Add(YamlNode key, string value)
		{
			children.Add(key, new YamlScalarNode(value));
		}

		public void Add(string key, string value)
		{
			children.Add(new YamlScalarNode(key), new YamlScalarNode(value));
		}

		internal override void ResolveAliases(DocumentLoadingState state)
		{
			Dictionary<YamlNode, YamlNode> dictionary = null;
			Dictionary<YamlNode, YamlNode> dictionary2 = null;
			foreach (KeyValuePair<YamlNode, YamlNode> child in children)
			{
				if (child.Key is YamlAliasNode)
				{
					if (dictionary == null)
					{
						dictionary = new Dictionary<YamlNode, YamlNode>();
					}
					dictionary.Add(child.Key, state.GetNode(child.Key.Anchor, true, child.Key.Start, child.Key.End));
				}
				if (child.Value is YamlAliasNode)
				{
					if (dictionary2 == null)
					{
						dictionary2 = new Dictionary<YamlNode, YamlNode>();
					}
					dictionary2.Add(child.Key, state.GetNode(child.Value.Anchor, true, child.Value.Start, child.Value.End));
				}
			}
			if (dictionary2 != null)
			{
				foreach (KeyValuePair<YamlNode, YamlNode> item in dictionary2)
				{
					children[item.Key] = item.Value;
				}
			}
			if (dictionary != null)
			{
				foreach (KeyValuePair<YamlNode, YamlNode> item2 in dictionary)
				{
					YamlNode value = children[item2.Key];
					children.Remove(item2.Key);
					children.Add(item2.Value, value);
				}
			}
		}

		internal override void Emit(IEmitter emitter, EmitterState state)
		{
			emitter.Emit(new MappingStart(base.Anchor, base.Tag, true, Style));
			foreach (KeyValuePair<YamlNode, YamlNode> child in children)
			{
				child.Key.Save(emitter, state);
				child.Value.Save(emitter, state);
			}
			emitter.Emit(new MappingEnd());
		}

		public override void Accept(IYamlVisitor visitor)
		{
			visitor.Visit(this);
		}

		public override bool Equals(object obj)
		{
			YamlMappingNode yamlMappingNode = obj as YamlMappingNode;
			if (yamlMappingNode == null || !Equals(yamlMappingNode) || children.Count != yamlMappingNode.children.Count)
			{
				return false;
			}
			foreach (KeyValuePair<YamlNode, YamlNode> child in children)
			{
				if (!yamlMappingNode.children.TryGetValue(child.Key, out YamlNode value) || !YamlNode.SafeEquals(child.Value, value))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = base.GetHashCode();
			foreach (KeyValuePair<YamlNode, YamlNode> child in children)
			{
				num = YamlNode.CombineHashCodes(num, YamlNode.GetHashCode(child.Key));
				num = YamlNode.CombineHashCodes(num, YamlNode.GetHashCode(child.Value));
			}
			return num;
		}

		internal override IEnumerable<YamlNode> SafeAllNodes(RecursionLevel level)
		{
			level.Increment();
			yield return (YamlNode)this;
			/*Error: Unable to find new state assignment for yield return*/;
		}

		internal override string ToString(RecursionLevel level)
		{
			if (!level.TryIncrement())
			{
				return "WARNING! INFINITE RECURSION!";
			}
			StringBuilder stringBuilder = new StringBuilder("{ ");
			foreach (KeyValuePair<YamlNode, YamlNode> child in children)
			{
				if (stringBuilder.Length > 2)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append("{ ").Append(child.Key.ToString(level)).Append(", ")
					.Append(child.Value.ToString(level))
					.Append(" }");
			}
			stringBuilder.Append(" }");
			level.Decrement();
			return stringBuilder.ToString();
		}

		public IEnumerator<KeyValuePair<YamlNode, YamlNode>> GetEnumerator()
		{
			return children.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		void IYamlConvertible.Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer)
		{
			Load(parser, new DocumentLoadingState());
		}

		void IYamlConvertible.Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer)
		{
			Emit(emitter, new EmitterState());
		}

		public static YamlMappingNode FromObject(object mapping)
		{
			if (mapping == null)
			{
				throw new ArgumentNullException("mapping");
			}
			YamlMappingNode yamlMappingNode = new YamlMappingNode();
			foreach (PropertyInfo publicProperty in mapping.GetType().GetPublicProperties())
			{
				if (publicProperty.CanRead && publicProperty.GetGetMethod().GetParameters().Length == 0)
				{
					object value = publicProperty.GetValue(mapping, null);
					YamlNode value2 = (value as YamlNode) ?? ((YamlNode)Convert.ToString(value));
					yamlMappingNode.Add(publicProperty.Name, value2);
				}
			}
			return yamlMappingNode;
		}
	}
}
