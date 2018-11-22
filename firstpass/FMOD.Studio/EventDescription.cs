using System;
using System.Runtime.InteropServices;

namespace FMOD.Studio
{
	public struct EventDescription
	{
		public IntPtr handle;

		public RESULT getID(out Guid id)
		{
			return FMOD_Studio_EventDescription_GetID(handle, out id);
		}

		public RESULT getPath(out string path)
		{
			path = null;
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				IntPtr intPtr = Marshal.AllocHGlobal(256);
				int retrieved = 0;
				RESULT rESULT = FMOD_Studio_EventDescription_GetPath(handle, intPtr, 256, out retrieved);
				if (rESULT == RESULT.ERR_TRUNCATED)
				{
					Marshal.FreeHGlobal(intPtr);
					intPtr = Marshal.AllocHGlobal(retrieved);
					rESULT = FMOD_Studio_EventDescription_GetPath(handle, intPtr, retrieved, out retrieved);
				}
				if (rESULT == RESULT.OK)
				{
					path = threadSafeEncoding.stringFromNative(intPtr);
				}
				Marshal.FreeHGlobal(intPtr);
				return rESULT;
			}
		}

		public RESULT getParameterCount(out int count)
		{
			return FMOD_Studio_EventDescription_GetParameterCount(handle, out count);
		}

		public RESULT getParameterByIndex(int index, out PARAMETER_DESCRIPTION parameter)
		{
			return FMOD_Studio_EventDescription_GetParameterByIndex(handle, index, out parameter);
		}

		public RESULT getParameter(string name, out PARAMETER_DESCRIPTION parameter)
		{
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				return FMOD_Studio_EventDescription_GetParameter(handle, threadSafeEncoding.byteFromStringUTF8(name), out parameter);
			}
		}

		public RESULT getUserPropertyCount(out int count)
		{
			return FMOD_Studio_EventDescription_GetUserPropertyCount(handle, out count);
		}

		public RESULT getUserPropertyByIndex(int index, out USER_PROPERTY property)
		{
			return FMOD_Studio_EventDescription_GetUserPropertyByIndex(handle, index, out property);
		}

		public RESULT getUserProperty(string name, out USER_PROPERTY property)
		{
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				return FMOD_Studio_EventDescription_GetUserProperty(handle, threadSafeEncoding.byteFromStringUTF8(name), out property);
			}
		}

		public RESULT getLength(out int length)
		{
			return FMOD_Studio_EventDescription_GetLength(handle, out length);
		}

		public RESULT getMinimumDistance(out float distance)
		{
			return FMOD_Studio_EventDescription_GetMinimumDistance(handle, out distance);
		}

		public RESULT getMaximumDistance(out float distance)
		{
			return FMOD_Studio_EventDescription_GetMaximumDistance(handle, out distance);
		}

		public RESULT getSoundSize(out float size)
		{
			return FMOD_Studio_EventDescription_GetSoundSize(handle, out size);
		}

		public RESULT isSnapshot(out bool snapshot)
		{
			return FMOD_Studio_EventDescription_IsSnapshot(handle, out snapshot);
		}

		public RESULT isOneshot(out bool oneshot)
		{
			return FMOD_Studio_EventDescription_IsOneshot(handle, out oneshot);
		}

		public RESULT isStream(out bool isStream)
		{
			return FMOD_Studio_EventDescription_IsStream(handle, out isStream);
		}

		public RESULT is3D(out bool is3D)
		{
			return FMOD_Studio_EventDescription_Is3D(handle, out is3D);
		}

		public RESULT hasCue(out bool cue)
		{
			return FMOD_Studio_EventDescription_HasCue(handle, out cue);
		}

		public RESULT createInstance(out EventInstance instance)
		{
			return FMOD_Studio_EventDescription_CreateInstance(handle, out instance.handle);
		}

		public RESULT getInstanceCount(out int count)
		{
			return FMOD_Studio_EventDescription_GetInstanceCount(handle, out count);
		}

		public RESULT getInstanceList(out EventInstance[] array)
		{
			array = null;
			RESULT rESULT = FMOD_Studio_EventDescription_GetInstanceCount(handle, out int count);
			if (rESULT == RESULT.OK)
			{
				if (count != 0)
				{
					IntPtr[] array2 = new IntPtr[count];
					rESULT = FMOD_Studio_EventDescription_GetInstanceList(handle, array2, count, out int count2);
					if (rESULT == RESULT.OK)
					{
						if (count2 > count)
						{
							count2 = count;
						}
						array = new EventInstance[count2];
						for (int i = 0; i < count2; i++)
						{
							array[i].handle = array2[i];
						}
						return RESULT.OK;
					}
					return rESULT;
				}
				array = new EventInstance[0];
				return rESULT;
			}
			return rESULT;
		}

		public RESULT loadSampleData()
		{
			return FMOD_Studio_EventDescription_LoadSampleData(handle);
		}

		public RESULT unloadSampleData()
		{
			return FMOD_Studio_EventDescription_UnloadSampleData(handle);
		}

		public RESULT getSampleLoadingState(out LOADING_STATE state)
		{
			return FMOD_Studio_EventDescription_GetSampleLoadingState(handle, out state);
		}

		public RESULT releaseAllInstances()
		{
			return FMOD_Studio_EventDescription_ReleaseAllInstances(handle);
		}

		public RESULT setCallback(EVENT_CALLBACK callback, EVENT_CALLBACK_TYPE callbackmask = EVENT_CALLBACK_TYPE.ALL)
		{
			return FMOD_Studio_EventDescription_SetCallback(handle, callback, callbackmask);
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return FMOD_Studio_EventDescription_GetUserData(handle, out userdata);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return FMOD_Studio_EventDescription_SetUserData(handle, userdata);
		}

		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_EventDescription_IsValid(IntPtr eventdescription);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetID(IntPtr eventdescription, out Guid id);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetPath(IntPtr eventdescription, IntPtr path, int size, out int retrieved);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetParameterCount(IntPtr eventdescription, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetParameterByIndex(IntPtr eventdescription, int index, out PARAMETER_DESCRIPTION parameter);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetParameter(IntPtr eventdescription, byte[] name, out PARAMETER_DESCRIPTION parameter);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetUserPropertyCount(IntPtr eventdescription, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetUserPropertyByIndex(IntPtr eventdescription, int index, out USER_PROPERTY property);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetUserProperty(IntPtr eventdescription, byte[] name, out USER_PROPERTY property);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetLength(IntPtr eventdescription, out int length);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetMinimumDistance(IntPtr eventdescription, out float distance);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetMaximumDistance(IntPtr eventdescription, out float distance);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetSoundSize(IntPtr eventdescription, out float size);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_IsSnapshot(IntPtr eventdescription, out bool snapshot);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_IsOneshot(IntPtr eventdescription, out bool oneshot);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_IsStream(IntPtr eventdescription, out bool isStream);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_Is3D(IntPtr eventdescription, out bool is3D);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_HasCue(IntPtr eventdescription, out bool cue);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_CreateInstance(IntPtr eventdescription, out IntPtr instance);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetInstanceCount(IntPtr eventdescription, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetInstanceList(IntPtr eventdescription, IntPtr[] array, int capacity, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_LoadSampleData(IntPtr eventdescription);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_UnloadSampleData(IntPtr eventdescription);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetSampleLoadingState(IntPtr eventdescription, out LOADING_STATE state);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_ReleaseAllInstances(IntPtr eventdescription);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_SetCallback(IntPtr eventdescription, EVENT_CALLBACK callback, EVENT_CALLBACK_TYPE callbackmask);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_GetUserData(IntPtr eventdescription, out IntPtr userdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_EventDescription_SetUserData(IntPtr eventdescription, IntPtr userdata);

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
			return hasHandle() && FMOD_Studio_EventDescription_IsValid(handle);
		}
	}
}
