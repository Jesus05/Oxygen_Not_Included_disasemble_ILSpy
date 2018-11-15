using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public struct Channel : IChannelControl
	{
		public IntPtr handle;

		public RESULT setFrequency(float frequency)
		{
			return FMOD5_Channel_SetFrequency(handle, frequency);
		}

		public RESULT getFrequency(out float frequency)
		{
			return FMOD5_Channel_GetFrequency(handle, out frequency);
		}

		public RESULT setPriority(int priority)
		{
			return FMOD5_Channel_SetPriority(handle, priority);
		}

		public RESULT getPriority(out int priority)
		{
			return FMOD5_Channel_GetPriority(handle, out priority);
		}

		public RESULT setPosition(uint position, TIMEUNIT postype)
		{
			return FMOD5_Channel_SetPosition(handle, position, postype);
		}

		public RESULT getPosition(out uint position, TIMEUNIT postype)
		{
			return FMOD5_Channel_GetPosition(handle, out position, postype);
		}

		public RESULT setChannelGroup(ChannelGroup channelgroup)
		{
			return FMOD5_Channel_SetChannelGroup(handle, channelgroup.handle);
		}

		public RESULT getChannelGroup(out ChannelGroup channelgroup)
		{
			return FMOD5_Channel_GetChannelGroup(handle, out channelgroup.handle);
		}

		public RESULT setLoopCount(int loopcount)
		{
			return FMOD5_Channel_SetLoopCount(handle, loopcount);
		}

		public RESULT getLoopCount(out int loopcount)
		{
			return FMOD5_Channel_GetLoopCount(handle, out loopcount);
		}

		public RESULT setLoopPoints(uint loopstart, TIMEUNIT loopstarttype, uint loopend, TIMEUNIT loopendtype)
		{
			return FMOD5_Channel_SetLoopPoints(handle, loopstart, loopstarttype, loopend, loopendtype);
		}

		public RESULT getLoopPoints(out uint loopstart, TIMEUNIT loopstarttype, out uint loopend, TIMEUNIT loopendtype)
		{
			return FMOD5_Channel_GetLoopPoints(handle, out loopstart, loopstarttype, out loopend, loopendtype);
		}

		public RESULT isVirtual(out bool isvirtual)
		{
			return FMOD5_Channel_IsVirtual(handle, out isvirtual);
		}

		public RESULT getCurrentSound(out Sound sound)
		{
			return FMOD5_Channel_GetCurrentSound(handle, out sound.handle);
		}

		public RESULT getIndex(out int index)
		{
			return FMOD5_Channel_GetIndex(handle, out index);
		}

		public RESULT getSystemObject(out System system)
		{
			return FMOD5_Channel_GetSystemObject(handle, out system.handle);
		}

		public RESULT stop()
		{
			return FMOD5_Channel_Stop(handle);
		}

		public RESULT setPaused(bool paused)
		{
			return FMOD5_Channel_SetPaused(handle, paused);
		}

		public RESULT getPaused(out bool paused)
		{
			return FMOD5_Channel_GetPaused(handle, out paused);
		}

		public RESULT setVolume(float volume)
		{
			return FMOD5_Channel_SetVolume(handle, volume);
		}

		public RESULT getVolume(out float volume)
		{
			return FMOD5_Channel_GetVolume(handle, out volume);
		}

		public RESULT setVolumeRamp(bool ramp)
		{
			return FMOD5_Channel_SetVolumeRamp(handle, ramp);
		}

		public RESULT getVolumeRamp(out bool ramp)
		{
			return FMOD5_Channel_GetVolumeRamp(handle, out ramp);
		}

		public RESULT getAudibility(out float audibility)
		{
			return FMOD5_Channel_GetAudibility(handle, out audibility);
		}

		public RESULT setPitch(float pitch)
		{
			return FMOD5_Channel_SetPitch(handle, pitch);
		}

		public RESULT getPitch(out float pitch)
		{
			return FMOD5_Channel_GetPitch(handle, out pitch);
		}

		public RESULT setMute(bool mute)
		{
			return FMOD5_Channel_SetMute(handle, mute);
		}

		public RESULT getMute(out bool mute)
		{
			return FMOD5_Channel_GetMute(handle, out mute);
		}

		public RESULT setReverbProperties(int instance, float wet)
		{
			return FMOD5_Channel_SetReverbProperties(handle, instance, wet);
		}

		public RESULT getReverbProperties(int instance, out float wet)
		{
			return FMOD5_Channel_GetReverbProperties(handle, instance, out wet);
		}

		public RESULT setLowPassGain(float gain)
		{
			return FMOD5_Channel_SetLowPassGain(handle, gain);
		}

		public RESULT getLowPassGain(out float gain)
		{
			return FMOD5_Channel_GetLowPassGain(handle, out gain);
		}

		public RESULT setMode(MODE mode)
		{
			return FMOD5_Channel_SetMode(handle, mode);
		}

		public RESULT getMode(out MODE mode)
		{
			return FMOD5_Channel_GetMode(handle, out mode);
		}

		public RESULT setCallback(CHANNEL_CALLBACK callback)
		{
			return FMOD5_Channel_SetCallback(handle, callback);
		}

		public RESULT isPlaying(out bool isplaying)
		{
			return FMOD5_Channel_IsPlaying(handle, out isplaying);
		}

		public RESULT setPan(float pan)
		{
			return FMOD5_Channel_SetPan(handle, pan);
		}

		public RESULT setMixLevelsOutput(float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright)
		{
			return FMOD5_Channel_SetMixLevelsOutput(handle, frontleft, frontright, center, lfe, surroundleft, surroundright, backleft, backright);
		}

		public RESULT setMixLevelsInput(float[] levels, int numlevels)
		{
			return FMOD5_Channel_SetMixLevelsInput(handle, levels, numlevels);
		}

		public RESULT setMixMatrix(float[] matrix, int outchannels, int inchannels, int inchannel_hop)
		{
			return FMOD5_Channel_SetMixMatrix(handle, matrix, outchannels, inchannels, inchannel_hop);
		}

		public RESULT getMixMatrix(float[] matrix, out int outchannels, out int inchannels, int inchannel_hop)
		{
			return FMOD5_Channel_GetMixMatrix(handle, matrix, out outchannels, out inchannels, inchannel_hop);
		}

		public RESULT getDSPClock(out ulong dspclock, out ulong parentclock)
		{
			return FMOD5_Channel_GetDSPClock(handle, out dspclock, out parentclock);
		}

		public RESULT setDelay(ulong dspclock_start, ulong dspclock_end, bool stopchannels)
		{
			return FMOD5_Channel_SetDelay(handle, dspclock_start, dspclock_end, stopchannels);
		}

		public RESULT getDelay(out ulong dspclock_start, out ulong dspclock_end, out bool stopchannels)
		{
			return FMOD5_Channel_GetDelay(handle, out dspclock_start, out dspclock_end, out stopchannels);
		}

		public RESULT addFadePoint(ulong dspclock, float volume)
		{
			return FMOD5_Channel_AddFadePoint(handle, dspclock, volume);
		}

		public RESULT setFadePointRamp(ulong dspclock, float volume)
		{
			return FMOD5_Channel_SetFadePointRamp(handle, dspclock, volume);
		}

		public RESULT removeFadePoints(ulong dspclock_start, ulong dspclock_end)
		{
			return FMOD5_Channel_RemoveFadePoints(handle, dspclock_start, dspclock_end);
		}

		public RESULT getFadePoints(ref uint numpoints, ulong[] point_dspclock, float[] point_volume)
		{
			return FMOD5_Channel_GetFadePoints(handle, ref numpoints, point_dspclock, point_volume);
		}

		public RESULT getDSP(int index, out DSP dsp)
		{
			return FMOD5_Channel_GetDSP(handle, index, out dsp.handle);
		}

		public RESULT addDSP(int index, DSP dsp)
		{
			return FMOD5_Channel_AddDSP(handle, index, dsp.handle);
		}

		public RESULT removeDSP(DSP dsp)
		{
			return FMOD5_Channel_RemoveDSP(handle, dsp.handle);
		}

		public RESULT getNumDSPs(out int numdsps)
		{
			return FMOD5_Channel_GetNumDSPs(handle, out numdsps);
		}

		public RESULT setDSPIndex(DSP dsp, int index)
		{
			return FMOD5_Channel_SetDSPIndex(handle, dsp.handle, index);
		}

		public RESULT getDSPIndex(DSP dsp, out int index)
		{
			return FMOD5_Channel_GetDSPIndex(handle, dsp.handle, out index);
		}

		public RESULT set3DAttributes(ref VECTOR pos, ref VECTOR vel, ref VECTOR alt_pan_pos)
		{
			return FMOD5_Channel_Set3DAttributes(handle, ref pos, ref vel, ref alt_pan_pos);
		}

		public RESULT get3DAttributes(out VECTOR pos, out VECTOR vel, out VECTOR alt_pan_pos)
		{
			return FMOD5_Channel_Get3DAttributes(handle, out pos, out vel, out alt_pan_pos);
		}

		public RESULT set3DMinMaxDistance(float mindistance, float maxdistance)
		{
			return FMOD5_Channel_Set3DMinMaxDistance(handle, mindistance, maxdistance);
		}

		public RESULT get3DMinMaxDistance(out float mindistance, out float maxdistance)
		{
			return FMOD5_Channel_Get3DMinMaxDistance(handle, out mindistance, out maxdistance);
		}

		public RESULT set3DConeSettings(float insideconeangle, float outsideconeangle, float outsidevolume)
		{
			return FMOD5_Channel_Set3DConeSettings(handle, insideconeangle, outsideconeangle, outsidevolume);
		}

		public RESULT get3DConeSettings(out float insideconeangle, out float outsideconeangle, out float outsidevolume)
		{
			return FMOD5_Channel_Get3DConeSettings(handle, out insideconeangle, out outsideconeangle, out outsidevolume);
		}

		public RESULT set3DConeOrientation(ref VECTOR orientation)
		{
			return FMOD5_Channel_Set3DConeOrientation(handle, ref orientation);
		}

		public RESULT get3DConeOrientation(out VECTOR orientation)
		{
			return FMOD5_Channel_Get3DConeOrientation(handle, out orientation);
		}

		public RESULT set3DCustomRolloff(ref VECTOR points, int numpoints)
		{
			return FMOD5_Channel_Set3DCustomRolloff(handle, ref points, numpoints);
		}

		public RESULT get3DCustomRolloff(out IntPtr points, out int numpoints)
		{
			return FMOD5_Channel_Get3DCustomRolloff(handle, out points, out numpoints);
		}

		public RESULT set3DOcclusion(float directocclusion, float reverbocclusion)
		{
			return FMOD5_Channel_Set3DOcclusion(handle, directocclusion, reverbocclusion);
		}

		public RESULT get3DOcclusion(out float directocclusion, out float reverbocclusion)
		{
			return FMOD5_Channel_Get3DOcclusion(handle, out directocclusion, out reverbocclusion);
		}

		public RESULT set3DSpread(float angle)
		{
			return FMOD5_Channel_Set3DSpread(handle, angle);
		}

		public RESULT get3DSpread(out float angle)
		{
			return FMOD5_Channel_Get3DSpread(handle, out angle);
		}

		public RESULT set3DLevel(float level)
		{
			return FMOD5_Channel_Set3DLevel(handle, level);
		}

		public RESULT get3DLevel(out float level)
		{
			return FMOD5_Channel_Get3DLevel(handle, out level);
		}

		public RESULT set3DDopplerLevel(float level)
		{
			return FMOD5_Channel_Set3DDopplerLevel(handle, level);
		}

		public RESULT get3DDopplerLevel(out float level)
		{
			return FMOD5_Channel_Get3DDopplerLevel(handle, out level);
		}

		public RESULT set3DDistanceFilter(bool custom, float customLevel, float centerFreq)
		{
			return FMOD5_Channel_Set3DDistanceFilter(handle, custom, customLevel, centerFreq);
		}

		public RESULT get3DDistanceFilter(out bool custom, out float customLevel, out float centerFreq)
		{
			return FMOD5_Channel_Get3DDistanceFilter(handle, out custom, out customLevel, out centerFreq);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return FMOD5_Channel_SetUserData(handle, userdata);
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return FMOD5_Channel_GetUserData(handle, out userdata);
		}

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetFrequency(IntPtr channel, float frequency);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetFrequency(IntPtr channel, out float frequency);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetPriority(IntPtr channel, int priority);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetPriority(IntPtr channel, out int priority);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetPosition(IntPtr channel, uint position, TIMEUNIT postype);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetPosition(IntPtr channel, out uint position, TIMEUNIT postype);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetChannelGroup(IntPtr channel, IntPtr channelgroup);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetChannelGroup(IntPtr channel, out IntPtr channelgroup);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetLoopCount(IntPtr channel, int loopcount);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetLoopCount(IntPtr channel, out int loopcount);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetLoopPoints(IntPtr channel, uint loopstart, TIMEUNIT loopstarttype, uint loopend, TIMEUNIT loopendtype);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetLoopPoints(IntPtr channel, out uint loopstart, TIMEUNIT loopstarttype, out uint loopend, TIMEUNIT loopendtype);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_IsVirtual(IntPtr channel, out bool isvirtual);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetCurrentSound(IntPtr channel, out IntPtr sound);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetIndex(IntPtr channel, out int index);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetSystemObject(IntPtr channel, out IntPtr system);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Stop(IntPtr channel);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetPaused(IntPtr channel, bool paused);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetPaused(IntPtr channel, out bool paused);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetVolume(IntPtr channel, float volume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetVolume(IntPtr channel, out float volume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetVolumeRamp(IntPtr channel, bool ramp);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetVolumeRamp(IntPtr channel, out bool ramp);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetAudibility(IntPtr channel, out float audibility);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetPitch(IntPtr channel, float pitch);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetPitch(IntPtr channel, out float pitch);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetMute(IntPtr channel, bool mute);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetMute(IntPtr channel, out bool mute);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetReverbProperties(IntPtr channel, int instance, float wet);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetReverbProperties(IntPtr channel, int instance, out float wet);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetLowPassGain(IntPtr channel, float gain);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetLowPassGain(IntPtr channel, out float gain);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetMode(IntPtr channel, MODE mode);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetMode(IntPtr channel, out MODE mode);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetCallback(IntPtr channel, CHANNEL_CALLBACK callback);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_IsPlaying(IntPtr channel, out bool isplaying);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetPan(IntPtr channel, float pan);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetMixLevelsOutput(IntPtr channel, float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetMixLevelsInput(IntPtr channel, float[] levels, int numlevels);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetMixMatrix(IntPtr channel, float[] matrix, int outchannels, int inchannels, int inchannel_hop);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetMixMatrix(IntPtr channel, float[] matrix, out int outchannels, out int inchannels, int inchannel_hop);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetDSPClock(IntPtr channel, out ulong dspclock, out ulong parentclock);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetDelay(IntPtr channel, ulong dspclock_start, ulong dspclock_end, bool stopchannels);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetDelay(IntPtr channel, out ulong dspclock_start, out ulong dspclock_end, out bool stopchannels);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_AddFadePoint(IntPtr channel, ulong dspclock, float volume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetFadePointRamp(IntPtr channel, ulong dspclock, float volume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_RemoveFadePoints(IntPtr channel, ulong dspclock_start, ulong dspclock_end);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetFadePoints(IntPtr channel, ref uint numpoints, ulong[] point_dspclock, float[] point_volume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetDSP(IntPtr channel, int index, out IntPtr dsp);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_AddDSP(IntPtr channel, int index, IntPtr dsp);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_RemoveDSP(IntPtr channel, IntPtr dsp);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetNumDSPs(IntPtr channel, out int numdsps);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetDSPIndex(IntPtr channel, IntPtr dsp, int index);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetDSPIndex(IntPtr channel, IntPtr dsp, out int index);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Set3DAttributes(IntPtr channel, ref VECTOR pos, ref VECTOR vel, ref VECTOR alt_pan_pos);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Get3DAttributes(IntPtr channel, out VECTOR pos, out VECTOR vel, out VECTOR alt_pan_pos);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Set3DMinMaxDistance(IntPtr channel, float mindistance, float maxdistance);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Get3DMinMaxDistance(IntPtr channel, out float mindistance, out float maxdistance);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Set3DConeSettings(IntPtr channel, float insideconeangle, float outsideconeangle, float outsidevolume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Get3DConeSettings(IntPtr channel, out float insideconeangle, out float outsideconeangle, out float outsidevolume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Set3DConeOrientation(IntPtr channel, ref VECTOR orientation);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Get3DConeOrientation(IntPtr channel, out VECTOR orientation);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Set3DCustomRolloff(IntPtr channel, ref VECTOR points, int numpoints);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Get3DCustomRolloff(IntPtr channel, out IntPtr points, out int numpoints);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Set3DOcclusion(IntPtr channel, float directocclusion, float reverbocclusion);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Get3DOcclusion(IntPtr channel, out float directocclusion, out float reverbocclusion);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Set3DSpread(IntPtr channel, float angle);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Get3DSpread(IntPtr channel, out float angle);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Set3DLevel(IntPtr channel, float level);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Get3DLevel(IntPtr channel, out float level);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Set3DDopplerLevel(IntPtr channel, float level);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Get3DDopplerLevel(IntPtr channel, out float level);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Set3DDistanceFilter(IntPtr channel, bool custom, float customLevel, float centerFreq);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_Get3DDistanceFilter(IntPtr channel, out bool custom, out float customLevel, out float centerFreq);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_SetUserData(IntPtr channel, IntPtr userdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_Channel_GetUserData(IntPtr channel, out IntPtr userdata);

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
