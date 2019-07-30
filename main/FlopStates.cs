using STRINGS;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FlopStates : GameStateMachine<FlopStates, FlopStates.Instance, IStateMachineTarget, FlopStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public float currentDir = 1f;

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Flopping);
		}
	}

	private State flop_pre;

	private State flop_cycle;

	private State pst;

	[CompilerGenerated]
	private static StateMachine<FlopStates, Instance, IStateMachineTarget, Def>.State.Callback _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static Transition.ConditionCallback _003C_003Ef__mg_0024cache3;

	[CompilerGenerated]
	private static Action<Instance, float> _003C_003Ef__mg_0024cache4;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = flop_pre;
		root.ToggleStatusItem(CREATURES.STATUSITEMS.FLOPPING.NAME, CREATURES.STATUSITEMS.FLOPPING.TOOLTIP, category: Db.Get().StatusItemCategories.Main, icon: string.Empty, icon_type: StatusItem.IconType.Info, notification_type: NotificationType.Neutral, allow_multiples: false, render_overlay: default(HashedString), status_overlays: 129022, resolve_string_callback: null, resolve_tooltip_callback: null);
		flop_pre.Enter(ChooseDirection).Transition(flop_cycle, ShouldFlop, UpdateRate.SIM_200ms).Transition(pst, GameStateMachine<FlopStates, Instance, IStateMachineTarget, Def>.Not(ShouldFlop), UpdateRate.SIM_200ms);
		flop_cycle.PlayAnim("flop_loop", KAnim.PlayMode.Once).Transition(pst, IsSubstantialLiquid, UpdateRate.SIM_200ms).Update("Flop", FlopForward, UpdateRate.SIM_33ms, false)
			.OnAnimQueueComplete(flop_pre);
		pst.QueueAnim("flop_loop", true, null).BehaviourComplete(GameTags.Creatures.Flopping, false);
	}

	public static bool ShouldFlop(Instance smi)
	{
		int cell = Grid.PosToCell(smi.transform.GetPosition());
		int num = Grid.CellBelow(cell);
		return Grid.IsValidCell(num) && Grid.Solid[num];
	}

	public static void ChooseDirection(Instance smi)
	{
		int cell = Grid.PosToCell(smi.transform.GetPosition());
		if (SearchForLiquid(cell, 1))
		{
			smi.currentDir = 1f;
		}
		else if (SearchForLiquid(cell, -1))
		{
			smi.currentDir = -1f;
		}
		else if (UnityEngine.Random.value > 0.5f)
		{
			smi.currentDir = 1f;
		}
		else
		{
			smi.currentDir = -1f;
		}
	}

	private static bool SearchForLiquid(int cell, int delta_x)
	{
		while (true)
		{
			if (!Grid.IsValidCell(cell))
			{
				return false;
			}
			if (Grid.IsSubstantialLiquid(cell, 0.35f))
			{
				return true;
			}
			if (Grid.Solid[cell])
			{
				return false;
			}
			if (Grid.CritterImpassable[cell])
			{
				break;
			}
			int num = Grid.CellBelow(cell);
			cell = ((!Grid.IsValidCell(num) || !Grid.Solid[num]) ? num : (cell + delta_x));
		}
		return false;
	}

	public static void FlopForward(Instance smi, float dt)
	{
		int currentFrame = smi.GetComponent<KBatchedAnimController>().currentFrame;
		if (currentFrame >= 23 && currentFrame <= 36)
		{
			Vector3 position = smi.transform.GetPosition();
			Vector3 vector = position;
			vector.x = position.x + smi.currentDir * dt * 1f;
			int num = Grid.PosToCell(vector);
			if (Grid.IsValidCell(num) && !Grid.Solid[num] && !Grid.CritterImpassable[num])
			{
				smi.transform.SetPosition(vector);
			}
			else
			{
				smi.currentDir = 0f - smi.currentDir;
			}
		}
	}

	public static bool IsSubstantialLiquid(Instance smi)
	{
		return Grid.IsSubstantialLiquid(Grid.PosToCell(smi.transform.GetPosition()), 0.35f);
	}
}
