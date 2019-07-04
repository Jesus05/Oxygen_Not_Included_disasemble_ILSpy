using STRINGS;
using System.Runtime.CompilerServices;
using UnityEngine;

public class InhaleStates : GameStateMachine<InhaleStates, InhaleStates.Instance, IStateMachineTarget, InhaleStates.Def>
{
	public class Def : BaseDef
	{
		public string inhaleSound;

		public float inhaleTime = 3f;
	}

	public new class Instance : GameInstance
	{
		public string inhaleSound;

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToEat);
			inhaleSound = GlobalAssets.GetSound(def.inhaleSound, false);
		}

		public void StartInhaleSound()
		{
			LoopingSounds component = GetComponent<LoopingSounds>();
			if ((Object)component != (Object)null)
			{
				component.StartSound(base.smi.inhaleSound);
			}
		}

		public void StopInhaleSound()
		{
			LoopingSounds component = GetComponent<LoopingSounds>();
			if ((Object)component != (Object)null)
			{
				component.StopSound(base.smi.inhaleSound);
			}
		}
	}

	public class InhalingStates : State
	{
		public State pre;

		public State pst;

		public State full;
	}

	public State goingtoeat;

	public InhalingStates inhaling;

	public State behaviourcomplete;

	public IntParameter targetCell;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache1;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = goingtoeat;
		root.Enter("SetTarget", delegate(Instance smi)
		{
			targetCell.Set(smi.GetSMI<GasAndLiquidConsumerMonitor.Instance>().targetCell, smi);
		});
		State state = goingtoeat.MoveTo((Instance smi) => targetCell.Get(smi), inhaling, null, false);
		string name = CREATURES.STATUSITEMS.LOOKINGFORFOOD.NAME;
		string tooltip = CREATURES.STATUSITEMS.LOOKINGFORFOOD.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(name, tooltip, "", StatusItem.IconType.Info, (NotificationType)0, false, default(HashedString), 0, null, null, main);
		State state2 = inhaling.DefaultState(inhaling.pre);
		tooltip = CREATURES.STATUSITEMS.INHALING.NAME;
		name = CREATURES.STATUSITEMS.INHALING.TOOLTIP;
		main = Db.Get().StatusItemCategories.Main;
		state2.ToggleStatusItem(tooltip, name, "", StatusItem.IconType.Info, (NotificationType)0, false, default(HashedString), 0, null, null, main);
		inhaling.pre.PlayAnim("inhale_pre").QueueAnim("inhale_loop", true, null).Update("Consume", delegate(Instance smi, float dt)
		{
			smi.GetSMI<GasAndLiquidConsumerMonitor.Instance>().Consume(dt);
		}, UpdateRate.SIM_200ms, false)
			.EventTransition(GameHashes.ElementNoLongerAvailable, inhaling.pst, null)
			.Enter("StartInhaleSound", delegate(Instance smi)
			{
				smi.StartInhaleSound();
			})
			.Exit("StopInhaleSound", delegate(Instance smi)
			{
				smi.StopInhaleSound();
			})
			.ScheduleGoTo((Instance smi) => smi.def.inhaleTime, inhaling.pst);
		inhaling.pst.Transition(inhaling.full, IsFull, UpdateRate.SIM_200ms).Transition(behaviourcomplete, GameStateMachine<InhaleStates, Instance, IStateMachineTarget, Def>.Not(IsFull), UpdateRate.SIM_200ms);
		inhaling.full.QueueAnim("inhale_pst", false, null).OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.PlayAnim("idle_loop", KAnim.PlayMode.Loop).BehaviourComplete(GameTags.Creatures.WantsToEat, false);
	}

	private static bool IsFull(Instance smi)
	{
		CreatureCalorieMonitor.Instance sMI = smi.GetSMI<CreatureCalorieMonitor.Instance>();
		if (sMI == null)
		{
			return false;
		}
		return sMI.stomach.GetFullness() >= 1f;
	}
}
