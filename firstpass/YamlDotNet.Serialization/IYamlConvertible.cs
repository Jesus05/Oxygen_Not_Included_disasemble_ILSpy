using System;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization
{
	public interface IYamlConvertible
	{
		void Read(IParser parser, Type expectedType, ObjectDeserializer nestedObjectDeserializer);

		void Write(IEmitter emitter, ObjectSerializer nestedObjectSerializer);
	}
}
