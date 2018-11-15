using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class OverviewScreen : KTabMenu
{
	private List<KScreen> TabScreens = new List<KScreen>();

	public InstantiateUIPrefabChild ScreenInstantiator;

	public TitleBar titleBar;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ScreenInstantiator.Instantiate();
		KScreen[] componentsInChildren = ScreenInstantiator.GetComponentsInChildren<KScreen>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			TabScreens.Add(componentsInChildren[i]);
		}
		foreach (KScreen tabScreen in TabScreens)
		{
			AddTab(tabScreen.displayName, tabScreen);
			tabScreen.gameObject.SetActive(false);
		}
	}

	public override void ActivateTab(int tabIdx)
	{
		switch (tabIdx)
		{
		case 0:
			titleBar.SetTitle(UI.JOBS);
			break;
		case 1:
			titleBar.SetTitle(UI.VITALS);
			break;
		}
		base.ActivateTab(tabIdx);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	protected override void OnActivate()
	{
		base.OnActivate();
	}

	protected override void OnDeactivate()
	{
		foreach (KScreen tabScreen in TabScreens)
		{
			tabScreen.Deactivate();
			Object.Destroy(tabScreen);
		}
	}
}
