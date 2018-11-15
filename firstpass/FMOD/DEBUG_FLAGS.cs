using System;

namespace FMOD
{
	[Flags]
	public enum DEBUG_FLAGS : uint
	{
		NONE = 0x0,
		ERROR = 0x1,
		WARNING = 0x2,
		LOG = 0x4,
		TYPE_MEMORY = 0x100,
		TYPE_FILE = 0x200,
		TYPE_CODEC = 0x400,
		TYPE_TRACE = 0x800,
		DISPLAY_TIMESTAMPS = 0x10000,
		DISPLAY_LINENUMBERS = 0x20000,
		DISPLAY_THREAD = 0x40000
	}
}
