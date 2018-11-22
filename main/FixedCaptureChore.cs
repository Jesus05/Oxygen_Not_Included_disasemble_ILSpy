using STRINGS;
using System;
using System.Runtime.CompilerServices;

public class FixedCaptureChore : Chore<FixedCaptureChore.FixedCaptureChoreStates.Instance>
{
	public class FixedCaptureChoreStates : GameStateMachine<FixedCaptureChoreStates, FixedCaptureChoreStates.Instance>
	{
		public new class Instance : GameInstance
		{
			public FixedCapturePoint.Instance fixedCapturePoint;

			public Instance(KPrefabID capture_point)
				: base((IStateMachineTarget)capture_point)
			{
				fixedCapturePoint = capture_point.GetSMI<FixedCapturePoint.Instance>();
			}
		}

		public TargetParameter rancher;

		public TargetParameter creature;

		private State movetopoint;

		private State waitforcreature_pre;

		private State waitforcreature;

		private State capturecreature;

		private State failed;

		private State success;

		[CompilerGenerated]
		private static Transition.ConditionCallback _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static Transition.ConditionCallback _003C_003Ef__mg_0024cache1;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = movetopoint;
			Target(rancher);
			root.Exit("ResetCapturePoint", delegate(Instance smi)
			{
				smi.fixedCapturePoint.ResetCapturePoint();
			});
			movetopoint.MoveTo((Instance smi) => Grid.PosToCell(smi.transform.GetPosition()), waitforcreature_pre, null, false).Target(masterTarget).EventTransition(GameHashes.CreatureAbandonedCapturePoint, failed, null);
			waitforcreature_pre.EnterTransition(null, (Instance smi) => smi.fixedCapturePoint.IsNullOrStopped()).EnterTransition(failed, HasCreatureLeft).EnterTransition(waitforcreature, (Instance smi) => true);
			waitforcreature.ToggleAnims("anim_interacts_rancherstation_kanim", 0f).PlayAnim("calling_loop", KAnim.PlayMode.Loop).Face(creature, 0f)
				.Enter("SetRancherIsAvailableForCapturing", delegate(Instance smi)
				{
					smi.fixedCapturePoint.SetRancherIsAvailableForCapturing();
				})
				.Exit("ClearRancherIsAvailableForCapturing", delegate(Instance smi)
				{
					smi.fixedCapturePoint.ClearRancherIsAvailableForCapturing();
				})
				.Transition(failed, HasCreatureLeft, UpdateRate.SIM_200ms)
				.Target(masterTarget)
				.EventTransition(GameHashes.CreatureArrivedAtCapturePoint, capturecreature, null);
			capturecreature.EventTransition(GameHashes.CreatureAbandonedCapturePoint, failed, null).EnterTransition(failed, (Instance smi) => smi.fixedCapturePoint.targetCapturable.IsNullOrStopped()).ToggleWork<Capturable>(creature, success, failed, null);
			failed.GoTo(null);
			success.ReturnSuccess();
		}

		private static bool HasCreatureLeft(Instance smi)
		{
			return smi.fixedCapturePoint.targetCapturable.IsNullOrStopped() || !smi.fixedCapturePoint.targetCapturable.GetComponent<ChoreConsumer>().IsChoreEqualOrAboveCurrentChorePriority<FixedCaptureStates>();
		}
	}

	public Precondition IsCreatureAvailableForFixedCapture = new Precondition
	{
		id = "IsCreatureAvailableForFixedCapture",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.IS_CREATURE_AVAILABLE_FOR_FIXED_CAPTURE,
		fn = (PreconditionFn)delegate(ref Precondition.Context context, object data)
		{
			FixedCapturePoint.Instance instance = data as FixedCapturePoint.Instance;
			return instance.IsCreatureAvailableForFixedCapture();
		}
	};

	public FixedCaptureChore(KPrefabID capture_point)
		: base(Db.Get().ChoreTypes.Ranch, (IStateMachineTarget)capture_point, (ChoreProvider)null, false, (Action<Chore>)null, (Action<Chore>)null, (Action<Chore>)null, PriorityScreen.PriorityClass.basic, 5, false, true, 0, (Tag[])null)
	{
		AddPrecondition(IsCreatureAvailableForFixedCapture, capture_point.GetSMI<FixedCapturePoint.Instance>());
		AddPrecondition(ChorePreconditions.instance.HasRolePerk, RoleManager.rolePerks.CanWrangleCreatures.id);
		AddPrecondition(ChorePreconditions.instance.IsScheduledTime, Db.Get().ScheduleBlockTypes.Work);
		AddPrecondition(ChorePreconditions.instance.CanMoveTo, capture_point.GetComponent<Building>());
		Operational component = capture_point.GetComponent<Operational>();
		AddPrecondition(ChorePreconditions.instance.IsOperational, component);
		Deconstructable component2 = capture_point.GetComponent<Deconstructable>();
		AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDeconstruction, component2);
		BuildingEnabledButton component3 = capture_point.GetComponent<BuildingEnabledButton>();
		AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDisable, component3);
		smi = new FixedCaptureChoreStates.Instance(capture_point);
		SetPrioritizable(capture_point.GetComponent<Prioritizable>());
	}

	public override void Begin(Precondition.Context context)
	{
		smi.sm.rancher.Set(context.consumerState.gameObject, smi);
		smi.sm.creature.Set(smi.fixedCapturePoint.targetCapturable.gameObject, smi);
		base.Begin(context);
	}
}
