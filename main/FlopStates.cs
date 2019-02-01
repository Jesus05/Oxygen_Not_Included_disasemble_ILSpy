using STRINGS;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

internal class FlopStates : GameStateMachine<FlopStates, FlopStates.Instance, IStateMachineTarget, FlopStates.Def>
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
	private static Action<Instance, float> _003C_003Ef__mg_0024cache2;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = flop_pre;
		State root = base.root;
		string name = CREATURES.STATUSITEMS.FLOPPING.NAME;
		string tooltip = CREATURES.STATUSITEMS.FLOPPING.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		root.ToggleStatusItem(name, tooltip, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 63486, null, null, main);
		flop_pre.Enter(ChooseDirection).GoTo(flop_cycle);
		flop_cycle.PlayAnim("flop_loop", KAnim.PlayMode.Once).Transition(pst, IsSubstantialLiquid, UpdateRate.SIM_200ms).Update("Flop", FlopForward, UpdateRate.SIM_33ms, false)
			.OnAnimQueueComplete(flop_pre);
		pst.QueueAnim("idle_loop", true, null).BehaviourComplete(GameTags.Creatures.Flopping, false);
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
			if (!Grid.Solid[Grid.PosToCell(vector)])
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
