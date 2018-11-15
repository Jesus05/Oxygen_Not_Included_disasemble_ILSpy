using Klei.AI;
using STRINGS;

[SkipSaveFileSerialization]
public class PrefersWarmer : StateMachineComponent<PrefersWarmer.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, PrefersWarmer, object>.GameInstance
	{
		public StatesInstance(PrefersWarmer master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, PrefersWarmer>
	{
		private AttributeModifier modifier = new AttributeModifier("ThermalConductivityBarrier", -0.005f, DUPLICANTS.TRAITS.NEEDS.PREFERSWARMER.NAME, false, false, true);

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = root;
			root.ToggleAttributeModifier(DUPLICANTS.TRAITS.NEEDS.PREFERSWARMER.NAME, (StatesInstance smi) => modifier, null);
		}
	}

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}
}
