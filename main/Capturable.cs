using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Capturable : Workable, IGameObjectEffectDescriptor
{
	[MyCmpAdd]
	private Baggable baggable;

	[MyCmpAdd]
	private Prioritizable prioritizable;

	public bool allowCapture = true;

	[Serialize]
	private bool markedForCapture;

	private Chore chore;

	private static readonly EventSystem.IntraObjectHandler<Capturable> OnDeathDelegate = new EventSystem.IntraObjectHandler<Capturable>(delegate(Capturable component, object data)
	{
		component.OnDeath(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Capturable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Capturable>(delegate(Capturable component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<Capturable> OnTagsChangedDelegate = new EventSystem.IntraObjectHandler<Capturable>(delegate(Capturable component, object data)
	{
		component.OnTagsChanged(data);
	});

	public bool IsMarkedForCapture => markedForCapture;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Components.Capturables.Add(this);
		SetOffsetTable(OffsetGroups.InvertedStandardTable);
		requiredRolePerk = RoleManager.rolePerks.CanWrangleCreatures.id;
		resetProgressOnStop = true;
		faceTargetWhenWorking = true;
		synchronizeAnims = false;
		multitoolContext = "capture";
		multitoolHitEffectTag = "fx_capture_splash";
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(1623392196, OnDeathDelegate);
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		Subscribe(-1582839653, OnTagsChangedDelegate);
		if (markedForCapture)
		{
			Prioritizable.AddRef(base.gameObject);
		}
		UpdateStatusItem();
		UpdateChore();
		SetWorkTime(10f);
	}

	protected override void OnCleanUp()
	{
		Components.Capturables.Remove(this);
		base.OnCleanUp();
	}

	private void OnDeath(object data)
	{
		allowCapture = false;
		markedForCapture = false;
	}

	private void OnTagsChanged(object data)
	{
		MarkForCapture(markedForCapture);
	}

	public void MarkForCapture(bool mark)
	{
		mark = (mark && IsCapturable());
		if (markedForCapture && !mark)
		{
			Prioritizable.RemoveRef(base.gameObject);
		}
		else if (!markedForCapture && mark)
		{
			Prioritizable.AddRef(base.gameObject);
		}
		markedForCapture = mark;
		UpdateStatusItem();
		UpdateChore();
	}

	public bool IsCapturable()
	{
		if (allowCapture)
		{
			if (!base.gameObject.HasTag(GameTags.Trapped))
			{
				if (!base.gameObject.HasTag(GameTags.Stored))
				{
					if (!base.gameObject.HasTag(GameTags.Creatures.Bagged))
					{
						return true;
					}
					return false;
				}
				return false;
			}
			return false;
		}
		return false;
	}

	private void OnRefreshUserMenu(object data)
	{
		if (IsCapturable())
		{
			object buttonInfo;
			if (!markedForCapture)
			{
				string iconName = "action_capture";
				string text = UI.USERMENUACTIONS.CAPTURE.NAME;
				System.Action on_click = delegate
				{
					MarkForCapture(true);
				};
				string tooltipText = UI.USERMENUACTIONS.CAPTURE.TOOLTIP;
				buttonInfo = new KIconButtonMenu.ButtonInfo(iconName, text, on_click, Action.NumActions, null, null, null, tooltipText, true);
			}
			else
			{
				string tooltipText = "action_capture";
				string text = UI.USERMENUACTIONS.CANCELCAPTURE.NAME;
				System.Action on_click = delegate
				{
					MarkForCapture(false);
				};
				string iconName = UI.USERMENUACTIONS.CANCELCAPTURE.TOOLTIP;
				buttonInfo = new KIconButtonMenu.ButtonInfo(tooltipText, text, on_click, Action.NumActions, null, null, null, iconName, true);
			}
			KIconButtonMenu.ButtonInfo button = (KIconButtonMenu.ButtonInfo)buttonInfo;
			Game.Instance.userMenu.AddButton(base.gameObject, button, 1f);
		}
	}

	private void UpdateStatusItem()
	{
		shouldShowRolePerkStatusItem = markedForCapture;
		base.UpdateStatusItem(null);
		if (markedForCapture)
		{
			GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.OrderCapture, this);
		}
		else
		{
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.OrderCapture, false);
		}
	}

	private void UpdateChore()
	{
		if (markedForCapture && chore == null)
		{
			ChoreType capture = Db.Get().ChoreTypes.Capture;
			Tag[] chore_tags = new Tag[1]
			{
				GameTags.ChoreTypes.Ranching
			};
			chore = new WorkChore<Capturable>(capture, this, null, chore_tags, true, null, null, null, true, null, false, true, null, true, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
		}
		else if (!markedForCapture && chore != null)
		{
			chore.Cancel("not marked for capture");
			chore = null;
		}
	}

	protected override void OnStartWork(Worker worker)
	{
		KPrefabID component = GetComponent<KPrefabID>();
		component.AddTag(GameTags.Creatures.Stunned);
	}

	protected override void OnStopWork(Worker worker)
	{
		KPrefabID component = GetComponent<KPrefabID>();
		component.RemoveTag(GameTags.Creatures.Stunned);
	}

	protected override void OnCompleteWork(Worker worker)
	{
		int num = this.NaturalBuildingCell();
		if (Grid.Solid[num])
		{
			int num2 = Grid.CellAbove(num);
			if (Grid.IsValidCell(num2) && !Grid.Solid[num2])
			{
				num = num2;
			}
		}
		MarkForCapture(false);
		baggable.SetWrangled();
		baggable.transform.SetPosition(Grid.CellToPosCCC(num, Grid.SceneLayer.Ore));
	}

	public override List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> descriptors = base.GetDescriptors(go);
		if (allowCapture)
		{
			descriptors.Add(new Descriptor(UI.BUILDINGEFFECTS.CAPTURE_METHOD_WRANGLE, UI.BUILDINGEFFECTS.TOOLTIPS.CAPTURE_METHOD_WRANGLE, Descriptor.DescriptorType.Effect, false));
		}
		return descriptors;
	}
}
