using Klei;
using STRINGS;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

internal class LayEggStates : GameStateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>
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
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Fertile);
		}
	}

	public State layeggpre;

	public State layeggpst;

	public State moveaside;

	public State lookategg;

	public State behaviourcomplete;

	[CompilerGenerated]
	private static StateMachine<LayEggStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static StateMachine<LayEggStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static Func<Instance, int> _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static StateMachine<LayEggStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache3;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = layeggpre;
		State root = base.root;
		string name = CREATURES.STATUSITEMS.LAYINGANEGG.NAME;
		string tooltip = CREATURES.STATUSITEMS.LAYINGANEGG.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		root.ToggleStatusItem(name, tooltip, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 63486, null, null, main);
		layeggpre.Enter(LayEgg).Exit(ShowEgg).PlayAnim("lay_egg_pre")
			.OnAnimQueueComplete(layeggpst);
		layeggpst.PlayAnim("lay_egg_pst").OnAnimQueueComplete(moveaside);
		moveaside.MoveTo(GetMoveAsideCell, lookategg, behaviourcomplete, false);
		lookategg.Enter(FaceEgg).GoTo(behaviourcomplete);
		behaviourcomplete.QueueAnim("idle_loop", true, null).BehaviourComplete(GameTags.Creatures.Fertile, false);
	}

	private static void LayEgg(Instance smi)
	{
		smi.eggPos = smi.transform.GetPosition();
		smi.GetSMI<FertilityMonitor.Instance>().LayEgg();
	}

	private static void ShowEgg(Instance smi)
	{
		smi.GetSMI<FertilityMonitor.Instance>().ShowEgg();
	}

	private static void FaceEgg(Instance smi)
	{
		smi.Get<Facing>().Face(smi.eggPos);
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
