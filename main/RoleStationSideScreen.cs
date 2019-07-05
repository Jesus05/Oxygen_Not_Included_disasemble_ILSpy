using UnityEngine;

public class RoleStationSideScreen : SideScreenContent
{
	public GameObject content;

	private GameObject target;

	public LocText DescriptionText;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return false;
	}
}
