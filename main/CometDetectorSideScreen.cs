using STRINGS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CometDetectorSideScreen : SideScreenContent
{
	private CometDetector.Instance detector;

	public GameObject rowPrefab;

	public RectTransform rowContainer;

	public Dictionary<object, GameObject> rows = new Dictionary<object, GameObject>();

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			RefreshOptions();
		}
	}

	private void RefreshOptions()
	{
		int num = 0;
		SetRow(num++, UI.UISIDESCREENS.COMETDETECTORSIDESCREEN.COMETS, Assets.GetSprite("asteroid"), null);
		foreach (Spacecraft item in SpacecraftManager.instance.GetSpacecraft())
		{
			SetRow(num++, item.GetRocketName(), Assets.GetSprite("icon_category_rocketry"), item.launchConditions);
		}
		for (int i = num; i < rowContainer.childCount; i++)
		{
			rowContainer.GetChild(i).gameObject.SetActive(false);
		}
	}

	private void ClearRows()
	{
		for (int num = rowContainer.childCount - 1; num >= 0; num--)
		{
			Util.KDestroyGameObject(rowContainer.GetChild(num));
		}
		rows.Clear();
	}

	public override void SetTarget(GameObject target)
	{
		detector = target.GetSMI<CometDetector.Instance>();
		RefreshOptions();
	}

	private void SetRow(int idx, string name, Sprite icon, LaunchConditionManager target)
	{
		GameObject gameObject = (idx >= rowContainer.childCount) ? Util.KInstantiateUI(rowPrefab, rowContainer.gameObject, true) : rowContainer.GetChild(idx).gameObject;
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		component.GetReference<LocText>("label").text = name;
		component.GetReference<Image>("icon").sprite = icon;
		MultiToggle component2 = gameObject.GetComponent<MultiToggle>();
		component2.ChangeState(((Object)detector.GetTargetCraft() == (Object)target) ? 1 : 0);
		component2.onClick = delegate
		{
			detector.SetTargetCraft(target);
			RefreshOptions();
		};
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetSMI<CometDetector.Instance>() != null;
	}
}
