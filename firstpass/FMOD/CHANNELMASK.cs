using System;

namespace FMOD
{
	[Flags]
	public enum CHANNELMASK : uint
	{
		FRONT_LEFT = 0x1,
		FRONT_RIGHT = 0x2,
		FRONT_CENTER = 0x4,
		LOW_FREQUENCY = 0x8,
		SURROUND_LEFT = 0x10,
		SURROUND_RIGHT = 0x20,
		BACK_LEFT = 0x40,
		BACK_RIGHT = 0x80,
		BACK_CENTER = 0x100,
		MONO = 0x1,
		STEREO = 0x3,
		LRC = 0x7,
		QUAD = 0x33,
		SURROUND = 0x37,
		_5POINT1 = 0x3F,
		_5POINT1_REARS = 0xCF,
		_7POINT0 = 0xF7,
		_7POINT1 = 0xFF
	}
}
