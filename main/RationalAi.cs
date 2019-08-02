using UnityEngine;

public class RationalAi : GameStateMachine<RationalAi, RationalAi.Instance>
{
	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			ChoreConsumer component = GetComponent<ChoreConsumer>();
			component.AddUrge(Db.Get().Urges.EmoteHighPriority);
			component.AddUrge(Db.Get().Urges.EmoteIdle);
		}

		public void RefreshUserMenu()
		{
			Game.Instance.userMenu.Refresh(base.master.gameObject);
		}
	}

	public State alive;

	public State dead;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = alive;
		base.serializable = true;
		root.ToggleStateMachine((Instance smi) => new DeathMonitor.Instance(smi.master, new DeathMonitor.Def()));
		alive.TagTransition(GameTags.Dead, dead, false).ToggleStateMachine((Instance smi) => new ThoughtGraph.Instance(smi.master)).ToggleStateMachine((Instance smi) => new StaminaMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new StressMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new EmoteMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new SneezeMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new DecorMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new IncapacitationMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new IdleMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new RationMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new CalorieMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new DoctorMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new SicknessMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new GermExposureMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new BreathMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new RoomMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new TemperatureMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new ExternalTemperatureMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new BladderMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new SteppedInMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new LightMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new RedAlertMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new CringeMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new HygieneMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new FallMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new ThreatMonitor.Instance(smi.master, new ThreatMonitor.Def()))
			.ToggleStateMachine((Instance smi) => new WoundMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new TiredMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new MoveToLocationMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new ReactionMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new SuitWearer.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new TubeTraveller.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new MingleMonitor.Instance(smi.master))
			.ToggleStateMachine((Instance smi) => new MournMonitor.Instance(smi.master));
		dead.ToggleStateMachine((Instance smi) => new FallWhenDeadMonitor.Instance(smi.master)).ToggleBrain("dead").Enter("RefreshUserMenu", delegate(Instance smi)
		{
			smi.RefreshUserMenu();
		})
			.Enter("DropStorage", delegate(Instance smi)
			{
				smi.GetComponent<Storage>().DropAll(false, false, default(Vector3), true);
			});
	}
}
