using System;
using System.Runtime.InteropServices;

namespace FMOD.Studio
{
	public struct Bank
	{
		public IntPtr handle;

		public RESULT getID(out Guid id)
		{
			return FMOD_Studio_Bank_GetID(handle, out id);
		}

		public RESULT getPath(out string path)
		{
			path = null;
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				IntPtr intPtr = Marshal.AllocHGlobal(256);
				int retrieved = 0;
				RESULT rESULT = FMOD_Studio_Bank_GetPath(handle, intPtr, 256, out retrieved);
				if (rESULT == RESULT.ERR_TRUNCATED)
				{
					Marshal.FreeHGlobal(intPtr);
					intPtr = Marshal.AllocHGlobal(retrieved);
					rESULT = FMOD_Studio_Bank_GetPath(handle, intPtr, retrieved, out retrieved);
				}
				if (rESULT == RESULT.OK)
				{
					path = threadSafeEncoding.stringFromNative(intPtr);
				}
				Marshal.FreeHGlobal(intPtr);
				return rESULT;
			}
		}

		public RESULT unload()
		{
			return FMOD_Studio_Bank_Unload(handle);
		}

		public RESULT loadSampleData()
		{
			return FMOD_Studio_Bank_LoadSampleData(handle);
		}

		public RESULT unloadSampleData()
		{
			return FMOD_Studio_Bank_UnloadSampleData(handle);
		}

		public RESULT getLoadingState(out LOADING_STATE state)
		{
			return FMOD_Studio_Bank_GetLoadingState(handle, out state);
		}

		public RESULT getSampleLoadingState(out LOADING_STATE state)
		{
			return FMOD_Studio_Bank_GetSampleLoadingState(handle, out state);
		}

		public RESULT getStringCount(out int count)
		{
			return FMOD_Studio_Bank_GetStringCount(handle, out count);
		}

		public RESULT getStringInfo(int index, out Guid id, out string path)
		{
			path = null;
			id = Guid.Empty;
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				IntPtr intPtr = Marshal.AllocHGlobal(256);
				int retrieved = 0;
				RESULT rESULT = FMOD_Studio_Bank_GetStringInfo(handle, index, out id, intPtr, 256, out retrieved);
				if (rESULT == RESULT.ERR_TRUNCATED)
				{
					Marshal.FreeHGlobal(intPtr);
					intPtr = Marshal.AllocHGlobal(retrieved);
					rESULT = FMOD_Studio_Bank_GetStringInfo(handle, index, out id, intPtr, retrieved, out retrieved);
				}
				if (rESULT == RESULT.OK)
				{
					path = threadSafeEncoding.stringFromNative(intPtr);
				}
				Marshal.FreeHGlobal(intPtr);
				return rESULT;
			}
		}

		public RESULT getEventCount(out int count)
		{
			return FMOD_Studio_Bank_GetEventCount(handle, out count);
		}

		public RESULT getEventList(out EventDescription[] array)
		{
			array = null;
			RESULT rESULT = FMOD_Studio_Bank_GetEventCount(handle, out int count);
			if (rESULT == RESULT.OK)
			{
				if (count != 0)
				{
					IntPtr[] array2 = new IntPtr[count];
					rESULT = FMOD_Studio_Bank_GetEventList(handle, array2, count, out int count2);
					if (rESULT == RESULT.OK)
					{
						if (count2 > count)
						{
							count2 = count;
						}
						array = new EventDescription[count2];
						for (int i = 0; i < count2; i++)
						{
							array[i].handle = array2[i];
						}
						return RESULT.OK;
					}
					return rESULT;
				}
				array = new EventDescription[0];
				return rESULT;
			}
			return rESULT;
		}

		public RESULT getBusCount(out int count)
		{
			return FMOD_Studio_Bank_GetBusCount(handle, out count);
		}

		public RESULT getBusList(out Bus[] array)
		{
			array = null;
			RESULT rESULT = FMOD_Studio_Bank_GetBusCount(handle, out int count);
			if (rESULT == RESULT.OK)
			{
				if (count != 0)
				{
					IntPtr[] array2 = new IntPtr[count];
					rESULT = FMOD_Studio_Bank_GetBusList(handle, array2, count, out int count2);
					if (rESULT == RESULT.OK)
					{
						if (count2 > count)
						{
							count2 = count;
						}
						array = new Bus[count2];
						for (int i = 0; i < count2; i++)
						{
							array[i].handle = array2[i];
						}
						return RESULT.OK;
					}
					return rESULT;
				}
				array = new Bus[0];
				return rESULT;
			}
			return rESULT;
		}

		public RESULT getVCACount(out int count)
		{
			return FMOD_Studio_Bank_GetVCACount(handle, out count);
		}

		public RESULT getVCAList(out VCA[] array)
		{
			array = null;
			RESULT rESULT = FMOD_Studio_Bank_GetVCACount(handle, out int count);
			if (rESULT == RESULT.OK)
			{
				if (count != 0)
				{
					IntPtr[] array2 = new IntPtr[count];
					rESULT = FMOD_Studio_Bank_GetVCAList(handle, array2, count, out int count2);
					if (rESULT == RESULT.OK)
					{
						if (count2 > count)
						{
							count2 = count;
						}
						array = new VCA[count2];
						for (int i = 0; i < count2; i++)
						{
							array[i].handle = array2[i];
						}
						return RESULT.OK;
					}
					return rESULT;
				}
				array = new VCA[0];
				return rESULT;
			}
			return rESULT;
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return FMOD_Studio_Bank_GetUserData(handle, out userdata);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return FMOD_Studio_Bank_SetUserData(handle, userdata);
		}

		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_Bank_IsValid(IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetID(IntPtr bank, out Guid id);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetPath(IntPtr bank, IntPtr path, int size, out int retrieved);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_Unload(IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_LoadSampleData(IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_UnloadSampleData(IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetLoadingState(IntPtr bank, out LOADING_STATE state);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetSampleLoadingState(IntPtr bank, out LOADING_STATE state);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetStringCount(IntPtr bank, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetStringInfo(IntPtr bank, int index, out Guid id, IntPtr path, int size, out int retrieved);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetEventCount(IntPtr bank, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetEventList(IntPtr bank, IntPtr[] array, int capacity, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetBusCount(IntPtr bank, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetBusList(IntPtr bank, IntPtr[] array, int capacity, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetVCACount(IntPtr bank, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetVCAList(IntPtr bank, IntPtr[] array, int capacity, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_GetUserData(IntPtr bank, out IntPtr userdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_Bank_SetUserData(IntPtr bank, IntPtr userdata);

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
			return hasHandle() && FMOD_Studio_Bank_IsValid(handle);
		}
	}
}
