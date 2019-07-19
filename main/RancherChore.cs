using STRINGS;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RancherChore : Chore<RancherChore.RancherChoreStates.Instance>
{
	public class RancherChoreStates : GameStateMachine<RancherChoreStates, RancherChoreStates.Instance>
	{
		private class RanchState : State
		{
			public State pre;

			public State loop;

			public State pst;
		}

		public new class Instance : GameInstance
		{
			public RanchStation.Instance ranchStation;

			public Instance(KPrefabID rancher_station)
				: base((IStateMachineTarget)rancher_station)
			{
				ranchStation = rancher_station.GetSMI<RanchStation.Instance>();
			}

			public void CheckForMoreRanchables()
			{
				ranchStation.FindRanchable();
				if (ranchStation.IsCreatureAvailableForRanching())
				{
					GoTo(base.sm.movetoranch);
				}
				else
				{
					GoTo((BaseState)null);
				}
			}

			public void TriggerRanchStationNoLongerAvailable()
			{
				ranchStation.TriggerRanchStationNoLongerAvailable();
			}

			public void TellCreatureRancherIsReady()
			{
				if (!ranchStation.targetRanchable.IsNullOrStopped())
				{
					ranchStation.targetRanchable.Trigger(1084749845, null);
				}
			}
		}

		public TargetParameter rancher;

		private State movetoranch;

		private State waitforcreature_pre;

		private State waitforcreature;

		private RanchState ranchcreature;

		private State wavegoodbye;

		private State checkformoreranchables;

		[CompilerGenerated]
		private static Transition.ConditionCallback _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Transition.ConditionCallback _003C_003Ef__mg_0024cache1;

		[CompilerGenerated]
		private static Transition.ConditionCallback _003C_003Ef__mg_0024cache2;

		[CompilerGenerated]
		private static StateMachine<RancherChoreStates, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cache3;

		[CompilerGenerated]
		private static Transition.ConditionCallback _003C_003Ef__mg_0024cache4;

		[CompilerGenerated]
		private static Func<Instance, HashedString> _003C_003Ef__mg_0024cache5;

		[CompilerGenerated]
		private static StateMachine<RancherChoreStates, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cache6;

		[CompilerGenerated]
		private static StateMachine<RancherChoreStates, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cache7;

		[CompilerGenerated]
		private static StateMachine<RancherChoreStates, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cache8;

		[CompilerGenerated]
		private static StateMachine<RancherChoreStates, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cache9;

		[CompilerGenerated]
		private static StateMachine<RancherChoreStates, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cacheA;

		[CompilerGenerated]
		private static StateMachine<RancherChoreStates, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cacheB;

		[CompilerGenerated]
		private static StateMachine<RancherChoreStates, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cacheC;

		[CompilerGenerated]
		private static StateMachine<RancherChoreStates, Instance, IStateMachineTarget, object>.State.Callback _003C_003Ef__mg_0024cacheD;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = movetoranch;
			Target(rancher);
			root.Exit("TriggerRanchStationNoLongerAvailable", delegate(Instance smi)
			{
				smi.TriggerRanchStationNoLongerAvailable();
			});
			movetoranch.MoveTo((Instance smi) => Grid.PosToCell(smi.transform.GetPosition()), waitforcreature_pre, null, false).Transition(checkformoreranchables, HasCreatureLeft, UpdateRate.SIM_1000ms);
			waitforcreature_pre.EnterTransition(null, (Instance smi) => smi.ranchStation.IsNullOrStopped()).Transition(checkformoreranchables, HasCreatureLeft, UpdateRate.SIM_1000ms).EnterTransition(waitforcreature, (Instance smi) => true);
			waitforcreature.Transition(checkformoreranchables, HasCreatureLeft, UpdateRate.SIM_1000ms).ToggleAnims("anim_interacts_rancherstation_kanim", 0f).PlayAnim("calling_loop", KAnim.PlayMode.Loop)
				.Enter(FaceCreature)
				.Enter("TellCreatureToGoGetRanched", delegate(Instance smi)
				{
					smi.ranchStation.SetRancherIsAvailableForRanching();
				})
				.Exit("ClearRancherIsAvailableForRanching", delegate(Instance smi)
				{
					smi.ranchStation.ClearRancherIsAvailableForRanching();
				})
				.Target(masterTarget)
				.EventTransition(GameHashes.CreatureArrivedAtRanchStation, ranchcreature, null);
			ranchcreature.Transition(checkformoreranchables, HasCreatureLeft, UpdateRate.SIM_1000ms).ToggleAnims(GetRancherInteractAnim).DefaultState(ranchcreature.pre)
				.EventTransition(GameHashes.CreatureAbandonedRanchStation, checkformoreranchables, null)
				.Enter(SetCreatureLayer)
				.Exit(ClearCreatureLayer);
			ranchcreature.pre.Enter(FaceCreature).Enter(PlayBuildingWorkingPre).QueueAnim("working_pre", false, null)
				.OnAnimQueueComplete(ranchcreature.loop);
			ranchcreature.loop.Enter("TellCreatureRancherIsReady", delegate(Instance smi)
			{
				smi.TellCreatureRancherIsReady();
			}).Enter(PlayBuildingWorkingLoop).Enter(PlayRancherWorkingLoops)
				.Target(rancher)
				.OnAnimQueueComplete(ranchcreature.pst);
			ranchcreature.pst.Enter(RanchCreature).Enter(PlayBuildingWorkingPst).QueueAnim("working_pst", false, null)
				.QueueAnim("wipe_brow", false, null)
				.OnAnimQueueComplete(checkformoreranchables);
			checkformoreranchables.Enter("FindRanchable", delegate(Instance smi)
			{
				smi.CheckForMoreRanchables();
			}).Update("FindRanchable", delegate(Instance smi, float dt)
			{
				smi.CheckForMoreRanchables();
			}, UpdateRate.SIM_200ms, false);
		}

		private static bool HasCreatureLeft(Instance smi)
		{
			return smi.ranchStation.targetRanchable.IsNullOrStopped() || !smi.ranchStation.targetRanchable.GetComponent<ChoreConsumer>().IsChoreEqualOrAboveCurrentChorePriority<RanchedStates>();
		}

		private static void SetCreatureLayer(Instance smi)
		{
			if (!smi.ranchStation.targetRanchable.IsNullOrStopped())
			{
				smi.ranchStation.targetRanchable.Get<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.BuildingUse);
			}
		}

		private static void ClearCreatureLayer(Instance smi)
		{
			if (!smi.ranchStation.targetRanchable.IsNullOrStopped())
			{
				smi.ranchStation.targetRanchable.Get<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Creatures);
			}
		}

		private static HashedString GetRancherInteractAnim(Instance smi)
		{
			return smi.ranchStation.def.rancherInteractAnim;
		}

		private static void FaceCreature(Instance smi)
		{
			Facing facing = smi.sm.rancher.Get<Facing>(smi);
			Vector3 position = smi.ranchStation.targetRanchable.transform.GetPosition();
			facing.Face(position);
		}

		private static void RanchCreature(Instance smi)
		{
			Debug.Assert(smi.ranchStation != null, "smi.ranchStation was null");
			RanchableMonitor.Instance targetRanchable = smi.ranchStation.targetRanchable;
			if (!targetRanchable.IsNullOrStopped())
			{
				KPrefabID component = targetRanchable.GetComponent<KPrefabID>();
				smi.sm.rancher.Get(smi).Trigger(937885943, component.PrefabTag.Name);
				smi.ranchStation.RanchCreature();
			}
		}

		private static bool ShouldSynchronizeBuilding(Instance smi)
		{
			return smi.ranchStation.def.synchronizeBuilding;
		}

		private static void PlayBuildingWorkingPre(Instance smi)
		{
			if (ShouldSynchronizeBuilding(smi))
			{
				smi.ranchStation.GetComponent<KBatchedAnimController>().Queue("working_pre", KAnim.PlayMode.Once, 1f, 0f);
			}
		}

		private static void PlayRancherWorkingLoops(Instance smi)
		{
			KBatchedAnimController kBatchedAnimController = smi.sm.rancher.Get<KBatchedAnimController>(smi);
			for (int i = 0; i < smi.ranchStation.def.interactLoopCount; i++)
			{
				kBatchedAnimController.Queue("working_loop", KAnim.PlayMode.Once, 1f, 0f);
			}
		}

		private static void PlayBuildingWorkingLoop(Instance smi)
		{
			if (ShouldSynchronizeBuilding(smi))
			{
				smi.ranchStation.GetComponent<KBatchedAnimController>().Queue("working_loop", KAnim.PlayMode.Loop, 1f, 0f);
			}
		}

		private static void PlayBuildingWorkingPst(Instance smi)
		{
			if (ShouldSynchronizeBuilding(smi))
			{
				smi.ranchStation.GetComponent<KBatchedAnimController>().Queue("working_pst", KAnim.PlayMode.Once, 1f, 0f);
			}
		}
	}

	public Precondition IsCreatureAvailableForRanching = new Precondition
	{
		id = "IsCreatureAvailableForRanching",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_CREATURE_AVAILABLE_FOR_RANCHING,
		fn = (PreconditionFn)delegate(ref Precondition.Context context, object data)
		{
			RanchStation.Instance instance = data as RanchStation.Instance;
			return instance.IsCreatureAvailableForRanching();
		}
	};

	public RancherChore(KPrefabID rancher_station)
		: base(Db.Get().ChoreTypes.Ranch, (IStateMachineTarget)rancher_station, (ChoreProvider)null, false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.basic, 5, false, true, 0, false, ReportManager.ReportType.WorkTime)
	{
		AddPrecondition(IsCreatureAvailableForRanching, rancher_station.GetSMI<RanchStation.Instance>());
		AddPrecondition(ChorePreconditions.instance.HasSkillPerk, Db.Get().SkillPerks.CanUseRanchStation.Id);
		AddPrecondition(ChorePreconditions.instance.IsScheduledTime, Db.Get().ScheduleBlockTypes.Work);
		AddPrecondition(ChorePreconditions.instance.CanMoveTo, rancher_station.GetComponent<Building>());
		Operational component = rancher_station.GetComponent<Operational>();
		AddPrecondition(ChorePreconditions.instance.IsOperational, component);
		Deconstructable component2 = rancher_station.GetComponent<Deconstructable>();
		AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDeconstruction, component2);
		BuildingEnabledButton component3 = rancher_station.GetComponent<BuildingEnabledButton>();
		AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDisable, component3);
		base.smi = new RancherChoreStates.Instance(rancher_station);
		SetPrioritizable(rancher_station.GetComponent<Prioritizable>());
	}

	public override void Begin(Precondition.Context context)
	{
		base.smi.sm.rancher.Set(context.consumerState.gameObject, base.smi);
		base.Begin(context);
	}
}
