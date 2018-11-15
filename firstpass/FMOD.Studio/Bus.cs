using System;
using System.Runtime.InteropServices;

namespace FMOD.Studio
{
	public struct Bus
	{
		public IntPtr handle;

		public RESULT getID(out Guid id)
		{
			return FMOD_Studio_Bus_GetID(handle, out id);
		}

		public RESULT getPath(out string path)
		{
			path = null;
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				IntPtr intPtr = Marshal.AllocHGlobal(256);
				int retrieved = 0;
				RESULT rESULT = FMOD_Studio_Bus_GetPath(handle, intPtr, 256, out retrieved);
				if (rESULT == RESULT.ERR_TRUNCATED)
				{
					Marshal.FreeHGlobal(intPtr);
					intPtr = Marshal.AllocHGlobal(retrieved);
					rESULT = FMOD_Studio_Bus_GetPath(handle, intPtr, retrieved, out retrieved);
				}
				if (rESULT == RESULT.OK)
				{
					path = threadSafeEncoding.stringFromNative(intPtr);
				}
				Marshal.FreeHGlobal(intPtr);
				return rESULT;
			}
		}

		public RESULT getVolume(out float volume, out float finalvolume)
		{
			return FMOD_Studio_Bus_GetVolume(handle, out volume, out finalvolume);
		}

		public RESULT setVolume(float volume)
		{
			return FMOD_Studio_Bus_SetVolume(handle, volume);
		}

		public RESULT getPaused(out bool paused)
		{
			return FMOD_Studio_Bus_GetPaused(handle, out paused);
		}

		public RESULT setPaused(bool paused)
		{
			return FMOD_Studio_Bus_SetPaused(handle, paused);
		}

		public RESULT getMute(out bool mute)
		{
			return FMOD_Studio_Bus_GetMute(handle, out mute);
		}

		public RESULT setMute(bool mute)
		{
			return FMOD_Studio_Bus_SetMute(handle, mute);
		}

		public RESULT stopAllEvents(STOP_MODE mode)
		{
			return FMOD_Studio_Bus_StopAllEvents(handle, mode);
		}

		public RESULT lockChannelGroup()
		{
			return FMOD_Studio_Bus_LockChannelGroup(handle);
		}

		public RESULT unlockChannelGroup()
		{
			return FMOD_Studio_Bus_UnlockChannelGroup(handle);
		}

		public RESULT getChannelGroup(out ChannelGroup group)
		{
			return FMOD_Studio_Bus_GetChannelGroup(handle, out group.handle);
		}

		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_Bus_IsValid(IntPtr bus);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_GetID(IntPtr bus, out Guid id);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_GetPath(IntPtr bus, IntPtr path, int size, out int retrieved);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_GetVolume(IntPtr bus, out float volume, out float finalvolume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_SetVolume(IntPtr bus, float value);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_GetPaused(IntPtr bus, out bool paused);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_SetPaused(IntPtr bus, bool paused);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_GetMute(IntPtr bus, out bool mute);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_SetMute(IntPtr bus, bool mute);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_StopAllEvents(IntPtr bus, STOP_MODE mode);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_LockChannelGroup(IntPtr bus);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_UnlockChannelGroup(IntPtr bus);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bus_GetChannelGroup(IntPtr bus, out IntPtr group);

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
			return hasHandle() && FMOD_Studio_Bus_IsValid(handle);
		}
	}
}
