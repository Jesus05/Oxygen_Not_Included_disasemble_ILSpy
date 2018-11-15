using System;
using System.Runtime.CompilerServices;
using UnityEngine;

internal class DiggerStates : GameStateMachine<DiggerStates, DiggerStates.Instance, IStateMachineTarget, DiggerStates.Def>
{
	public class Def : BaseDef
	{
		public int depthToDig
		{
			get;
			private set;
		}

		public Def(int depth)
		{
			depthToDig = depth;
		}
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Tunnel);
		}
	}

	public State surface;

	public State move;

	public State hide;

	public State behaviourcomplete;

	[CompilerGenerated]
	private static Func<Instance, int> _003C_003Ef__mg_0024cache0;

	private static int MoveToNewCell(Instance smi)
	{
		int num = Grid.OffsetCell(Grid.PosToCell(smi.master.gameObject), 0, smi.def.depthToDig);
		if (Grid.IsValidCell(num))
		{
			return num;
		}
		return Grid.PosToCell(smi.master.gameObject);
	}

	private static float GetHideDuration()
	{
		if ((UnityEngine.Object)SaveGame.Instance != (UnityEngine.Object)null && (UnityEngine.Object)SaveGame.Instance.GetComponent<SeasonManager>() != (UnityEngine.Object)null)
		{
			return SaveGame.Instance.GetComponent<SeasonManager>().GetBombardmentDuration();
		}
		return 0f;
	}

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = move;
		move.MoveTo(MoveToNewCell, hide, behaviourcomplete, false);
		hide.ScheduleGoTo(GetHideDuration(), behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.Tunnel, false);
	}
}
