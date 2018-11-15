using System;

namespace FMOD.Studio
{
	[Flags]
	public enum LOAD_BANK_FLAGS : uint
	{
		NORMAL = 0x0,
		NONBLOCKING = 0x1,
		DECOMPRESS_SAMPLES = 0x2
	}
}
