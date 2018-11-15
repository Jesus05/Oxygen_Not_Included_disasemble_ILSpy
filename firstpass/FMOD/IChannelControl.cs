using System;

namespace FMOD
{
	internal interface IChannelControl
	{
		RESULT getSystemObject(out System system);

		RESULT stop();

		RESULT setPaused(bool paused);

		RESULT getPaused(out bool paused);

		RESULT setVolume(float volume);

		RESULT getVolume(out float volume);

		RESULT setVolumeRamp(bool ramp);

		RESULT getVolumeRamp(out bool ramp);

		RESULT getAudibility(out float audibility);

		RESULT setPitch(float pitch);

		RESULT getPitch(out float pitch);

		RESULT setMute(bool mute);

		RESULT getMute(out bool mute);

		RESULT setReverbProperties(int instance, float wet);

		RESULT getReverbProperties(int instance, out float wet);

		RESULT setLowPassGain(float gain);

		RESULT getLowPassGain(out float gain);

		RESULT setMode(MODE mode);

		RESULT getMode(out MODE mode);

		RESULT setCallback(CHANNEL_CALLBACK callback);

		RESULT isPlaying(out bool isplaying);

		RESULT setPan(float pan);

		RESULT setMixLevelsOutput(float frontleft, float frontright, float center, float lfe, float surroundleft, float surroundright, float backleft, float backright);

		RESULT setMixLevelsInput(float[] levels, int numlevels);

		RESULT setMixMatrix(float[] matrix, int outchannels, int inchannels, int inchannel_hop);

		RESULT getMixMatrix(float[] matrix, out int outchannels, out int inchannels, int inchannel_hop);

		RESULT getDSPClock(out ulong dspclock, out ulong parentclock);

		RESULT setDelay(ulong dspclock_start, ulong dspclock_end, bool stopchannels);

		RESULT getDelay(out ulong dspclock_start, out ulong dspclock_end, out bool stopchannels);

		RESULT addFadePoint(ulong dspclock, float volume);

		RESULT setFadePointRamp(ulong dspclock, float volume);

		RESULT removeFadePoints(ulong dspclock_start, ulong dspclock_end);

		RESULT getFadePoints(ref uint numpoints, ulong[] point_dspclock, float[] point_volume);

		RESULT getDSP(int index, out DSP dsp);

		RESULT addDSP(int index, DSP dsp);

		RESULT removeDSP(DSP dsp);

		RESULT getNumDSPs(out int numdsps);

		RESULT setDSPIndex(DSP dsp, int index);

		RESULT getDSPIndex(DSP dsp, out int index);

		RESULT set3DAttributes(ref VECTOR pos, ref VECTOR vel, ref VECTOR alt_pan_pos);

		RESULT get3DAttributes(out VECTOR pos, out VECTOR vel, out VECTOR alt_pan_pos);

		RESULT set3DMinMaxDistance(float mindistance, float maxdistance);

		RESULT get3DMinMaxDistance(out float mindistance, out float maxdistance);

		RESULT set3DConeSettings(float insideconeangle, float outsideconeangle, float outsidevolume);

		RESULT get3DConeSettings(out float insideconeangle, out float outsideconeangle, out float outsidevolume);

		RESULT set3DConeOrientation(ref VECTOR orientation);

		RESULT get3DConeOrientation(out VECTOR orientation);

		RESULT set3DCustomRolloff(ref VECTOR points, int numpoints);

		RESULT get3DCustomRolloff(out IntPtr points, out int numpoints);

		RESULT set3DOcclusion(float directocclusion, float reverbocclusion);

		RESULT get3DOcclusion(out float directocclusion, out float reverbocclusion);

		RESULT set3DSpread(float angle);

		RESULT get3DSpread(out float angle);

		RESULT set3DLevel(float level);

		RESULT get3DLevel(out float level);

		RESULT set3DDopplerLevel(float level);

		RESULT get3DDopplerLevel(out float level);

		RESULT set3DDistanceFilter(bool custom, float customLevel, float centerFreq);

		RESULT get3DDistanceFilter(out bool custom, out float customLevel, out float centerFreq);

		RESULT setUserData(IntPtr userdata);

		RESULT getUserData(out IntPtr userdata);
	}
}
