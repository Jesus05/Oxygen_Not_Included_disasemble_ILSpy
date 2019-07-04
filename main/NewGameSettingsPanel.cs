using Klei.CustomSettings;
using KMod;
using ProcGen;
using ProcGenGame;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NewGameSettingsPanel : KMonoBehaviour
{
	[SerializeField]
	private Transform content;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton background;

	[Header("Prefab UI Refs")]
	[SerializeField]
	private GameObject prefab_cycle_setting;

	[SerializeField]
	private GameObject prefab_slider_setting;

	[SerializeField]
	private GameObject prefab_checkbox_setting;

	[SerializeField]
	private GameObject prefab_seed_input_setting;

	private CustomGameSettings settings;

	private List<NewGameSettingWidget> widgets;

	public void SetCloseAction(System.Action onClose)
	{
		if ((UnityEngine.Object)closeButton != (UnityEngine.Object)null)
		{
			closeButton.onClick += onClose;
		}
		if ((UnityEngine.Object)background != (UnityEngine.Object)null)
		{
			background.onClick += onClose;
		}
	}

	public void Init()
	{
		Global.Instance.modManager.Load(Content.LayerableFiles);
		SettingsCache.Clear();
		WorldGen.LoadSettings();
		CustomGameSettings.Instance.LoadWorlds();
		Global.Instance.modManager.Report(base.gameObject);
		settings = CustomGameSettings.Instance;
		widgets = new List<NewGameSettingWidget>();
		foreach (KeyValuePair<string, SettingConfig> qualitySetting in settings.QualitySettings)
		{
			if (!qualitySetting.Value.debug_only || DebugHandler.enabled)
			{
				ListSettingConfig listSettingConfig = qualitySetting.Value as ListSettingConfig;
				if (listSettingConfig != null)
				{
					NewGameSettingList newGameSettingList = Util.KInstantiateUI<NewGameSettingList>(prefab_cycle_setting, content.gameObject, true);
					newGameSettingList.Initialize(listSettingConfig);
					widgets.Add(newGameSettingList);
				}
				else
				{
					ToggleSettingConfig toggleSettingConfig = qualitySetting.Value as ToggleSettingConfig;
					if (toggleSettingConfig != null)
					{
						NewGameSettingToggle newGameSettingToggle = Util.KInstantiateUI<NewGameSettingToggle>(prefab_checkbox_setting, content.gameObject, true);
						newGameSettingToggle.Initialize(toggleSettingConfig);
						widgets.Add(newGameSettingToggle);
					}
					else
					{
						SeedSettingConfig seedSettingConfig = qualitySetting.Value as SeedSettingConfig;
						if (seedSettingConfig != null)
						{
							NewGameSettingSeed newGameSettingSeed = Util.KInstantiateUI<NewGameSettingSeed>(prefab_seed_input_setting, content.gameObject, true);
							newGameSettingSeed.Initialize(seedSettingConfig);
							widgets.Add(newGameSettingSeed);
						}
					}
				}
			}
		}
		Refresh();
	}

	public void Refresh()
	{
		foreach (NewGameSettingWidget widget in widgets)
		{
			widget.Refresh();
		}
	}

	public void SetSetting(SettingConfig setting, string level)
	{
		settings.SetQualitySetting(setting, level);
	}

	public string GetSetting(SettingConfig setting)
	{
		return settings.GetCurrentQualitySetting(setting).id;
	}

	public void Cancel()
	{
		Global.Instance.modManager.Unload(Content.LayerableFiles);
		SettingsCache.Clear();
	}
}
