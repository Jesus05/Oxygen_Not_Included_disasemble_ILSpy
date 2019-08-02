using UnityEngine;

public class DiggerStates : GameStateMachine<DiggerStates, DiggerStates.Instance, IStateMachineTarget, DiggerStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Tunnel);
		}

		public int GetTunnelCell()
		{
			return base.smi.GetSMI<DiggerMonitor.Instance>()?.lastDigCell ?? (-1);
		}
	}

	public State move;

	public State hide;

	public State behaviourcomplete;

	private static float GetHideDuration()
	{
		if ((Object)SaveGame.Instance != (Object)null && (Object)SaveGame.Instance.GetComponent<SeasonManager>() != (Object)null)
		{
			return SaveGame.Instance.GetComponent<SeasonManager>().GetBombardmentDuration();
		}
		return 0f;
	}

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = move;
		move.MoveTo((Instance smi) => smi.GetTunnelCell(), hide, behaviourcomplete, false);
		hide.ScheduleGoTo(GetHideDuration(), behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.Tunnel, false);
	}
}
