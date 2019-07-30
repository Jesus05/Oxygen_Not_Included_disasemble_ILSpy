using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SuitMarker : KMonoBehaviour
{
	private class SuitMarkerReactable : Reactable
	{
		private SuitMarker suitMarker;

		private float startTime;

		public SuitMarkerReactable(SuitMarker suit_marker)
			: base(suit_marker.gameObject, "SuitMarkerReactable", Db.Get().ChoreTypes.SuitMarker, 1, 1, false, 0f, 0f, float.PositiveInfinity)
		{
			suitMarker = suit_marker;
		}

		public override bool InternalCanBegin(GameObject new_reactor, Navigator.ActiveTransition transition)
		{
			if ((UnityEngine.Object)reactor != (UnityEngine.Object)null)
			{
				return false;
			}
			if ((UnityEngine.Object)suitMarker == (UnityEngine.Object)null)
			{
				Cleanup();
				return false;
			}
			if (!suitMarker.isOperational)
			{
				return false;
			}
			int num = transition.navGridTransition.x;
			if (num == 0)
			{
				return false;
			}
			MinionIdentity component = new_reactor.GetComponent<MinionIdentity>();
			if (component.GetEquipment().IsSlotOccupied(Db.Get().AssignableSlots.Suit))
			{
				if (num < 0 && suitMarker.isRotated)
				{
					return false;
				}
				if (num > 0 && !suitMarker.isRotated)
				{
					return false;
				}
				return true;
			}
			if (num > 0 && suitMarker.isRotated)
			{
				return false;
			}
			if (num < 0 && !suitMarker.isRotated)
			{
				return false;
			}
			return Grid.HasSuit(Grid.PosToCell(suitMarker), new_reactor.GetComponent<KPrefabID>().InstanceID);
		}

		protected override void InternalBegin()
		{
			startTime = Time.time;
			KBatchedAnimController component = reactor.GetComponent<KBatchedAnimController>();
			component.AddAnimOverrides(suitMarker.interactAnim, 1f);
			component.Play("working_pre", KAnim.PlayMode.Once, 1f, 0f);
			component.Queue("working_loop", KAnim.PlayMode.Once, 1f, 0f);
			component.Queue("working_pst", KAnim.PlayMode.Once, 1f, 0f);
			if (suitMarker.HasTag(GameTags.JetSuitBlocker))
			{
				KBatchedAnimController component2 = suitMarker.GetComponent<KBatchedAnimController>();
				component2.Play("working_pre", KAnim.PlayMode.Once, 1f, 0f);
				component2.Queue("working_loop", KAnim.PlayMode.Once, 1f, 0f);
				component2.Queue("working_pst", KAnim.PlayMode.Once, 1f, 0f);
			}
			suitMarker.CreateNewReactable();
		}

		public override void Update(float dt)
		{
			Facing facing = (!(bool)reactor) ? null : reactor.GetComponent<Facing>();
			if ((bool)facing && (bool)suitMarker)
			{
				facing.SetFacing(suitMarker.GetComponent<Rotatable>().GetOrientation() == Orientation.FlipH);
			}
			if (Time.time - startTime > 2.8f)
			{
				Run();
				Cleanup();
			}
		}

		private void Run()
		{
			if (!((UnityEngine.Object)base.reactor == (UnityEngine.Object)null) && !((UnityEngine.Object)suitMarker == (UnityEngine.Object)null))
			{
				GameObject reactor = base.reactor;
				Equipment equipment = reactor.GetComponent<MinionIdentity>().GetEquipment();
				bool flag = !equipment.IsSlotOccupied(Db.Get().AssignableSlots.Suit);
				reactor.GetComponent<KBatchedAnimController>().RemoveAnimOverrides(suitMarker.interactAnim);
				bool flag2 = false;
				Navigator component = reactor.GetComponent<Navigator>();
				bool flag3 = (UnityEngine.Object)component != (UnityEngine.Object)null && (component.flags & suitMarker.PathFlag) != PathFinder.PotentialPath.Flags.None;
				if (flag || flag3)
				{
					ListPool<SuitLocker, SuitMarker>.PooledList pooledList = ListPool<SuitLocker, SuitMarker>.Allocate();
					suitMarker.GetAttachedLockers(pooledList);
					foreach (SuitLocker item in pooledList)
					{
						KPrefabID fullyChargedOutfit = item.GetFullyChargedOutfit();
						if ((UnityEngine.Object)fullyChargedOutfit != (UnityEngine.Object)null && flag)
						{
							item.EquipTo(equipment);
							flag2 = true;
							break;
						}
						if (!flag && item.CanDropOffSuit())
						{
							item.UnequipFrom(equipment);
							flag2 = true;
							break;
						}
					}
					if (flag && !flag2)
					{
						SuitLocker suitLocker = null;
						float num = 0f;
						foreach (SuitLocker item2 in pooledList)
						{
							if (item2.GetSuitScore() > num)
							{
								suitLocker = item2;
								num = item2.GetSuitScore();
							}
						}
						if ((UnityEngine.Object)suitLocker != (UnityEngine.Object)null)
						{
							suitLocker.EquipTo(equipment);
							flag2 = true;
						}
					}
					pooledList.Recycle();
				}
				if (!flag2 && !flag)
				{
					Assignable assignable = equipment.GetAssignable(Db.Get().AssignableSlots.Suit);
					assignable.Unassign();
					Notification notification = new Notification(MISC.NOTIFICATIONS.SUIT_DROPPED.NAME, NotificationType.BadMinor, HashedString.Invalid, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.SUIT_DROPPED.TOOLTIP, null, true, 0f, null, null, null);
					assignable.GetComponent<Notifier>().Add(notification, string.Empty);
				}
			}
		}

		protected override void InternalEnd()
		{
			if ((UnityEngine.Object)reactor != (UnityEngine.Object)null)
			{
				reactor.GetComponent<KBatchedAnimController>().RemoveAnimOverrides(suitMarker.interactAnim);
			}
		}

		protected override void InternalCleanup()
		{
		}
	}

	[MyCmpGet]
	private Building building;

	private ScenePartitionerEntry partitionerEntry;

	private SuitMarkerReactable reactable;

	private bool hasAvailableSuit;

	[Serialize]
	private bool onlyTraverseIfUnequipAvailable;

	private Grid.SuitMarker.Flags gridFlags;

	private int cell;

	public Tag[] LockerTags;

	public PathFinder.PotentialPath.Flags PathFlag;

	public KAnimFile interactAnim = Assets.GetAnim("anim_equip_clothing_kanim");

	private static readonly EventSystem.IntraObjectHandler<SuitMarker> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<SuitMarker>(delegate(SuitMarker component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<SuitMarker> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<SuitMarker>(delegate(SuitMarker component, object data)
	{
		component.OnOperationalChanged((bool)data);
	});

	private static readonly EventSystem.IntraObjectHandler<SuitMarker> OnRotatedDelegate = new EventSystem.IntraObjectHandler<SuitMarker>(delegate(SuitMarker component, object data)
	{
		component.isRotated = ((Rotatable)data).IsRotated;
	});

	private bool OnlyTraverseIfUnequipAvailable
	{
		get
		{
			DebugUtil.Assert(onlyTraverseIfUnequipAvailable == ((gridFlags & Grid.SuitMarker.Flags.OnlyTraverseIfUnequipAvailable) != (Grid.SuitMarker.Flags)0));
			return onlyTraverseIfUnequipAvailable;
		}
		set
		{
			onlyTraverseIfUnequipAvailable = value;
			UpdateGridFlag(Grid.SuitMarker.Flags.OnlyTraverseIfUnequipAvailable, onlyTraverseIfUnequipAvailable);
		}
	}

	private bool isRotated
	{
		get
		{
			return (gridFlags & Grid.SuitMarker.Flags.Rotated) != (Grid.SuitMarker.Flags)0;
		}
		set
		{
			UpdateGridFlag(Grid.SuitMarker.Flags.Rotated, value);
		}
	}

	private bool isOperational
	{
		get
		{
			return (gridFlags & Grid.SuitMarker.Flags.Operational) != (Grid.SuitMarker.Flags)0;
		}
		set
		{
			UpdateGridFlag(Grid.SuitMarker.Flags.Operational, value);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		OnlyTraverseIfUnequipAvailable = onlyTraverseIfUnequipAvailable;
		Debug.Assert((UnityEngine.Object)interactAnim != (UnityEngine.Object)null, "interactAnim is null");
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		isOperational = GetComponent<Operational>().IsOperational;
		Subscribe(-592767678, OnOperationalChangedDelegate);
		isRotated = GetComponent<Rotatable>().IsRotated;
		Subscribe(-1643076535, OnRotatedDelegate);
		CreateNewReactable();
		cell = Grid.PosToCell(this);
		Grid.RegisterSuitMarker(cell);
		GetComponent<KAnimControllerBase>().Play("no_suit", KAnim.PlayMode.Once, 1f, 0f);
		Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Suits, true);
		RefreshTraverseIfUnequipStatusItem();
		SuitLocker.UpdateSuitMarkerStates(Grid.PosToCell(base.transform.position), base.gameObject);
	}

	private void CreateNewReactable()
	{
		reactable = new SuitMarkerReactable(this);
	}

	public void GetAttachedLockers(List<SuitLocker> suit_lockers)
	{
		int num = isRotated ? 1 : (-1);
		int num2 = 1;
		while (true)
		{
			int num3 = Grid.OffsetCell(cell, num2 * num, 0);
			GameObject gameObject = Grid.Objects[num3, 1];
			if ((UnityEngine.Object)gameObject == (UnityEngine.Object)null)
			{
				break;
			}
			KPrefabID component = gameObject.GetComponent<KPrefabID>();
			if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
			{
				if (!component.HasAnyTags(LockerTags))
				{
					break;
				}
				SuitLocker component2 = gameObject.GetComponent<SuitLocker>();
				if ((UnityEngine.Object)component2 == (UnityEngine.Object)null)
				{
					break;
				}
				if (!suit_lockers.Contains(component2))
				{
					suit_lockers.Add(component2);
				}
			}
			num2++;
		}
	}

	public static bool DoesTraversalDirectionRequireSuit(int source_cell, int dest_cell, Grid.SuitMarker.Flags flags)
	{
		bool flag = Grid.CellColumn(dest_cell) > Grid.CellColumn(source_cell);
		return flag == ((flags & Grid.SuitMarker.Flags.Rotated) == (Grid.SuitMarker.Flags)0);
	}

	public bool DoesTraversalDirectionRequireSuit(int source_cell, int dest_cell)
	{
		return DoesTraversalDirectionRequireSuit(source_cell, dest_cell, gridFlags);
	}

	private void Update()
	{
		ListPool<SuitLocker, SuitMarker>.PooledList pooledList = ListPool<SuitLocker, SuitMarker>.Allocate();
		GetAttachedLockers(pooledList);
		int num = 0;
		int num2 = 0;
		KPrefabID x = null;
		foreach (SuitLocker item in pooledList)
		{
			if (item.CanDropOffSuit())
			{
				num++;
			}
			if ((UnityEngine.Object)item.GetPartiallyChargedOutfit() != (UnityEngine.Object)null)
			{
				num2++;
			}
			if ((UnityEngine.Object)x == (UnityEngine.Object)null)
			{
				x = item.GetStoredOutfit();
			}
		}
		pooledList.Recycle();
		bool flag = (UnityEngine.Object)x != (UnityEngine.Object)null;
		if (flag != hasAvailableSuit)
		{
			GetComponent<KAnimControllerBase>().Play((!flag) ? "no_suit" : "off", KAnim.PlayMode.Once, 1f, 0f);
			hasAvailableSuit = flag;
		}
		Grid.UpdateSuitMarker(cell, num2, num, gridFlags, PathFlag);
	}

	private void RefreshTraverseIfUnequipStatusItem()
	{
		if (OnlyTraverseIfUnequipAvailable)
		{
			GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.SuitMarkerTraversalOnlyWhenRoomAvailable, null);
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.SuitMarkerTraversalAnytime, false);
		}
		else
		{
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.SuitMarkerTraversalOnlyWhenRoomAvailable, false);
			GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.SuitMarkerTraversalAnytime, null);
		}
	}

	private void OnEnableTraverseIfUnequipAvailable()
	{
		OnlyTraverseIfUnequipAvailable = true;
		RefreshTraverseIfUnequipStatusItem();
	}

	private void OnDisableTraverseIfUnequipAvailable()
	{
		OnlyTraverseIfUnequipAvailable = false;
		RefreshTraverseIfUnequipStatusItem();
	}

	private void UpdateGridFlag(Grid.SuitMarker.Flags flag, bool state)
	{
		if (state)
		{
			gridFlags |= flag;
		}
		else
		{
			gridFlags &= (Grid.SuitMarker.Flags)(byte)(~(uint)flag);
		}
	}

	private void OnOperationalChanged(bool isOperational)
	{
		SuitLocker.UpdateSuitMarkerStates(Grid.PosToCell(base.transform.position), base.gameObject);
		this.isOperational = isOperational;
	}

	private void OnRefreshUserMenu(object data)
	{
		object buttonInfo;
		if (!OnlyTraverseIfUnequipAvailable)
		{
			string iconName = "action_clearance";
			string text = UI.USERMENUACTIONS.SUIT_MARKER_TRAVERSAL.ONLY_WHEN_ROOM_AVAILABLE.NAME;
			System.Action on_click = OnEnableTraverseIfUnequipAvailable;
			string tooltipText = UI.USERMENUACTIONS.SUIT_MARKER_TRAVERSAL.ONLY_WHEN_ROOM_AVAILABLE.TOOLTIP;
			buttonInfo = new KIconButtonMenu.ButtonInfo(iconName, text, on_click, Action.NumActions, null, null, null, tooltipText, true);
		}
		else
		{
			string tooltipText = "action_clearance";
			string text = UI.USERMENUACTIONS.SUIT_MARKER_TRAVERSAL.ALWAYS.NAME;
			System.Action on_click = OnDisableTraverseIfUnequipAvailable;
			string iconName = UI.USERMENUACTIONS.SUIT_MARKER_TRAVERSAL.ALWAYS.TOOLTIP;
			buttonInfo = new KIconButtonMenu.ButtonInfo(tooltipText, text, on_click, Action.NumActions, null, null, null, iconName, true);
		}
		KIconButtonMenu.ButtonInfo button = (KIconButtonMenu.ButtonInfo)buttonInfo;
		Game.Instance.userMenu.AddButton(base.gameObject, button, 1f);
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		if (base.isSpawned)
		{
			Grid.UnregisterSuitMarker(cell);
		}
		if (partitionerEntry != null)
		{
			partitionerEntry.Release();
			partitionerEntry = null;
		}
		if (reactable != null)
		{
			reactable.Cleanup();
		}
		SuitLocker.UpdateSuitMarkerStates(Grid.PosToCell(base.transform.position), null);
	}
}
