using System;

namespace FMOD
{
	[Flags]
	public enum MEMORY_TYPE : uint
	{
		NORMAL = 0x0,
		STREAM_FILE = 0x1,
		STREAM_DECODE = 0x2,
		SAMPLEDATA = 0x4,
		DSP_BUFFER = 0x8,
		PLUGIN = 0x10,
		XBOX360_PHYSICAL = 0x100000,
		PERSISTENT = 0x200000,
		SECONDARY = 0x400000,
		ALL = uint.MaxValue
	}
}
