using Klei.AI;
using STRINGS;

[SkipSaveFileSerialization]
public class PrefersColder : StateMachineComponent<PrefersColder.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, PrefersColder, object>.GameInstance
	{
		public StatesInstance(PrefersColder master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, PrefersColder>
	{
		private AttributeModifier modifier = new AttributeModifier("ThermalConductivityBarrier", 0.005f, DUPLICANTS.TRAITS.NEEDS.PREFERSCOOLER.NAME, false, false, true);

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = root;
			root.ToggleAttributeModifier(DUPLICANTS.TRAITS.NEEDS.PREFERSCOOLER.NAME, (StatesInstance smi) => modifier, null);
		}
	}

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}
}
