using Klei;
using Steamworks;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MainMenu : KMonoBehaviour
{
	private struct ButtonInfo
	{
		public LocString text;

		public System.Action action;

		public int fontSize;

		public ButtonInfo(LocString text, System.Action action, int font_size)
		{
			this.text = text;
			this.action = action;
			fontSize = font_size;
		}
	}

	private struct SaveFileEntry
	{
		public System.DateTime timeStamp;

		public SaveGame.Header header;

		public SaveGame.GameInfo headerData;
	}

	public RectTransform LogoAndMenu;

	public KButton Button_ResumeGame;

	public GameObject patchNotesScreen;

	public GameObject topLeftAlphaMessage;

	private float lastUpdateTime;

	private GameObject GameSettingsScreen;

	[SerializeField]
	private KButton buttonPrefab;

	[SerializeField]
	private GameObject buttonParent;

	private static bool HasAutoresumedOnce;

	private static int LANGUAGE_CONFIRMATION_VERSION = 2;

	private Dictionary<string, SaveFileEntry> saveFileEntries = new Dictionary<string, SaveFileEntry>();

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Global.Instance.modManager.DeactivateWorldGenMod();
		List<ButtonInfo> list = new List<ButtonInfo>();
		list.Add(new ButtonInfo(UI.FRONTEND.MAINMENU.NEWGAME, NewGame, 22));
		list.Add(new ButtonInfo(UI.FRONTEND.MAINMENU.LOADGAME, LoadGame, 14));
		list.Add(new ButtonInfo(UI.FRONTEND.MAINMENU.TRANSLATIONS, Translations, 14));
		list.Add(new ButtonInfo(UI.FRONTEND.MAINMENU.OPTIONS, Options, 14));
		list.Add(new ButtonInfo(UI.FRONTEND.MAINMENU.QUITTODESKTOP, QuitGame, 14));
		List<ButtonInfo> list2 = list;
		if (!DistributionPlatform.Initialized)
		{
			int num = list2.FindIndex((ButtonInfo x) => x.text == UI.FRONTEND.MAINMENU.TRANSLATIONS);
			if (num >= 0)
			{
				list2.RemoveAt(num);
			}
		}
		foreach (ButtonInfo item in list2)
		{
			ButtonInfo current = item;
			KButton kButton = Util.KInstantiateUI<KButton>(buttonPrefab.gameObject, buttonParent, true);
			kButton.onClick += current.action;
			LocText componentInChildren = kButton.GetComponentInChildren<LocText>();
			componentInChildren.text = current.text;
			componentInChildren.fontSize = (float)current.fontSize;
		}
		KCrashReporter.MOST_RECENT_SAVEFILE = null;
		RefreshResumeButton();
		Button_ResumeGame.onClick += ResumeGame;
		StartFEAudio();
		if (PatchNotesScreen.ShouldShowScreen())
		{
			patchNotesScreen.SetActive(true);
		}
		CheckDoubleBoundKeys();
		lastUpdateTime = Time.unscaledTime;
	}

	public void RefreshMainMenu()
	{
		RefreshResumeButton();
	}

	private void PlayMouseOverSound()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover", false));
	}

	private void PlayMouseClickSound()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Open", false));
	}

	protected override void OnSpawn()
	{
		Debug.Log("-- MAIN MENU -- ", null);
		base.OnSpawn();
		Canvas.ForceUpdateCanvases();
		ShowLanguageConfirmation();
		string savePrefix = SaveLoader.GetSavePrefix();
		try
		{
			string path = Path.Combine(savePrefix, "__SPCCHK");
			using (FileStream fileStream = File.OpenWrite(path))
			{
				byte[] array = new byte[1024];
				for (int i = 0; i < 15360; i++)
				{
					fileStream.Write(array, 0, array.Length);
				}
			}
			File.Delete(path);
		}
		catch (Exception ex)
		{
			string format = (!(ex is IOException)) ? string.Format(UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_READ_ONLY, savePrefix) : string.Format(UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_INSUFFICIENT_SPACE, savePrefix);
			string text = string.Format(format, savePrefix);
			ConfirmDialogScreen confirmDialogScreen = Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, true);
			confirmDialogScreen.PopupConfirmDialog(text, null, null, null, null, null, null, null, null);
		}
		if (GenericGameSettings.instance.autoResumeGame && !HasAutoresumedOnce)
		{
			HasAutoresumedOnce = true;
			ResumeGame();
		}
	}

	private void ShowLanguageConfirmation()
	{
		if (SteamManager.Initialized)
		{
			string steamUILanguage = SteamUtils.GetSteamUILanguage();
			if (!(steamUILanguage != "schinese") && KPlayerPrefs.GetInt("LanguageConfirmationVersion") < LANGUAGE_CONFIRMATION_VERSION)
			{
				KPlayerPrefs.SetInt("LanguageConfirmationVersion", LANGUAGE_CONFIRMATION_VERSION);
				Translations();
			}
		}
	}

	private void ResumeGame()
	{
		string latestSaveFile = SaveLoader.GetLatestSaveFile();
		if (!string.IsNullOrEmpty(latestSaveFile))
		{
			KCrashReporter.MOST_RECENT_SAVEFILE = latestSaveFile;
			SaveLoader.SetActiveSaveFilePath(latestSaveFile);
			LoadingOverlay.Load(delegate
			{
				App.LoadScene("backend");
			});
		}
	}

	private void NewGame()
	{
		GameSettingsScreen = Util.KInstantiateUI(ScreenPrefabs.Instance.ModeSelectScreen.gameObject, base.gameObject, true);
		GameSettingsScreen.GetComponent<KScreen>().Activate();
	}

	private void LoadGame()
	{
		if ((UnityEngine.Object)LoadScreen.Instance == (UnityEngine.Object)null)
		{
			GameObject gameObject = Util.KInstantiateUI(ScreenPrefabs.Instance.LoadScreen.gameObject, base.gameObject, true);
			LoadScreen component = gameObject.GetComponent<LoadScreen>();
			component.requireConfirmation = false;
			component.SetBackgroundActive(true);
		}
		LoadScreen.Instance.gameObject.SetActive(true);
	}

	private void Update()
	{
		if (Time.unscaledTime - lastUpdateTime > 1f)
		{
			RefreshMainMenu();
			lastUpdateTime = Time.unscaledTime;
		}
	}

	private void RefreshResumeButton()
	{
		string latestSaveFile = SaveLoader.GetLatestSaveFile();
		bool flag = !string.IsNullOrEmpty(latestSaveFile) && File.Exists(latestSaveFile);
		if (flag)
		{
			try
			{
				if (GenericGameSettings.instance.demoMode)
				{
					flag = false;
				}
				System.DateTime lastWriteTime = File.GetLastWriteTime(latestSaveFile);
				SaveFileEntry value = default(SaveFileEntry);
				SaveGame.Header header = default(SaveGame.Header);
				SaveGame.GameInfo gameInfo = default(SaveGame.GameInfo);
				if (!saveFileEntries.TryGetValue(latestSaveFile, out value) || value.timeStamp != lastWriteTime)
				{
					gameInfo = SaveLoader.LoadHeader(latestSaveFile, out header);
					SaveFileEntry saveFileEntry = default(SaveFileEntry);
					saveFileEntry.timeStamp = lastWriteTime;
					saveFileEntry.header = header;
					saveFileEntry.headerData = gameInfo;
					value = saveFileEntry;
					saveFileEntries[latestSaveFile] = value;
				}
				else
				{
					header = value.header;
					gameInfo = value.headerData;
				}
				if (header.buildVersion > 303707 || gameInfo.saveMajorVersion < 7)
				{
					flag = false;
				}
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(latestSaveFile);
				if (!string.IsNullOrEmpty(gameInfo.baseName))
				{
					Button_ResumeGame.GetComponentsInChildren<LocText>()[1].text = string.Format(UI.FRONTEND.MAINMENU.RESUMEBUTTON_BASENAME, gameInfo.baseName, gameInfo.numberOfCycles + 1);
				}
				else
				{
					Button_ResumeGame.GetComponentsInChildren<LocText>()[1].text = fileNameWithoutExtension;
				}
			}
			catch (Exception obj)
			{
				Debug.LogWarning(obj, null);
				flag = false;
			}
		}
		if ((UnityEngine.Object)Button_ResumeGame != (UnityEngine.Object)null && (UnityEngine.Object)Button_ResumeGame.gameObject != (UnityEngine.Object)null)
		{
			Button_ResumeGame.gameObject.SetActive(flag);
		}
		else
		{
			Debug.LogWarning("Why is the resume game button null?", null);
		}
	}

	private void Translations()
	{
		LanguageOptionsScreen languageOptionsScreen = Util.KInstantiateUI<LanguageOptionsScreen>(ScreenPrefabs.Instance.languageOptionsScreen.gameObject, base.transform.parent.gameObject, false);
		languageOptionsScreen.SetBackgroundActive(true);
	}

	private void Options()
	{
		OptionsMenuScreen optionsMenuScreen = Util.KInstantiateUI<OptionsMenuScreen>(ScreenPrefabs.Instance.OptionsScreen.gameObject, base.gameObject, true);
		optionsMenuScreen.SetBackgroundActive(true);
	}

	private void QuitGame()
	{
		if (!Application.isEditor)
		{
			Application.Quit();
		}
	}

	public void StartFEAudio()
	{
		AudioMixer.instance.Reset();
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().FrontEndSnapshot);
		if (!AudioMixer.instance.SnapshotIsActive(AudioMixerSnapshots.Get().UserVolumeSettingsSnapshot))
		{
			AudioMixer.instance.StartUserVolumesSnapshot();
		}
		if (AudioDebug.Get().musicEnabled && !MusicManager.instance.SongIsPlaying("Music_TitleTheme"))
		{
			MusicManager.instance.PlaySong("Music_TitleTheme", false);
		}
		CheckForAudioDriverIssue();
	}

	private void CheckForAudioDriverIssue()
	{
		if (!KFMOD.didFmodInitializeSuccessfully)
		{
			ConfirmDialogScreen confirmDialogScreen = Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, true);
			ConfirmDialogScreen confirmDialogScreen2 = confirmDialogScreen;
			string text = UI.FRONTEND.SUPPORTWARNINGS.AUDIO_DRIVERS;
			System.Action on_confirm = null;
			System.Action on_cancel = null;
			string configurable_text = UI.FRONTEND.SUPPORTWARNINGS.AUDIO_DRIVERS_MORE_INFO;
			System.Action on_configurable_clicked = delegate
			{
				Application.OpenURL("http://support.kleientertainment.com/customer/en/portal/articles/2947881-no-audio-when-playing-oxygen-not-included");
			};
			Sprite sadDupeAudio = GlobalResources.Instance().sadDupeAudio;
			confirmDialogScreen2.PopupConfirmDialog(text, on_confirm, on_cancel, configurable_text, on_configurable_clicked, null, null, null, sadDupeAudio);
		}
	}

	private void CheckDoubleBoundKeys()
	{
		string text = string.Empty;
		HashSet<BindingEntry> hashSet = new HashSet<BindingEntry>();
		for (int i = 0; i < GameInputMapping.KeyBindings.Length; i++)
		{
			if (GameInputMapping.KeyBindings[i].mKeyCode != KKeyCode.Mouse1)
			{
				for (int j = 0; j < GameInputMapping.KeyBindings.Length; j++)
				{
					if (i != j)
					{
						BindingEntry bindingEntry = GameInputMapping.KeyBindings[j];
						if (!hashSet.Contains(bindingEntry))
						{
							BindingEntry bindingEntry2 = GameInputMapping.KeyBindings[i];
							if (bindingEntry2.mKeyCode != 0 && bindingEntry2.mKeyCode == bindingEntry.mKeyCode && bindingEntry2.mModifier == bindingEntry.mModifier && bindingEntry2.mRebindable && bindingEntry.mRebindable)
							{
								string mGroup = GameInputMapping.KeyBindings[i].mGroup;
								string mGroup2 = GameInputMapping.KeyBindings[j].mGroup;
								if ((mGroup == "Root" || mGroup2 == "Root" || mGroup == mGroup2) && (!(mGroup == "Root") || !bindingEntry.mIgnoreRootConflics) && (!(mGroup2 == "Root") || !bindingEntry2.mIgnoreRootConflics))
								{
									string text2 = text;
									text = text2 + "\n\n" + bindingEntry2.mAction + ": <b>" + bindingEntry2.mKeyCode + "</b>\n" + bindingEntry.mAction + ": <b>" + bindingEntry.mKeyCode + "</b>";
									BindingEntry bindingEntry3 = bindingEntry2;
									bindingEntry3.mKeyCode = KKeyCode.None;
									bindingEntry3.mModifier = Modifier.None;
									GameInputMapping.KeyBindings[i] = bindingEntry3;
									bindingEntry3 = bindingEntry;
									bindingEntry3.mKeyCode = KKeyCode.None;
									bindingEntry3.mModifier = Modifier.None;
									GameInputMapping.KeyBindings[j] = bindingEntry3;
								}
							}
						}
					}
				}
				hashSet.Add(GameInputMapping.KeyBindings[i]);
			}
		}
		if (text != string.Empty)
		{
			ConfirmDialogScreen confirmDialogScreen = Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, true);
			ConfirmDialogScreen confirmDialogScreen2 = confirmDialogScreen;
			string text2 = string.Format(UI.FRONTEND.SUPPORTWARNINGS.DUPLICATE_KEY_BINDINGS, text);
			System.Action on_confirm = null;
			System.Action on_cancel = null;
			Sprite sadDupe = GlobalResources.Instance().sadDupe;
			confirmDialogScreen2.PopupConfirmDialog(text2, on_confirm, on_cancel, null, null, null, null, null, sadDupe);
		}
	}

	private void RestartGame()
	{
		App.instance.Restart();
	}
}
