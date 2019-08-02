using STRINGS;
using UnityEngine;

public class GameOptionsScreen : KModalButtonMenu
{
	[SerializeField]
	private SaveConfigurationScreen saveConfiguration;

	[SerializeField]
	private UnitConfigurationScreen unitConfiguration;

	[SerializeField]
	private KButton resetTutorialButton;

	[SerializeField]
	private KButton controlsButton;

	[SerializeField]
	private KButton sandboxButton;

	[SerializeField]
	private KButton doneButton;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private GameObject savePanel;

	[SerializeField]
	private InputBindingsScreen inputBindingsScreenPrefab;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		unitConfiguration.Init();
		if ((Object)SaveGame.Instance != (Object)null)
		{
			saveConfiguration.ToggleDisabledContent(true);
			saveConfiguration.Init();
			SetSandboxModeActive(SaveGame.Instance.sandboxEnabled);
		}
		else
		{
			saveConfiguration.ToggleDisabledContent(false);
		}
		resetTutorialButton.onClick += OnTutorialReset;
		controlsButton.onClick += OnKeyBindings;
		sandboxButton.onClick += OnUnlockSandboxMode;
		doneButton.onClick += Deactivate;
		closeButton.onClick += Deactivate;
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if ((Object)SaveGame.Instance != (Object)null)
		{
			savePanel.SetActive(true);
			saveConfiguration.Show(show);
			SetSandboxModeActive(SaveGame.Instance.sandboxEnabled);
		}
		else
		{
			savePanel.SetActive(false);
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

	private void OnTutorialReset()
	{
		ConfirmDialogScreen component = ActivateChildScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject).GetComponent<ConfirmDialogScreen>();
		component.PopupConfirmDialog(UI.FRONTEND.OPTIONS_SCREEN.RESET_TUTORIAL_WARNING, delegate
		{
			Tutorial.ResetHiddenTutorialMessages();
		}, delegate
		{
		}, null, null, null, null, null, null, true);
		component.Activate();
	}

	private void OnUnlockSandboxMode()
	{
		ConfirmDialogScreen component = ActivateChildScreen(ScreenPrefabs.Instance.ConfirmDialogScreen.gameObject).GetComponent<ConfirmDialogScreen>();
		component.PopupConfirmDialog(UI.FRONTEND.OPTIONS_SCREEN.TOGGLE_SANDBOX_SCREEN.UNLOCK_SANDBOX_WARNING, delegate
		{
			SaveGame.Instance.sandboxEnabled = true;
			SetSandboxModeActive(SaveGame.Instance.sandboxEnabled);
			TopLeftControlScreen.Instance.UpdateSandboxToggleState();
			Deactivate();
		}, delegate
		{
			string savePrefixAndCreateFolder = SaveLoader.GetSavePrefixAndCreateFolder();
			string text2 = savePrefixAndCreateFolder;
			savePrefixAndCreateFolder = text2 + "\\" + SaveGame.Instance.BaseName + UI.FRONTEND.OPTIONS_SCREEN.TOGGLE_SANDBOX_SCREEN.BACKUP_SAVE_GAME_APPEND + ".sav";
			SaveLoader.Instance.Save(savePrefixAndCreateFolder, false, false);
			SetSandboxModeActive(SaveGame.Instance.sandboxEnabled);
			TopLeftControlScreen.Instance.UpdateSandboxToggleState();
			Deactivate();
		}, UI.FRONTEND.OPTIONS_SCREEN.TOGGLE_SANDBOX_SCREEN.CANCEL, delegate
		{
		}, cancel_text: UI.FRONTEND.OPTIONS_SCREEN.TOGGLE_SANDBOX_SCREEN.CONFIRM_SAVE_BACKUP, title_text: null, confirm_text: UI.FRONTEND.OPTIONS_SCREEN.TOGGLE_SANDBOX_SCREEN.CONFIRM, image_sprite: null, activateBlackBackground: true);
		component.Activate();
	}

	private void OnKeyBindings()
	{
		ActivateChildScreen(inputBindingsScreenPrefab.gameObject);
	}

	private void SetSandboxModeActive(bool active)
	{
		sandboxButton.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(active);
		sandboxButton.isInteractable = !active;
		sandboxButton.gameObject.GetComponentInParent<CanvasGroup>().alpha = ((!active) ? 1f : 0.5f);
	}
}
