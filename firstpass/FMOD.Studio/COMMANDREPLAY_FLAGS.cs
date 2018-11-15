using System;

namespace FMOD.Studio
{
	[Flags]
	public enum COMMANDREPLAY_FLAGS : uint
	{
		NORMAL = 0x0,
		SKIP_CLEANUP = 0x1,
		FAST_FORWARD = 0x2
	}
}
