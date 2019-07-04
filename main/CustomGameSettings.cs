using Klei;
using Klei.CustomSettings;
using KSerialization;
using ProcGen;
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

	private static CustomGameSettings instance;

	[Serialize]
	public bool is_custom_game = false;

	[Serialize]
	public CustomGameMode customGameMode = CustomGameMode.Survival;

	[Serialize]
	private Dictionary<string, string> CurrentQualityLevelsBySetting = new Dictionary<string, string>();

	public Dictionary<string, SettingConfig> QualitySettings = new Dictionary<string, SettingConfig>();

	public static CustomGameSettings Instance => instance;

	public event Action<SettingConfig, SettingLevel> OnSettingChanged;

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 6))
		{
			customGameMode = (is_custom_game ? CustomGameMode.Custom : CustomGameMode.Survival);
		}
		if (CurrentQualityLevelsBySetting.ContainsKey("CarePackages "))
		{
			CurrentQualityLevelsBySetting.Add(CustomGameSettingConfigs.CarePackages.id, CurrentQualityLevelsBySetting["CarePackages "]);
			CurrentQualityLevelsBySetting.Remove("CarePackages ");
		}
	}

	protected override void OnPrefabInit()
	{
		instance = this;
		AddSettingConfig(CustomGameSettingConfigs.World);
		AddSettingConfig(CustomGameSettingConfigs.WorldgenSeed);
		AddSettingConfig(CustomGameSettingConfigs.ImmuneSystem);
		AddSettingConfig(CustomGameSettingConfigs.CalorieBurn);
		AddSettingConfig(CustomGameSettingConfigs.Morale);
		AddSettingConfig(CustomGameSettingConfigs.Stress);
		AddSettingConfig(CustomGameSettingConfigs.StressBreaks);
		AddSettingConfig(CustomGameSettingConfigs.CarePackages);
		AddSettingConfig(CustomGameSettingConfigs.SandboxMode);
		VerifySettingCoordinates();
	}

	public void SetSurvivalDefaults()
	{
		customGameMode = CustomGameMode.Survival;
		LoadWorlds();
		foreach (KeyValuePair<string, SettingConfig> qualitySetting in QualitySettings)
		{
			SetQualitySetting(qualitySetting.Value, qualitySetting.Value.default_level_id);
		}
	}

	public void SetNosweatDefaults()
	{
		customGameMode = CustomGameMode.Nosweat;
		LoadWorlds();
		foreach (KeyValuePair<string, SettingConfig> qualitySetting in QualitySettings)
		{
			SetQualitySetting(qualitySetting.Value, qualitySetting.Value.nosweat_default_level_id);
		}
	}

	public SettingLevel CycleSettingLevel(ListSettingConfig config, int direction)
	{
		SetQualitySetting(config, config.CycleSettingLevelID(CurrentQualityLevelsBySetting[config.id], direction));
		return config.GetLevel(CurrentQualityLevelsBySetting[config.id]);
	}

	public SettingLevel ToggleSettingLevel(ToggleSettingConfig config)
	{
		SetQualitySetting(config, config.ToggleSettingLevelID(CurrentQualityLevelsBySetting[config.id]));
		return config.GetLevel(CurrentQualityLevelsBySetting[config.id]);
	}

	public void SetQualitySetting(SettingConfig config, string value)
	{
		CurrentQualityLevelsBySetting[config.id] = value;
		bool flag = true;
		bool flag2 = true;
		foreach (KeyValuePair<string, string> item in CurrentQualityLevelsBySetting)
		{
			if (QualitySettings[item.Key].triggers_custom_game)
			{
				if (item.Value != QualitySettings[item.Key].default_level_id)
				{
					flag = false;
				}
				if (item.Value != QualitySettings[item.Key].nosweat_default_level_id)
				{
					flag2 = false;
				}
				if (!flag && !flag2)
				{
					break;
				}
			}
		}
		CustomGameMode customGameMode = (!flag) ? (flag2 ? CustomGameMode.Nosweat : CustomGameMode.Custom) : CustomGameMode.Survival;
		if (customGameMode != this.customGameMode)
		{
			DebugUtil.LogArgs("Game mode changed from", this.customGameMode, "to", customGameMode);
			this.customGameMode = customGameMode;
		}
		if (this.OnSettingChanged != null)
		{
			this.OnSettingChanged(config, GetCurrentQualitySetting(config));
		}
	}

	public SettingLevel GetCurrentQualitySetting(SettingConfig setting)
	{
		return GetCurrentQualitySetting(setting.id);
	}

	public SettingLevel GetCurrentQualitySetting(string setting_id)
	{
		SettingConfig settingConfig = QualitySettings[setting_id];
		if (customGameMode != 0)
		{
			if (customGameMode != CustomGameMode.Nosweat)
			{
				if (!CurrentQualityLevelsBySetting.ContainsKey(setting_id))
				{
					CurrentQualityLevelsBySetting[setting_id] = QualitySettings[setting_id].default_level_id;
				}
				string level_id = CurrentQualityLevelsBySetting[setting_id];
				return QualitySettings[setting_id].GetLevel(level_id);
			}
			return (!settingConfig.triggers_custom_game) ? settingConfig.GetLevel(CurrentQualityLevelsBySetting[setting_id]) : settingConfig.GetLevel(settingConfig.nosweat_default_level_id);
		}
		return (!settingConfig.triggers_custom_game) ? settingConfig.GetLevel(CurrentQualityLevelsBySetting[setting_id]) : settingConfig.GetLevel(settingConfig.default_level_id);
	}

	public string GetCurrentQualitySettingLevelId(SettingConfig config)
	{
		return CurrentQualityLevelsBySetting[config.id];
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
		Debug.LogWarning("No label string for setting: " + setting_id + " level: " + level_id);
		return "";
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
		Debug.LogWarning("No tooltip string for setting: " + setting_id + " level: " + level_id);
		return "";
	}

	public void AddSettingConfig(SettingConfig config)
	{
		QualitySettings.Add(config.id, config);
		if (!CurrentQualityLevelsBySetting.ContainsKey(config.id) || string.IsNullOrEmpty(CurrentQualityLevelsBySetting[config.id]))
		{
			CurrentQualityLevelsBySetting[config.id] = config.default_level_id;
		}
	}

	private static void AddWorldMods(object user_data, List<SettingLevel> levels)
	{
		string path = FileSystem.Normalize(System.IO.Path.Combine(SettingsCache.GetPath(), "worlds"));
		ListPool<string, CustomGameSettings>.PooledList pooledList = ListPool<string, CustomGameSettings>.Allocate();
		FileSystem.GetFiles(path, "*.yaml", pooledList);
		foreach (string item in pooledList)
		{
			ProcGen.World world = YamlIO.LoadFile<ProcGen.World>(item, null, null);
			string worldName = Worlds.GetWorldName(item);
			string id = worldName;
			string name = world.name;
			string description = world.description;
			levels.Add(new SettingLevel(id, name, description, 0, user_data));
		}
		pooledList.Recycle();
	}

	public void LoadWorlds()
	{
		Dictionary<string, ProcGen.World> worldCache = SettingsCache.worlds.worldCache;
		List<SettingLevel> list = new List<SettingLevel>(worldCache.Count);
		foreach (KeyValuePair<string, ProcGen.World> item in worldCache)
		{
			StringEntry result;
			string label = (!Strings.TryGet(new StringKey(item.Value.name), out result)) ? item.Value.name : result.ToString();
			string tooltip = (!Strings.TryGet(new StringKey(item.Value.description), out result)) ? item.Value.description : result.ToString();
			list.Add(new SettingLevel(item.Key, label, tooltip, 0, null));
		}
		CustomGameSettingConfigs.World.StompLevels(list, "worlds/SandstoneDefault", "worlds/SandstoneDefault");
	}

	public void Print()
	{
		string text = "Custom Settings: ";
		foreach (KeyValuePair<string, string> item in CurrentQualityLevelsBySetting)
		{
			string text2 = text;
			text = text2 + item.Key + "=" + item.Value + ",";
		}
		Debug.Log(text);
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
					result = false;
					break;
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

	public bool VerifySettingCoordinates()
	{
		Dictionary<int, string> dictionary = new Dictionary<int, string>();
		bool result = false;
		foreach (KeyValuePair<string, SettingConfig> qualitySetting in QualitySettings)
		{
			if (qualitySetting.Value.coordinate_dimension < 0 || qualitySetting.Value.coordinate_dimension_width < 0)
			{
				if (qualitySetting.Value.coordinate_dimension >= 0 || qualitySetting.Value.coordinate_dimension_width >= 0)
				{
					result = true;
					Debug.Assert(false, qualitySetting.Value.id + ": Both coordinate dimension props must be unset (-1) if either is unset.");
				}
			}
			else
			{
				List<SettingLevel> levels = qualitySetting.Value.GetLevels();
				if (qualitySetting.Value.coordinate_dimension_width < levels.Count)
				{
					result = true;
					Debug.Assert(false, qualitySetting.Value.id + ": Range between coordinate min and max insufficient for all levels (" + qualitySetting.Value.coordinate_dimension_width + "<" + levels.Count + ")");
				}
				foreach (SettingLevel item in levels)
				{
					int key = qualitySetting.Value.coordinate_dimension * item.coordinate_offset;
					string text = qualitySetting.Value.id + " > " + item.id;
					if (item.coordinate_offset < 0)
					{
						result = true;
						Debug.Assert(false, text + ": Level coordinate offset must be >= 0");
					}
					else if (item.coordinate_offset == 0)
					{
						if (item.id != qualitySetting.Value.default_level_id)
						{
							result = true;
							Debug.Assert(false, text + ": Only the default level should have a coordinate offset of 0");
						}
					}
					else if (item.coordinate_offset > qualitySetting.Value.coordinate_dimension_width)
					{
						result = true;
						Debug.Assert(false, text + ": level coordinate must be <= dimension width");
					}
					else
					{
						string value;
						bool flag = !dictionary.TryGetValue(key, out value);
						dictionary[key] = text;
						if (item.id == qualitySetting.Value.default_level_id)
						{
							result = true;
							Debug.Assert(false, text + ": Default level must be coordinate 0");
						}
						if (!flag)
						{
							result = true;
							Debug.Assert(false, text + ": Combined coordinate conflicts with another coordinate (" + value + "). Ensure this SettingConfig's min and max don't overlap with another SettingConfig's");
						}
					}
				}
			}
		}
		return result;
	}

	public string GetSettingsCoordinate()
	{
		ProcGen.World worldData = SettingsCache.worlds.GetWorldData(Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.World).id);
		SettingLevel currentQualitySetting = Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed);
		string otherSettingsCode = GetOtherSettingsCode();
		return $"{worldData.GetCoordinatePrefix()}-{currentQualitySetting.id}-{otherSettingsCode}";
	}

	private string GetOtherSettingsCode()
	{
		int num = 0;
		foreach (KeyValuePair<string, string> item in CurrentQualityLevelsBySetting)
		{
			SettingConfig settingConfig = QualitySettings[item.Key];
			if (settingConfig.coordinate_dimension >= 0 && settingConfig.coordinate_dimension_width >= 0)
			{
				SettingLevel level = settingConfig.GetLevel(item.Value);
				int num2 = settingConfig.coordinate_dimension * level.coordinate_offset;
				num += num2;
			}
		}
		return Base10toBase36(num);
	}

	private string Base10toBase36(int input)
	{
		if (input != 0)
		{
			int num = input;
			string text = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
			string text2 = "";
			while (num > 0)
			{
				text2 += text[num % 36];
				num /= 36;
			}
			return text2;
		}
		return "0";
	}
}
