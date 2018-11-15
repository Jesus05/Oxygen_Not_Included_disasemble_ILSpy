using System;

namespace FMOD.Studio
{
	public delegate RESULT EVENT_CALLBACK(EVENT_CALLBACK_TYPE type, EventInstance eventInstance, IntPtr parameters);
}
