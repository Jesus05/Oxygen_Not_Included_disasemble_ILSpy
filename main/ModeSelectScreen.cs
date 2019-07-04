using STRINGS;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ModeSelectScreen : NewGameFlowScreen
{
	[SerializeField]
	private MultiToggle nosweatButton;

	private Image nosweatButtonHeader;

	private Image nosweatButtonSelectionFrame;

	[SerializeField]
	private MultiToggle survivalButton;

	private Image survivalButtonHeader;

	private Image survivalButtonSelectionFrame;

	[SerializeField]
	private LocText descriptionArea;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KBatchedAnimController nosweatAnim;

	[SerializeField]
	private KBatchedAnimController survivalAnim;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		HierarchyReferences component = survivalButton.GetComponent<HierarchyReferences>();
		survivalButtonHeader = component.GetReference<RectTransform>("HeaderBackground").GetComponent<Image>();
		survivalButtonSelectionFrame = component.GetReference<RectTransform>("SelectionFrame").GetComponent<Image>();
		MultiToggle multiToggle = survivalButton;
		multiToggle.onEnter = (System.Action)Delegate.Combine(multiToggle.onEnter, new System.Action(OnHoverEnterSurvival));
		MultiToggle multiToggle2 = survivalButton;
		multiToggle2.onExit = (System.Action)Delegate.Combine(multiToggle2.onExit, new System.Action(OnHoverExitSurvival));
		MultiToggle multiToggle3 = survivalButton;
		multiToggle3.onClick = (System.Action)Delegate.Combine(multiToggle3.onClick, new System.Action(OnClickSurvival));
		HierarchyReferences component2 = nosweatButton.GetComponent<HierarchyReferences>();
		nosweatButtonHeader = component2.GetReference<RectTransform>("HeaderBackground").GetComponent<Image>();
		nosweatButtonSelectionFrame = component2.GetReference<RectTransform>("SelectionFrame").GetComponent<Image>();
		MultiToggle multiToggle4 = nosweatButton;
		multiToggle4.onEnter = (System.Action)Delegate.Combine(multiToggle4.onEnter, new System.Action(OnHoverEnterNosweat));
		MultiToggle multiToggle5 = nosweatButton;
		multiToggle5.onExit = (System.Action)Delegate.Combine(multiToggle5.onExit, new System.Action(OnHoverExitNosweat));
		MultiToggle multiToggle6 = nosweatButton;
		multiToggle6.onClick = (System.Action)Delegate.Combine(multiToggle6.onClick, new System.Action(OnClickNosweat));
		closeButton.onClick += base.NavigateBackward;
		SetAnimScale();
	}

	private void OnHoverEnterSurvival()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover", false));
		survivalButtonSelectionFrame.SetAlpha(1f);
		survivalButtonHeader.color = new Color(0.7019608f, 0.3647059f, 0.533333361f, 1f);
		descriptionArea.text = UI.FRONTEND.MODESELECTSCREEN.SURVIVAL_DESC;
	}

	private void OnHoverExitSurvival()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover", false));
		survivalButtonSelectionFrame.SetAlpha(0f);
		survivalButtonHeader.color = new Color(0.309803933f, 0.34117648f, 0.384313732f, 1f);
		descriptionArea.text = UI.FRONTEND.MODESELECTSCREEN.BLANK_DESC;
	}

	private void OnClickSurvival()
	{
		Deactivate();
		CustomGameSettings.Instance.SetSurvivalDefaults();
		NavigateForward();
	}

	private void OnHoverEnterNosweat()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover", false));
		nosweatButtonSelectionFrame.SetAlpha(1f);
		nosweatButtonHeader.color = new Color(0.7019608f, 0.3647059f, 0.533333361f, 1f);
		descriptionArea.text = UI.FRONTEND.MODESELECTSCREEN.NOSWEAT_DESC;
	}

	private void OnHoverExitNosweat()
	{
		KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover", false));
		nosweatButtonSelectionFrame.SetAlpha(0f);
		nosweatButtonHeader.color = new Color(0.309803933f, 0.34117648f, 0.384313732f, 1f);
		descriptionArea.text = UI.FRONTEND.MODESELECTSCREEN.BLANK_DESC;
	}

	private void OnClickNosweat()
	{
		Deactivate();
		CustomGameSettings.Instance.SetNosweatDefaults();
		NavigateForward();
	}

	private void SetAnimScale()
	{
		float num = 1f;
		num = GetComponentInParent<KCanvasScaler>().GetCanvasScale();
		if ((UnityEngine.Object)nosweatAnim != (UnityEngine.Object)null)
		{
			nosweatAnim.animScale *= 1f / num;
		}
		if ((UnityEngine.Object)survivalAnim != (UnityEngine.Object)null)
		{
			survivalAnim.animScale *= 1f / num;
		}
	}
}
