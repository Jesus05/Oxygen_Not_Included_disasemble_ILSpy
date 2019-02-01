using System;
using UnityEngine;

public class WorkChore<WorkableType> : Chore<WorkChore<WorkableType>.StatesInstance> where WorkableType : Workable
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, WorkChore<WorkableType>, object>.GameInstance
	{
		private KAnimFile overrideAnims;

		public StatesInstance(WorkChore<WorkableType> master, GameObject workable, KAnimFile override_anims)
			: base(master)
		{
			overrideAnims = override_anims;
			base.sm.workable.Set(workable, base.smi);
		}

		public void EnableAnimOverrides()
		{
			if ((UnityEngine.Object)overrideAnims != (UnityEngine.Object)null)
			{
				base.sm.worker.Get<KAnimControllerBase>(base.smi).AddAnimOverrides(overrideAnims, 0f);
			}
		}

		public void DisableAnimOverrides()
		{
			if ((UnityEngine.Object)overrideAnims != (UnityEngine.Object)null)
			{
				base.sm.worker.Get<KAnimControllerBase>(base.smi).RemoveAnimOverrides(overrideAnims);
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, WorkChore<WorkableType>>
	{
		public ApproachSubState<WorkableType> approach;

		public State work;

		public State success;

		public TargetParameter workable;

		public TargetParameter worker;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = approach;
			Target(worker);
			approach.InitializeStates(worker, workable, work, null, null, null).Update("CheckOperational", delegate(StatesInstance smi, float dt)
			{
				if (!smi.master.IsOperationalValid())
				{
					smi.StopSM("Building not operational");
				}
			}, UpdateRate.SIM_200ms, false);
			work.Enter(delegate(StatesInstance smi)
			{
				smi.EnableAnimOverrides();
			}).ToggleWork<WorkableType>(workable, success, null, (StatesInstance smi) => smi.master.IsOperationalValid()).Exit(delegate(StatesInstance smi)
			{
				smi.DisableAnimOverrides();
			});
			success.ReturnSuccess();
		}
	}

	public Func<Precondition.Context, bool> preemption_cb;

	public bool onlyWhenOperational
	{
		get;
		private set;
	}

	public WorkChore(ChoreType chore_type, IStateMachineTarget target, ChoreProvider chore_provider = null, Tag[] chore_tags = null, bool run_until_complete = true, Action<Chore> on_complete = null, Action<Chore> on_begin = null, Action<Chore> on_end = null, bool allow_in_red_alert = true, ScheduleBlockType schedule_block = null, bool ignore_schedule_block = false, bool only_when_operational = true, KAnimFile override_anims = null, bool is_preemptable = false, bool allow_in_context_menu = true, bool allow_prioritization = true, PriorityScreen.PriorityClass priority_class = PriorityScreen.PriorityClass.basic, int priority_class_value = 5, bool ignore_building_assignment = false, bool add_to_daily_report = true)
		: base(chore_type, target, chore_provider, run_until_complete, on_complete, on_begin, on_end, priority_class, priority_class_value, is_preemptable, allow_in_context_menu, 0, chore_tags, add_to_daily_report)
	{
		smi = new StatesInstance(this, target.gameObject, override_anims);
		onlyWhenOperational = only_when_operational;
		if (allow_prioritization)
		{
			SetPrioritizable(target.GetComponent<Prioritizable>());
		}
		AddPrecondition(ChorePreconditions.instance.IsNotTransferArm, null);
		if (!allow_in_red_alert)
		{
			AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
		}
		if (schedule_block != null)
		{
			AddPrecondition(ChorePreconditions.instance.IsScheduledTime, schedule_block);
		}
		else if (!ignore_schedule_block)
		{
			AddPrecondition(ChorePreconditions.instance.IsScheduledTime, Db.Get().ScheduleBlockTypes.Work);
		}
		AddPrecondition(ChorePreconditions.instance.CanMoveTo, smi.sm.workable.Get<WorkableType>(smi));
		Operational component = target.GetComponent<Operational>();
		if (only_when_operational && (UnityEngine.Object)component != (UnityEngine.Object)null)
		{
			AddPrecondition(ChorePreconditions.instance.IsOperational, component);
		}
		if (only_when_operational)
		{
			Deconstructable component2 = target.GetComponent<Deconstructable>();
			if ((UnityEngine.Object)component2 != (UnityEngine.Object)null)
			{
				AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDeconstruction, component2);
			}
			BuildingEnabledButton component3 = target.GetComponent<BuildingEnabledButton>();
			if ((UnityEngine.Object)component3 != (UnityEngine.Object)null)
			{
				AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDisable, component3);
			}
		}
		if (!ignore_building_assignment && (UnityEngine.Object)smi.sm.workable.Get(smi).GetComponent<Assignable>() != (UnityEngine.Object)null)
		{
			AddPrecondition(ChorePreconditions.instance.IsAssignedtoMe, smi.sm.workable.Get<Assignable>(smi));
		}
		WorkableType val = target as WorkableType;
		if ((UnityEngine.Object)val != (UnityEngine.Object)null && val.requiredRolePerk.IsValid)
		{
			AddPrecondition(ChorePreconditions.instance.HasRolePerk, val.requiredRolePerk);
		}
	}

	public override string ToString()
	{
		return "WorkChore<" + typeof(WorkableType).ToString() + ">";
	}

	public override void Begin(Precondition.Context context)
	{
		smi.sm.worker.Set(context.consumerState.gameObject, smi);
		base.Begin(context);
	}

	public bool IsOperationalValid()
	{
		if (onlyWhenOperational)
		{
			Operational component = smi.master.GetComponent<Operational>();
			if ((UnityEngine.Object)component != (UnityEngine.Object)null && !component.IsOperational)
			{
				return false;
			}
		}
		return true;
	}

	public override bool CanPreempt(Precondition.Context context)
	{
		if (base.CanPreempt(context))
		{
			if (!((UnityEngine.Object)context.chore.driver == (UnityEngine.Object)null))
			{
				if (!((UnityEngine.Object)context.chore.driver == (UnityEngine.Object)context.consumerState.choreDriver))
				{
					Workable workable = smi.sm.workable.Get<WorkableType>(smi);
					if (!((UnityEngine.Object)workable == (UnityEngine.Object)null))
					{
						if (preemption_cb != null)
						{
							if (!preemption_cb(context))
							{
								return false;
							}
						}
						else
						{
							int num = 4;
							int navigationCost = ((Component)context.chore.driver).GetComponent<Navigator>().GetNavigationCost(workable);
							if (navigationCost == -1 || navigationCost < num)
							{
								return false;
							}
							int navigationCost2 = context.consumerState.navigator.GetNavigationCost(workable);
							if (navigationCost2 * 2 > navigationCost)
							{
								return false;
							}
						}
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
}
