using System;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization
{
	public interface IValueSerializer
	{
		void SerializeValue(IEmitter emitter, object value, Type type);
	}
}
