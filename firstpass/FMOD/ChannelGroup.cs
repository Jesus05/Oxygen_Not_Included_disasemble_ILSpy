using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public struct ChannelGroup : IChannelControl
	{
		public IntPtr handle;

		public RESULT release()
		{
			return FMOD5_ChannelGroup_Release(handle);
		}

		public RESULT addGroup(ChannelGroup group, bool propagatedspclock, out DSPConnection connection)
		{
			return FMOD5_ChannelGroup_AddGroup(handle, group.handle, propagatedspclock, out connection.handle);
		}

		public RESULT getNumGroups(out int numgroups)
		{
			return FMOD5_ChannelGroup_GetNumGroups(handle, out numgroups);
		}

		public RESULT getGroup(int index, out ChannelGroup group)
		{
			return FMOD5_ChannelGroup_GetGroup(handle, index, out group.handle);
		}

		public RESULT getParentGroup(out ChannelGroup group)
		{
			return FMOD5_ChannelGroup_GetParentGroup(handle, out group.handle);
		}

		public RESULT getName(out string name, int namelen)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(namelen);
			RESULT result = FMOD5_ChannelGroup_GetName(handle, intPtr, namelen);
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				name = threadSafeEncoding.stringFromNative(intPtr);
			}
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public RESULT getNumChannels(out int numchannels)
		{
			return FMOD5_ChannelGroup_GetNumChannels(handle, out numchannels);
		}

		public RESULT getChannel(int index, out Channel channel)
		{
			return FMOD5_ChannelGroup_GetChannel(handle, index, out channel.handle);
		}

		public RESULT getSystemObject(out System system)
		{
			return FMOD5_ChannelGroup_GetSystemObject(handle, out system.handle);
		}

		public RESULT stop()
		{
			return FMOD5_ChannelGroup_Stop(handle);
		}

		public RESULT setPaused(bool paused)
		{
			return FMOD5_ChannelGroup_SetPaused(handle, paused);
		}

		public RESULT getPaused(out bool paused)
		{
			return FMOD5_ChannelGroup_GetPaused(handle, out paused);
		}

		public RESULT setVolume(float volume)
		{
			return FMOD5_ChannelGroup_SetVolume(handle, volume);
		}

		public RESULT getVolume(out float volume)
		{
			return FMOD5_ChannelGroup_GetVolume(handle, out volume);
		}

		public RESULT setVolumeRamp(bool ramp)
		{
			return FMOD5_ChannelGroup_SetVolumeRamp(handle, ramp);
		}

		public RESULT getVolumeRamp(out bool ramp)
		{
			return FMOD5_ChannelGroup_GetVolumeRamp(handle, out ramp);
		}

		public RESULT getAudibility(out float audibility)
		{
			return FMOD5_ChannelGroup_GetAudibility(handle, out audibility);
		}

		public RESULT setPitch(float pitch)
		{
			return FMOD5_ChannelGroup_SetPitch(handle, pitch);
		}

		public RESULT getPitch(out float pitch)
		{
			return FMOD5_ChannelGroup_GetPitch(handle, out pitch);
		}

		public RESULT setMute(bool mute)
		{
			return FMOD5_ChannelGroup_SetMute(handle, mute);
		}

		public RESULT getMute(out bool mute)
		{
			return FMOD5_ChannelGroup_GetMute(handle, out mute);
		}

		public RESULT setReverbProperties(int instance, float wet)
		{
			return FMOD5_ChannelGroup_SetReverbProperties(handle, instance, wet);
		}

		public RESULT getReverbProperties(int instance, out float wet)
		{
			return FMOD5_ChannelGroup_GetReverbProperties(handle, instance, out wet);
		}

		public RESULT setLowPassGain(float gain)
		{
			return FMOD5_ChannelGroup_SetLowPassGain(handle, gain);
		}

		public RESULT getLowPassGain(out float gain)
		{
			return FMOD5_ChannelGroup_GetLowPassGain(handle, out gain);
		}

		public RESULT setMode(MODE mode)
		{
			return FMOD5_ChannelGroup_SetMode(handle, mode);
		}

		public RESULT getMode(out MODE mode)
		{
			return FMOD5_ChannelGroup_GetMode(handle, out mode);
		}

		public RESULT setCallback(CHANNEL_CALLBACK callback)
		{
			return FMOD5_ChannelGroup_SetCallback(handle, callback);
		}

		public RESULT isPlaying(out bool isplaying)
		{
			return FMOD5_ChannelGroup_IsPlaying(handle, out isplaying);
		}

		public RESULT setPan(float pan)
		{
			return FMOD5_ChannelGroup_SetPan(handle, pan);
		}

		public RESULT setMixLevelsOutput(float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright)
		{
			return FMOD5_ChannelGroup_SetMixLevelsOutput(handle, frontleft, frontright, center, lfe, surroundleft, surroundright, backleft, backright);
		}

		public RESULT setMixLevelsInput(float[] levels, int numlevels)
		{
			return FMOD5_ChannelGroup_SetMixLevelsInput(handle, levels, numlevels);
		}

		public RESULT setMixMatrix(float[] matrix, int outchannels, int inchannels, int inchannel_hop)
		{
			return FMOD5_ChannelGroup_SetMixMatrix(handle, matrix, outchannels, inchannels, inchannel_hop);
		}

		public RESULT getMixMatrix(float[] matrix, out int outchannels, out int inchannels, int inchannel_hop)
		{
			return FMOD5_ChannelGroup_GetMixMatrix(handle, matrix, out outchannels, out inchannels, inchannel_hop);
		}

		public RESULT getDSPClock(out ulong dspclock, out ulong parentclock)
		{
			return FMOD5_ChannelGroup_GetDSPClock(handle, out dspclock, out parentclock);
		}

		public RESULT setDelay(ulong dspclock_start, ulong dspclock_end, bool stopchannels)
		{
			return FMOD5_ChannelGroup_SetDelay(handle, dspclock_start, dspclock_end, stopchannels);
		}

		public RESULT getDelay(out ulong dspclock_start, out ulong dspclock_end, out bool stopchannels)
		{
			return FMOD5_ChannelGroup_GetDelay(handle, out dspclock_start, out dspclock_end, out stopchannels);
		}

		public RESULT addFadePoint(ulong dspclock, float volume)
		{
			return FMOD5_ChannelGroup_AddFadePoint(handle, dspclock, volume);
		}

		public RESULT setFadePointRamp(ulong dspclock, float volume)
		{
			return FMOD5_ChannelGroup_SetFadePointRamp(handle, dspclock, volume);
		}

		public RESULT removeFadePoints(ulong dspclock_start, ulong dspclock_end)
		{
			return FMOD5_ChannelGroup_RemoveFadePoints(handle, dspclock_start, dspclock_end);
		}

		public RESULT getFadePoints(ref uint numpoints, ulong[] point_dspclock, float[] point_volume)
		{
			return FMOD5_ChannelGroup_GetFadePoints(handle, ref numpoints, point_dspclock, point_volume);
		}

		public RESULT getDSP(int index, out DSP dsp)
		{
			return FMOD5_ChannelGroup_GetDSP(handle, index, out dsp.handle);
		}

		public RESULT addDSP(int index, DSP dsp)
		{
			return FMOD5_ChannelGroup_AddDSP(handle, index, dsp.handle);
		}

		public RESULT removeDSP(DSP dsp)
		{
			return FMOD5_ChannelGroup_RemoveDSP(handle, dsp.handle);
		}

		public RESULT getNumDSPs(out int numdsps)
		{
			return FMOD5_ChannelGroup_GetNumDSPs(handle, out numdsps);
		}

		public RESULT setDSPIndex(DSP dsp, int index)
		{
			return FMOD5_ChannelGroup_SetDSPIndex(handle, dsp.handle, index);
		}

		public RESULT getDSPIndex(DSP dsp, out int index)
		{
			return FMOD5_ChannelGroup_GetDSPIndex(handle, dsp.handle, out index);
		}

		public RESULT set3DAttributes(ref VECTOR pos, ref VECTOR vel, ref VECTOR alt_pan_pos)
		{
			return FMOD5_ChannelGroup_Set3DAttributes(handle, ref pos, ref vel, ref alt_pan_pos);
		}

		public RESULT get3DAttributes(out VECTOR pos, out VECTOR vel, out VECTOR alt_pan_pos)
		{
			return FMOD5_ChannelGroup_Get3DAttributes(handle, out pos, out vel, out alt_pan_pos);
		}

		public RESULT set3DMinMaxDistance(float mindistance, float maxdistance)
		{
			return FMOD5_ChannelGroup_Set3DMinMaxDistance(handle, mindistance, maxdistance);
		}

		public RESULT get3DMinMaxDistance(out float mindistance, out float maxdistance)
		{
			return FMOD5_ChannelGroup_Get3DMinMaxDistance(handle, out mindistance, out maxdistance);
		}

		public RESULT set3DConeSettings(float insideconeangle, float outsideconeangle, float outsidevolume)
		{
			return FMOD5_ChannelGroup_Set3DConeSettings(handle, insideconeangle, outsideconeangle, outsidevolume);
		}

		public RESULT get3DConeSettings(out float insideconeangle, out float outsideconeangle, out float outsidevolume)
		{
			return FMOD5_ChannelGroup_Get3DConeSettings(handle, out insideconeangle, out outsideconeangle, out outsidevolume);
		}

		public RESULT set3DConeOrientation(ref VECTOR orientation)
		{
			return FMOD5_ChannelGroup_Set3DConeOrientation(handle, ref orientation);
		}

		public RESULT get3DConeOrientation(out VECTOR orientation)
		{
			return FMOD5_ChannelGroup_Get3DConeOrientation(handle, out orientation);
		}

		public RESULT set3DCustomRolloff(ref VECTOR points, int numpoints)
		{
			return FMOD5_ChannelGroup_Set3DCustomRolloff(handle, ref points, numpoints);
		}

		public RESULT get3DCustomRolloff(out IntPtr points, out int numpoints)
		{
			return FMOD5_ChannelGroup_Get3DCustomRolloff(handle, out points, out numpoints);
		}

		public RESULT set3DOcclusion(float directocclusion, float reverbocclusion)
		{
			return FMOD5_ChannelGroup_Set3DOcclusion(handle, directocclusion, reverbocclusion);
		}

		public RESULT get3DOcclusion(out float directocclusion, out float reverbocclusion)
		{
			return FMOD5_ChannelGroup_Get3DOcclusion(handle, out directocclusion, out reverbocclusion);
		}

		public RESULT set3DSpread(float angle)
		{
			return FMOD5_ChannelGroup_Set3DSpread(handle, angle);
		}

		public RESULT get3DSpread(out float angle)
		{
			return FMOD5_ChannelGroup_Get3DSpread(handle, out angle);
		}

		public RESULT set3DLevel(float level)
		{
			return FMOD5_ChannelGroup_Set3DLevel(handle, level);
		}

		public RESULT get3DLevel(out float level)
		{
			return FMOD5_ChannelGroup_Get3DLevel(handle, out level);
		}

		public RESULT set3DDopplerLevel(float level)
		{
			return FMOD5_ChannelGroup_Set3DDopplerLevel(handle, level);
		}

		public RESULT get3DDopplerLevel(out float level)
		{
			return FMOD5_ChannelGroup_Get3DDopplerLevel(handle, out level);
		}

		public RESULT set3DDistanceFilter(bool custom, float customLevel, float centerFreq)
		{
			return FMOD5_ChannelGroup_Set3DDistanceFilter(handle, custom, customLevel, centerFreq);
		}

		public RESULT get3DDistanceFilter(out bool custom, out float customLevel, out float centerFreq)
		{
			return FMOD5_ChannelGroup_Get3DDistanceFilter(handle, out custom, out customLevel, out centerFreq);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return FMOD5_ChannelGroup_SetUserData(handle, userdata);
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return FMOD5_ChannelGroup_GetUserData(handle, out userdata);
		}

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_Release(IntPtr channelgroup);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_AddGroup(IntPtr channelgroup, IntPtr group, bool propogatedspclocks, out IntPtr connection);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_GetNumGroups(IntPtr channelgroup, out int numgroups);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_GetGroup(IntPtr channelgroup, int index, out IntPtr group);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_GetParentGroup(IntPtr channelgroup, out IntPtr group);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_GetName(IntPtr channelgroup, IntPtr name, int namelen);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_GetNumChannels(IntPtr channelgroup, out int numchannels);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_GetChannel(IntPtr channelgroup, int index, out IntPtr channel);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_GetSystemObject(IntPtr channelgroup, out IntPtr system);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_Stop(IntPtr channelgroup);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_SetPaused(IntPtr channelgroup, bool paused);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_GetPaused(IntPtr channelgroup, out bool paused);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_SetVolume(IntPtr channelgroup, float volume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_GetVolume(IntPtr channelgroup, out float volume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_SetVolumeRamp(IntPtr channelgroup, bool ramp);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_GetVolumeRamp(IntPtr channelgroup, out bool ramp);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_GetAudibility(IntPtr channelgroup, out float audibility);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_SetPitch(IntPtr channelgroup, float pitch);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_GetPitch(IntPtr channelgroup, out float pitch);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_SetMute(IntPtr channelgroup, bool mute);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_GetMute(IntPtr channelgroup, out bool mute);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_SetReverbProperties(IntPtr channelgroup, int instance, float wet);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_GetReverbProperties(IntPtr channelgroup, int instance, out float wet);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_SetLowPassGain(IntPtr channelgroup, float gain);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_GetLowPassGain(IntPtr channelgroup, out float gain);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_SetMode(IntPtr channelgroup, MODE mode);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_GetMode(IntPtr channelgroup, out MODE mode);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_SetCallback(IntPtr channelgroup, CHANNEL_CALLBACK callback);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_IsPlaying(IntPtr channelgroup, out bool isplaying);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_SetPan(IntPtr channelgroup, float pan);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_SetMixLevelsOutput(IntPtr channelgroup, float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_SetMixLevelsInput(IntPtr channelgroup, float[] levels, int numlevels);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_SetMixMatrix(IntPtr channelgroup, float[] matrix, int outchannels, int inchannels, int inchannel_hop);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_GetMixMatrix(IntPtr channelgroup, float[] matrix, out int outchannels, out int inchannels, int inchannel_hop);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_GetDSPClock(IntPtr channelgroup, out ulong dspclock, out ulong parentclock);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_SetDelay(IntPtr channelgroup, ulong dspclock_start, ulong dspclock_end, bool stopchannels);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_GetDelay(IntPtr channelgroup, out ulong dspclock_start, out ulong dspclock_end, out bool stopchannels);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_AddFadePoint(IntPtr channelgroup, ulong dspclock, float volume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_SetFadePointRamp(IntPtr channelgroup, ulong dspclock, float volume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_RemoveFadePoints(IntPtr channelgroup, ulong dspclock_start, ulong dspclock_end);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_GetFadePoints(IntPtr channelgroup, ref uint numpoints, ulong[] point_dspclock, float[] point_volume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_GetDSP(IntPtr channelgroup, int index, out IntPtr dsp);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_AddDSP(IntPtr channelgroup, int index, IntPtr dsp);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_RemoveDSP(IntPtr channelgroup, IntPtr dsp);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_GetNumDSPs(IntPtr channelgroup, out int numdsps);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_SetDSPIndex(IntPtr channelgroup, IntPtr dsp, int index);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_GetDSPIndex(IntPtr channelgroup, IntPtr dsp, out int index);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_Set3DAttributes(IntPtr channelgroup, ref VECTOR pos, ref VECTOR vel, ref VECTOR alt_pan_pos);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_Get3DAttributes(IntPtr channelgroup, out VECTOR pos, out VECTOR vel, out VECTOR alt_pan_pos);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_Set3DMinMaxDistance(IntPtr channelgroup, float mindistance, float maxdistance);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_Get3DMinMaxDistance(IntPtr channelgroup, out float mindistance, out float maxdistance);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_Set3DConeSettings(IntPtr channelgroup, float insideconeangle, float outsideconeangle, float outsidevolume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_Get3DConeSettings(IntPtr channelgroup, out float insideconeangle, out float outsideconeangle, out float outsidevolume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_Set3DConeOrientation(IntPtr channelgroup, ref VECTOR orientation);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_Get3DConeOrientation(IntPtr channelgroup, out VECTOR orientation);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_Set3DCustomRolloff(IntPtr channelgroup, ref VECTOR points, int numpoints);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_Get3DCustomRolloff(IntPtr channelgroup, out IntPtr points, out int numpoints);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_Set3DOcclusion(IntPtr channelgroup, float directocclusion, float reverbocclusion);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_Get3DOcclusion(IntPtr channelgroup, out float directocclusion, out float reverbocclusion);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_Set3DSpread(IntPtr channelgroup, float angle);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_Get3DSpread(IntPtr channelgroup, out float angle);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_Set3DLevel(IntPtr channelgroup, float level);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_Get3DLevel(IntPtr channelgroup, out float level);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_Set3DDopplerLevel(IntPtr channelgroup, float level);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_Get3DDopplerLevel(IntPtr channelgroup, out float level);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_Set3DDistanceFilter(IntPtr channelgroup, bool custom, float customLevel, float centerFreq);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_Get3DDistanceFilter(IntPtr channelgroup, out bool custom, out float customLevel, out float centerFreq);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_SetUserData(IntPtr channelgroup, IntPtr userdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_ChannelGroup_GetUserData(IntPtr channelgroup, out IntPtr userdata);

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
