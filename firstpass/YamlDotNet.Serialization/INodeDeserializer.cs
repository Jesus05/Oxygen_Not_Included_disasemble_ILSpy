using System;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization
{
	public interface INodeDeserializer
	{
		bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value);
	}
}
