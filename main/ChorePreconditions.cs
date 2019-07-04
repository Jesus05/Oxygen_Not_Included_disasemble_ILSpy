using Database;
using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class ChorePreconditions
{
	private static ChorePreconditions _instance;

	public Chore.Precondition IsPreemptable = new Chore.Precondition
	{
		id = "IsPreemptable",
		sortOrder = 1,
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_PREEMPTABLE,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			return context.isAttemptingOverride || context.chore.CanPreempt(context) || (Object)context.chore.driver == (Object)null;
		}
	};

	public Chore.Precondition HasUrge = new Chore.Precondition
	{
		id = "HasUrge",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.HAS_URGE,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			if (context.chore.choreType.urge != null)
			{
				foreach (Urge urge in context.consumerState.consumer.GetUrges())
				{
					if (context.chore.SatisfiesUrge(urge))
					{
						return true;
					}
				}
				return false;
			}
			return true;
		}
	};

	public Chore.Precondition IsValid = new Chore.Precondition
	{
		id = "IsValid",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_VALID,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			return context.chore.IsValid();
		}
	};

	public Chore.Precondition IsPermitted = new Chore.Precondition
	{
		id = "IsPermitted",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_PERMITTED,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			return context.consumerState.consumer.IsPermittedOrEnabled(context.choreTypeForPermission, context.chore);
		}
	};

	public Chore.Precondition IsAssignedtoMe = new Chore.Precondition
	{
		id = "IsAssignedToMe",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_ASSIGNED_TO_ME,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			Assignable assignable3 = (Assignable)data;
			IAssignableIdentity component = context.consumerState.gameObject.GetComponent<IAssignableIdentity>();
			return assignable3.IsAssignedTo(component);
		}
	};

	public Chore.Precondition IsInMyRoom = new Chore.Precondition
	{
		id = "IsInMyRoom",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_IN_MY_ROOM,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			Room room = (Room)data;
			if (room != null)
			{
				if (!((Object)context.consumerState.ownable != (Object)null))
				{
					Room room2 = null;
					FetchChore fetchChore = context.chore as FetchChore;
					if (fetchChore != null && (Object)fetchChore.destination != (Object)null)
					{
						CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(fetchChore.destination));
						if (cavityForCell != null)
						{
							room2 = cavityForCell.room;
						}
						if (room2 == null)
						{
							return false;
						}
						return room2 == room;
					}
					if (!(context.chore is WorkChore<Tinkerable>))
					{
						return false;
					}
					CavityInfo cavityForCell2 = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell((context.chore as WorkChore<Tinkerable>).gameObject));
					if (cavityForCell2 != null)
					{
						room2 = cavityForCell2.room;
					}
					if (room2 == null)
					{
						return false;
					}
					return room2 == room;
				}
				foreach (Ownables owner in room.GetOwners())
				{
					if ((Object)owner.gameObject == (Object)context.consumerState.gameObject)
					{
						return true;
					}
				}
			}
			return false;
		}
	};

	public Chore.Precondition IsPreferredAssignable = new Chore.Precondition
	{
		id = "IsPreferredAssignable",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_PREFERRED_ASSIGNABLE,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			Assignable assignable2 = (Assignable)data;
			if (!Game.Instance.assignmentManager.GetPreferredAssignables(context.consumerState.assignables, assignable2.slot).Contains(assignable2))
			{
				return false;
			}
			return true;
		}
	};

	public Chore.Precondition IsPreferredAssignableOrUrgentBladder = new Chore.Precondition
	{
		id = "IsPreferredAssignableOrUrgent",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_PREFERRED_ASSIGNABLE_OR_URGENT_BLADDER,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			Assignable assignable = (Assignable)data;
			if (!Game.Instance.assignmentManager.GetPreferredAssignables(context.consumerState.assignables, assignable.slot).Contains(assignable))
			{
				PeeChoreMonitor.Instance sMI4 = context.consumerState.gameObject.GetSMI<PeeChoreMonitor.Instance>();
				return sMI4?.IsInsideState(sMI4.sm.critical) ?? false;
			}
			return true;
		}
	};

	public Chore.Precondition IsNotTransferArm = new Chore.Precondition
	{
		id = "IsNotTransferArm",
		description = "",
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			return !context.consumerState.hasSolidTransferArm;
		}
	};

	public Chore.Precondition HasSkillPerk = new Chore.Precondition
	{
		id = "HasSkillPerk",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.HAS_SKILL_PERK,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			MinionResume resume = context.consumerState.resume;
			if ((bool)resume)
			{
				if (!(data is SkillPerk))
				{
					if (!(data is HashedString))
					{
						if (!(data is string))
						{
							return false;
						}
						HashedString perkId = (string)data;
						return resume.HasPerk(perkId);
					}
					HashedString perkId2 = (HashedString)data;
					return resume.HasPerk(perkId2);
				}
				SkillPerk perk = data as SkillPerk;
				return resume.HasPerk(perk);
			}
			return false;
		}
	};

	public Chore.Precondition IsMoreSatisfyingEarly = new Chore.Precondition
	{
		id = "IsMoreSatisfyingEarly",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_MORE_SATISFYING,
		sortOrder = -1,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			if (!context.isAttemptingOverride)
			{
				if (!context.consumerState.selectable.IsSelected)
				{
					Chore currentChore3 = context.consumerState.choreDriver.GetCurrentChore();
					if (currentChore3 == null)
					{
						return true;
					}
					if (context.masterPriority.priority_class == currentChore3.masterPriority.priority_class)
					{
						if ((Object)context.consumerState.consumer != (Object)null && context.personalPriority != context.consumerState.consumer.GetPersonalPriority(currentChore3.choreType))
						{
							return context.personalPriority > context.consumerState.consumer.GetPersonalPriority(currentChore3.choreType);
						}
						if (context.masterPriority.priority_value == currentChore3.masterPriority.priority_value)
						{
							return context.priority > currentChore3.choreType.priority;
						}
						return context.masterPriority.priority_value > currentChore3.masterPriority.priority_value;
					}
					return context.masterPriority.priority_class > currentChore3.masterPriority.priority_class;
				}
				return true;
			}
			return true;
		}
	};

	public Chore.Precondition IsMoreSatisfyingLate = new Chore.Precondition
	{
		id = "IsMoreSatisfyingLate",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_MORE_SATISFYING,
		sortOrder = 10000,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			if (!context.isAttemptingOverride)
			{
				if (context.consumerState.selectable.IsSelected)
				{
					Chore currentChore2 = context.consumerState.choreDriver.GetCurrentChore();
					if (currentChore2 == null)
					{
						return true;
					}
					if (context.masterPriority.priority_class == currentChore2.masterPriority.priority_class)
					{
						if ((Object)context.consumerState.consumer != (Object)null && context.personalPriority != context.consumerState.consumer.GetPersonalPriority(currentChore2.choreType))
						{
							return context.personalPriority > context.consumerState.consumer.GetPersonalPriority(currentChore2.choreType);
						}
						if (context.masterPriority.priority_value == currentChore2.masterPriority.priority_value)
						{
							return context.priority > currentChore2.choreType.priority;
						}
						return context.masterPriority.priority_value > currentChore2.masterPriority.priority_value;
					}
					return context.masterPriority.priority_class > currentChore2.masterPriority.priority_class;
				}
				return true;
			}
			return true;
		}
	};

	public Chore.Precondition IsChattable = new Chore.Precondition
	{
		id = "CanChat",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.CAN_CHAT,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			KMonoBehaviour kMonoBehaviour2 = (KMonoBehaviour)data;
			if (!((Object)context.consumerState.consumer == (Object)null))
			{
				if (!((Object)context.consumerState.navigator == (Object)null))
				{
					if (!((Object)kMonoBehaviour2 == (Object)null))
					{
						return context.consumerState.navigator.CanReach(kMonoBehaviour2.GetComponent<Chattable>());
					}
					return false;
				}
				return false;
			}
			return false;
		}
	};

	public Chore.Precondition IsNotRedAlert = new Chore.Precondition
	{
		id = "IsNotRedAlert",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_NOT_RED_ALERT,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			if (context.chore.masterPriority.priority_class != PriorityScreen.PriorityClass.topPriority)
			{
				return !VignetteManager.Instance.Get().IsRedAlert();
			}
			return true;
		}
	};

	public Chore.Precondition IsScheduledTime = new Chore.Precondition
	{
		id = "IsScheduledTime",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_SCHEDULED_TIME,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			if (!VignetteManager.Instance.Get().IsRedAlert())
			{
				ScheduleBlockType type = (ScheduleBlockType)data;
				return context.consumerState.scheduleBlock?.IsAllowed(type) ?? true;
			}
			return true;
		}
	};

	public Chore.Precondition CanMoveTo = new Chore.Precondition
	{
		id = "CanMoveTo",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.CAN_MOVE_TO,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			if (!((Object)context.consumerState.consumer == (Object)null))
			{
				KMonoBehaviour kMonoBehaviour = (KMonoBehaviour)data;
				if (!((Object)kMonoBehaviour == (Object)null))
				{
					IApproachable approachable = (IApproachable)kMonoBehaviour;
					if (!context.consumerState.consumer.GetNavigationCost(approachable, out int cost))
					{
						return false;
					}
					context.cost += cost;
					return true;
				}
				return false;
			}
			return false;
		}
	};

	public Chore.Precondition CanPickup = new Chore.Precondition
	{
		id = "CanPickup",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.CAN_PICKUP,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			Pickupable pickupable = (Pickupable)data;
			if (!((Object)pickupable == (Object)null))
			{
				if (!((Object)context.consumerState.consumer == (Object)null))
				{
					if (!pickupable.HasTag(GameTags.StoredPrivate))
					{
						if (pickupable.CouldBePickedUpByMinion(context.consumerState.gameObject))
						{
							return context.consumerState.consumer.CanReach(pickupable);
						}
						return false;
					}
					return false;
				}
				return false;
			}
			return false;
		}
	};

	public Chore.Precondition IsAwake = new Chore.Precondition
	{
		id = "IsAwake",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_AWAKE,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			if (!((Object)context.consumerState.consumer == (Object)null))
			{
				StaminaMonitor.Instance sMI3 = context.consumerState.consumer.GetSMI<StaminaMonitor.Instance>();
				return !sMI3.IsInsideState(sMI3.sm.sleepy.sleeping);
			}
			return false;
		}
	};

	public Chore.Precondition IsStanding = new Chore.Precondition
	{
		id = "IsStanding",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_STANDING,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			if (!((Object)context.consumerState.consumer == (Object)null))
			{
				if (!((Object)context.consumerState.navigator == (Object)null))
				{
					return context.consumerState.navigator.CurrentNavType == NavType.Floor;
				}
				return false;
			}
			return false;
		}
	};

	public Chore.Precondition IsMoving = new Chore.Precondition
	{
		id = "IsMoving",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_MOVING,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			if (!((Object)context.consumerState.consumer == (Object)null))
			{
				if (!((Object)context.consumerState.navigator == (Object)null))
				{
					return context.consumerState.navigator.IsMoving();
				}
				return false;
			}
			return false;
		}
	};

	public Chore.Precondition IsOffLadder = new Chore.Precondition
	{
		id = "IsOffLadder",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_OFF_LADDER,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			if (!((Object)context.consumerState.consumer == (Object)null))
			{
				if (!((Object)context.consumerState.navigator == (Object)null))
				{
					return context.consumerState.navigator.CurrentNavType != NavType.Ladder && context.consumerState.navigator.CurrentNavType != NavType.Pole;
				}
				return false;
			}
			return false;
		}
	};

	public Chore.Precondition NotInTube = new Chore.Precondition
	{
		id = "NotInTube",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.NOT_IN_TUBE,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			if (!((Object)context.consumerState.consumer == (Object)null))
			{
				if (!((Object)context.consumerState.navigator == (Object)null))
				{
					return context.consumerState.navigator.CurrentNavType != NavType.Tube;
				}
				return false;
			}
			return false;
		}
	};

	public Chore.Precondition ConsumerHasTrait = new Chore.Precondition
	{
		id = "ConsumerHasTrait",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.HAS_TRAIT,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			string trait_id = (string)data;
			Traits traits = context.consumerState.traits;
			return !((Object)traits == (Object)null) && traits.HasTrait(trait_id);
		}
	};

	public Chore.Precondition IsOperational = new Chore.Precondition
	{
		id = "IsOperational",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_OPERATIONAL,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			Operational operational2 = data as Operational;
			return operational2.IsOperational;
		}
	};

	public Chore.Precondition IsNotMarkedForDeconstruction = new Chore.Precondition
	{
		id = "IsNotMarkedForDeconstruction",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_MARKED_FOR_DECONSTRUCTION,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			Deconstructable deconstructable = data as Deconstructable;
			return (Object)deconstructable == (Object)null || !deconstructable.IsMarkedForDeconstruction();
		}
	};

	public Chore.Precondition IsNotMarkedForDisable = new Chore.Precondition
	{
		id = "IsNotMarkedForDisable",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_MARKED_FOR_DISABLE,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			BuildingEnabledButton buildingEnabledButton = data as BuildingEnabledButton;
			return (Object)buildingEnabledButton == (Object)null || (buildingEnabledButton.IsEnabled && !buildingEnabledButton.WaitingForDisable);
		}
	};

	public Chore.Precondition IsFunctional = new Chore.Precondition
	{
		id = "IsFunctional",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_FUNCTIONAL,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			Operational operational = data as Operational;
			return operational.IsFunctional;
		}
	};

	public Chore.Precondition IsOverrideTargetNullOrMe = new Chore.Precondition
	{
		id = "IsOverrideTargetNullOrMe",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_OVERRIDE_TARGET_NULL_OR_ME,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			return context.isAttemptingOverride || (Object)context.chore.overrideTarget == (Object)null || (Object)context.chore.overrideTarget == (Object)context.consumerState.consumer;
		}
	};

	public Chore.Precondition NotChoreCreator = new Chore.Precondition
	{
		id = "NotChoreCreator",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.NOT_CHORE_CREATOR,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			GameObject y = (GameObject)data;
			if (!((Object)context.consumerState.consumer == (Object)null))
			{
				if (!((Object)context.consumerState.gameObject == (Object)y))
				{
					return true;
				}
				return false;
			}
			return false;
		}
	};

	public Chore.Precondition IsGettingMoreStressed = new Chore.Precondition
	{
		id = "IsGettingMoreStressed",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_GETTING_MORE_STRESSED,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			AmountInstance amountInstance = Db.Get().Amounts.Stress.Lookup(context.consumerState.gameObject);
			return amountInstance.GetDelta() > 0f;
		}
	};

	public Chore.Precondition IsAllowedByAutomation = new Chore.Precondition
	{
		id = "IsAllowedByAutomation",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_ALLOWED_BY_AUTOMATION,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			Automatable automatable = (Automatable)data;
			return automatable.AllowedByAutomation(context.consumerState.hasSolidTransferArm);
		}
	};

	public Chore.Precondition HasTag = new Chore.Precondition
	{
		id = "HasTag",
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			Tag tag2 = (Tag)data;
			return context.consumerState.prefabid.HasTag(tag2);
		}
	};

	public Chore.Precondition CheckBehaviourPrecondition = new Chore.Precondition
	{
		id = "CheckBehaviourPrecondition",
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			Tag tag = (Tag)data;
			return context.consumerState.consumer.RunBehaviourPrecondition(tag);
		}
	};

	public Chore.Precondition CanDoWorkerPrioritizable = new Chore.Precondition
	{
		id = "CanDoWorkerPrioritizable",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.CAN_DO_RECREATION,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			if (!((Object)context.consumerState.consumer == (Object)null))
			{
				IWorkerPrioritizable workerPrioritizable = data as IWorkerPrioritizable;
				if (workerPrioritizable != null)
				{
					int priority = 0;
					if (!workerPrioritizable.GetWorkerPriority(context.consumerState.worker, out priority))
					{
						return false;
					}
					context.consumerPriority += priority;
					return true;
				}
				return false;
			}
			return false;
		}
	};

	public Chore.Precondition IsExclusivelyAvailableWithOtherChores = new Chore.Precondition
	{
		id = "IsExclusivelyAvailableWithOtherChores",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.EXCLUSIVELY_AVAILABLE,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			List<Chore> list = (List<Chore>)data;
			foreach (Chore item in list)
			{
				if (item != context.chore && (Object)item.driver != (Object)null)
				{
					return false;
				}
			}
			return true;
		}
	};

	public Chore.Precondition IsBladderFull = new Chore.Precondition
	{
		id = "IsBladderFull",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.BLADDER_FULL,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			BladderMonitor.Instance sMI2 = context.consumerState.gameObject.GetSMI<BladderMonitor.Instance>();
			if (sMI2 != null && sMI2.NeedsToPee())
			{
				return true;
			}
			return false;
		}
	};

	public Chore.Precondition IsBladderNotFull = new Chore.Precondition
	{
		id = "IsBladderNotFull",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.BLADDER_NOT_FULL,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			BladderMonitor.Instance sMI = context.consumerState.gameObject.GetSMI<BladderMonitor.Instance>();
			if (sMI != null && sMI.NeedsToPee())
			{
				return false;
			}
			return true;
		}
	};

	public Chore.Precondition NoDeadBodies = new Chore.Precondition
	{
		id = "NoDeadBodies",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.NO_DEAD_BODIES,
		fn = (Chore.PreconditionFn)delegate
		{
			return Components.LiveMinionIdentities.Count == Components.MinionIdentities.Count;
		}
	};

	public Chore.Precondition NotCurrentlyPeeing = new Chore.Precondition
	{
		id = "NotCurrentlyPeeing",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.CURRENTLY_PEEING,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			bool result = true;
			Chore currentChore = context.consumerState.choreDriver.GetCurrentChore();
			if (currentChore != null)
			{
				string id = currentChore.choreType.Id;
				result = (id != Db.Get().ChoreTypes.BreakPee.Id && id != Db.Get().ChoreTypes.Pee.Id);
			}
			return result;
		}
	};

	public static ChorePreconditions instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new ChorePreconditions();
			}
			return _instance;
		}
	}

	public static void DestroyInstance()
	{
		_instance = null;
	}
}
