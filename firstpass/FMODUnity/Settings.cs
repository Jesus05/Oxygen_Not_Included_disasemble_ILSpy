using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FMODUnity
{
	public class Settings : ScriptableObject
	{
		private const string SettingsAssetName = "FMODStudioSettings";

		private static Settings instance;

		[SerializeField]
		public bool HasSourceProject = true;

		[SerializeField]
		public bool HasPlatforms = true;

		[SerializeField]
		private string sourceProjectPath;

		[SerializeField]
		public string SourceProjectPathUnformatted;

		private string sourceBankPath;

		[SerializeField]
		public string SourceBankPathUnformatted;

		[SerializeField]
		public bool AutomaticEventLoading;

		[SerializeField]
		public bool AutomaticSampleLoading;

		[SerializeField]
		public ImportType ImportType;

		[SerializeField]
		public string TargetAssetPath;

		[SerializeField]
		public List<PlatformIntSetting> SpeakerModeSettings;

		[SerializeField]
		public List<PlatformIntSetting> SampleRateSettings;

		[SerializeField]
		public List<PlatformBoolSetting> LiveUpdateSettings;

		[SerializeField]
		public List<PlatformBoolSetting> OverlaySettings;

		[SerializeField]
		public List<PlatformBoolSetting> LoggingSettings;

		[SerializeField]
		public List<PlatformStringSetting> BankDirectorySettings;

		[SerializeField]
		public List<PlatformIntSetting> VirtualChannelSettings;

		[SerializeField]
		public List<PlatformIntSetting> RealChannelSettings;

		[SerializeField]
		public List<string> Plugins = new List<string>();

		[SerializeField]
		public string MasterBank;

		[SerializeField]
		public List<string> Banks;

		public static Settings Instance
		{
			get
			{
				if ((UnityEngine.Object)instance == (UnityEngine.Object)null)
				{
					instance = (Resources.Load("FMODStudioSettings") as Settings);
					if ((UnityEngine.Object)instance == (UnityEngine.Object)null)
					{
						UnityEngine.Debug.Log("FMOD Studio: cannot find integration settings, creating default settings");
						instance = ScriptableObject.CreateInstance<Settings>();
						instance.name = "FMOD Studio Integration Settings";
					}
				}
				return instance;
			}
		}

		public string SourceProjectPath
		{
			get
			{
				if (string.IsNullOrEmpty(sourceProjectPath) && !string.IsNullOrEmpty(SourceProjectPathUnformatted))
				{
					sourceProjectPath = GetPlatformSpecificPath(SourceProjectPathUnformatted);
				}
				return sourceProjectPath;
			}
			set
			{
				sourceProjectPath = GetPlatformSpecificPath(value);
			}
		}

		public string SourceBankPath
		{
			get
			{
				if (string.IsNullOrEmpty(sourceBankPath) && !string.IsNullOrEmpty(SourceBankPathUnformatted))
				{
					sourceBankPath = GetPlatformSpecificPath(SourceBankPathUnformatted);
				}
				return sourceBankPath;
			}
			set
			{
				sourceBankPath = GetPlatformSpecificPath(value);
			}
		}

		private Settings()
		{
			Banks = new List<string>();
			RealChannelSettings = new List<PlatformIntSetting>();
			VirtualChannelSettings = new List<PlatformIntSetting>();
			LoggingSettings = new List<PlatformBoolSetting>();
			LiveUpdateSettings = new List<PlatformBoolSetting>();
			OverlaySettings = new List<PlatformBoolSetting>();
			SampleRateSettings = new List<PlatformIntSetting>();
			SpeakerModeSettings = new List<PlatformIntSetting>();
			BankDirectorySettings = new List<PlatformStringSetting>();
			SetSetting(LoggingSettings, FMODPlatform.PlayInEditor, TriStateBool.Enabled);
			SetSetting(LiveUpdateSettings, FMODPlatform.PlayInEditor, TriStateBool.Enabled);
			SetSetting(OverlaySettings, FMODPlatform.PlayInEditor, TriStateBool.Enabled);
			SetSetting(RealChannelSettings, FMODPlatform.PlayInEditor, 256);
			SetSetting(VirtualChannelSettings, FMODPlatform.PlayInEditor, 1024);
			SetSetting(LoggingSettings, FMODPlatform.Default, TriStateBool.Disabled);
			SetSetting(LiveUpdateSettings, FMODPlatform.Default, TriStateBool.Disabled);
			SetSetting(OverlaySettings, FMODPlatform.Default, TriStateBool.Disabled);
			SetSetting(RealChannelSettings, FMODPlatform.Default, 32);
			SetSetting(VirtualChannelSettings, FMODPlatform.Default, 128);
			SetSetting(SampleRateSettings, FMODPlatform.Default, 0);
			SetSetting(SpeakerModeSettings, FMODPlatform.Default, 3);
			ImportType = ImportType.StreamingAssets;
			AutomaticEventLoading = true;
			AutomaticSampleLoading = false;
			TargetAssetPath = string.Empty;
		}

		public static FMODPlatform GetParent(FMODPlatform platform)
		{
			switch (platform)
			{
			case FMODPlatform.Windows:
			case FMODPlatform.Mac:
			case FMODPlatform.Linux:
			case FMODPlatform.UWP:
				return FMODPlatform.Desktop;
			case FMODPlatform.MobileHigh:
			case FMODPlatform.MobileLow:
			case FMODPlatform.iOS:
			case FMODPlatform.Android:
			case FMODPlatform.WindowsPhone:
			case FMODPlatform.PSVita:
			case FMODPlatform.AppleTV:
			case FMODPlatform.Switch:
				return FMODPlatform.Mobile;
			case FMODPlatform.XboxOne:
			case FMODPlatform.PS4:
			case FMODPlatform.WiiU:
				return FMODPlatform.Console;
			case FMODPlatform.Desktop:
			case FMODPlatform.Mobile:
			case FMODPlatform.Console:
				return FMODPlatform.Default;
			default:
				return FMODPlatform.None;
			}
		}

		public static bool HasSetting<T>(List<T> list, FMODPlatform platform) where T : PlatformSettingBase
		{
			return list.Exists((Predicate<T>)((T x) => x.Platform == platform));
		}

		public static U GetSetting<T, U>(List<T> list, FMODPlatform platform, U def) where T : PlatformSetting<U>
		{
			T val = list.Find((Predicate<T>)((T x) => x.Platform == platform));
			if (val == null)
			{
				FMODPlatform parent = GetParent(platform);
				if (parent != 0)
				{
					return GetSetting(list, parent, def);
				}
				return def;
			}
			return ((PlatformSetting<U>)val).Value;
		}

		public static void SetSetting<T, U>(List<T> list, FMODPlatform platform, U value) where T : PlatformSetting<U>, new()
		{
			T val = list.Find((Predicate<T>)((T x) => x.Platform == platform));
			if (val == null)
			{
				val = new T();
				val.Platform = platform;
				list.Add(val);
			}
			((PlatformSetting<U>)val).Value = value;
		}

		public static void RemoveSetting<T>(List<T> list, FMODPlatform platform) where T : PlatformSettingBase
		{
			list.RemoveAll((Predicate<T>)((T x) => x.Platform == platform));
		}

		public bool IsLiveUpdateEnabled(FMODPlatform platform)
		{
			return GetSetting(LiveUpdateSettings, platform, TriStateBool.Disabled) == TriStateBool.Enabled;
		}

		public bool IsOverlayEnabled(FMODPlatform platform)
		{
			return GetSetting(OverlaySettings, platform, TriStateBool.Disabled) == TriStateBool.Enabled;
		}

		public int GetRealChannels(FMODPlatform platform)
		{
			return GetSetting(RealChannelSettings, platform, 64);
		}

		public int GetVirtualChannels(FMODPlatform platform)
		{
			return GetSetting(VirtualChannelSettings, platform, 128);
		}

		public int GetSpeakerMode(FMODPlatform platform)
		{
			return GetSetting(SpeakerModeSettings, platform, 3);
		}

		public int GetSampleRate(FMODPlatform platform)
		{
			return GetSetting(SampleRateSettings, platform, 48000);
		}

		public string GetBankPlatform(FMODPlatform platform)
		{
			if (!HasPlatforms)
			{
				return string.Empty;
			}
			return GetSetting(BankDirectorySettings, platform, "Desktop");
		}

		private string GetPlatformSpecificPath(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return path;
			}
			if (Path.DirectorySeparatorChar == '/')
			{
				return path.Replace('\\', '/');
			}
			return path.Replace('/', '\\');
		}
	}
}
