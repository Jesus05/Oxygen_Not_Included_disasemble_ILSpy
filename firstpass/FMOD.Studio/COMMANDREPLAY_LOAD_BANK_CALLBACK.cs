using System;

namespace FMOD.Studio
{
	public delegate RESULT COMMANDREPLAY_LOAD_BANK_CALLBACK(CommandReplay replay, Guid guid, StringWrapper bankFilename, LOAD_BANK_FLAGS flags, out Bank bank, IntPtr userdata);
}
