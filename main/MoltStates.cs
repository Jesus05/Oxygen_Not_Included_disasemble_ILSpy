using Klei;
using STRINGS;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MoltStates : GameStateMachine<MoltStates, MoltStates.Instance, IStateMachineTarget, MoltStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Vector3 eggPos;

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.ScalesGrown);
		}
	}

	public State moltpre;

	public State moltpst;

	public State behaviourcomplete;

	[CompilerGenerated]
	private static StateMachine<MoltStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache0;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = moltpre;
		State root = base.root;
		string name = CREATURES.STATUSITEMS.MOLTING.NAME;
		string tooltip = CREATURES.STATUSITEMS.MOLTING.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		root.ToggleStatusItem(name, tooltip, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, main);
		moltpre.Enter(Molt).QueueAnim("lay_egg_pre", false, null).OnAnimQueueComplete(moltpst);
		moltpst.QueueAnim("lay_egg_pst", false, null).OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.ScalesGrown, false);
	}

	private static void Molt(Instance smi)
	{
		smi.eggPos = smi.transform.GetPosition();
		smi.GetSMI<ScaleGrowthMonitor.Instance>().Shear();
	}

	private static int GetMoveAsideCell(Instance smi)
	{
		int num = 1;
		if (GenericGameSettings.instance.acceleratedLifecycle)
		{
			num = 8;
		}
		int cell = Grid.PosToCell(smi);
		if (Grid.IsValidCell(cell))
		{
			int num2 = Grid.OffsetCell(cell, num, 0);
			if (Grid.IsValidCell(num2) && !Grid.Solid[num2])
			{
				return num2;
			}
			int num3 = Grid.OffsetCell(cell, -num, 0);
			if (Grid.IsValidCell(num3))
			{
				return num3;
			}
		}
		return Grid.InvalidCell;
	}
}
