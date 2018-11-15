public class SubmergedMonitor : GameStateMachine<SubmergedMonitor, SubmergedMonitor.Instance, IStateMachineTarget, SubmergedMonitor.Def>
{
	public class Def : BaseDef
	{
	}

	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public bool IsSubmerged()
		{
			return Grid.IsSubstantialLiquid(Grid.PosToCell(base.transform.GetPosition()), 0.35f);
		}
	}

	public State satisfied;

	public State submerged;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		root.ToggleBehaviour(GameTags.Creatures.Submerged, (Instance smi) => smi.IsSubmerged(), null);
		satisfied.Enter("SetNavType", delegate(Instance smi)
		{
			smi.GetComponent<Navigator>().SetCurrentNavType(NavType.Hover);
		}).Update("SetNavType", delegate(Instance smi, float dt)
		{
			smi.GetComponent<Navigator>().SetCurrentNavType(NavType.Hover);
		}, UpdateRate.SIM_1000ms, false).Transition(submerged, (Instance smi) => smi.IsSubmerged(), UpdateRate.SIM_1000ms);
		submerged.Enter("SetNavType", delegate(Instance smi)
		{
			smi.GetComponent<Navigator>().SetCurrentNavType(NavType.Swim);
		}).Update("SetNavType", delegate(Instance smi, float dt)
		{
			smi.GetComponent<Navigator>().SetCurrentNavType(NavType.Swim);
		}, UpdateRate.SIM_1000ms, false).Transition(satisfied, (Instance smi) => !smi.IsSubmerged(), UpdateRate.SIM_1000ms);
	}
}
