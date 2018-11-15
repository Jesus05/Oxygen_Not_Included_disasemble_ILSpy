using STRINGS;
using System;
using UnityEngine;
using UnityEngine.UI;

public class TelescopeSideScreen : SideScreenContent
{
	public KButton selectStarmapScreen;

	public Image researchButtonIcon;

	public GameObject content;

	private GameObject target;

	private Action<object> refreshDisplayStateDelegate;

	public LocText DescriptionText;

	public TelescopeSideScreen()
	{
		refreshDisplayStateDelegate = RefreshDisplayState;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		selectStarmapScreen.onClick += delegate
		{
			ManagementMenu.Instance.ToggleStarmap();
		};
		SpacecraftManager.instance.Subscribe(532901469, refreshDisplayStateDelegate);
		RefreshDisplayState(null);
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		RefreshDisplayState(null);
		target = SelectTool.Instance.selected.GetComponent<KMonoBehaviour>().gameObject;
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		if ((bool)target)
		{
			target = null;
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if ((bool)target)
		{
			target = null;
		}
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return (UnityEngine.Object)target.GetComponent<Telescope>() != (UnityEngine.Object)null;
	}

	private void RefreshDisplayState(object data = null)
	{
		if (!((UnityEngine.Object)SelectTool.Instance.selected == (UnityEngine.Object)null))
		{
			Telescope component = SelectTool.Instance.selected.GetComponent<Telescope>();
			if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
			{
				if (!SpacecraftManager.instance.HasAnalysisTarget())
				{
					DescriptionText.text = "<b><color=#FF0000>" + UI.UISIDESCREENS.TELESCOPESIDESCREEN.NO_SELECTED_ANALYSIS_TARGET + "</color></b>";
				}
				else
				{
					string text = UI.UISIDESCREENS.TELESCOPESIDESCREEN.ANALYSIS_TARGET_SELECTED;
					DescriptionText.text = text;
				}
			}
		}
	}
}
