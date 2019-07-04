using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization.NodeDeserializers
{
	public sealed class NullNodeDeserializer : INodeDeserializer
	{
		bool INodeDeserializer.Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
		{
			value = null;
			NodeEvent nodeEvent = parser.Peek<NodeEvent>();
			bool flag = nodeEvent != null && NodeIsNull(nodeEvent);
			if (flag)
			{
				parser.SkipThisAndNestedEvents();
			}
			return flag;
		}

		private bool NodeIsNull(NodeEvent nodeEvent)
		{
			if (!(nodeEvent.Tag == "tag:yaml.org,2002:null"))
			{
				Scalar scalar = nodeEvent as Scalar;
				if (scalar != null && scalar.Style == ScalarStyle.Plain)
				{
					string value = scalar.Value;
					int result;
					switch (value)
					{
					default:
						result = ((value == "NULL") ? 1 : 0);
						break;
					case "":
					case "~":
					case "null":
					case "Null":
						result = 1;
						break;
					}
					return (byte)result != 0;
				}
				return false;
			}
			return true;
		}
	}
}
