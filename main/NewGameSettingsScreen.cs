using FMOD.Studio;
using Klei.CustomSettings;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewGameSettingsScreen : KModalScreen
{
	[Header("Static UI Refs")]
	[SerializeField]
	private MultiToggle toggle_standard_game;

	[SerializeField]
	private MultiToggle toggle_custom_game;

	[SerializeField]
	private KButton button_cancel;

	[SerializeField]
	private KButton button_start;

	[SerializeField]
	private KButton button_close;

	[SerializeField]
	private GameObject disable_custom_settings_shroud;

	[SerializeField]
	private Transform content;

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

	private CustomGameSettings.CustomGameMode baseGameMode;

	private const int MAX_VALID_SEED = int.MaxValue;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		WorldGen.LoadSettings();
		CustomGameSettings.Instance.LoadWorlds();
		MultiToggle multiToggle = toggle_standard_game;
		multiToggle.onClick = (System.Action)Delegate.Combine(multiToggle.onClick, (System.Action)delegate
		{
			SetGameTypeToggle(false);
		});
		MultiToggle multiToggle2 = toggle_custom_game;
		multiToggle2.onClick = (System.Action)Delegate.Combine(multiToggle2.onClick, (System.Action)delegate
		{
			SetGameTypeToggle(true);
		});
		button_start.onClick += delegate
		{
			NewGame();
		};
		button_cancel.onClick += delegate
		{
			Deactivate();
		};
		button_close.onClick += delegate
		{
			Deactivate();
		};
		settings = CustomGameSettings.Instance;
		baseGameMode = settings.customGameMode;
		SetGameTypeToggle(false);
		Color color = new Color(0.95f, 0.95f, 1f, 1f);
		bool flag = true;
		foreach (KeyValuePair<string, SettingConfig> qualitySetting in settings.QualitySettings)
		{
			if (!qualitySetting.Value.debug_only || DebugHandler.enabled)
			{
				flag = !flag;
				ListSettingConfig list_setting = qualitySetting.Value as ListSettingConfig;
				if (list_setting != null)
				{
					GameObject gameObject = Util.KInstantiateUI(prefab_cycle_setting, content.gameObject, true);
					HierarchyReferences refs = gameObject.GetComponent<HierarchyReferences>();
					refs.GetReference<Image>("BG").color = ((!flag) ? Color.white : color);
					refs.GetReference<LocText>("Label").text = qualitySetting.Value.label;
					refs.GetReference<LocText>("Label").GetComponent<ToolTip>().toolTip = qualitySetting.Value.tooltip;
					refs.GetReference<KButton>("CycleLeft").onClick += delegate
					{
						CycleSetting(list_setting, refs, -1);
					};
					refs.GetReference<KButton>("CycleRight").onClick += delegate
					{
						CycleSetting(list_setting, refs, 1);
					};
					CycleSetting(list_setting, refs, 0);
				}
				else
				{
					ToggleSettingConfig toggle_setting = qualitySetting.Value as ToggleSettingConfig;
					if (toggle_setting != null)
					{
						GameObject gameObject2 = Util.KInstantiateUI(prefab_checkbox_setting, content.gameObject, true);
						HierarchyReferences refs2 = gameObject2.GetComponent<HierarchyReferences>();
						refs2.GetReference<Image>("BG").color = ((!flag) ? Color.white : color);
						refs2.GetReference<LocText>("Label").text = qualitySetting.Value.label;
						refs2.GetReference<LocText>("Label").GetComponent<ToolTip>().toolTip = qualitySetting.Value.tooltip;
						MultiToggle reference = refs2.GetReference<MultiToggle>("Toggle");
						reference.onClick = (System.Action)Delegate.Combine(reference.onClick, (System.Action)delegate
						{
							ToggleSetting(toggle_setting, refs2, false);
						});
						ToggleSetting(toggle_setting, refs2, true);
					}
					else
					{
						SeedSettingConfig seed_setting = qualitySetting.Value as SeedSettingConfig;
						if (seed_setting != null)
						{
							GameObject gameObject3 = Util.KInstantiateUI(prefab_seed_input_setting, content.gameObject, true);
							HierarchyReferences refs3 = gameObject3.GetComponent<HierarchyReferences>();
							TMP_InputField input = gameObject3.GetComponentInChildren<TMP_InputField>(true);
							TMP_InputField tMP_InputField = input;
							tMP_InputField.onValidateInput = (TMP_InputField.OnValidateInput)Delegate.Combine(tMP_InputField.onValidateInput, (TMP_InputField.OnValidateInput)((string text, int charIndxex, char addedChar) => ('0' <= addedChar && addedChar <= '9') ? addedChar : '\0'));
							input.onEndEdit.AddListener(delegate(string text)
							{
								int a;
								try
								{
									a = Convert.ToInt32(text);
								}
								catch
								{
									a = 0;
								}
								a = Mathf.Min(a, 2147483647);
								input.text = a.ToString();
							});
							input.onValueChanged.AddListener(delegate(string text)
							{
								int num = 0;
								try
								{
									num = Convert.ToInt32(text);
								}
								catch
								{
									if (text.Length > 0)
									{
										input.text = text.Substring(0, text.Length - 1);
									}
									else
									{
										input.text = "";
									}
								}
								if (num > 2147483647)
								{
									input.text = text.Substring(0, text.Length - 1);
								}
							});
							refs3.GetReference<Image>("BG").color = ((!flag) ? Color.white : color);
							refs3.GetReference<LocText>("Label").text = qualitySetting.Value.label;
							refs3.GetReference<LocText>("Label").GetComponent<ToolTip>().toolTip = qualitySetting.Value.tooltip;
							refs3.GetReference<TMP_InputField>("Input").onEndEdit.AddListener(delegate(string s)
							{
								SetSeedSetting(seed_setting, refs3, s);
							});
							refs3.GetReference<KButton>("Randomize").onClick += delegate
							{
								GetNewRandomSeed(seed_setting, refs3);
							};
							GetNewRandomSeed(seed_setting, refs3);
						}
					}
				}
			}
		}
	}

	private void CycleSetting(ListSettingConfig setting, HierarchyReferences refs, int direction)
	{
		SettingLevel settingLevel = settings.CycleSettingLevel(setting, direction);
		refs.GetReference<LocText>("ValueLabel").text = settingLevel.label;
		refs.GetReference<LocText>("ValueLabel").GetComponent<ToolTip>().toolTip = settingLevel.tooltip;
		refs.GetReference<KButton>("CycleLeft").isInteractable = !setting.IsFirstLevel(settingLevel.id);
		refs.GetReference<KButton>("CycleRight").isInteractable = !setting.IsLastLevel(settingLevel.id);
	}

	private void ToggleSetting(ToggleSettingConfig setting, HierarchyReferences refs, bool just_update_widgets = false)
	{
		SettingLevel settingLevel = (!just_update_widgets) ? settings.ToggleSettingLevel(setting) : settings.GetCurrentQualitySetting(setting.id);
		refs.GetReference<MultiToggle>("Toggle").ChangeState(setting.IsOnLevel(settingLevel.id) ? 1 : 0);
		refs.GetReference<MultiToggle>("Toggle").GetComponent<ToolTip>().toolTip = settingLevel.tooltip;
	}

	private void SetSeedSetting(SeedSettingConfig setting, HierarchyReferences refs, string input)
	{
		settings.SetQualitySetting(setting, input);
		int seed;
		try
		{
			seed = Convert.ToInt32(input);
		}
		catch
		{
			seed = 0;
		}
		OfflineWorldGen.SetSeed(seed);
		Output.Log("Set worldgen seed to", input);
	}

	private void GetNewRandomSeed(SeedSettingConfig setting, HierarchyReferences refs)
	{
		int num = UnityEngine.Random.Range(0, 2147483647);
		refs.GetReference<TMP_InputField>("Input").text = num.ToString();
		SetSeedSetting(setting, refs, num.ToString());
	}

	private void SetGameTypeToggle(bool custom_game)
	{
		settings.customGameMode = ((!custom_game) ? baseGameMode : CustomGameSettings.CustomGameMode.Custom);
		bool flag = settings.customGameMode == CustomGameSettings.CustomGameMode.Custom;
		toggle_standard_game.ChangeState((!flag) ? 1 : 0);
		toggle_custom_game.ChangeState(flag ? 1 : 0);
		disable_custom_settings_shroud.SetActive(!flag);
		KPlayerPrefs.SetInt(OfflineWorldGen.USE_WORLD_SEED_KEY, flag ? 1 : 0);
	}

	private void NewGame()
	{
		SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.World);
		if (currentQualitySetting.userdata != null)
		{
			ModInfo info = (ModInfo)currentQualitySetting.userdata;
			Global.Instance.modManager.ActivateWorldGenMod(info);
		}
		TriggerLoadingMusic();
		SaveLoader.SetActiveSaveFilePath(null);
		try
		{
			File.Delete(WorldGen.SIM_SAVE_FILENAME);
		}
		catch (Exception ex)
		{
			Output.LogWarning(ex.ToString());
		}
		Util.KInstantiateUI(ScreenPrefabs.Instance.WorldGenScreen.gameObject, base.transform.parent.gameObject, true);
		UnityEngine.Object.FindObjectOfType<FrontEndBackground>().gameObject.SetActive(false);
		Deactivate();
	}

	private void TriggerLoadingMusic()
	{
		if (AudioDebug.Get().musicEnabled && !MusicManager.instance.SongIsPlaying("Music_FrontEnd"))
		{
			MusicManager.instance.StopSong("Music_TitleTheme", true, STOP_MODE.ALLOWFADEOUT);
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndSnapshot, STOP_MODE.ALLOWFADEOUT);
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().FrontEndWorldGenerationSnapshot);
			MusicManager.instance.PlaySong("Music_FrontEnd", false);
			MusicManager.instance.SetSongParameter("Music_FrontEnd", "songSection", 1f, true);
		}
	}
}
