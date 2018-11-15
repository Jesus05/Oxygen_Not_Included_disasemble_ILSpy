using System;
using System.Runtime.InteropServices;

namespace FMOD.Studio
{
	public struct CommandReplay
	{
		public IntPtr handle;

		public RESULT getSystem(out System system)
		{
			return FMOD_Studio_CommandReplay_GetSystem(handle, out system.handle);
		}

		public RESULT getLength(out float totalTime)
		{
			return FMOD_Studio_CommandReplay_GetLength(handle, out totalTime);
		}

		public RESULT getCommandCount(out int count)
		{
			return FMOD_Studio_CommandReplay_GetCommandCount(handle, out count);
		}

		public RESULT getCommandInfo(int commandIndex, out COMMAND_INFO info)
		{
			return FMOD_Studio_CommandReplay_GetCommandInfo(handle, commandIndex, out info);
		}

		public RESULT getCommandString(int commandIndex, out string description)
		{
			description = null;
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				int num = 256;
				IntPtr intPtr = Marshal.AllocHGlobal(256);
				RESULT rESULT = FMOD_Studio_CommandReplay_GetCommandString(handle, commandIndex, intPtr, num);
				while (true)
				{
					switch (rESULT)
					{
					case RESULT.ERR_TRUNCATED:
						break;
					case RESULT.OK:
						description = threadSafeEncoding.stringFromNative(intPtr);
						goto default;
					default:
						Marshal.FreeHGlobal(intPtr);
						return rESULT;
					}
					Marshal.FreeHGlobal(intPtr);
					num *= 2;
					intPtr = Marshal.AllocHGlobal(num);
					rESULT = FMOD_Studio_CommandReplay_GetCommandString(handle, commandIndex, intPtr, num);
				}
			}
		}

		public RESULT getCommandAtTime(float time, out int commandIndex)
		{
			return FMOD_Studio_CommandReplay_GetCommandAtTime(handle, time, out commandIndex);
		}

		public RESULT setBankPath(string bankPath)
		{
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				return FMOD_Studio_CommandReplay_SetBankPath(handle, threadSafeEncoding.byteFromStringUTF8(bankPath));
			}
		}

		public RESULT start()
		{
			return FMOD_Studio_CommandReplay_Start(handle);
		}

		public RESULT stop()
		{
			return FMOD_Studio_CommandReplay_Stop(handle);
		}

		public RESULT seekToTime(float time)
		{
			return FMOD_Studio_CommandReplay_SeekToTime(handle, time);
		}

		public RESULT seekToCommand(int commandIndex)
		{
			return FMOD_Studio_CommandReplay_SeekToCommand(handle, commandIndex);
		}

		public RESULT getPaused(out bool paused)
		{
			return FMOD_Studio_CommandReplay_GetPaused(handle, out paused);
		}

		public RESULT setPaused(bool paused)
		{
			return FMOD_Studio_CommandReplay_SetPaused(handle, paused);
		}

		public RESULT getPlaybackState(out PLAYBACK_STATE state)
		{
			return FMOD_Studio_CommandReplay_GetPlaybackState(handle, out state);
		}

		public RESULT getCurrentCommand(out int commandIndex, out float currentTime)
		{
			return FMOD_Studio_CommandReplay_GetCurrentCommand(handle, out commandIndex, out currentTime);
		}

		public RESULT release()
		{
			return FMOD_Studio_CommandReplay_Release(handle);
		}

		public RESULT setFrameCallback(COMMANDREPLAY_FRAME_CALLBACK callback)
		{
			return FMOD_Studio_CommandReplay_SetFrameCallback(handle, callback);
		}

		public RESULT setLoadBankCallback(COMMANDREPLAY_LOAD_BANK_CALLBACK callback)
		{
			return FMOD_Studio_CommandReplay_SetLoadBankCallback(handle, callback);
		}

		public RESULT setCreateInstanceCallback(COMMANDREPLAY_CREATE_INSTANCE_CALLBACK callback)
		{
			return FMOD_Studio_CommandReplay_SetCreateInstanceCallback(handle, callback);
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return FMOD_Studio_CommandReplay_GetUserData(handle, out userdata);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return FMOD_Studio_CommandReplay_SetUserData(handle, userdata);
		}

		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_CommandReplay_IsValid(IntPtr replay);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetSystem(IntPtr replay, out IntPtr system);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetLength(IntPtr replay, out float totalTime);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetCommandCount(IntPtr replay, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetCommandInfo(IntPtr replay, int commandIndex, out COMMAND_INFO info);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetCommandString(IntPtr replay, int commandIndex, IntPtr description, int capacity);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetCommandAtTime(IntPtr replay, float time, out int commandIndex);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_SetBankPath(IntPtr replay, byte[] bankPath);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_Start(IntPtr replay);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_Stop(IntPtr replay);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_SeekToTime(IntPtr replay, float time);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_SeekToCommand(IntPtr replay, int commandIndex);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetPaused(IntPtr replay, out bool paused);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_SetPaused(IntPtr replay, bool paused);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetPlaybackState(IntPtr replay, out PLAYBACK_STATE state);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetCurrentCommand(IntPtr replay, out int commandIndex, out float currentTime);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_Release(IntPtr replay);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_SetFrameCallback(IntPtr replay, COMMANDREPLAY_FRAME_CALLBACK callback);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_SetLoadBankCallback(IntPtr replay, COMMANDREPLAY_LOAD_BANK_CALLBACK callback);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_SetCreateInstanceCallback(IntPtr replay, COMMANDREPLAY_CREATE_INSTANCE_CALLBACK callback);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_GetUserData(IntPtr replay, out IntPtr userdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_CommandReplay_SetUserData(IntPtr replay, IntPtr userdata);

		public bool hasHandle()
		{
			return handle != IntPtr.Zero;
		}

		public void clearHandle()
		{
			handle = IntPtr.Zero;
		}

		public bool isValid()
		{
			return hasHandle() && FMOD_Studio_CommandReplay_IsValid(handle);
		}
	}
}
