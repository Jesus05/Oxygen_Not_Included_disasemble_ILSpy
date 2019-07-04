using System;
using UnityEngine;

public class PortraitTableColumn : TableColumn
{
	public GameObject prefab_portrait = Assets.UIPrefabs.TableScreenWidgets.MinionPortrait;

	private bool double_click_to_target;

	public PortraitTableColumn(Action<IAssignableIdentity, GameObject> on_load_action, Comparison<IAssignableIdentity> sort_comparison, bool double_click_to_target = true)
		: base(on_load_action, sort_comparison, null, null, null, false, "")
	{
		this.double_click_to_target = double_click_to_target;
	}

	public override GameObject GetDefaultWidget(GameObject parent)
	{
		GameObject gameObject = Util.KInstantiateUI(prefab_portrait, parent, true);
		gameObject.GetComponent<CrewPortrait>().targetImage.enabled = true;
		return gameObject;
	}

	public override GameObject GetHeaderWidget(GameObject parent)
	{
		return Util.KInstantiateUI(prefab_portrait, parent, true);
	}

	public override GameObject GetMinionWidget(GameObject parent)
	{
		GameObject gameObject = Util.KInstantiateUI(prefab_portrait, parent, true);
		if (double_click_to_target)
		{
			gameObject.GetComponent<KButton>().onClick += delegate
			{
				parent.GetComponent<TableRow>().SelectMinion();
			};
			gameObject.GetComponent<KButton>().onDoubleClick += delegate
			{
				parent.GetComponent<TableRow>().SelectAndFocusMinion();
			};
		}
		return gameObject;
	}
}
