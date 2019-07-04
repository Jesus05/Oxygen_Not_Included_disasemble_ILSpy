using System;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.NodeDeserializers
{
	public sealed class YamlConvertibleNodeDeserializer : INodeDeserializer
	{
		private readonly IObjectFactory objectFactory;

		public YamlConvertibleNodeDeserializer(IObjectFactory objectFactory)
		{
			this.objectFactory = objectFactory;
		}

		public bool Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
		{
			if (!typeof(IYamlConvertible).IsAssignableFrom(expectedType))
			{
				value = null;
				return false;
			}
			IYamlConvertible yamlConvertible = (IYamlConvertible)objectFactory.Create(expectedType);
			yamlConvertible.Read(parser, expectedType, (Type type) => nestedObjectDeserializer(parser, type));
			value = yamlConvertible;
			return true;
		}
	}
}
