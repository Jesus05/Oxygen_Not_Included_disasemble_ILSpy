using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SuitMarker : KMonoBehaviour, Pathfinding.INavigationFeature
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
			if (!suitMarker.GetComponent<Operational>().IsOperational)
			{
				return false;
			}
			Rotatable component = gameObject.GetComponent<Rotatable>();
			SuitWearer.Instance sMI = new_reactor.GetSMI<SuitWearer.Instance>();
			int num = transition.navGridTransition.x;
			if (num == 0)
			{
				return false;
			}
			if (new_reactor.GetComponent<MinionIdentity>().GetEquipment().IsSlotOccupied(Db.Get().AssignableSlots.Suit))
			{
				if (num < 0 && component.IsRotated)
				{
					return false;
				}
				if (num > 0 && !component.IsRotated)
				{
					return false;
				}
				return true;
			}
			if (num > 0 && component.IsRotated)
			{
				return false;
			}
			if (num < 0 && !component.IsRotated)
			{
				return false;
			}
			return suitMarker.IsSuitAvailableForTraversal(sMI);
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
			if ((UnityEngine.Object)suitMarker != (UnityEngine.Object)null)
			{
				Rotatable component = suitMarker.GetComponent<Rotatable>();
				Facing facing = (!(bool)reactor) ? null : reactor.GetComponent<Facing>();
				if ((bool)facing)
				{
					facing.SetFacing(component.GetOrientation() == Orientation.FlipH);
				}
			}
			if (Time.time - startTime > 2.8f)
			{
				Run();
				Cleanup();
			}
		}

		private void Run()
		{
			if ((UnityEngine.Object)base.reactor != (UnityEngine.Object)null)
			{
				GameObject reactor = base.reactor;
				bool flag = !reactor.GetComponent<MinionIdentity>().GetEquipment().IsSlotOccupied(Db.Get().AssignableSlots.Suit);
				if ((UnityEngine.Object)suitMarker != (UnityEngine.Object)null)
				{
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
								item.EquipTo(reactor.GetComponent<MinionIdentity>().GetEquipment());
								flag2 = true;
								break;
							}
							if (!flag && item.CanDropOffSuit())
							{
								item.UnequipFrom(reactor.GetComponent<MinionIdentity>().GetEquipment());
								flag2 = true;
								break;
							}
						}
						pooledList.Recycle();
					}
					if (!flag2 && !flag)
					{
						Assignable assignable = reactor.GetComponent<MinionIdentity>().GetEquipment().GetAssignable(Db.Get().AssignableSlots.Suit);
						assignable.Unassign();
						Notification notification = new Notification(MISC.NOTIFICATIONS.SUIT_DROPPED.NAME, NotificationType.BadMinor, HashedString.Invalid, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.SUIT_DROPPED.TOOLTIP, null, true, 0f, null, null);
						assignable.GetComponent<Notifier>().Add(notification, string.Empty);
					}
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

	private List<SuitWearer.Instance> equipReservations = new List<SuitWearer.Instance>();

	private List<SuitWearer.Instance> unequipReservations = new List<SuitWearer.Instance>();

	[Serialize]
	private bool onlyTraverseIfUnequipAvailable;

	public Tag[] LockerTags;

	public PathFinder.PotentialPath.Flags PathFlag;

	public KAnimFile interactAnim = Assets.GetAnim("anim_equip_clothing_kanim");

	private static readonly EventSystem.IntraObjectHandler<SuitMarker> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<SuitMarker>(delegate(SuitMarker component, object data)
	{
		component.OnRefreshUserMenu(data);
	});

	private static readonly EventSystem.IntraObjectHandler<SuitMarker> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<SuitMarker>(delegate(SuitMarker component, object data)
	{
		component.OnOperationalChanged(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(493375141, OnRefreshUserMenuDelegate);
		Subscribe(-592767678, OnOperationalChangedDelegate);
		CreateNewReactable();
		Pathfinding.Instance.AddNavigationFeature(Grid.PosToCell(this), this);
		GetComponent<KAnimControllerBase>().Play("no_suit", KAnim.PlayMode.Once, 1f, 0f);
		Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Suits);
		RefreshTraverseIfUnequipStatusItem();
		SuitLocker.UpdateSuitMarkerStates(Grid.PosToCell(base.transform.position), base.gameObject);
	}

	private void CreateNewReactable()
	{
		reactable = new SuitMarkerReactable(this);
	}

	public void GetAttachedLockers(List<SuitLocker> suit_lockers)
	{
		int num = -1;
		if (GetComponent<Rotatable>().IsRotated)
		{
			num = 1;
		}
		int cell = Grid.PosToCell(this);
		int num2 = 1;
		while (true)
		{
			int cell2 = Grid.OffsetCell(cell, num2 * num, 0);
			GameObject gameObject = Grid.Objects[cell2, 1];
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

	private KPrefabID GetAvailableSuit()
	{
		ListPool<SuitLocker, SuitMarker>.PooledList pooledList = ListPool<SuitLocker, SuitMarker>.Allocate();
		GetAttachedLockers(pooledList);
		KPrefabID kPrefabID = null;
		foreach (SuitLocker item in pooledList)
		{
			kPrefabID = item.GetStoredOutfit();
			if ((UnityEngine.Object)kPrefabID != (UnityEngine.Object)null)
			{
				break;
			}
		}
		pooledList.Recycle();
		return kPrefabID;
	}

	public bool DoesTraversalDirectionRequireSuit(int source_cell, int dest_cell)
	{
		Grid.CellToXY(source_cell, out int x, out int _);
		Grid.CellToXY(dest_cell, out int x2, out int _);
		bool flag = x2 > x;
		bool isRotated = GetComponent<Rotatable>().IsRotated;
		return (flag && !isRotated) || (!flag && isRotated);
	}

	private int GetFullyChargedSuitCount()
	{
		int num = 0;
		ListPool<SuitLocker, SuitMarker>.PooledList pooledList = ListPool<SuitLocker, SuitMarker>.Allocate();
		GetAttachedLockers(pooledList);
		foreach (SuitLocker item in pooledList)
		{
			if ((UnityEngine.Object)item.GetFullyChargedOutfit() != (UnityEngine.Object)null)
			{
				num++;
			}
		}
		pooledList.Recycle();
		return num;
	}

	public bool IsSuitAvailableForTraversal(SuitWearer.Instance suit_wearer)
	{
		int fullyChargedSuitCount = GetFullyChargedSuitCount();
		int count = equipReservations.Count;
		if (count < fullyChargedSuitCount)
		{
			return true;
		}
		if (count == fullyChargedSuitCount && equipReservations.Contains(suit_wearer))
		{
			return true;
		}
		return false;
	}

	public bool IsUnequipAvailableForSuitWearer(SuitWearer.Instance suit_wearer)
	{
		int num = 0;
		ListPool<SuitLocker, SuitMarker>.PooledList pooledList = ListPool<SuitLocker, SuitMarker>.Allocate();
		GetAttachedLockers(pooledList);
		foreach (SuitLocker item in pooledList)
		{
			if (item.CanDropOffSuit())
			{
				num++;
			}
		}
		pooledList.Recycle();
		if (num > unequipReservations.Count)
		{
			return true;
		}
		if (num == unequipReservations.Count)
		{
			return unequipReservations.Contains(suit_wearer);
		}
		return false;
	}

	public bool IsTraversable(Navigator agent, PathFinder.PotentialPath path, int from_cell, int cost, PathFinderAbilities abilities)
	{
		if (!GetComponent<Operational>().IsOperational)
		{
			return true;
		}
		if (!path.HasFlag(PathFinder.PotentialPath.Flags.PerformSuitChecks))
		{
			return true;
		}
		SuitWearer.Instance sMI = agent.GetSMI<SuitWearer.Instance>();
		bool flag = DoesTraversalDirectionRequireSuit(from_cell, path.cell);
		bool flag2 = path.HasFlag(PathFlag);
		bool flag3 = path.HasFlag(PathFinder.PotentialPath.Flags.HasAtmoSuit) | path.HasFlag(PathFinder.PotentialPath.Flags.HasJetPack);
		if (flag)
		{
			if (flag3)
			{
				return true;
			}
			if (IsSuitAvailableForTraversal(sMI))
			{
				return true;
			}
			return false;
		}
		if (flag3 && onlyTraverseIfUnequipAvailable)
		{
			return flag2 && IsUnequipAvailableForSuitWearer(sMI);
		}
		return true;
	}

	public void ApplyTraversalToPath(Navigator agent, ref PathFinder.PotentialPath path, int from_cell)
	{
		if (path.HasFlag(PathFinder.PotentialPath.Flags.PerformSuitChecks) && GetComponent<Operational>().IsOperational)
		{
			if (DoesTraversalDirectionRequireSuit(from_cell, path.cell))
			{
				bool flag = path.HasFlag(PathFlag);
				if (!(path.HasFlag(PathFinder.PotentialPath.Flags.HasAtmoSuit) | path.HasFlag(PathFinder.PotentialPath.Flags.HasJetPack)) || flag)
				{
					path.SetFlags(PathFlag);
				}
			}
			else
			{
				path.ClearFlags(PathFinder.PotentialPath.Flags.HasAtmoSuit | PathFinder.PotentialPath.Flags.HasJetPack);
			}
		}
	}

	public void Reserve(SuitWearer.Instance suit_wearer, bool reserve_for_equipping)
	{
		if (reserve_for_equipping)
		{
			if (equipReservations.Contains(suit_wearer))
			{
				Output.LogWarningWithObj(base.gameObject, "Reserve called more than once for same suit wearer: " + suit_wearer.gameObject);
			}
			else if (!IsSuitAvailableForTraversal(suit_wearer))
			{
				Output.LogWarningWithObj(base.gameObject, "Reserve called with no suit available: " + suit_wearer.gameObject);
			}
			else
			{
				equipReservations.Add(suit_wearer);
			}
		}
		else
		{
			unequipReservations.Add(suit_wearer);
		}
	}

	public void Unreserve(SuitWearer.Instance suit_wearer, bool unreserve_for_equipping)
	{
		if (unreserve_for_equipping)
		{
			equipReservations.Remove(suit_wearer);
		}
		else
		{
			unequipReservations.Remove(suit_wearer);
		}
	}

	private void Update()
	{
		bool flag = (UnityEngine.Object)GetAvailableSuit() != (UnityEngine.Object)null;
		if (flag != hasAvailableSuit)
		{
			if (flag)
			{
				GetComponent<KAnimControllerBase>().Play("off", KAnim.PlayMode.Once, 1f, 0f);
			}
			else
			{
				GetComponent<KAnimControllerBase>().Play("no_suit", KAnim.PlayMode.Once, 1f, 0f);
			}
			hasAvailableSuit = flag;
		}
	}

	private void RefreshTraverseIfUnequipStatusItem()
	{
		if (onlyTraverseIfUnequipAvailable)
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
		onlyTraverseIfUnequipAvailable = true;
		RefreshTraverseIfUnequipStatusItem();
	}

	private void OnDisableTraverseIfUnequipAvailable()
	{
		onlyTraverseIfUnequipAvailable = false;
		RefreshTraverseIfUnequipStatusItem();
	}

	private void OnOperationalChanged(object data)
	{
		SuitLocker.UpdateSuitMarkerStates(Grid.PosToCell(base.transform.position), base.gameObject);
	}

	private void OnRefreshUserMenu(object data)
	{
		object buttonInfo;
		if (!onlyTraverseIfUnequipAvailable)
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
			Pathfinding.Instance.RemoveNavigationFeature(Grid.PosToCell(this), this);
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
