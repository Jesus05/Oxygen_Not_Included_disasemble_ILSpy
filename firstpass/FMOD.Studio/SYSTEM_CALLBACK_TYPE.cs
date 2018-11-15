using System;

namespace FMOD.Studio
{
	[Flags]
	public enum SYSTEM_CALLBACK_TYPE : uint
	{
		PREUPDATE = 0x1,
		POSTUPDATE = 0x2,
		BANK_UNLOAD = 0x4,
		ALL = uint.MaxValue
	}
}
