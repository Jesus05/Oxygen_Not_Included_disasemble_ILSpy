using System;

namespace KMod
{
	[Flags]
	public enum Content : byte
	{
		LayerableFiles = 0x1,
		Strings = 0x2,
		DLL = 0x4,
		Translation = 0x8,
		Animation = 0x10
	}
}
