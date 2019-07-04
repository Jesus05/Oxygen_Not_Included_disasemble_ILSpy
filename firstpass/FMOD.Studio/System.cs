using System;
using System.Runtime.InteropServices;

namespace FMOD.Studio
{
	public struct System
	{
		public IntPtr handle;

		public static RESULT create(out System studiosystem)
		{
			return FMOD_Studio_System_Create(out studiosystem.handle, 69637u);
		}

		public RESULT setAdvancedSettings(ADVANCEDSETTINGS settings)
		{
			settings.cbsize = Marshal.SizeOf(typeof(ADVANCEDSETTINGS));
			return FMOD_Studio_System_SetAdvancedSettings(handle, ref settings);
		}

		public RESULT getAdvancedSettings(out ADVANCEDSETTINGS settings)
		{
			settings.cbsize = Marshal.SizeOf(typeof(ADVANCEDSETTINGS));
			return FMOD_Studio_System_GetAdvancedSettings(handle, out settings);
		}

		public RESULT initialize(int maxchannels, INITFLAGS studioFlags, FMOD.INITFLAGS flags, IntPtr extradriverdata)
		{
			return FMOD_Studio_System_Initialize(handle, maxchannels, studioFlags, flags, extradriverdata);
		}

		public RESULT release()
		{
			return FMOD_Studio_System_Release(handle);
		}

		public RESULT update()
		{
			return FMOD_Studio_System_Update(handle);
		}

		public RESULT getLowLevelSystem(out FMOD.System system)
		{
			return FMOD_Studio_System_GetLowLevelSystem(handle, out system.handle);
		}

		public RESULT getEvent(string path, out EventDescription _event)
		{
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				return FMOD_Studio_System_GetEvent(handle, threadSafeEncoding.byteFromStringUTF8(path), out _event.handle);
			}
		}

		public RESULT getBus(string path, out Bus bus)
		{
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				return FMOD_Studio_System_GetBus(handle, threadSafeEncoding.byteFromStringUTF8(path), out bus.handle);
			}
		}

		public RESULT getVCA(string path, out VCA vca)
		{
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				return FMOD_Studio_System_GetVCA(handle, threadSafeEncoding.byteFromStringUTF8(path), out vca.handle);
			}
		}

		public RESULT getBank(string path, out Bank bank)
		{
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				return FMOD_Studio_System_GetBank(handle, threadSafeEncoding.byteFromStringUTF8(path), out bank.handle);
			}
		}

		public RESULT getEventByID(Guid guid, out EventDescription _event)
		{
			return FMOD_Studio_System_GetEventByID(handle, ref guid, out _event.handle);
		}

		public RESULT getBusByID(Guid guid, out Bus bus)
		{
			return FMOD_Studio_System_GetBusByID(handle, ref guid, out bus.handle);
		}

		public RESULT getVCAByID(Guid guid, out VCA vca)
		{
			return FMOD_Studio_System_GetVCAByID(handle, ref guid, out vca.handle);
		}

		public RESULT getBankByID(Guid guid, out Bank bank)
		{
			return FMOD_Studio_System_GetBankByID(handle, ref guid, out bank.handle);
		}

		public RESULT getSoundInfo(string key, out SOUND_INFO info)
		{
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				return FMOD_Studio_System_GetSoundInfo(handle, threadSafeEncoding.byteFromStringUTF8(key), out info);
			}
		}

		public RESULT lookupID(string path, out Guid guid)
		{
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				return FMOD_Studio_System_LookupID(handle, threadSafeEncoding.byteFromStringUTF8(path), out guid);
			}
		}

		public RESULT lookupPath(Guid guid, out string path)
		{
			path = null;
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				IntPtr intPtr = Marshal.AllocHGlobal(256);
				int retrieved = 0;
				RESULT rESULT = FMOD_Studio_System_LookupPath(handle, ref guid, intPtr, 256, out retrieved);
				if (rESULT == RESULT.ERR_TRUNCATED)
				{
					Marshal.FreeHGlobal(intPtr);
					intPtr = Marshal.AllocHGlobal(retrieved);
					rESULT = FMOD_Studio_System_LookupPath(handle, ref guid, intPtr, retrieved, out retrieved);
				}
				if (rESULT == RESULT.OK)
				{
					path = threadSafeEncoding.stringFromNative(intPtr);
				}
				Marshal.FreeHGlobal(intPtr);
				return rESULT;
			}
		}

		public RESULT getNumListeners(out int numlisteners)
		{
			return FMOD_Studio_System_GetNumListeners(handle, out numlisteners);
		}

		public RESULT setNumListeners(int numlisteners)
		{
			return FMOD_Studio_System_SetNumListeners(handle, numlisteners);
		}

		public RESULT getListenerAttributes(int listener, out ATTRIBUTES_3D attributes)
		{
			return FMOD_Studio_System_GetListenerAttributes(handle, listener, out attributes);
		}

		public RESULT setListenerAttributes(int listener, ATTRIBUTES_3D attributes)
		{
			return FMOD_Studio_System_SetListenerAttributes(handle, listener, ref attributes);
		}

		public RESULT getListenerWeight(int listener, out float weight)
		{
			return FMOD_Studio_System_GetListenerWeight(handle, listener, out weight);
		}

		public RESULT setListenerWeight(int listener, float weight)
		{
			return FMOD_Studio_System_SetListenerWeight(handle, listener, weight);
		}

		public RESULT loadBankFile(string name, LOAD_BANK_FLAGS flags, out Bank bank)
		{
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				return FMOD_Studio_System_LoadBankFile(handle, threadSafeEncoding.byteFromStringUTF8(name), flags, out bank.handle);
			}
		}

		public RESULT loadBankMemory(byte[] buffer, LOAD_BANK_FLAGS flags, out Bank bank)
		{
			GCHandle gCHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			IntPtr buffer2 = gCHandle.AddrOfPinnedObject();
			RESULT result = FMOD_Studio_System_LoadBankMemory(handle, buffer2, buffer.Length, LOAD_MEMORY_MODE.LOAD_MEMORY, flags, out bank.handle);
			gCHandle.Free();
			return result;
		}

		public RESULT loadBankCustom(BANK_INFO info, LOAD_BANK_FLAGS flags, out Bank bank)
		{
			info.size = Marshal.SizeOf(info);
			return FMOD_Studio_System_LoadBankCustom(handle, ref info, flags, out bank.handle);
		}

		public RESULT unloadAll()
		{
			return FMOD_Studio_System_UnloadAll(handle);
		}

		public RESULT flushCommands()
		{
			return FMOD_Studio_System_FlushCommands(handle);
		}

		public RESULT flushSampleLoading()
		{
			return FMOD_Studio_System_FlushSampleLoading(handle);
		}

		public RESULT startCommandCapture(string path, COMMANDCAPTURE_FLAGS flags)
		{
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				return FMOD_Studio_System_StartCommandCapture(handle, threadSafeEncoding.byteFromStringUTF8(path), flags);
			}
		}

		public RESULT stopCommandCapture()
		{
			return FMOD_Studio_System_StopCommandCapture(handle);
		}

		public RESULT loadCommandReplay(string path, COMMANDREPLAY_FLAGS flags, out CommandReplay replay)
		{
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				return FMOD_Studio_System_LoadCommandReplay(handle, threadSafeEncoding.byteFromStringUTF8(path), flags, out replay.handle);
			}
		}

		public RESULT getBankCount(out int count)
		{
			return FMOD_Studio_System_GetBankCount(handle, out count);
		}

		public RESULT getBankList(out Bank[] array)
		{
			array = null;
			RESULT rESULT = FMOD_Studio_System_GetBankCount(handle, out int count);
			if (rESULT == RESULT.OK)
			{
				if (count != 0)
				{
					IntPtr[] array2 = new IntPtr[count];
					rESULT = FMOD_Studio_System_GetBankList(handle, array2, count, out int count2);
					if (rESULT == RESULT.OK)
					{
						if (count2 > count)
						{
							count2 = count;
						}
						array = new Bank[count2];
						for (int i = 0; i < count2; i++)
						{
							array[i].handle = array2[i];
						}
						return RESULT.OK;
					}
					return rESULT;
				}
				array = new Bank[0];
				return rESULT;
			}
			return rESULT;
		}

		public RESULT getCPUUsage(out CPU_USAGE usage)
		{
			return FMOD_Studio_System_GetCPUUsage(handle, out usage);
		}

		public RESULT getBufferUsage(out BUFFER_USAGE usage)
		{
			return FMOD_Studio_System_GetBufferUsage(handle, out usage);
		}

		public RESULT resetBufferUsage()
		{
			return FMOD_Studio_System_ResetBufferUsage(handle);
		}

		public RESULT setCallback(SYSTEM_CALLBACK callback, SYSTEM_CALLBACK_TYPE callbackmask = SYSTEM_CALLBACK_TYPE.ALL)
		{
			return FMOD_Studio_System_SetCallback(handle, callback, callbackmask);
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return FMOD_Studio_System_GetUserData(handle, out userdata);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return FMOD_Studio_System_SetUserData(handle, userdata);
		}

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_Create(out IntPtr studiosystem, uint headerversion);

		[DllImport("fmodstudio")]
		private static extern bool FMOD_Studio_System_IsValid(IntPtr studiosystem);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_SetAdvancedSettings(IntPtr studiosystem, ref ADVANCEDSETTINGS settings);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetAdvancedSettings(IntPtr studiosystem, out ADVANCEDSETTINGS settings);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_Initialize(IntPtr studiosystem, int maxchannels, INITFLAGS studioFlags, FMOD.INITFLAGS flags, IntPtr extradriverdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_Release(IntPtr studiosystem);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_Update(IntPtr studiosystem);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetLowLevelSystem(IntPtr studiosystem, out IntPtr system);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetEvent(IntPtr studiosystem, byte[] path, out IntPtr description);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBus(IntPtr studiosystem, byte[] path, out IntPtr bus);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetVCA(IntPtr studiosystem, byte[] path, out IntPtr vca);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBank(IntPtr studiosystem, byte[] path, out IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetEventByID(IntPtr studiosystem, ref Guid guid, out IntPtr description);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBusByID(IntPtr studiosystem, ref Guid guid, out IntPtr bus);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetVCAByID(IntPtr studiosystem, ref Guid guid, out IntPtr vca);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBankByID(IntPtr studiosystem, ref Guid guid, out IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetSoundInfo(IntPtr studiosystem, byte[] key, out SOUND_INFO info);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_LookupID(IntPtr studiosystem, byte[] path, out Guid guid);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_LookupPath(IntPtr studiosystem, ref Guid guid, IntPtr path, int size, out int retrieved);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetNumListeners(IntPtr studiosystem, out int numlisteners);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_SetNumListeners(IntPtr studiosystem, int numlisteners);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetListenerAttributes(IntPtr studiosystem, int listener, out ATTRIBUTES_3D attributes);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_SetListenerAttributes(IntPtr studiosystem, int listener, ref ATTRIBUTES_3D attributes);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetListenerWeight(IntPtr studiosystem, int listener, out float weight);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_SetListenerWeight(IntPtr studiosystem, int listener, float weight);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_LoadBankFile(IntPtr studiosystem, byte[] filename, LOAD_BANK_FLAGS flags, out IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_LoadBankMemory(IntPtr studiosystem, IntPtr buffer, int length, LOAD_MEMORY_MODE mode, LOAD_BANK_FLAGS flags, out IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_LoadBankCustom(IntPtr studiosystem, ref BANK_INFO info, LOAD_BANK_FLAGS flags, out IntPtr bank);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_UnloadAll(IntPtr studiosystem);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_FlushCommands(IntPtr studiosystem);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_FlushSampleLoading(IntPtr studiosystem);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_StartCommandCapture(IntPtr studiosystem, byte[] path, COMMANDCAPTURE_FLAGS flags);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_StopCommandCapture(IntPtr studiosystem);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_LoadCommandReplay(IntPtr studiosystem, byte[] path, COMMANDREPLAY_FLAGS flags, out IntPtr commandReplay);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBankCount(IntPtr studiosystem, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBankList(IntPtr studiosystem, IntPtr[] array, int capacity, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetCPUUsage(IntPtr studiosystem, out CPU_USAGE usage);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetBufferUsage(IntPtr studiosystem, out BUFFER_USAGE usage);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_ResetBufferUsage(IntPtr studiosystem);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_SetCallback(IntPtr studiosystem, SYSTEM_CALLBACK callback, SYSTEM_CALLBACK_TYPE callbackmask);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_GetUserData(IntPtr studiosystem, out IntPtr userdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_System_SetUserData(IntPtr studiosystem, IntPtr userdata);

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
			return hasHandle() && FMOD_Studio_System_IsValid(handle);
		}
	}
}
