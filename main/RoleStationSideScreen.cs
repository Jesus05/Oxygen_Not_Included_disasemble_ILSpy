using UnityEngine;

public class RoleStationSideScreen : SideScreenContent
{
	public KButton openRolesScreenButton;

	public GameObject content;

	private GameObject target;

	public LocText DescriptionText;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		openRolesScreenButton.onClick += delegate
		{
			ManagementMenu.Instance.ToggleRoles();
		};
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return (Object)target.GetComponent<RoleStation>() != (Object)null;
	}
}
