using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public struct Sound
	{
		public IntPtr handle;

		public RESULT release()
		{
			return FMOD5_Sound_Release(handle);
		}

		public RESULT getSystemObject(out System system)
		{
			return FMOD5_Sound_GetSystemObject(handle, out system.handle);
		}

		public RESULT @lock(uint offset, uint length, out IntPtr ptr1, out IntPtr ptr2, out uint len1, out uint len2)
		{
			return FMOD5_Sound_Lock(handle, offset, length, out ptr1, out ptr2, out len1, out len2);
		}

		public RESULT unlock(IntPtr ptr1, IntPtr ptr2, uint len1, uint len2)
		{
			return FMOD5_Sound_Unlock(handle, ptr1, ptr2, len1, len2);
		}

		public RESULT setDefaults(float frequency, int priority)
		{
			return FMOD5_Sound_SetDefaults(handle, frequency, priority);
		}

		public RESULT getDefaults(out float frequency, out int priority)
		{
			return FMOD5_Sound_GetDefaults(handle, out frequency, out priority);
		}

		public RESULT set3DMinMaxDistance(float min, float max)
		{
			return FMOD5_Sound_Set3DMinMaxDistance(handle, min, max);
		}

		public RESULT get3DMinMaxDistance(out float min, out float max)
		{
			return FMOD5_Sound_Get3DMinMaxDistance(handle, out min, out max);
		}

		public RESULT set3DConeSettings(float insideconeangle, float outsideconeangle, float outsidevolume)
		{
			return FMOD5_Sound_Set3DConeSettings(handle, insideconeangle, outsideconeangle, outsidevolume);
		}

		public RESULT get3DConeSettings(out float insideconeangle, out float outsideconeangle, out float outsidevolume)
		{
			return FMOD5_Sound_Get3DConeSettings(handle, out insideconeangle, out outsideconeangle, out outsidevolume);
		}

		public RESULT set3DCustomRolloff(ref VECTOR points, int numpoints)
		{
			return FMOD5_Sound_Set3DCustomRolloff(handle, ref points, numpoints);
		}

		public RESULT get3DCustomRolloff(out IntPtr points, out int numpoints)
		{
			return FMOD5_Sound_Get3DCustomRolloff(handle, out points, out numpoints);
		}

		public RESULT getSubSound(int index, out Sound subsound)
		{
			return FMOD5_Sound_GetSubSound(handle, index, out subsound.handle);
		}

		public RESULT getSubSoundParent(out Sound parentsound)
		{
			return FMOD5_Sound_GetSubSoundParent(handle, out parentsound.handle);
		}

		public RESULT getName(out string name, int namelen)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(namelen);
			RESULT result = FMOD5_Sound_GetName(handle, intPtr, namelen);
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				name = threadSafeEncoding.stringFromNative(intPtr);
			}
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public RESULT getLength(out uint length, TIMEUNIT lengthtype)
		{
			return FMOD5_Sound_GetLength(handle, out length, lengthtype);
		}

		public RESULT getFormat(out SOUND_TYPE type, out SOUND_FORMAT format, out int channels, out int bits)
		{
			return FMOD5_Sound_GetFormat(handle, out type, out format, out channels, out bits);
		}

		public RESULT getNumSubSounds(out int numsubsounds)
		{
			return FMOD5_Sound_GetNumSubSounds(handle, out numsubsounds);
		}

		public RESULT getNumTags(out int numtags, out int numtagsupdated)
		{
			return FMOD5_Sound_GetNumTags(handle, out numtags, out numtagsupdated);
		}

		public RESULT getTag(string name, int index, out TAG tag)
		{
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				return FMOD5_Sound_GetTag(handle, threadSafeEncoding.byteFromStringUTF8(name), index, out tag);
			}
		}

		public RESULT getOpenState(out OPENSTATE openstate, out uint percentbuffered, out bool starving, out bool diskbusy)
		{
			return FMOD5_Sound_GetOpenState(handle, out openstate, out percentbuffered, out starving, out diskbusy);
		}

		public RESULT readData(IntPtr buffer, uint lenbytes, out uint read)
		{
			return FMOD5_Sound_ReadData(handle, buffer, lenbytes, out read);
		}

		public RESULT seekData(uint pcm)
		{
			return FMOD5_Sound_SeekData(handle, pcm);
		}

		public RESULT setSoundGroup(SoundGroup soundgroup)
		{
			return FMOD5_Sound_SetSoundGroup(handle, soundgroup.handle);
		}

		public RESULT getSoundGroup(out SoundGroup soundgroup)
		{
			return FMOD5_Sound_GetSoundGroup(handle, out soundgroup.handle);
		}

		public RESULT getNumSyncPoints(out int numsyncpoints)
		{
			return FMOD5_Sound_GetNumSyncPoints(handle, out numsyncpoints);
		}

		public RESULT getSyncPoint(int index, out IntPtr point)
		{
			return FMOD5_Sound_GetSyncPoint(handle, index, out point);
		}

		public RESULT getSyncPointInfo(IntPtr point, out string name, int namelen, out uint offset, TIMEUNIT offsettype)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(namelen);
			RESULT result = FMOD5_Sound_GetSyncPointInfo(handle, point, intPtr, namelen, out offset, offsettype);
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				name = threadSafeEncoding.stringFromNative(intPtr);
			}
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public RESULT getSyncPointInfo(IntPtr point, out uint offset, TIMEUNIT offsettype)
		{
			return FMOD5_Sound_GetSyncPointInfo(handle, point, IntPtr.Zero, 0, out offset, offsettype);
		}

		public RESULT addSyncPoint(uint offset, TIMEUNIT offsettype, string name, out IntPtr point)
		{
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				return FMOD5_Sound_AddSyncPoint(handle, offset, offsettype, threadSafeEncoding.byteFromStringUTF8(name), out point);
			}
		}

		public RESULT deleteSyncPoint(IntPtr point)
		{
			return FMOD5_Sound_DeleteSyncPoint(handle, point);
		}

		public RESULT setMode(MODE mode)
		{
			return FMOD5_Sound_SetMode(handle, mode);
		}

		public RESULT getMode(out MODE mode)
		{
			return FMOD5_Sound_GetMode(handle, out mode);
		}

		public RESULT setLoopCount(int loopcount)
		{
			return FMOD5_Sound_SetLoopCount(handle, loopcount);
		}

		public RESULT getLoopCount(out int loopcount)
		{
			return FMOD5_Sound_GetLoopCount(handle, out loopcount);
		}

		public RESULT setLoopPoints(uint loopstart, TIMEUNIT loopstarttype, uint loopend, TIMEUNIT loopendtype)
		{
			return FMOD5_Sound_SetLoopPoints(handle, loopstart, loopstarttype, loopend, loopendtype);
		}

		public RESULT getLoopPoints(out uint loopstart, TIMEUNIT loopstarttype, out uint loopend, TIMEUNIT loopendtype)
		{
			return FMOD5_Sound_GetLoopPoints(handle, out loopstart, loopstarttype, out loopend, loopendtype);
		}

		public RESULT getMusicNumChannels(out int numchannels)
		{
			return FMOD5_Sound_GetMusicNumChannels(handle, out numchannels);
		}

		public RESULT setMusicChannelVolume(int channel, float volume)
		{
			return FMOD5_Sound_SetMusicChannelVolume(handle, channel, volume);
		}

		public RESULT getMusicChannelVolume(int channel, out float volume)
		{
			return FMOD5_Sound_GetMusicChannelVolume(handle, channel, out volume);
		}

		public RESULT setMusicSpeed(float speed)
		{
			return FMOD5_Sound_SetMusicSpeed(handle, speed);
		}

		public RESULT getMusicSpeed(out float speed)
		{
			return FMOD5_Sound_GetMusicSpeed(handle, out speed);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return FMOD5_Sound_SetUserData(handle, userdata);
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return FMOD5_Sound_GetUserData(handle, out userdata);
		}

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_Release(IntPtr sound);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_GetSystemObject(IntPtr sound, out IntPtr system);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_Lock(IntPtr sound, uint offset, uint length, out IntPtr ptr1, out IntPtr ptr2, out uint len1, out uint len2);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_Unlock(IntPtr sound, IntPtr ptr1, IntPtr ptr2, uint len1, uint len2);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_SetDefaults(IntPtr sound, float frequency, int priority);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_GetDefaults(IntPtr sound, out float frequency, out int priority);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_Set3DMinMaxDistance(IntPtr sound, float min, float max);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_Get3DMinMaxDistance(IntPtr sound, out float min, out float max);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_Set3DConeSettings(IntPtr sound, float insideconeangle, float outsideconeangle, float outsidevolume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_Get3DConeSettings(IntPtr sound, out float insideconeangle, out float outsideconeangle, out float outsidevolume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_Set3DCustomRolloff(IntPtr sound, ref VECTOR points, int numpoints);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_Get3DCustomRolloff(IntPtr sound, out IntPtr points, out int numpoints);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_GetSubSound(IntPtr sound, int index, out IntPtr subsound);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_GetSubSoundParent(IntPtr sound, out IntPtr parentsound);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_GetName(IntPtr sound, IntPtr name, int namelen);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_GetLength(IntPtr sound, out uint length, TIMEUNIT lengthtype);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_GetFormat(IntPtr sound, out SOUND_TYPE type, out SOUND_FORMAT format, out int channels, out int bits);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_GetNumSubSounds(IntPtr sound, out int numsubsounds);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_GetNumTags(IntPtr sound, out int numtags, out int numtagsupdated);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_GetTag(IntPtr sound, byte[] name, int index, out TAG tag);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_GetOpenState(IntPtr sound, out OPENSTATE openstate, out uint percentbuffered, out bool starving, out bool diskbusy);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_ReadData(IntPtr sound, IntPtr buffer, uint lenbytes, out uint read);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_SeekData(IntPtr sound, uint pcm);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_SetSoundGroup(IntPtr sound, IntPtr soundgroup);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_GetSoundGroup(IntPtr sound, out IntPtr soundgroup);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_GetNumSyncPoints(IntPtr sound, out int numsyncpoints);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_GetSyncPoint(IntPtr sound, int index, out IntPtr point);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_GetSyncPointInfo(IntPtr sound, IntPtr point, IntPtr name, int namelen, out uint offset, TIMEUNIT offsettype);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_AddSyncPoint(IntPtr sound, uint offset, TIMEUNIT offsettype, byte[] name, out IntPtr point);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_DeleteSyncPoint(IntPtr sound, IntPtr point);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_SetMode(IntPtr sound, MODE mode);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_GetMode(IntPtr sound, out MODE mode);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_SetLoopCount(IntPtr sound, int loopcount);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_GetLoopCount(IntPtr sound, out int loopcount);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_SetLoopPoints(IntPtr sound, uint loopstart, TIMEUNIT loopstarttype, uint loopend, TIMEUNIT loopendtype);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_GetLoopPoints(IntPtr sound, out uint loopstart, TIMEUNIT loopstarttype, out uint loopend, TIMEUNIT loopendtype);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_GetMusicNumChannels(IntPtr sound, out int numchannels);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_SetMusicChannelVolume(IntPtr sound, int channel, float volume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_GetMusicChannelVolume(IntPtr sound, int channel, out float volume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_SetMusicSpeed(IntPtr sound, float speed);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_GetMusicSpeed(IntPtr sound, out float speed);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_SetUserData(IntPtr sound, IntPtr userdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Sound_GetUserData(IntPtr sound, out IntPtr userdata);

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
