using System;

namespace FMOD
{
	[Flags]
	public enum TIMEUNIT : uint
	{
		MS = 0x1,
		PCM = 0x2,
		PCMBYTES = 0x4,
		RAWBYTES = 0x8,
		PCMFRACTION = 0x10,
		MODORDER = 0x100,
		MODROW = 0x200,
		MODPATTERN = 0x400
	}
}
