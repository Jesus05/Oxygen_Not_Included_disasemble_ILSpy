using FMOD.Studio;
using Klei;
using STRINGS;
using System;
using System.IO;
using UnityEngine;

public class PauseScreen : KModalButtonMenu
{
	[SerializeField]
	private OptionsMenuScreen optionsScreen;

	[SerializeField]
	private SaveScreen saveScreenPrefab;

	[SerializeField]
	private LoadScreen loadScreenPrefab;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private LocText title;

	[SerializeField]
	private LocText worldSeed;

	private float originalTimeScale;

	private static PauseScreen instance;

	public static PauseScreen Instance => instance;

	public override bool IsModal()
	{
		return true;
	}

	public static void DestroyInstance()
	{
		instance = null;
	}

	protected override void OnPrefabInit()
	{
		keepMenuOpen = true;
		base.OnPrefabInit();
		if (!GenericGameSettings.instance.demoMode)
		{
			buttons = new ButtonInfo[7]
			{
				new ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.RESUME, Action.NumActions, OnResume, null, null),
				new ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.SAVE, Action.NumActions, OnSave, null, null),
				new ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.SAVEAS, Action.NumActions, OnSaveAs, null, null),
				new ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.LOAD, Action.NumActions, OnLoad, null, null),
				new ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.OPTIONS, Action.NumActions, OnOptions, null, null),
				new ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.QUIT, Action.NumActions, OnQuit, null, null),
				new ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.DESKTOPQUIT, Action.NumActions, OnDesktopQuit, null, null)
			};
		}
		else
		{
			buttons = new ButtonInfo[4]
			{
				new ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.RESUME, Action.NumActions, OnResume, null, null),
				new ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.OPTIONS, Action.NumActions, OnOptions, null, null),
				new ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.QUIT, Action.NumActions, OnQuit, null, null),
				new ButtonInfo(UI.FRONTEND.PAUSE_SCREEN.DESKTOPQUIT, Action.NumActions, OnDesktopQuit, null, null)
			};
		}
		closeButton.onClick += OnResume;
		instance = this;
		Show(false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		title.SetText(UI.FRONTEND.PAUSE_SCREEN.TITLE);
		worldSeed.SetText(string.Format(UI.FRONTEND.PAUSE_SCREEN.WORLD_SEED, SaveLoader.Instance.worldDetailSave.globalWorldSeed));
		worldSeed.transform.SetAsLastSibling();
	}

	private void OnResume()
	{
		ToolTipScreen.Instance.ClearToolTip(closeButton.GetComponent<ToolTip>());
		Show(false);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().ESCPauseSnapshot, STOP_MODE.ALLOWFADEOUT);
		MusicManager.instance.OnEscapeMenu(false);
		MusicManager.instance.StopSong("Music_ESC_Menu", true, STOP_MODE.ALLOWFADEOUT);
	}

	private void OnOptions()
	{
		ActivateChildScreen(optionsScreen.gameObject);
	}

	private void OnSaveAs()
	{
		ActivateChildScreen(saveScreenPrefab.gameObject);
	}

	private void OnSave()
	{
		string filename = SaveLoader.GetActiveSaveFilePath();
		if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
		{
			base.gameObject.SetActive(false);
			ConfirmDialogScreen confirmDialogScreen = (ConfirmDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.transform.parent.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
			confirmDialogScreen.PopupConfirmDialog(string.Format(UI.FRONTEND.SAVESCREEN.OVERWRITEMESSAGE, Path.GetFileNameWithoutExtension(filename)), delegate
			{
				DoSave(filename);
				base.gameObject.SetActive(true);
			}, OnCancelPopup, null, null, null, null, null, null);
		}
		else
		{
			OnSaveAs();
		}
	}

	private void DoSave(string filename)
	{
		try
		{
			SaveLoader.Instance.Save(filename, false, true);
			ReportErrorDialog.MOST_RECENT_SAVEFILE = filename;
		}
		catch (IOException ex)
		{
			IOException e;
			IOException ex2 = e = ex;
			ConfirmDialogScreen component = Util.KInstantiateUI(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.transform.parent.gameObject, true).GetComponent<ConfirmDialogScreen>();
			component.PopupConfirmDialog(string.Format(UI.FRONTEND.SAVESCREEN.IO_ERROR, e.ToString()), delegate
			{
				Deactivate();
			}, null, UI.FRONTEND.SAVESCREEN.REPORT_BUG, delegate
			{
				KCrashReporter.ReportError(e.Message, e.StackTrace.ToString(), null, null, string.Empty);
			}, null, null, null, null);
		}
	}

	private void ConfirmDecision(string text, System.Action onConfirm)
	{
		base.gameObject.SetActive(false);
		ConfirmDialogScreen confirmDialogScreen = (ConfirmDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject, base.transform.parent.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
		confirmDialogScreen.PopupConfirmDialog(text, onConfirm, OnCancelPopup, null, null, null, null, null, null);
	}

	private void OnLoad()
	{
		ActivateChildScreen(loadScreenPrefab.gameObject);
	}

	private void OnQuit()
	{
		ConfirmDecision(UI.FRONTEND.MAINMENU.QUITCONFIRM, OnQuitConfirm);
	}

	private void OnDesktopQuit()
	{
		ConfirmDecision(UI.FRONTEND.MAINMENU.DESKTOPQUITCONFIRM, OnDesktopQuitConfirm);
	}

	private void OnCancelPopup()
	{
		base.gameObject.SetActive(true);
	}

	private void OnLoadConfirm()
	{
		LoadingOverlay.Load(delegate
		{
			LoadScreen.ForceStopGame();
			Deactivate();
			App.LoadScene("frontend");
		});
	}

	private void OnQuitConfirm()
	{
		LoadingOverlay.Load(delegate
		{
			Deactivate();
			TriggerQuitGame();
		});
	}

	private void OnDesktopQuitConfirm()
	{
		if (!Application.isEditor)
		{
			Application.Quit();
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
		{
			Show(false);
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().ESCPauseSnapshot, STOP_MODE.ALLOWFADEOUT);
			MusicManager.instance.OnEscapeMenu(false);
			MusicManager.instance.StopSong("Music_ESC_Menu", true, STOP_MODE.ALLOWFADEOUT);
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	public static void TriggerQuitGame()
	{
		SaveGame.Instance.worldGen.Reset();
		ThreadedHttps<KleiMetrics>.Instance.EndGame();
		LoadScreen.ForceStopGame();
		App.LoadScene("frontend");
	}
}
