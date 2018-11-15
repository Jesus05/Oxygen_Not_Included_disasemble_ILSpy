using STRINGS;
using System;
using UnityEngine;

public class CopyBuildingSettings : KMonoBehaviour
{
	[MyCmpReq]
	private KPrefabID id;

	private static readonly EventSystem.IntraObjectHandler<CopyBuildingSettings> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<CopyBuildingSettings>(delegate(CopyBuildingSettings component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(493375141, OnRefreshUserMenuDelegate);
	}

	private void OnRefreshUserMenu(object data)
	{
		UserMenu userMenu = Game.Instance.userMenu;
		GameObject gameObject = base.gameObject;
		string iconName = "action_mirror";
		string text = UI.USERMENUACTIONS.COPY_BUILDING_SETTINGS.NAME;
		System.Action on_click = ActivateCopyTool;
		Action shortcutKey = Action.BuildingUtility1;
		string tooltipText = UI.USERMENUACTIONS.COPY_BUILDING_SETTINGS.TOOLTIP;
		userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo(iconName, text, on_click, shortcutKey, null, null, null, tooltipText, true), 1f);
	}

	private void ActivateCopyTool()
	{
		CopySettingsTool.Instance.SetSourceObject(base.gameObject);
		PlayerController.Instance.ActivateTool(CopySettingsTool.Instance);
	}

	public static bool ApplyCopy(int targetCell, GameObject sourceGameObject)
	{
		GameObject gameObject = Grid.Objects[targetCell, 1];
		if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null)
		{
			return false;
		}
		KPrefabID component = sourceGameObject.GetComponent<KPrefabID>();
		if ((UnityEngine.Object)component == (UnityEngine.Object)null)
		{
			return false;
		}
		KPrefabID component2 = gameObject.GetComponent<KPrefabID>();
		if ((UnityEngine.Object)component2 == (UnityEngine.Object)null)
		{
			return false;
		}
		if (component2.PrefabID() != component.PrefabID())
		{
			return false;
		}
		component2.Trigger(-905833192, sourceGameObject);
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, UI.COPIED_SETTINGS, gameObject.transform, new Vector3(0f, 0.5f, 0f), 1.5f, false, false);
		return true;
	}
}
