using STRINGS;
using UnityEngine;

internal class FleeStates : GameStateMachine<FleeStates, FleeStates.Instance, IStateMachineTarget, FleeStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Flee);
			base.sm.mover.Set(GetComponent<Navigator>(), base.smi);
		}
	}

	private TargetParameter mover;

	public TargetParameter fleeToTarget;

	public State plan;

	public ApproachSubState<IApproachable> approach;

	public State cower;

	public State behaviourcomplete;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = plan;
		State state = root.Enter("SetFleeTarget", delegate(Instance smi)
		{
			fleeToTarget.Set(CreatureHelpers.GetFleeTargetLocatorObject(smi.master.gameObject, smi.GetSMI<ThreatMonitor.Instance>().MainThreat), smi);
		});
		string name = CREATURES.STATUSITEMS.FLEEING.NAME;
		string tooltip = CREATURES.STATUSITEMS.FLEEING.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(name, tooltip, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 63486, null, null, main);
		plan.Enter(delegate(Instance smi)
		{
			ThreatMonitor.Instance sMI = smi.master.gameObject.GetSMI<ThreatMonitor.Instance>();
			fleeToTarget.Set(CreatureHelpers.GetFleeTargetLocatorObject(smi.master.gameObject, sMI.MainThreat), smi);
			if ((Object)fleeToTarget.Get(smi) != (Object)null)
			{
				smi.GoTo(approach);
			}
			else
			{
				smi.GoTo(cower);
			}
		});
		approach.InitializeStates(mover, fleeToTarget, cower, cower, null, NavigationTactics.ReduceTravelDistance).Enter(delegate(Instance smi)
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, CREATURES.STATUSITEMS.FLEEING.NAME.text, smi.master.transform, 1.5f, false);
		});
		cower.Enter(delegate(Instance smi)
		{
			string s = "DEFAULT COWER ANIMATION";
			if (smi.Get<KBatchedAnimController>().HasAnimation("cower"))
			{
				s = "cower";
			}
			else if (smi.Get<KBatchedAnimController>().HasAnimation("idle"))
			{
				s = "idle";
			}
			else if (smi.Get<KBatchedAnimController>().HasAnimation("idle_loop"))
			{
				s = "idle_loop";
			}
			smi.Get<KBatchedAnimController>().Play(s, KAnim.PlayMode.Loop, 1f, 0f);
		}).ScheduleGoTo(2f, behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.Flee, false);
	}
}
