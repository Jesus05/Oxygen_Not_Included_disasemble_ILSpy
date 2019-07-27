using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class SuitLocker : StateMachineComponent<SuitLocker.StatesInstance>
{
	public class ReturnSuitWorkable : Workable
	{
		public static readonly Chore.Precondition DoesSuitNeedRechargingUrgent = new Chore.Precondition
		{
			id = "DoesSuitNeedRechargingUrgent",
			description = (string)DUPLICANTS.CHORES.PRECONDITIONS.DOES_SUIT_NEED_RECHARGING_URGENT,
			fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
			{
				Equipment equipment2 = context.consumerState.equipment;
				AssignableSlotInstance slot2 = equipment2.GetSlot(Db.Get().AssignableSlots.Suit);
				if (!((UnityEngine.Object)slot2.assignable == (UnityEngine.Object)null))
				{
					SuitTank component2 = slot2.assignable.GetComponent<SuitTank>();
					if (!((UnityEngine.Object)component2 == (UnityEngine.Object)null))
					{
						if (!component2.NeedsRecharging())
						{
							JetSuitTank component3 = slot2.assignable.GetComponent<JetSuitTank>();
							if (!((UnityEngine.Object)component3 == (UnityEngine.Object)null))
							{
								if (!component3.NeedsRecharging())
								{
									return false;
								}
								return true;
							}
							return false;
						}
						return true;
					}
					return false;
				}
				return false;
			}
		};

		public static readonly Chore.Precondition DoesSuitNeedRechargingIdle = new Chore.Precondition
		{
			id = "DoesSuitNeedRechargingIdle",
			description = (string)DUPLICANTS.CHORES.PRECONDITIONS.DOES_SUIT_NEED_RECHARGING_IDLE,
			fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
			{
				Equipment equipment = context.consumerState.equipment;
				AssignableSlotInstance slot = equipment.GetSlot(Db.Get().AssignableSlots.Suit);
				if (!((UnityEngine.Object)slot.assignable == (UnityEngine.Object)null))
				{
					SuitTank component = slot.assignable.GetComponent<SuitTank>();
					if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
					{
						return true;
					}
					return false;
				}
				return false;
			}
		};

		public Chore.Precondition HasSuitMarker = new Chore.Precondition
		{
			id = "IsValid",
			description = (string)DUPLICANTS.CHORES.PRECONDITIONS.HAS_SUIT_MARKER,
			fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
			{
				SuitLocker suitLocker2 = (SuitLocker)data;
				return suitLocker2.suitMarkerState == SuitMarkerState.HasMarker;
			}
		};

		public Chore.Precondition SuitTypeMatchesLocker = new Chore.Precondition
		{
			id = "IsValid",
			description = (string)DUPLICANTS.CHORES.PRECONDITIONS.HAS_SUIT_MARKER,
			fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
			{
				SuitLocker suitLocker = (SuitLocker)data;
				Equipment equipment = context.consumerState.equipment;
				AssignableSlotInstance slot = equipment.GetSlot(Db.Get().AssignableSlots.Suit);
				if (!((UnityEngine.Object)slot.assignable == (UnityEngine.Object)null))
				{
					bool flag = (UnityEngine.Object)slot.assignable.GetComponent<JetSuitTank>() != (UnityEngine.Object)null;
					bool flag2 = (UnityEngine.Object)suitLocker.GetComponent<JetSuitLocker>() != (UnityEngine.Object)null;
					return flag == flag2;
				}
				return false;
			}
		};

		private WorkChore<ReturnSuitWorkable> urgentChore;

		private WorkChore<ReturnSuitWorkable> idleChore;

		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			resetProgressOnStop = true;
			workTime = 0.25f;
			synchronizeAnims = false;
		}

		public void CreateChore()
		{
			if (urgentChore == null)
			{
				SuitLocker component = GetComponent<SuitLocker>();
				urgentChore = new WorkChore<ReturnSuitWorkable>(Db.Get().ChoreTypes.ReturnSuitUrgent, this, null, true, null, null, null, true, null, false, false, null, false, true, false, PriorityScreen.PriorityClass.personalNeeds, 5, false, false);
				urgentChore.AddPrecondition(DoesSuitNeedRechargingUrgent, null);
				urgentChore.AddPrecondition(HasSuitMarker, component);
				urgentChore.AddPrecondition(SuitTypeMatchesLocker, component);
				idleChore = new WorkChore<ReturnSuitWorkable>(Db.Get().ChoreTypes.ReturnSuitIdle, this, null, true, null, null, null, true, null, false, false, null, false, true, false, PriorityScreen.PriorityClass.idle, 5, false, false);
				idleChore.AddPrecondition(DoesSuitNeedRechargingIdle, null);
				idleChore.AddPrecondition(HasSuitMarker, component);
				idleChore.AddPrecondition(SuitTypeMatchesLocker, component);
			}
		}

		public void CancelChore()
		{
			if (urgentChore != null)
			{
				urgentChore.Cancel("ReturnSuitWorkable.CancelChore");
				urgentChore = null;
			}
			if (idleChore != null)
			{
				idleChore.Cancel("ReturnSuitWorkable.CancelChore");
				idleChore = null;
			}
		}

		protected override void OnStartWork(Worker worker)
		{
			ShowProgressBar(false);
		}

		protected override bool OnWorkTick(Worker worker, float dt)
		{
			return true;
		}

		protected override void OnCompleteWork(Worker worker)
		{
			Equipment equipment = worker.GetComponent<MinionIdentity>().GetEquipment();
			if (equipment.IsSlotOccupied(Db.Get().AssignableSlots.Suit))
			{
				SuitLocker component = GetComponent<SuitLocker>();
				if (component.CanDropOffSuit())
				{
					GetComponent<SuitLocker>().UnequipFrom(equipment);
				}
				else
				{
					Assignable assignable = equipment.GetAssignable(Db.Get().AssignableSlots.Suit);
					assignable.Unassign();
				}
			}
			if (urgentChore != null)
			{
				CancelChore();
				CreateChore();
			}
		}

		public override HashedString[] GetWorkAnims(Worker worker)
		{
			return new HashedString[1]
			{
				new HashedString("none")
			};
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, SuitLocker, object>.GameInstance
	{
		public StatesInstance(SuitLocker suit_locker)
			: base(suit_locker)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, SuitLocker>
	{
		public class ChargingStates : State
		{
			public State pre;

			public State pst;

			public State operational;

			public State nooxygen;

			public State notoperational;
		}

		public class EmptyStates : State
		{
			public State configured;

			public State notconfigured;
		}

		public EmptyStates empty;

		public ChargingStates charging;

		public State waitingforsuit;

		public State suitfullycharged;

		public BoolParameter isWaitingForSuit;

		public BoolParameter isConfigured;

		public BoolParameter hasSuitMarker;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = empty;
			base.serializable = true;
			root.Update("RefreshMeter", delegate(StatesInstance smi, float dt)
			{
				smi.master.RefreshMeter();
			}, UpdateRate.RENDER_200ms, false);
			empty.DefaultState(empty.notconfigured).EventTransition(GameHashes.OnStorageChange, charging, (StatesInstance smi) => (UnityEngine.Object)smi.master.GetStoredOutfit() != (UnityEngine.Object)null).ParamTransition(isWaitingForSuit, waitingforsuit, GameStateMachine<States, StatesInstance, SuitLocker, object>.IsTrue)
				.Enter("CreateReturnSuitChore", delegate(StatesInstance smi)
				{
					smi.master.returnSuitWorkable.CreateChore();
				})
				.RefreshUserMenuOnEnter()
				.Exit("CancelReturnSuitChore", delegate(StatesInstance smi)
				{
					smi.master.returnSuitWorkable.CancelChore();
				})
				.PlayAnim("no_suit_pre")
				.QueueAnim("no_suit", false, null);
			State state = empty.notconfigured.ParamTransition(isConfigured, empty.configured, GameStateMachine<States, StatesInstance, SuitLocker, object>.IsTrue);
			string name = BUILDING.STATUSITEMS.SUIT_LOCKER_NEEDS_CONFIGURATION.NAME;
			string tooltip = BUILDING.STATUSITEMS.SUIT_LOCKER_NEEDS_CONFIGURATION.TOOLTIP;
			string icon = "status_item_no_filter_set";
			StatusItem.IconType icon_type = StatusItem.IconType.Custom;
			NotificationType notification_type = NotificationType.BadMinor;
			StatusItemCategory main = Db.Get().StatusItemCategories.Main;
			state.ToggleStatusItem(name, tooltip, icon, icon_type, notification_type, false, default(HashedString), 0, null, null, main);
			State state2 = empty.configured.RefreshUserMenuOnEnter();
			icon = BUILDING.STATUSITEMS.SUIT_LOCKER.READY.NAME;
			tooltip = BUILDING.STATUSITEMS.SUIT_LOCKER.READY.TOOLTIP;
			main = Db.Get().StatusItemCategories.Main;
			state2.ToggleStatusItem(icon, tooltip, "", StatusItem.IconType.Info, (NotificationType)0, false, default(HashedString), 0, null, null, main);
			State state3 = waitingforsuit.EventTransition(GameHashes.OnStorageChange, charging, (StatesInstance smi) => (UnityEngine.Object)smi.master.GetStoredOutfit() != (UnityEngine.Object)null).Enter("CreateFetchChore", delegate(StatesInstance smi)
			{
				smi.master.CreateFetchChore();
			}).ParamTransition(isWaitingForSuit, empty, GameStateMachine<States, StatesInstance, SuitLocker, object>.IsFalse)
				.RefreshUserMenuOnEnter()
				.PlayAnim("no_suit_pst")
				.QueueAnim("awaiting_suit", false, null)
				.Exit("ClearIsWaitingForSuit", delegate(StatesInstance smi)
				{
					isWaitingForSuit.Set(false, smi);
				})
				.Exit("CancelFetchChore", delegate(StatesInstance smi)
				{
					smi.master.CancelFetchChore();
				});
			tooltip = BUILDING.STATUSITEMS.SUIT_LOCKER.SUIT_REQUESTED.NAME;
			icon = BUILDING.STATUSITEMS.SUIT_LOCKER.SUIT_REQUESTED.TOOLTIP;
			main = Db.Get().StatusItemCategories.Main;
			state3.ToggleStatusItem(tooltip, icon, "", StatusItem.IconType.Info, (NotificationType)0, false, default(HashedString), 0, null, null, main);
			charging.DefaultState(charging.pre).RefreshUserMenuOnEnter().EventTransition(GameHashes.OnStorageChange, empty, (StatesInstance smi) => (UnityEngine.Object)smi.master.GetStoredOutfit() == (UnityEngine.Object)null);
			charging.pre.Enter(delegate(StatesInstance smi)
			{
				if (smi.master.IsSuitFullyCharged())
				{
					smi.GoTo(suitfullycharged);
				}
				else
				{
					smi.GetComponent<KBatchedAnimController>().Play("no_suit_pst", KAnim.PlayMode.Once, 1f, 0f);
					smi.GetComponent<KBatchedAnimController>().Queue("charging_pre", KAnim.PlayMode.Once, 1f, 0f);
				}
			}).OnAnimQueueComplete(charging.operational);
			State state4 = charging.operational.TagTransition(GameTags.Operational, charging.notoperational, true).Transition(charging.nooxygen, (StatesInstance smi) => !smi.master.HasOxygen(), UpdateRate.SIM_200ms).PlayAnim("charging_loop", KAnim.PlayMode.Loop)
				.Enter("SetActive", delegate(StatesInstance smi)
				{
					smi.master.GetComponent<Operational>().SetActive(true, false);
				})
				.Transition(charging.pst, (StatesInstance smi) => smi.master.IsSuitFullyCharged(), UpdateRate.SIM_200ms)
				.Update("ChargeSuit", delegate(StatesInstance smi, float dt)
				{
					smi.master.ChargeSuit(dt);
				}, UpdateRate.SIM_200ms, false)
				.Exit("ClearActive", delegate(StatesInstance smi)
				{
					smi.master.GetComponent<Operational>().SetActive(false, false);
				});
			icon = BUILDING.STATUSITEMS.SUIT_LOCKER.CHARGING.NAME;
			tooltip = BUILDING.STATUSITEMS.SUIT_LOCKER.CHARGING.TOOLTIP;
			main = Db.Get().StatusItemCategories.Main;
			state4.ToggleStatusItem(icon, tooltip, "", StatusItem.IconType.Info, (NotificationType)0, false, default(HashedString), 0, null, null, main);
			State state5 = charging.nooxygen.TagTransition(GameTags.Operational, charging.notoperational, true).Transition(charging.operational, (StatesInstance smi) => smi.master.HasOxygen(), UpdateRate.SIM_200ms).Transition(charging.pst, (StatesInstance smi) => smi.master.IsSuitFullyCharged(), UpdateRate.SIM_200ms)
				.PlayAnim("no_o2_loop", KAnim.PlayMode.Loop);
			tooltip = BUILDING.STATUSITEMS.SUIT_LOCKER.NO_OXYGEN.NAME;
			icon = BUILDING.STATUSITEMS.SUIT_LOCKER.NO_OXYGEN.TOOLTIP;
			name = "status_item_suit_locker_no_oxygen";
			icon_type = StatusItem.IconType.Custom;
			notification_type = NotificationType.BadMinor;
			main = Db.Get().StatusItemCategories.Main;
			state5.ToggleStatusItem(tooltip, icon, name, icon_type, notification_type, false, default(HashedString), 0, null, null, main);
			State state6 = charging.notoperational.TagTransition(GameTags.Operational, charging.operational, false).PlayAnim("not_charging_loop", KAnim.PlayMode.Loop).Transition(charging.pst, (StatesInstance smi) => smi.master.IsSuitFullyCharged(), UpdateRate.SIM_200ms);
			name = BUILDING.STATUSITEMS.SUIT_LOCKER.NOT_OPERATIONAL.NAME;
			icon = BUILDING.STATUSITEMS.SUIT_LOCKER.NOT_OPERATIONAL.TOOLTIP;
			main = Db.Get().StatusItemCategories.Main;
			state6.ToggleStatusItem(name, icon, "", StatusItem.IconType.Info, (NotificationType)0, false, default(HashedString), 0, null, null, main);
			charging.pst.PlayAnim("charging_pst").OnAnimQueueComplete(suitfullycharged);
			State state7 = suitfullycharged.EventTransition(GameHashes.OnStorageChange, empty, (StatesInstance smi) => (UnityEngine.Object)smi.master.GetStoredOutfit() == (UnityEngine.Object)null).PlayAnim("has_suit").RefreshUserMenuOnEnter();
			icon = BUILDING.STATUSITEMS.SUIT_LOCKER.FULLY_CHARGED.NAME;
			name = BUILDING.STATUSITEMS.SUIT_LOCKER.FULLY_CHARGED.TOOLTIP;
			main = Db.Get().StatusItemCategories.Main;
			state7.ToggleStatusItem(icon, name, "", StatusItem.IconType.Info, (NotificationType)0, false, default(HashedString), 0, null, null, main);
		}
	}

	private enum SuitMarkerState
	{
		HasMarker,
		NoMarker,
		WrongSide,
		NotOperational
	}

	private struct SuitLockerEntry
	{
		public class Comparer : IComparer<SuitLockerEntry>
		{
			public int Compare(SuitLockerEntry a, SuitLockerEntry b)
			{
				return a.cell - b.cell;
			}
		}

		public SuitLocker suitLocker;

		public int cell;

		public static Comparer comparer = new Comparer();
	}

	private struct SuitMarkerEntry
	{
		public SuitMarker suitMarker;

		public int cell;
	}

	[MyCmpGet]
	private Building building;

	public Tag[] OutfitTags;

	private FetchChore fetchChore;

	[MyCmpAdd]
	public ReturnSuitWorkable returnSuitWorkable;

	private MeterController meter;

	private SuitMarkerState suitMarkerState = SuitMarkerState.HasMarker;

	public float OxygenAvailable
	{
		get
		{
			GameObject oxygen = GetOxygen();
			float result = 0f;
			if ((UnityEngine.Object)oxygen != (UnityEngine.Object)null)
			{
				result = oxygen.GetComponent<PrimaryElement>().Mass / GetComponent<ConduitConsumer>().capacityKG;
				result = Math.Min(result, 1f);
			}
			return result;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_target", "meter_arrow", "meter_scale");
		UpdateSuitMarkerStates(Grid.PosToCell(base.transform.position), base.gameObject);
		base.smi.StartSM();
		Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Suits, true);
	}

	public KPrefabID GetStoredOutfit()
	{
		foreach (GameObject item in GetComponent<Storage>().items)
		{
			if (!((UnityEngine.Object)item == (UnityEngine.Object)null))
			{
				KPrefabID component = item.GetComponent<KPrefabID>();
				if (!((UnityEngine.Object)component == (UnityEngine.Object)null) && component.HasAnyTags(OutfitTags))
				{
					return component;
				}
			}
		}
		return null;
	}

	public float GetSuitScore()
	{
		float num = -1f;
		KPrefabID partiallyChargedOutfit = GetPartiallyChargedOutfit();
		if ((bool)partiallyChargedOutfit)
		{
			num = partiallyChargedOutfit.GetComponent<SuitTank>().PercentFull();
			JetSuitTank component = partiallyChargedOutfit.GetComponent<JetSuitTank>();
			if ((bool)component && component.PercentFull() < num)
			{
				num = component.PercentFull();
			}
		}
		return num;
	}

	public KPrefabID GetPartiallyChargedOutfit()
	{
		KPrefabID storedOutfit = GetStoredOutfit();
		if (!(bool)storedOutfit)
		{
			return null;
		}
		if (!(storedOutfit.GetComponent<SuitTank>().PercentFull() < TUNING.EQUIPMENT.SUITS.MINIMUM_USABLE_SUIT_CHARGE))
		{
			JetSuitTank component = storedOutfit.GetComponent<JetSuitTank>();
			if ((bool)component && component.PercentFull() < TUNING.EQUIPMENT.SUITS.MINIMUM_USABLE_SUIT_CHARGE)
			{
				return null;
			}
			return storedOutfit;
		}
		return null;
	}

	public KPrefabID GetFullyChargedOutfit()
	{
		KPrefabID storedOutfit = GetStoredOutfit();
		if (!(bool)storedOutfit)
		{
			return null;
		}
		if (storedOutfit.GetComponent<SuitTank>().IsFull())
		{
			JetSuitTank component = storedOutfit.GetComponent<JetSuitTank>();
			if ((bool)component && !component.IsFull())
			{
				return null;
			}
			return storedOutfit;
		}
		return null;
	}

	private void CreateFetchChore()
	{
		ChoreType storageFetch = Db.Get().ChoreTypes.StorageFetch;
		Storage component = GetComponent<Storage>();
		float amount = 1f;
		Tag[] outfitTags = OutfitTags;
		Tag[] forbidden_tags = new Tag[1]
		{
			GameTags.Assigned
		};
		fetchChore = new FetchChore(storageFetch, component, amount, outfitTags, null, forbidden_tags, null, true, null, null, null, FetchOrder2.OperationalRequirement.None, 0);
		fetchChore.allowMultifetch = false;
	}

	private void CancelFetchChore()
	{
		if (fetchChore != null)
		{
			fetchChore.Cancel("SuitLocker.CancelFetchChore");
			fetchChore = null;
		}
	}

	public bool HasOxygen()
	{
		GameObject oxygen = GetOxygen();
		if (!((UnityEngine.Object)oxygen != (UnityEngine.Object)null))
		{
			return false;
		}
		return oxygen.GetComponent<PrimaryElement>().Mass > 0f;
	}

	private void RefreshMeter()
	{
		GameObject oxygen = GetOxygen();
		float positionPercent = 0f;
		if ((UnityEngine.Object)oxygen != (UnityEngine.Object)null)
		{
			positionPercent = oxygen.GetComponent<PrimaryElement>().Mass / GetComponent<ConduitConsumer>().capacityKG;
			positionPercent = Math.Min(positionPercent, 1f);
		}
		meter.SetPositionPercent(positionPercent);
	}

	public bool IsSuitFullyCharged()
	{
		KPrefabID storedOutfit = GetStoredOutfit();
		if (!((UnityEngine.Object)storedOutfit != (UnityEngine.Object)null))
		{
			return false;
		}
		SuitTank component = storedOutfit.GetComponent<SuitTank>();
		if ((UnityEngine.Object)component != (UnityEngine.Object)null && component.PercentFull() < 1f)
		{
			return false;
		}
		JetSuitTank component2 = storedOutfit.GetComponent<JetSuitTank>();
		if ((UnityEngine.Object)component2 != (UnityEngine.Object)null && component2.PercentFull() < 1f)
		{
			return false;
		}
		return true;
	}

	public bool IsOxygenTankFull()
	{
		KPrefabID storedOutfit = GetStoredOutfit();
		if (!((UnityEngine.Object)storedOutfit != (UnityEngine.Object)null))
		{
			return false;
		}
		SuitTank component = storedOutfit.GetComponent<SuitTank>();
		if (!((UnityEngine.Object)component == (UnityEngine.Object)null))
		{
			return component.PercentFull() >= 1f;
		}
		return true;
	}

	private void OnRequestOutfit()
	{
		base.smi.sm.isWaitingForSuit.Set(true, base.smi);
	}

	private void OnCancelRequest()
	{
		base.smi.sm.isWaitingForSuit.Set(false, base.smi);
	}

	public void DropSuit()
	{
		KPrefabID storedOutfit = GetStoredOutfit();
		if (!((UnityEngine.Object)storedOutfit == (UnityEngine.Object)null))
		{
			GetComponent<Storage>().Drop(storedOutfit.gameObject, true);
		}
	}

	public void EquipTo(Equipment equipment)
	{
		KPrefabID storedOutfit = GetStoredOutfit();
		if (!((UnityEngine.Object)storedOutfit == (UnityEngine.Object)null))
		{
			GetComponent<Storage>().Drop(storedOutfit.gameObject, true);
			storedOutfit.GetComponent<Equippable>().Assign(equipment.GetComponent<IAssignableIdentity>());
			storedOutfit.GetComponent<EquippableWorkable>().CancelChore();
			equipment.Equip(storedOutfit.GetComponent<Equippable>());
			returnSuitWorkable.CreateChore();
		}
	}

	public void UnequipFrom(Equipment equipment)
	{
		Assignable assignable = equipment.GetAssignable(Db.Get().AssignableSlots.Suit);
		assignable.Unassign();
		GetComponent<Storage>().Store(assignable.gameObject, false, false, true, false);
	}

	public void ConfigRequestSuit()
	{
		base.smi.sm.isConfigured.Set(true, base.smi);
		base.smi.sm.isWaitingForSuit.Set(true, base.smi);
	}

	public void ConfigNoSuit()
	{
		base.smi.sm.isConfigured.Set(true, base.smi);
		base.smi.sm.isWaitingForSuit.Set(false, base.smi);
	}

	public bool CanDropOffSuit()
	{
		return base.smi.sm.isConfigured.Get(base.smi) && !base.smi.sm.isWaitingForSuit.Get(base.smi) && (UnityEngine.Object)GetStoredOutfit() == (UnityEngine.Object)null;
	}

	private GameObject GetOxygen()
	{
		return GetComponent<Storage>().FindFirst(GameTags.Oxygen);
	}

	private void ChargeSuit(float dt)
	{
		KPrefabID storedOutfit = GetStoredOutfit();
		if (!((UnityEngine.Object)storedOutfit == (UnityEngine.Object)null))
		{
			GameObject oxygen = GetOxygen();
			if (!((UnityEngine.Object)oxygen == (UnityEngine.Object)null))
			{
				SuitTank component = storedOutfit.GetComponent<SuitTank>();
				float a = component.capacity * 15f * dt / 600f;
				a = Mathf.Min(a, component.capacity - component.amount);
				a = Mathf.Min(oxygen.GetComponent<PrimaryElement>().Mass, a);
				oxygen.GetComponent<PrimaryElement>().Mass -= a;
				component.amount += a;
			}
		}
	}

	public void SetSuitMarker(SuitMarker suit_marker)
	{
		SuitMarkerState suitMarkerState = SuitMarkerState.HasMarker;
		if ((UnityEngine.Object)suit_marker == (UnityEngine.Object)null)
		{
			suitMarkerState = SuitMarkerState.NoMarker;
		}
		else
		{
			Vector3 position = suit_marker.transform.GetPosition();
			float x = position.x;
			Vector3 position2 = base.transform.GetPosition();
			if (x > position2.x && suit_marker.GetComponent<Rotatable>().IsRotated)
			{
				suitMarkerState = SuitMarkerState.WrongSide;
			}
			else
			{
				Vector3 position3 = suit_marker.transform.GetPosition();
				float x2 = position3.x;
				Vector3 position4 = base.transform.GetPosition();
				if (x2 < position4.x && !suit_marker.GetComponent<Rotatable>().IsRotated)
				{
					suitMarkerState = SuitMarkerState.WrongSide;
				}
				else if (!suit_marker.GetComponent<Operational>().IsOperational)
				{
					suitMarkerState = SuitMarkerState.NotOperational;
				}
			}
		}
		if (suitMarkerState != this.suitMarkerState)
		{
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoSuitMarker, false);
			GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.SuitMarkerWrongSide, false);
			switch (suitMarkerState)
			{
			case SuitMarkerState.NoMarker:
				GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoSuitMarker, null);
				break;
			case SuitMarkerState.WrongSide:
				GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.SuitMarkerWrongSide, null);
				break;
			}
			this.suitMarkerState = suitMarkerState;
		}
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		UpdateSuitMarkerStates(Grid.PosToCell(base.transform.position), null);
	}

	private static void GatherSuitBuildings(int cell, int dir, List<SuitLockerEntry> suit_lockers, List<SuitMarkerEntry> suit_markers)
	{
		int num = dir;
		while (true)
		{
			int cell2 = Grid.OffsetCell(cell, num, 0);
			if (Grid.IsValidCell(cell2) && !GatherSuitBuildingsOnCell(cell2, suit_lockers, suit_markers))
			{
				break;
			}
			num += dir;
		}
	}

	private static bool GatherSuitBuildingsOnCell(int cell, List<SuitLockerEntry> suit_lockers, List<SuitMarkerEntry> suit_markers)
	{
		GameObject gameObject = Grid.Objects[cell, 1];
		if (!((UnityEngine.Object)gameObject == (UnityEngine.Object)null))
		{
			SuitMarker component = gameObject.GetComponent<SuitMarker>();
			if (!((UnityEngine.Object)component != (UnityEngine.Object)null))
			{
				SuitLocker component2 = gameObject.GetComponent<SuitLocker>();
				if (!((UnityEngine.Object)component2 != (UnityEngine.Object)null))
				{
					return false;
				}
				suit_lockers.Add(new SuitLockerEntry
				{
					suitLocker = component2,
					cell = cell
				});
				return true;
			}
			suit_markers.Add(new SuitMarkerEntry
			{
				suitMarker = component,
				cell = cell
			});
			return true;
		}
		return false;
	}

	private static SuitMarker FindSuitMarker(int cell, List<SuitMarkerEntry> suit_markers)
	{
		if (Grid.IsValidCell(cell))
		{
			foreach (SuitMarkerEntry suit_marker in suit_markers)
			{
				SuitMarkerEntry current = suit_marker;
				if (current.cell == cell)
				{
					return current.suitMarker;
				}
			}
			return null;
		}
		return null;
	}

	public static void UpdateSuitMarkerStates(int cell, GameObject self)
	{
		ListPool<SuitLockerEntry, SuitLocker>.PooledList pooledList = ListPool<SuitLockerEntry, SuitLocker>.Allocate();
		ListPool<SuitMarkerEntry, SuitLocker>.PooledList pooledList2 = ListPool<SuitMarkerEntry, SuitLocker>.Allocate();
		if ((UnityEngine.Object)self != (UnityEngine.Object)null)
		{
			SuitLocker component = self.GetComponent<SuitLocker>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null)
			{
				pooledList.Add(new SuitLockerEntry
				{
					suitLocker = component,
					cell = cell
				});
			}
			SuitMarker component2 = self.GetComponent<SuitMarker>();
			if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
			{
				pooledList2.Add(new SuitMarkerEntry
				{
					suitMarker = component2,
					cell = cell
				});
			}
		}
		GatherSuitBuildings(cell, 1, pooledList, pooledList2);
		GatherSuitBuildings(cell, -1, pooledList, pooledList2);
		pooledList.Sort(SuitLockerEntry.comparer);
		for (int i = 0; i < pooledList.Count; i++)
		{
			SuitLockerEntry suitLockerEntry = pooledList[i];
			SuitLockerEntry suitLockerEntry2 = suitLockerEntry;
			ListPool<SuitLockerEntry, SuitLocker>.PooledList pooledList3 = ListPool<SuitLockerEntry, SuitLocker>.Allocate();
			pooledList3.Add(suitLockerEntry);
			for (int j = i + 1; j < pooledList.Count; j++)
			{
				SuitLockerEntry suitLockerEntry3 = pooledList[j];
				if (Grid.CellRight(suitLockerEntry2.cell) != suitLockerEntry3.cell)
				{
					break;
				}
				i++;
				suitLockerEntry2 = suitLockerEntry3;
				pooledList3.Add(suitLockerEntry3);
			}
			int cell2 = Grid.CellLeft(suitLockerEntry.cell);
			int cell3 = Grid.CellRight(suitLockerEntry2.cell);
			SuitMarker suitMarker = FindSuitMarker(cell2, pooledList2);
			if ((UnityEngine.Object)suitMarker == (UnityEngine.Object)null)
			{
				suitMarker = FindSuitMarker(cell3, pooledList2);
			}
			foreach (SuitLockerEntry item in pooledList3)
			{
				SuitLockerEntry current = item;
				current.suitLocker.SetSuitMarker(suitMarker);
			}
			pooledList3.Recycle();
		}
		pooledList.Recycle();
		pooledList2.Recycle();
	}
}
