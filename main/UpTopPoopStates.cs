using STRINGS;
using UnityEngine;

public class UpTopPoopStates : GameStateMachine<UpTopPoopStates, UpTopPoopStates.Instance, IStateMachineTarget, UpTopPoopStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Poop);
		}

		public int GetPoopCell()
		{
			int num = base.master.gameObject.GetComponent<Navigator>().maxProbingRadius - 1;
			int num2 = Grid.PosToCell(base.gameObject);
			int num3 = Grid.OffsetCell(num2, 0, 1);
			while (num > 0 && Grid.IsValidCell(num3) && !Grid.Solid[num3] && !IsClosedDoor(num3))
			{
				num--;
				num2 = num3;
				num3 = Grid.OffsetCell(num2, 0, 1);
			}
			return num2;
		}

		public bool IsClosedDoor(int cellAbove)
		{
			if (!Grid.HasDoor[cellAbove])
			{
				return false;
			}
			Door component = Grid.Objects[cellAbove, 1].GetComponent<Door>();
			return (Object)component != (Object)null && component.CurrentState != Door.ControlState.Opened;
		}
	}

	public State goingtopoop;

	public State pooping;

	public State behaviourcomplete;

	public IntParameter targetCell;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = goingtopoop;
		root.Enter("SetTarget", delegate(Instance smi)
		{
			targetCell.Set(smi.GetSMI<GasAndLiquidConsumerMonitor.Instance>().targetCell, smi);
		});
		goingtopoop.MoveTo((Instance smi) => smi.GetPoopCell(), pooping, pooping, false);
		State state = pooping.PlayAnim("poop");
		string name = CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME;
		string tooltip = CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(name, tooltip, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, main).OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.PlayAnim("idle_loop", KAnim.PlayMode.Loop).BehaviourComplete(GameTags.Creatures.Poop, false);
	}
}
