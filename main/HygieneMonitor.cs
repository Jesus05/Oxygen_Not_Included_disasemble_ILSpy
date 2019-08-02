using Klei.AI;

public class HygieneMonitor : GameStateMachine<HygieneMonitor, HygieneMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		private Effects effects;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			effects = master.GetComponent<Effects>();
		}

		public float GetDirtiness()
		{
			return base.sm.dirtiness.Get(this);
		}

		public void SetDirtiness(float dirtiness)
		{
			base.sm.dirtiness.Set(dirtiness, this);
		}

		public bool NeedsShower()
		{
			return !effects.HasEffect(Shower.SHOWER_EFFECT);
		}
	}

	public FloatParameter dirtiness;

	public State clean;

	public State needsshower;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = needsshower;
		base.serializable = true;
		clean.EventTransition(GameHashes.EffectRemoved, needsshower, (Instance smi) => smi.NeedsShower());
		needsshower.EventTransition(GameHashes.EffectAdded, clean, (Instance smi) => !smi.NeedsShower()).ToggleUrge(Db.Get().Urges.Shower).Enter(delegate(Instance smi)
		{
			smi.SetDirtiness(1f);
		});
	}
}
