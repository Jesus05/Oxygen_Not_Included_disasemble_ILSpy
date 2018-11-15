using Klei;
using Klei.CustomSettings;
using KSerialization;
using ProcGen;
using ProcGenGame;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class CustomGameSettings : KMonoBehaviour
{
	public enum CustomGameMode
	{
		Survival = 0,
		Nosweat = 1,
		Custom = 0xFF
	}

	public struct MetricSettingsData
	{
		public string Name;

		public string Value;
	}

	public const string TAG_WORLDGEN = "worldgen";

	private static CustomGameSettings instance;

	[Serialize]
	public bool is_custom_game;

	[Serialize]
	public CustomGameMode customGameMode;

	[Serialize]
	private Dictionary<string, string> CurrentQualityLevelsBySetting = new Dictionary<string, string>();

	public Dictionary<string, SettingConfig> QualitySettings = new Dictionary<string, SettingConfig>();

	public static CustomGameSettings Instance => instance;

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 6))
		{
			customGameMode = (is_custom_game ? CustomGameMode.Custom : CustomGameMode.Survival);
		}
	}

	protected override void OnPrefabInit()
	{
		instance = this;
		AddSettingConfig(CustomGameSettingConfigs.ImmuneSystem);
		AddSettingConfig(CustomGameSettingConfigs.Stress);
		AddSettingConfig(CustomGameSettingConfigs.StressBreaks);
		AddSettingConfig(CustomGameSettingConfigs.Morale);
		AddSettingConfig(CustomGameSettingConfigs.CalorieBurn);
		AddSettingConfig(CustomGameSettingConfigs.WorldgenSeed);
		AddSettingConfig(CustomGameSettingConfigs.SandboxMode);
		AddSettingConfig(CustomGameSettingConfigs.World);
	}

	public void SetSurvivalDefaults()
	{
		customGameMode = CustomGameMode.Survival;
		foreach (KeyValuePair<string, SettingConfig> qualitySetting in QualitySettings)
		{
			SetQualitySetting(qualitySetting.Value, qualitySetting.Value.default_level_id);
		}
	}

	public void SetNosweatDefaults()
	{
		customGameMode = CustomGameMode.Nosweat;
		foreach (KeyValuePair<string, SettingConfig> qualitySetting in QualitySettings)
		{
			SetQualitySetting(qualitySetting.Value, qualitySetting.Value.nosweat_default_level_id);
		}
	}

	public SettingLevel CycleSettingLevel(ListSettingConfig config, int direction)
	{
		CurrentQualityLevelsBySetting[config.id] = config.CycleSettingLevelID(CurrentQualityLevelsBySetting[config.id], direction);
		return config.GetLevel(CurrentQualityLevelsBySetting[config.id]);
	}

	public SettingLevel ToggleSettingLevel(ToggleSettingConfig config)
	{
		CurrentQualityLevelsBySetting[config.id] = config.ToggleSettingLevelID(CurrentQualityLevelsBySetting[config.id]);
		return config.GetLevel(CurrentQualityLevelsBySetting[config.id]);
	}

	public void SetQualitySetting(SettingConfig config, string value)
	{
		CurrentQualityLevelsBySetting[config.id] = value;
	}

	public SettingLevel GetCurrentQualitySetting(SettingConfig setting)
	{
		return GetCurrentQualitySetting(setting.id);
	}

	public SettingLevel GetCurrentQualitySetting(string setting_id)
	{
		if (customGameMode == CustomGameMode.Survival)
		{
			return QualitySettings[setting_id].GetLevel(QualitySettings[setting_id].default_level_id);
		}
		if (customGameMode == CustomGameMode.Nosweat)
		{
			return QualitySettings[setting_id].GetLevel(QualitySettings[setting_id].nosweat_default_level_id);
		}
		if (!CurrentQualityLevelsBySetting.ContainsKey(setting_id))
		{
			CurrentQualityLevelsBySetting[setting_id] = QualitySettings[setting_id].default_level_id;
		}
		string level_id = CurrentQualityLevelsBySetting[setting_id];
		return QualitySettings[setting_id].GetLevel(level_id);
	}

	public string GetSettingLevelLabel(string setting_id, string level_id)
	{
		SettingConfig settingConfig = QualitySettings[setting_id];
		if (settingConfig != null)
		{
			SettingLevel level = settingConfig.GetLevel(level_id);
			if (level != null)
			{
				return level.label;
			}
		}
		Debug.LogWarning("No label string for setting: " + setting_id + " level: " + level_id, null);
		return string.Empty;
	}

	public string GetSettingLevelTooltip(string setting_id, string level_id)
	{
		SettingConfig settingConfig = QualitySettings[setting_id];
		if (settingConfig != null)
		{
			SettingLevel level = settingConfig.GetLevel(level_id);
			if (level != null)
			{
				return level.tooltip;
			}
		}
		Debug.LogWarning("No tooltip string for setting: " + setting_id + " level: " + level_id, null);
		return string.Empty;
	}

	public void AddSettingConfig(SettingConfig config)
	{
		QualitySettings.Add(config.id, config);
		if (!CurrentQualityLevelsBySetting.ContainsKey(config.id) || string.IsNullOrEmpty(CurrentQualityLevelsBySetting[config.id]))
		{
			CurrentQualityLevelsBySetting[config.id] = config.default_level_id;
		}
	}

	public void LoadWorlds()
	{
		List<SettingLevel> levels = new List<SettingLevel>();
		AddLevels(Global.Instance.standardFS, null, levels);
		if (DistributionPlatform.Initialized)
		{
			List<SteamUGCService.Subscribed> subscribed = SteamUGCService.Instance.GetSubscribed("worldgen");
			foreach (SteamUGCService.Subscribed item in subscribed)
			{
				SteamUGC.GetItemInstallInfo(item.fileId, out ulong _, out string pchFolder, 1024u, out uint _);
				string path = WorldGen.GetPath();
				PublishedFileId_t fileId = item.fileId;
				string text = fileId.m_PublishedFileId.ToString();
				ModInfo modInfo = new ModInfo(ModInfo.Source.Steam, ModInfo.ModType.WorldGen, text, item.description, path, 0uL);
				FileStream zip_data_stream = File.OpenRead(pchFolder);
				ZipFileSystem fs = new ZipFileSystem(text, zip_data_stream, path);
				Global.Instance.layeredFileSystem.AddFileSystem(fs);
				AddLevels(fs, modInfo, levels);
				Global.Instance.layeredFileSystem.RemoveFileSystem(fs);
			}
		}
		CustomGameSettingConfigs.World.StompLevels(levels, "worlds/Default", "worlds/Default");
	}

	private void AddLevels(IFileSystem fs, object user_data, List<SettingLevel> levels)
	{
		string path = FSUtil.Normalize(System.IO.Path.Combine(WorldGen.GetPath(), "worlds"));
		List<string> list = new List<string>();
		FSUtil.GetFiles(fs, path, "*.yaml", list);
		foreach (string item in list)
		{
			ProcGen.World world = YamlIO<ProcGen.World>.LoadFile(item);
			string worldName = Worlds.GetWorldName(item);
			levels.Add(new SettingLevel(worldName, world.name, world.description, user_data));
		}
	}

	public void Print()
	{
		string text = "Custom Settings: ";
		foreach (KeyValuePair<string, string> item in CurrentQualityLevelsBySetting)
		{
			string text2 = text;
			text = text2 + item.Key + "=" + item.Value + ",";
		}
		Debug.Log(text, null);
	}

	private bool AllValuesMatch(Dictionary<string, string> data, CustomGameMode mode)
	{
		bool result = true;
		foreach (KeyValuePair<string, SettingConfig> qualitySetting in QualitySettings)
		{
			if (!(qualitySetting.Key == CustomGameSettingConfigs.WorldgenSeed.id))
			{
				string b = null;
				switch (mode)
				{
				case CustomGameMode.Nosweat:
					b = qualitySetting.Value.nosweat_default_level_id;
					break;
				case CustomGameMode.Survival:
					b = qualitySetting.Value.default_level_id;
					break;
				}
				if (data.ContainsKey(qualitySetting.Key) && data[qualitySetting.Key] != b)
				{
					return false;
				}
			}
		}
		return result;
	}

	public List<MetricSettingsData> GetSettingsForMetrics()
	{
		List<MetricSettingsData> list = new List<MetricSettingsData>();
		list.Add(new MetricSettingsData
		{
			Name = "CustomGameMode",
			Value = this.customGameMode.ToString()
		});
		foreach (KeyValuePair<string, string> item2 in CurrentQualityLevelsBySetting)
		{
			list.Add(new MetricSettingsData
			{
				Name = item2.Key,
				Value = item2.Value
			});
		}
		MetricSettingsData metricSettingsData = default(MetricSettingsData);
		metricSettingsData.Name = "CustomGameModeActual";
		metricSettingsData.Value = 255.ToString();
		MetricSettingsData item = metricSettingsData;
		IEnumerator enumerator2 = Enum.GetValues(typeof(CustomGameMode)).GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				CustomGameMode customGameMode = (CustomGameMode)enumerator2.Current;
				if (customGameMode != CustomGameMode.Custom && AllValuesMatch(CurrentQualityLevelsBySetting, customGameMode))
				{
					item.Value = customGameMode.ToString();
					break;
				}
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator2 as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		list.Add(item);
		return list;
	}
}
