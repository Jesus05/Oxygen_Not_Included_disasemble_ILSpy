using STRINGS;
using System;
using UnityEngine;

public class RescueIncapacitatedChore : Chore<RescueIncapacitatedChore.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, RescueIncapacitatedChore, object>.GameInstance
	{
		public StatesInstance(RescueIncapacitatedChore master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, RescueIncapacitatedChore>
	{
		public class HoldingIncapacitated : State
		{
			public State pickup;

			public ApproachSubState<IApproachable> delivering;

			public State deposit;

			public State ditch;
		}

		public ApproachSubState<Chattable> approachIncapacitated;

		public State failure;

		public HoldingIncapacitated holding;

		public TargetParameter rescueTarget;

		public TargetParameter deliverTarget;

		public TargetParameter rescuer;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = approachIncapacitated;
			approachIncapacitated.InitializeStates(rescuer, rescueTarget, holding.pickup, failure, Grid.DefaultOffset, null).Enter(delegate(StatesInstance smi)
			{
				DeathMonitor.Instance sMI2 = rescueTarget.GetSMI<DeathMonitor.Instance>(smi);
				if (sMI2 == null || sMI2.IsDead())
				{
					smi.StopSM("target died");
				}
			});
			holding.Target(rescuer).Enter(delegate(StatesInstance smi)
			{
				States states = this;
				smi.sm.rescueTarget.Get(smi).Subscribe(1623392196, delegate
				{
					smi.GoTo(states.holding.ditch);
				});
				KAnimFile anim2 = Assets.GetAnim("anim_incapacitated_carrier_kanim");
				smi.master.GetComponent<KAnimControllerBase>().RemoveAnimOverrides(anim2);
				smi.master.GetComponent<KAnimControllerBase>().AddAnimOverrides(anim2, 0f);
			}).Exit(delegate(StatesInstance smi)
			{
				KAnimFile anim = Assets.GetAnim("anim_incapacitated_carrier_kanim");
				smi.master.GetComponent<KAnimControllerBase>().RemoveAnimOverrides(anim);
			});
			holding.pickup.Target(rescuer).PlayAnim("pickup").Enter(delegate(StatesInstance smi)
			{
				rescueTarget.Get(smi).gameObject.GetComponent<KBatchedAnimController>().Play("pickup", KAnim.PlayMode.Once, 1f, 0f);
			})
				.Exit(delegate(StatesInstance smi)
				{
					rescuer.Get(smi).GetComponent<Storage>().Store(rescueTarget.Get(smi), false, false, true, false);
					rescueTarget.Get(smi).transform.SetLocalPosition(Vector3.zero);
					KBatchedAnimTracker component = rescueTarget.Get(smi).GetComponent<KBatchedAnimTracker>();
					component.symbol = new HashedString("snapTo_pivot");
					component.offset = new Vector3(0f, 0f, 1f);
				})
				.EventTransition(GameHashes.AnimQueueComplete, holding.delivering, null);
			holding.delivering.InitializeStates(rescuer, deliverTarget, holding.deposit, holding.ditch, null, null).Enter(delegate(StatesInstance smi)
			{
				DeathMonitor.Instance sMI = rescueTarget.GetSMI<DeathMonitor.Instance>(smi);
				if (sMI == null || sMI.IsDead())
				{
					smi.StopSM("target died");
				}
			}).Update(delegate(StatesInstance smi, float dt)
			{
				if ((UnityEngine.Object)deliverTarget.Get(smi) == (UnityEngine.Object)null)
				{
					smi.GoTo(holding.ditch);
				}
			}, UpdateRate.SIM_200ms, false);
			holding.deposit.PlayAnim("place").EventHandler(GameHashes.AnimQueueComplete, delegate(StatesInstance smi)
			{
				smi.master.DropIncapacitatedDuplicant();
				smi.SetStatus(Status.Success);
				smi.StopSM("complete");
			});
			holding.ditch.PlayAnim("place").ScheduleGoTo(0.5f, failure).Exit(delegate(StatesInstance smi)
			{
				smi.master.DropIncapacitatedDuplicant();
			});
			failure.Enter(delegate(StatesInstance smi)
			{
				smi.SetStatus(Status.Failed);
				smi.StopSM("failed");
			});
		}
	}

	public static Precondition CanReachIncapacitated = new Precondition
	{
		id = "CanReachIncapacitated",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.CAN_MOVE_TO,
		fn = (PreconditionFn)delegate(ref Precondition.Context context, object data)
		{
			GameObject gameObject = (GameObject)data;
			if (!((UnityEngine.Object)gameObject == (UnityEngine.Object)null))
			{
				int navigationCost = context.consumerState.navigator.GetNavigationCost(Grid.PosToCell(gameObject.transform.GetPosition()));
				if (navigationCost == -1)
				{
					return false;
				}
				context.cost += navigationCost;
				return true;
			}
			return false;
		}
	};

	public RescueIncapacitatedChore(IStateMachineTarget master, GameObject incapacitatedDuplicant)
		: base(Db.Get().ChoreTypes.RescueIncapacitated, master, (ChoreProvider)null, false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.personalNeeds, 5, false, true, 0, (Tag[])null, false)
	{
		smi = new StatesInstance(this);
		base.runUntilComplete = true;
		AddPrecondition(ChorePreconditions.instance.NotChoreCreator, incapacitatedDuplicant.gameObject);
		AddPrecondition(CanReachIncapacitated, incapacitatedDuplicant);
	}

	public override void Begin(Precondition.Context context)
	{
		smi.sm.rescuer.Set(context.consumerState.gameObject, smi);
		smi.sm.rescueTarget.Set(gameObject, smi);
		smi.sm.deliverTarget.Set(gameObject.GetSMI<BeIncapacitatedChore.StatesInstance>().master.GetChosenClinic(), smi);
		base.Begin(context);
	}

	protected override void End(string reason)
	{
		DropIncapacitatedDuplicant();
		base.End(reason);
	}

	private void DropIncapacitatedDuplicant()
	{
		if ((UnityEngine.Object)smi.sm.rescuer.Get(smi) != (UnityEngine.Object)null && (UnityEngine.Object)smi.sm.rescueTarget.Get(smi) != (UnityEngine.Object)null)
		{
			smi.sm.rescuer.Get(smi).GetComponent<Storage>().Drop(smi.sm.rescueTarget.Get(smi));
		}
	}
}
