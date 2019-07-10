using KSerialization;
using STRINGS;
using System;
using TUNING;
using UnityEngine;

public class Uprootable : Workable
{
	[Serialize]
	protected bool isMarkedForUproot;

	protected bool uprootComplete = false;

	[MyCmpReq]
	private Prioritizable prioritizable;

	[Serialize]
	protected bool canBeUprooted = true;

	public bool deselectOnUproot = true;

	protected Chore chore;

	private string buttonLabel;

	private string buttonTooltip;

	private string cancelButtonLabel;

	private string cancelButtonTooltip;

	private StatusItem pendingStatusItem;

	public OccupyArea area;

	private Storage planterStorage;

	public bool showUserMenuButtons = true;

	public HandleVector<int>.Handle partitionerEntry;

	private static readonly EventSystem.IntraObjectHandler<Uprootable> OnPlanterStorageDelegate = new EventSystem.IntraObjectHandler<Uprootable>(delegate(Uprootable component, object data)
	{
		component.OnPlanterStorage(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Uprootable> ForceCancelUprootDelegate = new EventSystem.IntraObjectHandler<Uprootable>(delegate(Uprootable component, object data)
	{
		component.ForceCancelUproot(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Uprootable> OnCancelDelegate = new EventSystem.IntraObjectHandler<Uprootable>(delegate(Uprootable component, object data)
	{
		component.OnCancel(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Uprootable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Uprootable>(delegate(Uprootable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	public bool IsMarkedForUproot => isMarkedForUproot;

	public Storage GetPlanterStorage => planterStorage;

	protected Uprootable()
	{
		SetOffsetTable(OffsetGroups.InvertedStandardTable);
		buttonLabel = UI.USERMENUACTIONS.UPROOT.NAME;
		buttonTooltip = UI.USERMENUACTIONS.UPROOT.TOOLTIP;
		cancelButtonLabel = UI.USERMENUACTIONS.CANCELUPROOT.NAME;
		cancelButtonTooltip = UI.USERMENUACTIONS.CANCELUPROOT.TOOLTIP;
		pendingStatusItem = Db.Get().MiscStatusItems.PendingUproot;
		workerStatusItem = Db.Get().DuplicantStatusItems.Uprooting;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		pendingStatusItem = Db.Get().MiscStatusItems.PendingUproot;
		workerStatusItem = Db.Get().DuplicantStatusItems.Uprooting;
		attributeConverter = Db.Get().AttributeConverters.HarvestSpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Farming.Id;
		skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		multitoolContext = "harvest";
		multitoolHitEffectTag = "fx_harvest_splash";
		Subscribe(1309017699, OnPlanterStorageDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(2127324410, ForceCancelUprootDelegate);
		SetWorkTime(12.5f);
		Subscribe(2127324410, OnCancelDelegate);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		faceTargetWhenWorking = true;
		Components.Uprootables.Add(this);
		area = GetComponent<OccupyArea>();
		Prioritizable.AddRef(base.gameObject);
		base.gameObject.AddTag(GameTags.Plant);
		Extents extents = new Extents(Grid.PosToCell(base.gameObject), base.gameObject.GetComponent<OccupyArea>().OccupiedCellsOffsets);
		partitionerEntry = GameScenePartitioner.Instance.Add(base.gameObject.name, base.gameObject.GetComponent<KPrefabID>(), extents, GameScenePartitioner.Instance.plants, null);
		if (isMarkedForUproot)
		{
			MarkForUproot(true);
		}
	}

	private void OnPlanterStorage(object data)
	{
		planterStorage = (Storage)data;
		Prioritizable component = GetComponent<Prioritizable>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			component.showIcon = ((UnityEngine.Object)planterStorage == (UnityEngine.Object)null);
		}
	}

	public bool IsInPlanterBox()
	{
		return (UnityEngine.Object)planterStorage != (UnityEngine.Object)null;
	}

	public void Uproot()
	{
		isMarkedForUproot = false;
		chore = null;
		uprootComplete = true;
		Trigger(-216549700, this);
		GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.PendingUproot, false);
		GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.Operating, false);
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	public void SetCanBeUprooted(bool state)
	{
		canBeUprooted = state;
		if (canBeUprooted)
		{
			SetUprootedComplete(false);
		}
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	public void SetUprootedComplete(bool state)
	{
		uprootComplete = state;
	}

	public void MarkForUproot(bool instantOnDebug = true)
	{
		if (canBeUprooted)
		{
			if (DebugHandler.InstantBuildMode && instantOnDebug)
			{
				Uproot();
			}
			else if (chore == null)
			{
				chore = new WorkChore<Uprootable>(Db.Get().ChoreTypes.Uproot, this, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
				GetComponent<KSelectable>().AddStatusItem(pendingStatusItem, this);
			}
			isMarkedForUproot = true;
		}
	}

	protected override void OnCompleteWork(Worker worker)
	{
		Uproot();
	}

	private void OnCancel(object data)
	{
		if (chore != null)
		{
			chore.Cancel("Cancel uproot");
			chore = null;
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.PendingUproot, false);
		}
		isMarkedForUproot = false;
		Game.Instance.userMenu.Refresh(base.gameObject);
	}

	public bool HasChore()
	{
		if (chore != null)
		{
			return true;
		}
		return false;
	}

	private void OnClickUproot()
	{
		MarkForUproot(true);
	}

	protected void OnClickCancelUproot()
	{
		OnCancel(null);
	}

	public virtual void ForceCancelUproot(object data = null)
	{
		OnCancel(null);
	}

	private void OnRefreshUserMenu(object data)
	{
		if (showUserMenuButtons)
		{
			if (uprootComplete)
			{
				if (deselectOnUproot)
				{
					KSelectable component = GetComponent<KSelectable>();
					if ((UnityEngine.Object)component != (UnityEngine.Object)null && (UnityEngine.Object)SelectTool.Instance.selected == (UnityEngine.Object)component)
					{
						SelectTool.Instance.Select(null, false);
					}
				}
			}
			else if (canBeUprooted)
			{
				object buttonInfo;
				if (chore != null)
				{
					string iconName = "action_uproot";
					string text = cancelButtonLabel;
					System.Action on_click = OnClickCancelUproot;
					string tooltipText = cancelButtonTooltip;
					buttonInfo = new KIconButtonMenu.ButtonInfo(iconName, text, on_click, Action.NumActions, null, null, null, tooltipText, true);
				}
				else
				{
					string tooltipText = "action_uproot";
					string text = buttonLabel;
					System.Action on_click = OnClickUproot;
					string iconName = buttonTooltip;
					buttonInfo = new KIconButtonMenu.ButtonInfo(tooltipText, text, on_click, Action.NumActions, null, null, null, iconName, true);
				}
				KIconButtonMenu.ButtonInfo button = (KIconButtonMenu.ButtonInfo)buttonInfo;
				Game.Instance.userMenu.AddButton(base.gameObject, button, 1f);
			}
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		GameScenePartitioner.Instance.Free(ref partitionerEntry);
		Components.Uprootables.Remove(this);
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.PendingUproot, false);
	}
}
