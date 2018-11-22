using KSerialization;
using STRINGS;

internal class SameSpotPoopStates : GameStateMachine<SameSpotPoopStates, SameSpotPoopStates.Instance, IStateMachineTarget, SameSpotPoopStates.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		[Serialize]
		private int lastPoopCell = -1;

		public Instance(Chore<Instance> chore, Def def)
			: base((IStateMachineTarget)chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.Poop);
		}

		public int GetLastPoopCell()
		{
			if (lastPoopCell == -1)
			{
				SetLastPoopCell();
			}
			return lastPoopCell;
		}

		public void SetLastPoopCell()
		{
			lastPoopCell = Grid.PosToCell(this);
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
		goingtopoop.MoveTo((Instance smi) => smi.GetLastPoopCell(), pooping, pooping, false);
		State state = pooping.PlayAnim("poop");
		string name = CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME;
		string tooltip = CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(name, tooltip, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 63486, null, null, main).OnAnimQueueComplete(behaviourcomplete);
		behaviourcomplete.BehaviourComplete(GameTags.Creatures.Poop, false);
	}
}
