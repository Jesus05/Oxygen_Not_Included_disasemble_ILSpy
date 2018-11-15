using System;

namespace FMOD.Studio
{
	[Flags]
	public enum INITFLAGS : uint
	{
		NORMAL = 0x0,
		LIVEUPDATE = 0x1,
		ALLOW_MISSING_PLUGINS = 0x2,
		SYNCHRONOUS_UPDATE = 0x4,
		DEFERRED_CALLBACKS = 0x8,
		LOAD_FROM_UPDATE = 0x10
	}
}
