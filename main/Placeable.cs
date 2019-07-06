using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Placeable : KMonoBehaviour
{
	[MyCmpReq]
	private KPrefabID prefabId;

	[Serialize]
	private int targetCell = -1;

	public Tag previewTag;

	public Tag spawnOnPlaceTag;

	private GameObject preview;

	private FetchChore chore;

	private static readonly EventSystem.IntraObjectHandler<Placeable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Placeable>(delegate(Placeable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(493375141, OnRefreshUserMenuDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		prefabId.AddTag(new Tag(prefabId.InstanceID.ToString()), false);
		if (targetCell != -1)
		{
			QueuePlacement(targetCell);
		}
	}

	protected override void OnCleanUp()
	{
		if ((UnityEngine.Object)preview != (UnityEngine.Object)null)
		{
			preview.DeleteObject();
		}
		base.OnCleanUp();
	}

	public void QueuePlacement(int target)
	{
		targetCell = target;
		Vector3 position = Grid.CellToPosCBC(targetCell, Grid.SceneLayer.Front);
		if ((UnityEngine.Object)preview == (UnityEngine.Object)null)
		{
			preview = GameUtil.KInstantiate(Assets.GetPrefab(previewTag), position, Grid.SceneLayer.Front, null, 0);
			preview.SetActive(true);
		}
		else
		{
			preview.transform.SetPosition(position);
		}
		if (chore != null)
		{
			chore.Cancel("new target");
		}
		chore = new FetchChore(Db.Get().ChoreTypes.Fetch, preview.GetComponent<Storage>(), 1f, new Tag[1]
		{
			new Tag(prefabId.InstanceID.ToString())
		}, null, null, null, true, OnChoreComplete, null, null, FetchOrder2.OperationalRequirement.None, 0);
	}

	private void OnChoreComplete(Chore completed_chore)
	{
		Place(targetCell);
	}

	public void Place(int target)
	{
		Vector3 position = Grid.CellToPosCBC(target, Grid.SceneLayer.Front);
		GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(spawnOnPlaceTag), position, Grid.SceneLayer.Front, null, 0);
		gameObject.SetActive(true);
		this.DeleteObject();
	}

	private void OpenPlaceTool()
	{
		PlaceTool.Instance.Activate(this, previewTag);
	}

	private void OnRefreshUserMenu(object data)
	{
		object buttonInfo;
		if (targetCell == -1)
		{
			string iconName = "action_deconstruct";
			string text = UI.USERMENUACTIONS.RELOCATE.NAME;
			System.Action on_click = OpenPlaceTool;
			string tooltipText = UI.USERMENUACTIONS.RELOCATE.TOOLTIP;
			buttonInfo = new KIconButtonMenu.ButtonInfo(iconName, text, on_click, Action.NumActions, null, null, null, tooltipText, true);
		}
		else
		{
			string tooltipText = "action_deconstruct";
			string text = UI.USERMENUACTIONS.RELOCATE.NAME_OFF;
			System.Action on_click = CancelRelocation;
			string iconName = UI.USERMENUACTIONS.RELOCATE.TOOLTIP_OFF;
			buttonInfo = new KIconButtonMenu.ButtonInfo(tooltipText, text, on_click, Action.NumActions, null, null, null, iconName, true);
		}
		KIconButtonMenu.ButtonInfo button = (KIconButtonMenu.ButtonInfo)buttonInfo;
		Game.Instance.userMenu.AddButton(base.gameObject, button, 1f);
	}

	private void CancelRelocation()
	{
		if ((UnityEngine.Object)preview != (UnityEngine.Object)null)
		{
			preview.DeleteObject();
			preview = null;
		}
		targetCell = -1;
	}
}
