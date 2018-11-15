using STRINGS;
using System;
using UnityEngine;

public class TopLeftControlScreen : KScreen
{
	public static TopLeftControlScreen Instance;

	[SerializeField]
	private MultiToggle SandboxToggle;

	[SerializeField]
	private LocText locText;

	public static void DestroyInstance()
	{
		Instance = null;
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		Instance = this;
		RefreshName();
		UpdateSandboxToggleState();
		MultiToggle sandboxToggle = SandboxToggle;
		sandboxToggle.onClick = (System.Action)Delegate.Combine(sandboxToggle.onClick, new System.Action(OnClickSandboxToggle));
		Game.Instance.Subscribe(-1948169901, delegate
		{
			UpdateSandboxToggleState();
		});
	}

	public void RefreshName()
	{
		if ((UnityEngine.Object)SaveGame.Instance != (UnityEngine.Object)null)
		{
			locText.text = SaveGame.Instance.BaseName;
		}
	}

	public void UpdateSandboxToggleState()
	{
		if (CheckSandboxModeLocked())
		{
			SandboxToggle.GetComponent<ToolTip>().SetSimpleTooltip(UI.SANDBOX_TOGGLE.TOOLTIP_LOCKED + GameUtil.GetHotkeyString(Action.ToggleSandboxTools));
			SandboxToggle.ChangeState(0);
		}
		else
		{
			SandboxToggle.GetComponent<ToolTip>().SetSimpleTooltip(UI.SANDBOX_TOGGLE.TOOLTIP_UNLOCKED + GameUtil.GetHotkeyString(Action.ToggleSandboxTools));
			SandboxToggle.ChangeState((!Game.Instance.SandboxModeActive) ? 1 : 2);
		}
		SandboxToggle.gameObject.SetActive(SaveGame.Instance.sandboxEnabled);
	}

	private void OnClickSandboxToggle()
	{
		if (CheckSandboxModeLocked())
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
		}
		else
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click", false));
			Game.Instance.SandboxModeActive = !Game.Instance.SandboxModeActive;
		}
		UpdateSandboxToggleState();
	}

	private bool CheckSandboxModeLocked()
	{
		return !SaveGame.Instance.sandboxEnabled;
	}
}
