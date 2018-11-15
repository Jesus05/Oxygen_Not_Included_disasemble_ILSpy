using System;

namespace YamlDotNet.Serialization
{
	[Flags]
	public enum SerializationOptions
	{
		None = 0x0,
		Roundtrip = 0x1,
		DisableAliases = 0x2,
		EmitDefaults = 0x4,
		JsonCompatible = 0x8,
		DefaultToStaticType = 0x10
	}
}
