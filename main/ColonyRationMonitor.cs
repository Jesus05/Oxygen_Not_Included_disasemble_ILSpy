public class ColonyRationMonitor : GameStateMachine<ColonyRationMonitor, ColonyRationMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			UpdateIsOutOfRations();
		}

		public void UpdateIsOutOfRations()
		{
			bool value = true;
			foreach (Edible item in Components.Edibles.Items)
			{
				if (item.GetComponent<Pickupable>().UnreservedAmount > 0f)
				{
					value = false;
					break;
				}
			}
			base.smi.sm.isOutOfRations.Set(value, base.smi);
		}

		public bool IsOutOfRations()
		{
			return base.smi.sm.isOutOfRations.Get(base.smi);
		}
	}

	public State satisfied;

	public State outofrations;

	private BoolParameter isOutOfRations = new BoolParameter();

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		root.Update("UpdateOutOfRations", delegate(Instance smi, float dt)
		{
			smi.UpdateIsOutOfRations();
		}, UpdateRate.SIM_200ms, false);
		satisfied.ParamTransition(isOutOfRations, outofrations, (Instance smi, bool p) => p).TriggerOnEnter(GameHashes.ColonyHasRationsChanged, null);
		outofrations.ParamTransition(isOutOfRations, satisfied, (Instance smi, bool p) => !p).TriggerOnEnter(GameHashes.ColonyHasRationsChanged, null);
	}
}
