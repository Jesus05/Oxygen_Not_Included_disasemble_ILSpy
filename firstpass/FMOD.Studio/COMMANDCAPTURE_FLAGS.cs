using System;

namespace FMOD.Studio
{
	[Flags]
	public enum COMMANDCAPTURE_FLAGS : uint
	{
		NORMAL = 0x0,
		FILEFLUSH = 0x1,
		SKIP_INITIAL_STATE = 0x2
	}
}
