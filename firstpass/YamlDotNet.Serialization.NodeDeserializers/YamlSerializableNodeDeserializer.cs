using System;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.NodeDeserializers
{
	public sealed class YamlSerializableNodeDeserializer : INodeDeserializer
	{
		private readonly IObjectFactory objectFactory;

		public YamlSerializableNodeDeserializer(IObjectFactory objectFactory)
		{
			this.objectFactory = objectFactory;
		}

		public bool Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
		{
			if (typeof(IYamlSerializable).IsAssignableFrom(expectedType))
			{
				IYamlSerializable yamlSerializable = (IYamlSerializable)objectFactory.Create(expectedType);
				yamlSerializable.ReadYaml(parser);
				value = yamlSerializable;
				return true;
			}
			value = null;
			return false;
		}
	}
}
