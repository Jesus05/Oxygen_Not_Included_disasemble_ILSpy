using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public struct System
	{
		public IntPtr handle;

		public RESULT release()
		{
			return FMOD5_System_Release(handle);
		}

		public RESULT setOutput(OUTPUTTYPE output)
		{
			return FMOD5_System_SetOutput(handle, output);
		}

		public RESULT getOutput(out OUTPUTTYPE output)
		{
			return FMOD5_System_GetOutput(handle, out output);
		}

		public RESULT getNumDrivers(out int numdrivers)
		{
			return FMOD5_System_GetNumDrivers(handle, out numdrivers);
		}

		public RESULT getDriverInfo(int id, out string name, int namelen, out Guid guid, out int systemrate, out SPEAKERMODE speakermode, out int speakermodechannels)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(namelen);
			RESULT result = FMOD5_System_GetDriverInfo(handle, id, intPtr, namelen, out guid, out systemrate, out speakermode, out speakermodechannels);
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				name = threadSafeEncoding.stringFromNative(intPtr);
			}
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public RESULT getDriverInfo(int id, out Guid guid, out int systemrate, out SPEAKERMODE speakermode, out int speakermodechannels)
		{
			return FMOD5_System_GetDriverInfo(handle, id, IntPtr.Zero, 0, out guid, out systemrate, out speakermode, out speakermodechannels);
		}

		public RESULT setDriver(int driver)
		{
			return FMOD5_System_SetDriver(handle, driver);
		}

		public RESULT getDriver(out int driver)
		{
			return FMOD5_System_GetDriver(handle, out driver);
		}

		public RESULT setSoftwareChannels(int numsoftwarechannels)
		{
			return FMOD5_System_SetSoftwareChannels(handle, numsoftwarechannels);
		}

		public RESULT getSoftwareChannels(out int numsoftwarechannels)
		{
			return FMOD5_System_GetSoftwareChannels(handle, out numsoftwarechannels);
		}

		public RESULT setSoftwareFormat(int samplerate, SPEAKERMODE speakermode, int numrawspeakers)
		{
			return FMOD5_System_SetSoftwareFormat(handle, samplerate, speakermode, numrawspeakers);
		}

		public RESULT getSoftwareFormat(out int samplerate, out SPEAKERMODE speakermode, out int numrawspeakers)
		{
			return FMOD5_System_GetSoftwareFormat(handle, out samplerate, out speakermode, out numrawspeakers);
		}

		public RESULT setDSPBufferSize(uint bufferlength, int numbuffers)
		{
			return FMOD5_System_SetDSPBufferSize(handle, bufferlength, numbuffers);
		}

		public RESULT getDSPBufferSize(out uint bufferlength, out int numbuffers)
		{
			return FMOD5_System_GetDSPBufferSize(handle, out bufferlength, out numbuffers);
		}

		public RESULT setFileSystem(FILE_OPENCALLBACK useropen, FILE_CLOSECALLBACK userclose, FILE_READCALLBACK userread, FILE_SEEKCALLBACK userseek, FILE_ASYNCREADCALLBACK userasyncread, FILE_ASYNCCANCELCALLBACK userasynccancel, int blockalign)
		{
			return FMOD5_System_SetFileSystem(handle, useropen, userclose, userread, userseek, userasyncread, userasynccancel, blockalign);
		}

		public RESULT attachFileSystem(FILE_OPENCALLBACK useropen, FILE_CLOSECALLBACK userclose, FILE_READCALLBACK userread, FILE_SEEKCALLBACK userseek)
		{
			return FMOD5_System_AttachFileSystem(handle, useropen, userclose, userread, userseek);
		}

		public RESULT setAdvancedSettings(ref ADVANCEDSETTINGS settings)
		{
			settings.cbSize = Marshal.SizeOf(settings);
			return FMOD5_System_SetAdvancedSettings(handle, ref settings);
		}

		public RESULT getAdvancedSettings(ref ADVANCEDSETTINGS settings)
		{
			settings.cbSize = Marshal.SizeOf(settings);
			return FMOD5_System_GetAdvancedSettings(handle, ref settings);
		}

		public RESULT setCallback(SYSTEM_CALLBACK callback, SYSTEM_CALLBACK_TYPE callbackmask)
		{
			return FMOD5_System_SetCallback(handle, callback, callbackmask);
		}

		public RESULT setPluginPath(string path)
		{
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				return FMOD5_System_SetPluginPath(handle, threadSafeEncoding.byteFromStringUTF8(path));
			}
		}

		public RESULT loadPlugin(string filename, out uint handle, uint priority)
		{
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				return FMOD5_System_LoadPlugin(this.handle, threadSafeEncoding.byteFromStringUTF8(filename), out handle, priority);
			}
		}

		public RESULT loadPlugin(string filename, out uint handle)
		{
			return loadPlugin(filename, out handle, 0u);
		}

		public RESULT unloadPlugin(uint handle)
		{
			return FMOD5_System_UnloadPlugin(this.handle, handle);
		}

		public RESULT getNumNestedPlugins(uint handle, out int count)
		{
			return FMOD5_System_GetNumNestedPlugins(this.handle, handle, out count);
		}

		public RESULT getNestedPlugin(uint handle, int index, out uint nestedhandle)
		{
			return FMOD5_System_GetNestedPlugin(this.handle, handle, index, out nestedhandle);
		}

		public RESULT getNumPlugins(PLUGINTYPE plugintype, out int numplugins)
		{
			return FMOD5_System_GetNumPlugins(handle, plugintype, out numplugins);
		}

		public RESULT getPluginHandle(PLUGINTYPE plugintype, int index, out uint handle)
		{
			return FMOD5_System_GetPluginHandle(this.handle, plugintype, index, out handle);
		}

		public RESULT getPluginInfo(uint handle, out PLUGINTYPE plugintype, out string name, int namelen, out uint version)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(namelen);
			RESULT result = FMOD5_System_GetPluginInfo(this.handle, handle, out plugintype, intPtr, namelen, out version);
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				name = threadSafeEncoding.stringFromNative(intPtr);
			}
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public RESULT getPluginInfo(uint handle, out PLUGINTYPE plugintype, out uint version)
		{
			return FMOD5_System_GetPluginInfo(this.handle, handle, out plugintype, IntPtr.Zero, 0, out version);
		}

		public RESULT setOutputByPlugin(uint handle)
		{
			return FMOD5_System_SetOutputByPlugin(this.handle, handle);
		}

		public RESULT getOutputByPlugin(out uint handle)
		{
			return FMOD5_System_GetOutputByPlugin(this.handle, out handle);
		}

		public RESULT createDSPByPlugin(uint handle, out DSP dsp)
		{
			return FMOD5_System_CreateDSPByPlugin(this.handle, handle, out dsp.handle);
		}

		public RESULT getDSPInfoByPlugin(uint handle, out IntPtr description)
		{
			return FMOD5_System_GetDSPInfoByPlugin(this.handle, handle, out description);
		}

		public RESULT registerDSP(ref DSP_DESCRIPTION description, out uint handle)
		{
			return FMOD5_System_RegisterDSP(this.handle, ref description, out handle);
		}

		public RESULT init(int maxchannels, INITFLAGS flags, IntPtr extradriverdata)
		{
			return FMOD5_System_Init(handle, maxchannels, flags, extradriverdata);
		}

		public RESULT close()
		{
			return FMOD5_System_Close(handle);
		}

		public RESULT update()
		{
			return FMOD5_System_Update(handle);
		}

		public RESULT setSpeakerPosition(SPEAKER speaker, float x, float y, bool active)
		{
			return FMOD5_System_SetSpeakerPosition(handle, speaker, x, y, active);
		}

		public RESULT getSpeakerPosition(SPEAKER speaker, out float x, out float y, out bool active)
		{
			return FMOD5_System_GetSpeakerPosition(handle, speaker, out x, out y, out active);
		}

		public RESULT setStreamBufferSize(uint filebuffersize, TIMEUNIT filebuffersizetype)
		{
			return FMOD5_System_SetStreamBufferSize(handle, filebuffersize, filebuffersizetype);
		}

		public RESULT getStreamBufferSize(out uint filebuffersize, out TIMEUNIT filebuffersizetype)
		{
			return FMOD5_System_GetStreamBufferSize(handle, out filebuffersize, out filebuffersizetype);
		}

		public RESULT set3DSettings(float dopplerscale, float distancefactor, float rolloffscale)
		{
			return FMOD5_System_Set3DSettings(handle, dopplerscale, distancefactor, rolloffscale);
		}

		public RESULT get3DSettings(out float dopplerscale, out float distancefactor, out float rolloffscale)
		{
			return FMOD5_System_Get3DSettings(handle, out dopplerscale, out distancefactor, out rolloffscale);
		}

		public RESULT set3DNumListeners(int numlisteners)
		{
			return FMOD5_System_Set3DNumListeners(handle, numlisteners);
		}

		public RESULT get3DNumListeners(out int numlisteners)
		{
			return FMOD5_System_Get3DNumListeners(handle, out numlisteners);
		}

		public RESULT set3DListenerAttributes(int listener, ref VECTOR pos, ref VECTOR vel, ref VECTOR forward, ref VECTOR up)
		{
			return FMOD5_System_Set3DListenerAttributes(handle, listener, ref pos, ref vel, ref forward, ref up);
		}

		public RESULT get3DListenerAttributes(int listener, out VECTOR pos, out VECTOR vel, out VECTOR forward, out VECTOR up)
		{
			return FMOD5_System_Get3DListenerAttributes(handle, listener, out pos, out vel, out forward, out up);
		}

		public RESULT set3DRolloffCallback(CB_3D_ROLLOFFCALLBACK callback)
		{
			return FMOD5_System_Set3DRolloffCallback(handle, callback);
		}

		public RESULT mixerSuspend()
		{
			return FMOD5_System_MixerSuspend(handle);
		}

		public RESULT mixerResume()
		{
			return FMOD5_System_MixerResume(handle);
		}

		public RESULT getDefaultMixMatrix(SPEAKERMODE sourcespeakermode, SPEAKERMODE targetspeakermode, float[] matrix, int matrixhop)
		{
			return FMOD5_System_GetDefaultMixMatrix(handle, sourcespeakermode, targetspeakermode, matrix, matrixhop);
		}

		public RESULT getSpeakerModeChannels(SPEAKERMODE mode, out int channels)
		{
			return FMOD5_System_GetSpeakerModeChannels(handle, mode, out channels);
		}

		public RESULT getVersion(out uint version)
		{
			return FMOD5_System_GetVersion(handle, out version);
		}

		public RESULT getOutputHandle(out IntPtr handle)
		{
			return FMOD5_System_GetOutputHandle(this.handle, out handle);
		}

		public RESULT getChannelsPlaying(out int channels, out int realchannels)
		{
			return FMOD5_System_GetChannelsPlaying(handle, out channels, out realchannels);
		}

		public RESULT getCPUUsage(out float dsp, out float stream, out float geometry, out float update, out float total)
		{
			return FMOD5_System_GetCPUUsage(handle, out dsp, out stream, out geometry, out update, out total);
		}

		public RESULT getFileUsage(out long sampleBytesRead, out long streamBytesRead, out long otherBytesRead)
		{
			return FMOD5_System_GetFileUsage(handle, out sampleBytesRead, out streamBytesRead, out otherBytesRead);
		}

		public RESULT getSoundRAM(out int currentalloced, out int maxalloced, out int total)
		{
			return FMOD5_System_GetSoundRAM(handle, out currentalloced, out maxalloced, out total);
		}

		public RESULT createSound(string name, MODE mode, ref CREATESOUNDEXINFO exinfo, out Sound sound)
		{
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				return FMOD5_System_CreateSound(handle, threadSafeEncoding.byteFromStringUTF8(name), mode, ref exinfo, out sound.handle);
			}
		}

		public RESULT createSound(byte[] data, MODE mode, ref CREATESOUNDEXINFO exinfo, out Sound sound)
		{
			return FMOD5_System_CreateSound(handle, data, mode, ref exinfo, out sound.handle);
		}

		public RESULT createSound(IntPtr name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, out Sound sound)
		{
			return FMOD5_System_CreateSound(handle, name_or_data, mode, ref exinfo, out sound.handle);
		}

		public RESULT createSound(string name, MODE mode, out Sound sound)
		{
			CREATESOUNDEXINFO exinfo = default(CREATESOUNDEXINFO);
			exinfo.cbsize = Marshal.SizeOf(exinfo);
			return createSound(name, mode, ref exinfo, out sound);
		}

		public RESULT createStream(string name, MODE mode, ref CREATESOUNDEXINFO exinfo, out Sound sound)
		{
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				return FMOD5_System_CreateStream(handle, threadSafeEncoding.byteFromStringUTF8(name), mode, ref exinfo, out sound.handle);
			}
		}

		public RESULT createStream(byte[] data, MODE mode, ref CREATESOUNDEXINFO exinfo, out Sound sound)
		{
			return FMOD5_System_CreateStream(handle, data, mode, ref exinfo, out sound.handle);
		}

		public RESULT createStream(IntPtr name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, out Sound sound)
		{
			return FMOD5_System_CreateStream(handle, name_or_data, mode, ref exinfo, out sound.handle);
		}

		public RESULT createStream(string name, MODE mode, out Sound sound)
		{
			CREATESOUNDEXINFO exinfo = default(CREATESOUNDEXINFO);
			exinfo.cbsize = Marshal.SizeOf(exinfo);
			return createStream(name, mode, ref exinfo, out sound);
		}

		public RESULT createDSP(ref DSP_DESCRIPTION description, out DSP dsp)
		{
			return FMOD5_System_CreateDSP(handle, ref description, out dsp.handle);
		}

		public RESULT createDSPByType(DSP_TYPE type, out DSP dsp)
		{
			return FMOD5_System_CreateDSPByType(handle, type, out dsp.handle);
		}

		public RESULT createChannelGroup(string name, out ChannelGroup channelgroup)
		{
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				return FMOD5_System_CreateChannelGroup(handle, threadSafeEncoding.byteFromStringUTF8(name), out channelgroup.handle);
			}
		}

		public RESULT createSoundGroup(string name, out SoundGroup soundgroup)
		{
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				return FMOD5_System_CreateSoundGroup(handle, threadSafeEncoding.byteFromStringUTF8(name), out soundgroup.handle);
			}
		}

		public RESULT createReverb3D(out Reverb3D reverb)
		{
			return FMOD5_System_CreateReverb3D(handle, out reverb.handle);
		}

		public RESULT playSound(Sound sound, ChannelGroup channelGroup, bool paused, out Channel channel)
		{
			return FMOD5_System_PlaySound(handle, sound.handle, channelGroup.handle, paused, out channel.handle);
		}

		public RESULT playDSP(DSP dsp, ChannelGroup channelGroup, bool paused, out Channel channel)
		{
			return FMOD5_System_PlayDSP(handle, dsp.handle, channelGroup.handle, paused, out channel.handle);
		}

		public RESULT getChannel(int channelid, out Channel channel)
		{
			return FMOD5_System_GetChannel(handle, channelid, out channel.handle);
		}

		public RESULT getMasterChannelGroup(out ChannelGroup channelgroup)
		{
			return FMOD5_System_GetMasterChannelGroup(handle, out channelgroup.handle);
		}

		public RESULT getMasterSoundGroup(out SoundGroup soundgroup)
		{
			return FMOD5_System_GetMasterSoundGroup(handle, out soundgroup.handle);
		}

		public RESULT attachChannelGroupToPort(uint portType, ulong portIndex, ChannelGroup channelgroup, bool passThru = false)
		{
			return FMOD5_System_AttachChannelGroupToPort(handle, portType, portIndex, channelgroup.handle, passThru);
		}

		public RESULT detachChannelGroupFromPort(ChannelGroup channelgroup)
		{
			return FMOD5_System_DetachChannelGroupFromPort(handle, channelgroup.handle);
		}

		public RESULT setReverbProperties(int instance, ref REVERB_PROPERTIES prop)
		{
			return FMOD5_System_SetReverbProperties(handle, instance, ref prop);
		}

		public RESULT getReverbProperties(int instance, out REVERB_PROPERTIES prop)
		{
			return FMOD5_System_GetReverbProperties(handle, instance, out prop);
		}

		public RESULT lockDSP()
		{
			return FMOD5_System_LockDSP(handle);
		}

		public RESULT unlockDSP()
		{
			return FMOD5_System_UnlockDSP(handle);
		}

		public RESULT getRecordNumDrivers(out int numdrivers, out int numconnected)
		{
			return FMOD5_System_GetRecordNumDrivers(handle, out numdrivers, out numconnected);
		}

		public RESULT getRecordDriverInfo(int id, out string name, int namelen, out Guid guid, out int systemrate, out SPEAKERMODE speakermode, out int speakermodechannels, out DRIVER_STATE state)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(namelen);
			RESULT result = FMOD5_System_GetRecordDriverInfo(handle, id, intPtr, namelen, out guid, out systemrate, out speakermode, out speakermodechannels, out state);
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				name = threadSafeEncoding.stringFromNative(intPtr);
			}
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public RESULT getRecordDriverInfo(int id, out Guid guid, out int systemrate, out SPEAKERMODE speakermode, out int speakermodechannels, out DRIVER_STATE state)
		{
			return FMOD5_System_GetRecordDriverInfo(handle, id, IntPtr.Zero, 0, out guid, out systemrate, out speakermode, out speakermodechannels, out state);
		}

		public RESULT getRecordPosition(int id, out uint position)
		{
			return FMOD5_System_GetRecordPosition(handle, id, out position);
		}

		public RESULT recordStart(int id, Sound sound, bool loop)
		{
			return FMOD5_System_RecordStart(handle, id, sound.handle, loop);
		}

		public RESULT recordStop(int id)
		{
			return FMOD5_System_RecordStop(handle, id);
		}

		public RESULT isRecording(int id, out bool recording)
		{
			return FMOD5_System_IsRecording(handle, id, out recording);
		}

		public RESULT createGeometry(int maxpolygons, int maxvertices, out Geometry geometry)
		{
			return FMOD5_System_CreateGeometry(handle, maxpolygons, maxvertices, out geometry.handle);
		}

		public RESULT setGeometrySettings(float maxworldsize)
		{
			return FMOD5_System_SetGeometrySettings(handle, maxworldsize);
		}

		public RESULT getGeometrySettings(out float maxworldsize)
		{
			return FMOD5_System_GetGeometrySettings(handle, out maxworldsize);
		}

		public RESULT loadGeometry(IntPtr data, int datasize, out Geometry geometry)
		{
			return FMOD5_System_LoadGeometry(handle, data, datasize, out geometry.handle);
		}

		public RESULT getGeometryOcclusion(ref VECTOR listener, ref VECTOR source, out float direct, out float reverb)
		{
			return FMOD5_System_GetGeometryOcclusion(handle, ref listener, ref source, out direct, out reverb);
		}

		public RESULT setNetworkProxy(string proxy)
		{
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				return FMOD5_System_SetNetworkProxy(handle, threadSafeEncoding.byteFromStringUTF8(proxy));
			}
		}

		public RESULT getNetworkProxy(out string proxy, int proxylen)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(proxylen);
			RESULT result = FMOD5_System_GetNetworkProxy(handle, intPtr, proxylen);
			using (StringHelper.ThreadSafeEncoding threadSafeEncoding = StringHelper.GetFreeHelper())
			{
				proxy = threadSafeEncoding.stringFromNative(intPtr);
			}
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public RESULT setNetworkTimeout(int timeout)
		{
			return FMOD5_System_SetNetworkTimeout(handle, timeout);
		}

		public RESULT getNetworkTimeout(out int timeout)
		{
			return FMOD5_System_GetNetworkTimeout(handle, out timeout);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return FMOD5_System_SetUserData(handle, userdata);
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return FMOD5_System_GetUserData(handle, out userdata);
		}

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_Release(IntPtr system);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetOutput(IntPtr system, OUTPUTTYPE output);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetOutput(IntPtr system, out OUTPUTTYPE output);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetNumDrivers(IntPtr system, out int numdrivers);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetDriverInfo(IntPtr system, int id, IntPtr name, int namelen, out Guid guid, out int systemrate, out SPEAKERMODE speakermode, out int speakermodechannels);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetDriver(IntPtr system, int driver);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetDriver(IntPtr system, out int driver);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetSoftwareChannels(IntPtr system, int numsoftwarechannels);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetSoftwareChannels(IntPtr system, out int numsoftwarechannels);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetSoftwareFormat(IntPtr system, int samplerate, SPEAKERMODE speakermode, int numrawspeakers);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetSoftwareFormat(IntPtr system, out int samplerate, out SPEAKERMODE speakermode, out int numrawspeakers);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetDSPBufferSize(IntPtr system, uint bufferlength, int numbuffers);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetDSPBufferSize(IntPtr system, out uint bufferlength, out int numbuffers);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetFileSystem(IntPtr system, FILE_OPENCALLBACK useropen, FILE_CLOSECALLBACK userclose, FILE_READCALLBACK userread, FILE_SEEKCALLBACK userseek, FILE_ASYNCREADCALLBACK userasyncread, FILE_ASYNCCANCELCALLBACK userasynccancel, int blockalign);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_AttachFileSystem(IntPtr system, FILE_OPENCALLBACK useropen, FILE_CLOSECALLBACK userclose, FILE_READCALLBACK userread, FILE_SEEKCALLBACK userseek);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetAdvancedSettings(IntPtr system, ref ADVANCEDSETTINGS settings);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetAdvancedSettings(IntPtr system, ref ADVANCEDSETTINGS settings);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetCallback(IntPtr system, SYSTEM_CALLBACK callback, SYSTEM_CALLBACK_TYPE callbackmask);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetPluginPath(IntPtr system, byte[] path);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_LoadPlugin(IntPtr system, byte[] filename, out uint handle, uint priority);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_UnloadPlugin(IntPtr system, uint handle);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetNumNestedPlugins(IntPtr system, uint handle, out int count);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetNestedPlugin(IntPtr system, uint handle, int index, out uint nestedhandle);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetNumPlugins(IntPtr system, PLUGINTYPE plugintype, out int numplugins);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetPluginHandle(IntPtr system, PLUGINTYPE plugintype, int index, out uint handle);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetPluginInfo(IntPtr system, uint handle, out PLUGINTYPE plugintype, IntPtr name, int namelen, out uint version);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetOutputByPlugin(IntPtr system, uint handle);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetOutputByPlugin(IntPtr system, out uint handle);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_CreateDSPByPlugin(IntPtr system, uint handle, out IntPtr dsp);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetDSPInfoByPlugin(IntPtr system, uint handle, out IntPtr description);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_RegisterDSP(IntPtr system, ref DSP_DESCRIPTION description, out uint handle);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_Init(IntPtr system, int maxchannels, INITFLAGS flags, IntPtr extradriverdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_Close(IntPtr system);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_Update(IntPtr system);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetSpeakerPosition(IntPtr system, SPEAKER speaker, float x, float y, bool active);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetSpeakerPosition(IntPtr system, SPEAKER speaker, out float x, out float y, out bool active);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetStreamBufferSize(IntPtr system, uint filebuffersize, TIMEUNIT filebuffersizetype);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetStreamBufferSize(IntPtr system, out uint filebuffersize, out TIMEUNIT filebuffersizetype);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_Set3DSettings(IntPtr system, float dopplerscale, float distancefactor, float rolloffscale);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_Get3DSettings(IntPtr system, out float dopplerscale, out float distancefactor, out float rolloffscale);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_Set3DNumListeners(IntPtr system, int numlisteners);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_Get3DNumListeners(IntPtr system, out int numlisteners);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_Set3DListenerAttributes(IntPtr system, int listener, ref VECTOR pos, ref VECTOR vel, ref VECTOR forward, ref VECTOR up);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_Get3DListenerAttributes(IntPtr system, int listener, out VECTOR pos, out VECTOR vel, out VECTOR forward, out VECTOR up);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_Set3DRolloffCallback(IntPtr system, CB_3D_ROLLOFFCALLBACK callback);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_MixerSuspend(IntPtr system);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_MixerResume(IntPtr system);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetDefaultMixMatrix(IntPtr system, SPEAKERMODE sourcespeakermode, SPEAKERMODE targetspeakermode, float[] matrix, int matrixhop);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetSpeakerModeChannels(IntPtr system, SPEAKERMODE mode, out int channels);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetVersion(IntPtr system, out uint version);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetOutputHandle(IntPtr system, out IntPtr handle);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetChannelsPlaying(IntPtr system, out int channels, out int realchannels);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetCPUUsage(IntPtr system, out float dsp, out float stream, out float geometry, out float update, out float total);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetFileUsage(IntPtr system, out long sampleBytesRead, out long streamBytesRead, out long otherBytesRead);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetSoundRAM(IntPtr system, out int currentalloced, out int maxalloced, out int total);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_CreateSound(IntPtr system, byte[] name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, out IntPtr sound);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_CreateSound(IntPtr system, IntPtr name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, out IntPtr sound);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_CreateStream(IntPtr system, byte[] name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, out IntPtr sound);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_CreateStream(IntPtr system, IntPtr name_or_data, MODE mode, ref CREATESOUNDEXINFO exinfo, out IntPtr sound);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_CreateDSP(IntPtr system, ref DSP_DESCRIPTION description, out IntPtr dsp);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_CreateDSPByType(IntPtr system, DSP_TYPE type, out IntPtr dsp);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_CreateChannelGroup(IntPtr system, byte[] name, out IntPtr channelgroup);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_CreateSoundGroup(IntPtr system, byte[] name, out IntPtr soundgroup);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_CreateReverb3D(IntPtr system, out IntPtr reverb);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_PlaySound(IntPtr system, IntPtr sound, IntPtr channelGroup, bool paused, out IntPtr channel);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_PlayDSP(IntPtr system, IntPtr dsp, IntPtr channelGroup, bool paused, out IntPtr channel);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetChannel(IntPtr system, int channelid, out IntPtr channel);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetMasterChannelGroup(IntPtr system, out IntPtr channelgroup);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetMasterSoundGroup(IntPtr system, out IntPtr soundgroup);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_AttachChannelGroupToPort(IntPtr system, uint portType, ulong portIndex, IntPtr channelgroup, bool passThru);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_DetachChannelGroupFromPort(IntPtr system, IntPtr channelgroup);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetReverbProperties(IntPtr system, int instance, ref REVERB_PROPERTIES prop);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetReverbProperties(IntPtr system, int instance, out REVERB_PROPERTIES prop);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_LockDSP(IntPtr system);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_UnlockDSP(IntPtr system);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetRecordNumDrivers(IntPtr system, out int numdrivers, out int numconnected);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetRecordDriverInfo(IntPtr system, int id, IntPtr name, int namelen, out Guid guid, out int systemrate, out SPEAKERMODE speakermode, out int speakermodechannels, out DRIVER_STATE state);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetRecordPosition(IntPtr system, int id, out uint position);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_RecordStart(IntPtr system, int id, IntPtr sound, bool loop);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_RecordStop(IntPtr system, int id);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_IsRecording(IntPtr system, int id, out bool recording);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_CreateGeometry(IntPtr system, int maxpolygons, int maxvertices, out IntPtr geometry);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetGeometrySettings(IntPtr system, float maxworldsize);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetGeometrySettings(IntPtr system, out float maxworldsize);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_LoadGeometry(IntPtr system, IntPtr data, int datasize, out IntPtr geometry);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetGeometryOcclusion(IntPtr system, ref VECTOR listener, ref VECTOR source, out float direct, out float reverb);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetNetworkProxy(IntPtr system, byte[] proxy);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetNetworkProxy(IntPtr system, IntPtr proxy, int proxylen);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetNetworkTimeout(IntPtr system, int timeout);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetNetworkTimeout(IntPtr system, out int timeout);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_SetUserData(IntPtr system, IntPtr userdata);

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD5_System_GetUserData(IntPtr system, out IntPtr userdata);

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
