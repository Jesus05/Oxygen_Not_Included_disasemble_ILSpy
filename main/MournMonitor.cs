using Klei.AI;

public class MournMonitor : GameStateMachine<MournMonitor, MournMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}
	}

	private State idle;

	private State needsToMourn;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = idle;
		idle.EventHandler(GameHashes.EffectAdded, OnEffectAdded).Enter(delegate(Instance smi)
		{
			if (ShouldMourn(smi))
			{
				smi.GoTo(needsToMourn);
			}
		});
		needsToMourn.ToggleChore((Instance smi) => new MournChore(smi.master), idle);
	}

	private bool ShouldMourn(Instance smi)
	{
		Effect effect = Db.Get().effects.Get("Mourning");
		Effects component = smi.master.GetComponent<Effects>();
		return component.HasEffect(effect);
	}

	private void OnEffectAdded(Instance smi, object data)
	{
		if (ShouldMourn(smi))
		{
			smi.GoTo(needsToMourn);
		}
	}
}
