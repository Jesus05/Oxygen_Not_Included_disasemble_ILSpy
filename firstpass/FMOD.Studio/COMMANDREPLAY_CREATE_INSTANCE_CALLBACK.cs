using System;

namespace FMOD.Studio
{
	public delegate RESULT COMMANDREPLAY_CREATE_INSTANCE_CALLBACK(CommandReplay replay, EventDescription eventDescription, IntPtr originalHandle, out EventInstance instance, IntPtr userdata);
}
