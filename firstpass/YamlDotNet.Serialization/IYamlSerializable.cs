using System;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization
{
	[Obsolete("Please use IYamlConvertible instead")]
	public interface IYamlSerializable
	{
		void ReadYaml(IParser parser);

		void WriteYaml(IEmitter emitter);
	}
}
