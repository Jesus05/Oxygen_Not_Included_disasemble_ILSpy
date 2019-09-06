using Klei;
using Steamworks;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : KScreen
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

	public GameObject topLeftAlphaMessage;

	private float lastUpdateTime;

	private MotdServerClient m_motdServerClient;

	private GameObject GameSettingsScreen;

	[SerializeField]
	private KButton buttonPrefab;

	[SerializeField]
	private GameObject buttonParent;

	[SerializeField]
	private LocText motdImageHeader;

	[SerializeField]
	private Button motdImageButton;

	[SerializeField]
	private Image motdImage;

	[SerializeField]
	private LocText motdNewsHeader;

	[SerializeField]
	private LocText motdNewsBody;

	[SerializeField]
	private PatchNotesScreen patchNotesScreen;

	[SerializeField]
	private NextUpdateTimer nextUpdateTimer;

	private static bool HasAutoresumedOnce;

	private bool refreshResumeButton = true;

	private static int LANGUAGE_CONFIRMATION_VERSION = 2;

	private Dictionary<string, SaveFileEntry> saveFileEntries = new Dictionary<string, SaveFileEntry>();

	private KButton MakeButton(ButtonInfo info)
	{
		KButton kButton = Util.KInstantiateUI<KButton>(buttonPrefab.gameObject, buttonParent, true);
		kButton.onClick += info.action;
		LocText componentInChildren = kButton.GetComponentInChildren<LocText>();
		componentInChildren.text = info.text;
		componentInChildren.fontSize = (float)info.fontSize;
		return kButton;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		MakeButton(new ButtonInfo(UI.FRONTEND.MAINMENU.NEWGAME, NewGame, 22));
		MakeButton(new ButtonInfo(UI.FRONTEND.MAINMENU.LOADGAME, LoadGame, 14));
		MakeButton(new ButtonInfo(UI.FRONTEND.MAINMENU.RETIREDCOLONIES, delegate
		{
			ActivateRetiredColoniesScreen(base.transform.gameObject, string.Empty, null);
		}, 14));
		if (DistributionPlatform.Initialized)
		{
			MakeButton(new ButtonInfo(UI.FRONTEND.MAINMENU.TRANSLATIONS, Translations, 14));
			MakeButton(new ButtonInfo(UI.FRONTEND.MODS.TITLE, Mods, 14));
		}
		MakeButton(new ButtonInfo(UI.FRONTEND.MAINMENU.OPTIONS, Options, 14));
		MakeButton(new ButtonInfo(UI.FRONTEND.MAINMENU.QUITTODESKTOP, QuitGame, 14));
		KCrashReporter.MOST_RECENT_SAVEFILE = null;
		RefreshResumeButton();
		Button_ResumeGame.onClick += ResumeGame;
		StartFEAudio();
		SpawnVideoScreen();
		CheckPlayerPrefsCorruption();
		if (PatchNotesScreen.ShouldShowScreen())
		{
			patchNotesScreen.gameObject.SetActive(true);
		}
		CheckDoubleBoundKeys();
		topLeftAlphaMessage.gameObject.SetActive(false);
		nextUpdateTimer.gameObject.SetActive(false);
		m_motdServerClient = new MotdServerClient();
		m_motdServerClient.GetMotd(delegate(MotdServerClient.MotdResponse response, string error)
		{
			MainMenu mainMenu = this;
			if (error == null)
			{
				topLeftAlphaMessage.gameObject.SetActive(true);
				nextUpdateTimer.gameObject.SetActive(true);
				motdImageHeader.text = response.image_header_text;
				motdNewsHeader.text = response.news_header_text;
				motdNewsBody.text = response.news_body_text;
				patchNotesScreen.UpdatePatchNotes(response.patch_notes_summary, response.patch_notes_link_url);
				nextUpdateTimer.UpdateReleaseTimes(response.last_update_time, response.next_update_time, response.update_text_override);
				if ((UnityEngine.Object)motdImage != (UnityEngine.Object)null && (UnityEngine.Object)response.image_texture != (UnityEngine.Object)null)
				{
					motdImage.sprite = Sprite.Create(response.image_texture, new Rect(0f, 0f, (float)response.image_texture.width, (float)response.image_texture.height), Vector2.zero);
					if (motdImage.sprite.rect.height != 0f)
					{
						AspectRatioFitter component = motdImage.gameObject.GetComponent<AspectRatioFitter>();
						if ((UnityEngine.Object)component != (UnityEngine.Object)null)
						{
							float num2 = component.aspectRatio = motdImage.sprite.rect.width / motdImage.sprite.rect.height;
						}
						else
						{
							Debug.LogWarning("Missing AspectRatioFitter on MainMenu motd image.");
						}
					}
					motdImageButton.onClick.AddListener(delegate
					{
						Application.OpenURL(response.image_link_url);
					});
				}
			}
			else
			{
				Debug.LogWarning("Motd Request error: " + error);
			}
		});
		lastUpdateTime = Time.unscaledTime;
		activateOnSpawn = true;
	}

	public void RefreshMainMenu()
	{
		if (refreshResumeButton)
		{
			RefreshResumeButton();
		}
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
		Debug.Log("-- MAIN MENU -- ");
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
			confirmDialogScreen.PopupConfirmDialog(text, null, null, null, null, null, null, null, null, true);
		}
		Global.Instance.modManager.Report(base.gameObject);
		if ((GenericGameSettings.instance.autoResumeGame && !HasAutoresumedOnce) || !string.IsNullOrEmpty(GenericGameSettings.instance.performanceCapture.saveGame))
		{
			HasAutoresumedOnce = true;
			ResumeGame();
		}
	}

	private void UnregisterMotdRequest()
	{
		if (m_motdServerClient != null)
		{
			m_motdServerClient.UnregisterCallback();
			m_motdServerClient = null;
		}
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		UnregisterMotdRequest();
	}

	public override void ScreenUpdate(bool topLevel)
	{
		refreshResumeButton = topLevel;
	}

	protected override void OnLoadLevel()
	{
		base.OnLoadLevel();
		UnregisterMotdRequest();
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
		string text = (!string.IsNullOrEmpty(GenericGameSettings.instance.performanceCapture.saveGame)) ? GenericGameSettings.instance.performanceCapture.saveGame : SaveLoader.GetLatestSaveFile();
		if (!string.IsNullOrEmpty(text))
		{
			KCrashReporter.MOST_RECENT_SAVEFILE = text;
			SaveLoader.SetActiveSaveFilePath(text);
			LoadingOverlay.Load(delegate
			{
				App.LoadScene("backend");
			});
		}
	}

	private void NewGame()
	{
		GetComponent<NewGameFlow>().BeginFlow();
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

	public static void ActivateRetiredColoniesScreen(GameObject parent, string colonyID = "", string[] newlyAchieved = null)
	{
		if ((UnityEngine.Object)RetiredColonyInfoScreen.Instance == (UnityEngine.Object)null)
		{
			Util.KInstantiateUI(ScreenPrefabs.Instance.RetiredColonyInfoScreen.gameObject, parent, true);
		}
		RetiredColonyInfoScreen.Instance.Show(true);
		if (!string.IsNullOrEmpty(colonyID))
		{
			if ((UnityEngine.Object)SaveGame.Instance != (UnityEngine.Object)null)
			{
				RetireColonyUtility.SaveColonySummaryData();
			}
			RetiredColonyInfoScreen.Instance.LoadColony(RetiredColonyInfoScreen.Instance.GetColonyDataByBaseName(colonyID));
		}
	}

	public static void ActivateRetiredColoniesScreenFromData(GameObject parent, RetiredColonyData data)
	{
		if ((UnityEngine.Object)RetiredColonyInfoScreen.Instance == (UnityEngine.Object)null)
		{
			Util.KInstantiateUI(ScreenPrefabs.Instance.RetiredColonyInfoScreen.gameObject, parent, true);
		}
		RetiredColonyInfoScreen.Instance.Show(true);
		RetiredColonyInfoScreen.Instance.LoadColony(data);
	}

	private void SpawnVideoScreen()
	{
		GameObject gameObject = Util.KInstantiateUI(ScreenPrefabs.Instance.VideoScreen.gameObject, base.gameObject, false);
		VideoScreen.Instance = gameObject.GetComponent<VideoScreen>();
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
				if (header.buildVersion > 365655 || gameInfo.saveMajorVersion < 7)
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
				Debug.LogWarning(obj);
				flag = false;
			}
		}
		if ((UnityEngine.Object)Button_ResumeGame != (UnityEngine.Object)null && (UnityEngine.Object)Button_ResumeGame.gameObject != (UnityEngine.Object)null)
		{
			Button_ResumeGame.gameObject.SetActive(flag);
		}
		else
		{
			Debug.LogWarning("Why is the resume game button null?");
		}
	}

	private void Translations()
	{
		LanguageOptionsScreen languageOptionsScreen = Util.KInstantiateUI<LanguageOptionsScreen>(ScreenPrefabs.Instance.languageOptionsScreen.gameObject, base.transform.parent.gameObject, false);
		languageOptionsScreen.SetBackgroundActive(true);
	}

	private void Mods()
	{
		ModsScreen modsScreen = Util.KInstantiateUI<ModsScreen>(ScreenPrefabs.Instance.modsMenu.gameObject, base.transform.parent.gameObject, false);
		modsScreen.SetBackgroundActive(true);
	}

	private void Options()
	{
		Util.KInstantiateUI<OptionsMenuScreen>(ScreenPrefabs.Instance.OptionsScreen.gameObject, base.gameObject, true);
	}

	private void QuitGame()
	{
		App.Quit();
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
			confirmDialogScreen2.PopupConfirmDialog(text, on_confirm, on_cancel, configurable_text, on_configurable_clicked, null, null, null, sadDupeAudio, true);
		}
	}

	private void CheckPlayerPrefsCorruption()
	{
		if (KPlayerPrefs.HasCorruptedFlag())
		{
			KPlayerPrefs.ResetCorruptedFlag();
			ConfirmDialogScreen confirmDialogScreen = Util.KInstantiateUI<ConfirmDialogScreen>(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.gameObject, true);
			ConfirmDialogScreen confirmDialogScreen2 = confirmDialogScreen;
			string text = UI.FRONTEND.SUPPORTWARNINGS.PLAYER_PREFS_CORRUPTED;
			System.Action on_confirm = null;
			System.Action on_cancel = null;
			Sprite sadDupe = GlobalResources.Instance().sadDupe;
			confirmDialogScreen2.PopupConfirmDialog(text, on_confirm, on_cancel, null, null, null, null, null, sadDupe, true);
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
			confirmDialogScreen2.PopupConfirmDialog(text2, on_confirm, on_cancel, null, null, null, null, null, sadDupe, true);
		}
	}

	private void RestartGame()
	{
		App.instance.Restart();
	}
}
