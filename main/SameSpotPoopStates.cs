using KSerialization;
using STRINGS;

public class SameSpotPoopStates : GameStateMachine<SameSpotPoopStates, SameSpotPoopStates.Instance, IStateMachineTarget, SameSpotPoopStates.Def>
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

	public State updatepoopcell;

	public IntParameter targetCell;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = goingtopoop;
		root.Enter("SetTarget", delegate(Instance smi)
		{
			targetCell.Set(smi.GetSMI<GasAndLiquidConsumerMonitor.Instance>().targetCell, smi);
		});
		goingtopoop.MoveTo((Instance smi) => smi.GetLastPoopCell(), pooping, updatepoopcell, false);
		State state = pooping.PlayAnim("poop");
		string name = CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME;
		string tooltip = CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(name, tooltip, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, main).OnAnimQueueComplete(behaviourcomplete);
		updatepoopcell.Enter(delegate(Instance smi)
		{
			smi.SetLastPoopCell();
		}).GoTo(pooping);
		behaviourcomplete.PlayAnim("idle_loop", KAnim.PlayMode.Loop).BehaviourComplete(GameTags.Creatures.Poop, false);
	}
}
