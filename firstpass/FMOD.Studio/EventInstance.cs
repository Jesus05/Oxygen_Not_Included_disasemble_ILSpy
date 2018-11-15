using System;
using System.Runtime.InteropServices;

namespace FMOD.Studio
{
	public struct EventInstance
	{
		public IntPtr handle;

		public RESULT getDescription(out EventDescription description)
		{
			return FMOD_Studio_EventInstance_GetDescription(handle, out description.handle);
		}

		public RESULT getVolume(out float volume, out float finalvolume)
		{
			return FMOD_Studio_EventInstance_GetVolume(handle, out volume, out finalvolume);
		}

		public RESULT setVolume(float volume)
		{
			return FMOD_Studio_EventInstance_SetVolume(handle, volume);
		}

		public RESULT getPitch(out float pitch, out float finalpitch)
		{
			return FMOD_Studio_EventInstance_GetPitch(handle, out pitch, out finalpitch);
		}

		public RESULT setPitch(float pitch)
		{
			return FMOD_Studio_EventInstance_SetPitch(handle, pitch);
		}

		public RESULT get3DAttributes(out ATTRIBUTES_3D attributes)
		{
			return FMOD_Studio_EventInstance_Get3DAttributes(handle, out attributes);
		}

		public RESULT set3DAttributes(ATTRIBUTES_3D attributes)
		{
			return FMOD_Studio_EventInstance_Set3DAttributes(handle, ref attributes);
		}

		public RESULT getListenerMask(out uint mask)
		{
			return FMOD_Studio_EventInstance_GetListenerMask(handle, out mask);
		}

		public RESULT setListenerMask(uint mask)
		{
			return FMOD_Studio_EventInstance_SetListenerMask(handle, mask);
		}

		public RESULT getProperty(EVENT_PROPERTY index, out float value)
		{
			return FMOD_Studio_EventInstance_GetProperty(handle, index, out value);
		}

		public RESULT setProperty(EVENT_PROPERTY index, float value)
		{
			return FMOD_Studio_EventInstance_SetProperty(handle, index, value);
		}

		public RESULT getReverbLevel(int index, out float level)
		{
			return FMOD_Studio_EventInstance_GetReverbLevel(handle, index, out level);
		}

		public RESULT setReverbLevel(int index, float level)
		{
			return FMOD_Studio_EventInstance_SetReverbLevel(handle, index, level);
		}

		public RESULT getPaused(out bool paused)
		{
			return FMOD_Studio_EventInstance_GetPaused(handle, out paused);
		}

		public RESULT setPaused(bool paused)
		{
			return FMOD_Studio_EventInstance_SetPaused(handle, paused);
		}

		public RESULT start()
		{
			return FMOD_Studio_EventInstance_Start(handle);
		}

		public RESULT stop(STOP_MODE mode)
		{
			return FMOD_Studio_EventInstance_Stop(handle, mode);
		}

		public RESULT getTimelinePosition(out int position)
		{
			return FMOD_Studio_EventInstance_GetTimelinePosition(handle, out position);
		}

		public RESULT setTimelinePosition(int position)
		{
			return FMOD_Studio_EventInstance_SetTimelinePosition(handle, position);
		}

		public RESULT getPlaybackState(out PLAYBACK_STATE state)
		{
			return FMOD_Studio_EventInstance_GetPlaybackState(handle, out state);
		}

		public RESULT getChannelGroup(out ChannelGroup group)
		{
			return FMOD_Studio_EventInstance_GetChannelGroup(handle, out group.handle);
		}

		public RESULT release()
		{
			return FMOD_Studio_EventInstance_Release(handle);
		}

		public RESULT isVirtual(out bool virtualState)
		{
			return FMOD_Studio_EventInstance_IsVirtual(handle, out virtualState);
		}

		public RESULT getParameter(string name, out ParameterInstance instance)
		{
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				return FMOD_Studio_EventInstance_GetParameter(handle, threadSafeEncoding.byteFromStringUTF8(name), out instance.handle);
			}
		}

		public RESULT getParameterCount(out int count)
		{
			return FMOD_Studio_EventInstance_GetParameterCount(handle, out count);
		}

		public RESULT getParameterByIndex(int index, out ParameterInstance instance)
		{
			return FMOD_Studio_EventInstance_GetParameterByIndex(handle, index, out instance.handle);
		}

		public RESULT getParameterValue(string name, out float value, out float finalvalue)
		{
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				return FMOD_Studio_EventInstance_GetParameterValue(handle, threadSafeEncoding.byteFromStringUTF8(name), out value, out finalvalue);
			}
		}

		public RESULT setParameterValue(string name, float value)
		{
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				return FMOD_Studio_EventInstance_SetParameterValue(handle, threadSafeEncoding.byteFromStringUTF8(name), value);
			}
		}

		public RESULT getParameterValueByIndex(int index, out float value, out float finalvalue)
		{
			return FMOD_Studio_EventInstance_GetParameterValueByIndex(handle, index, out value, out finalvalue);
		}

		public RESULT setParameterValueByIndex(int index, float value)
		{
			return FMOD_Studio_EventInstance_SetParameterValueByIndex(handle, index, value);
		}

		public RESULT setParameterValuesByIndices(int[] indices, float[] values, int count)
		{
			return FMOD_Studio_EventInstance_SetParameterValuesByIndices(handle, indices, values, count);
		}

		public RESULT triggerCue()
		{
			return FMOD_Studio_EventInstance_TriggerCue(handle);
		}

		public RESULT setCallback(EVENT_CALLBACK callback, EVENT_CALLBACK_TYPE callbackmask = EVENT_CALLBACK_TYPE.ALL)
		{
			return FMOD_Studio_EventInstance_SetCallback(handle, callback, callbackmask);
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return FMOD_Studio_EventInstance_GetUserData(handle, out userdata);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return FMOD_Studio_EventInstance_SetUserData(handle, userdata);
		}

		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_EventInstance_IsValid(IntPtr _event);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetDescription(IntPtr _event, out IntPtr description);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetVolume(IntPtr _event, out float volume, out float finalvolume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetVolume(IntPtr _event, float volume);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetPitch(IntPtr _event, out float pitch, out float finalpitch);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetPitch(IntPtr _event, float pitch);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_Get3DAttributes(IntPtr _event, out ATTRIBUTES_3D attributes);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_Set3DAttributes(IntPtr _event, ref ATTRIBUTES_3D attributes);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetListenerMask(IntPtr _event, out uint mask);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetListenerMask(IntPtr _event, uint mask);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetProperty(IntPtr _event, EVENT_PROPERTY index, out float value);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetProperty(IntPtr _event, EVENT_PROPERTY index, float value);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetReverbLevel(IntPtr _event, int index, out float level);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetReverbLevel(IntPtr _event, int index, float level);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetPaused(IntPtr _event, out bool paused);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetPaused(IntPtr _event, bool paused);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_Start(IntPtr _event);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_Stop(IntPtr _event, STOP_MODE mode);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetTimelinePosition(IntPtr _event, out int position);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetTimelinePosition(IntPtr _event, int position);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetPlaybackState(IntPtr _event, out PLAYBACK_STATE state);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetChannelGroup(IntPtr _event, out IntPtr group);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_Release(IntPtr _event);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_IsVirtual(IntPtr _event, out bool virtualState);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetParameter(IntPtr _event, byte[] name, out IntPtr parameter);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetParameterByIndex(IntPtr _event, int index, out IntPtr parameter);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetParameterCount(IntPtr _event, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetParameterValue(IntPtr _event, byte[] name, out float value, out float finalvalue);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetParameterValue(IntPtr _event, byte[] name, float value);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetParameterValueByIndex(IntPtr _event, int index, out float value, out float finalvalue);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetParameterValueByIndex(IntPtr _event, int index, float value);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetParameterValuesByIndices(IntPtr _event, int[] indices, float[] values, int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_TriggerCue(IntPtr _event);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetCallback(IntPtr _event, EVENT_CALLBACK callback, EVENT_CALLBACK_TYPE callbackmask);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_GetUserData(IntPtr _event, out IntPtr userdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventInstance_SetUserData(IntPtr _event, IntPtr userdata);

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
			return hasHandle() && FMOD_Studio_EventInstance_IsValid(handle);
		}
	}
}
