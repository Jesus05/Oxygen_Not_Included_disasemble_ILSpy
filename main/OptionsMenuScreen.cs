using STRINGS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuScreen : KModalButtonMenu
{
	[SerializeField]
	private UnitConfigurationScreen unitScreenPrefab;

	[SerializeField]
	private InputBindingsScreen inputBindingsScreenPrefab;

	[SerializeField]
	private AudioOptionsScreen audioOptionsScreenPrefab;

	[SerializeField]
	private GraphicsOptionsScreen graphicsOptionsScreenPrefab;

	[SerializeField]
	private CreditsScreen creditsScreenPrefab;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private MetricsOptionsScreen metricsScreenPrefab;

	[SerializeField]
	private LocText title;

	[SerializeField]
	private KButton backButton;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		keepMenuOpen = true;
		buttons = new List<ButtonInfo>
		{
			new ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.GRAPHICS, Action.NumActions, OnGraphicsOptions, null, null),
			new ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.AUDIO, Action.NumActions, OnAudioOptions, null, null),
			new ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.CONTROLS, Action.NumActions, OnKeyBindings, null, null),
			new ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.UNITS, Action.NumActions, OnUnits, null, null),
			new ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.METRICS, Action.NumActions, OnMetrics, null, null),
			new ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.CREDITS, Action.NumActions, OnCredits, null, null),
			new ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.RESET_TUTORIAL, Action.NumActions, OnTutorialReset, null, null)
		};
		if ((Object)SaveGame.Instance != (Object)null && !SaveGame.Instance.sandboxEnabled)
		{
			buttons.Add(new ButtonInfo(UI.FRONTEND.OPTIONS_SCREEN.UNLOCK_SANDBOX, Action.NumActions, OnUnlockSandboxMode, null, null));
		}
		closeButton.onClick += Deactivate;
		backButton.onClick += Deactivate;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		title.SetText(UI.FRONTEND.OPTIONS_SCREEN.TITLE);
		backButton.transform.SetAsLastSibling();
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		GameObject[] buttonObjects = base.buttonObjects;
		foreach (GameObject gameObject in buttonObjects)
		{
			gameObject.GetComponent<LayoutElement>().minWidth = 512f;
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(Action.Escape) || e.TryConsume(Action.MouseRight))
		{
			Deactivate();
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	private void OnGraphicsOptions()
	{
		ActivateChildScreen(graphicsOptionsScreenPrefab.gameObject);
	}

	private void OnAudioOptions()
	{
		ActivateChildScreen(audioOptionsScreenPrefab.gameObject);
	}

	private void OnKeyBindings()
	{
		ActivateChildScreen(inputBindingsScreenPrefab.gameObject);
	}

	private void OnUnits()
	{
		ActivateChildScreen(unitScreenPrefab.gameObject);
	}

	private void OnMetrics()
	{
		ActivateChildScreen(metricsScreenPrefab.gameObject);
	}

	private void OnCredits()
	{
		ActivateChildScreen(creditsScreenPrefab.gameObject);
	}

	private void OnTutorialReset()
	{
		ConfirmDialogScreen component = ActivateChildScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject).GetComponent<ConfirmDialogScreen>();
		component.PopupConfirmDialog(UI.FRONTEND.OPTIONS_SCREEN.RESET_TUTORIAL_WARNING, delegate
		{
			Tutorial.ResetHiddenTutorialMessages();
		}, delegate
		{
		}, null, null, null, null, null, null);
		component.Activate();
	}

	private void OnUnlockSandboxMode()
	{
		ConfirmDialogScreen component = ActivateChildScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject).GetComponent<ConfirmDialogScreen>();
		component.PopupConfirmDialog(UI.FRONTEND.OPTIONS_SCREEN.TOGGLE_SANDBOX_SCREEN.UNLOCK_SANDBOX_WARNING, delegate
		{
			SaveGame.Instance.sandboxEnabled = true;
			TopLeftControlScreen.Instance.UpdateSandboxToggleState();
			Deactivate();
		}, delegate
		{
			string savePrefixAndCreateFolder = SaveLoader.GetSavePrefixAndCreateFolder();
			string text2 = savePrefixAndCreateFolder;
			savePrefixAndCreateFolder = text2 + "\\" + SaveGame.Instance.BaseName + UI.FRONTEND.OPTIONS_SCREEN.TOGGLE_SANDBOX_SCREEN.BACKUP_SAVE_GAME_APPEND + ".sav";
			SaveLoader.Instance.Save(savePrefixAndCreateFolder, false, false);
			SaveGame.Instance.sandboxEnabled = true;
			TopLeftControlScreen.Instance.UpdateSandboxToggleState();
			Deactivate();
		}, UI.FRONTEND.OPTIONS_SCREEN.TOGGLE_SANDBOX_SCREEN.CANCEL, delegate
		{
		}, cancel_text: UI.FRONTEND.OPTIONS_SCREEN.TOGGLE_SANDBOX_SCREEN.CONFIRM_SAVE_BACKUP, title_text: null, confirm_text: UI.FRONTEND.OPTIONS_SCREEN.TOGGLE_SANDBOX_SCREEN.CONFIRM, image_sprite: null);
		component.Activate();
	}

	private void Update()
	{
		Debug.developerConsoleVisible = false;
	}
}
