using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public struct SoundGroup
	{
		public IntPtr handle;

		public RESULT release()
		{
			return FMOD5_SoundGroup_Release(handle);
		}

		public RESULT getSystemObject(out System system)
		{
			return FMOD5_SoundGroup_GetSystemObject(handle, out system.handle);
		}

		public RESULT setMaxAudible(int maxaudible)
		{
			return FMOD5_SoundGroup_SetMaxAudible(handle, maxaudible);
		}

		public RESULT getMaxAudible(out int maxaudible)
		{
			return FMOD5_SoundGroup_GetMaxAudible(handle, out maxaudible);
		}

		public RESULT setMaxAudibleBehavior(SOUNDGROUP_BEHAVIOR behavior)
		{
			return FMOD5_SoundGroup_SetMaxAudibleBehavior(handle, behavior);
		}

		public RESULT getMaxAudibleBehavior(out SOUNDGROUP_BEHAVIOR behavior)
		{
			return FMOD5_SoundGroup_GetMaxAudibleBehavior(handle, out behavior);
		}

		public RESULT setMuteFadeSpeed(float speed)
		{
			return FMOD5_SoundGroup_SetMuteFadeSpeed(handle, speed);
		}

		public RESULT getMuteFadeSpeed(out float speed)
		{
			return FMOD5_SoundGroup_GetMuteFadeSpeed(handle, out speed);
		}

		public RESULT setVolume(float volume)
		{
			return FMOD5_SoundGroup_SetVolume(handle, volume);
		}

		public RESULT getVolume(out float volume)
		{
			return FMOD5_SoundGroup_GetVolume(handle, out volume);
		}

		public RESULT stop()
		{
			return FMOD5_SoundGroup_Stop(handle);
		}

		public RESULT getName(out string name, int namelen)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(namelen);
			RESULT result = FMOD5_SoundGroup_GetName(handle, intPtr, namelen);
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				name = threadSafeEncoding.stringFromNative(intPtr);
			}
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public RESULT getNumSounds(out int numsounds)
		{
			return FMOD5_SoundGroup_GetNumSounds(handle, out numsounds);
		}

		public RESULT getSound(int index, out Sound sound)
		{
			return FMOD5_SoundGroup_GetSound(handle, index, out sound.handle);
		}

		public RESULT getNumPlaying(out int numplaying)
		{
			return FMOD5_SoundGroup_GetNumPlaying(handle, out numplaying);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return FMOD5_SoundGroup_SetUserData(handle, userdata);
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return FMOD5_SoundGroup_GetUserData(handle, out userdata);
		}

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_Release(IntPtr soundgroup);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_GetSystemObject(IntPtr soundgroup, out IntPtr system);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_SetMaxAudible(IntPtr soundgroup, int maxaudible);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_GetMaxAudible(IntPtr soundgroup, out int maxaudible);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_SetMaxAudibleBehavior(IntPtr soundgroup, SOUNDGROUP_BEHAVIOR behavior);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_GetMaxAudibleBehavior(IntPtr soundgroup, out SOUNDGROUP_BEHAVIOR behavior);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_SetMuteFadeSpeed(IntPtr soundgroup, float speed);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_GetMuteFadeSpeed(IntPtr soundgroup, out float speed);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_SetVolume(IntPtr soundgroup, float volume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_GetVolume(IntPtr soundgroup, out float volume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_Stop(IntPtr soundgroup);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_GetName(IntPtr soundgroup, IntPtr name, int namelen);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_GetNumSounds(IntPtr soundgroup, out int numsounds);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_GetSound(IntPtr soundgroup, int index, out IntPtr sound);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_GetNumPlaying(IntPtr soundgroup, out int numplaying);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_SetUserData(IntPtr soundgroup, IntPtr userdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_SoundGroup_GetUserData(IntPtr soundgroup, out IntPtr userdata);

		public bool hasHandle()
		{
			return handle != IntPtr.Zero;
		}

		public void clearHandle()
		{
			handle = IntPtr.Zero;
		}
	}
}
