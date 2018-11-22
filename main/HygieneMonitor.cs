using Klei.AI;

public class HygieneMonitor : GameStateMachine<HygieneMonitor, HygieneMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		private Effects effects;

		private static readonly string[] NeedsShowerEffectsIDs = new string[1]
		{
			"Unclean"
		};

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			effects = master.GetComponent<Effects>();
		}

		public void BecomeDirty()
		{
			SetDirtiness(1f);
			effects.Add("Unclean", true);
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
			bool result = false;
			for (int i = 0; i < NeedsShowerEffectsIDs.Length; i++)
			{
				string effect_id = NeedsShowerEffectsIDs[i];
				if (effects.HasEffect(effect_id))
				{
					result = true;
					break;
				}
			}
			return result;
		}

		private bool IsDirty(int cell)
		{
			if (Grid.IsValidCell(cell))
			{
				Element element = Grid.Element[cell];
				return element.IsLiquid && element.id != SimHashes.Water;
			}
			return false;
		}

		public void UpdateDirtiness()
		{
			int cell = Grid.PosToCell(base.master.transform.GetPosition());
			int cell2 = Grid.CellAbove(cell);
			if (IsDirty(cell) || IsDirty(cell2))
			{
				base.master.GetComponent<Effects>().Add("Unclean", true);
			}
		}
	}

	public FloatParameter dirtiness;

	public State clean;

	public State needsshower_pre;

	public State needsshower;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = clean;
		base.serializable = true;
		root.Update(delegate(Instance smi, float dt)
		{
			smi.UpdateDirtiness();
		}, UpdateRate.SIM_200ms, false).EventHandler(GameHashes.SleepFinished, delegate(Instance smi)
		{
			smi.BecomeDirty();
		});
		clean.EventTransition(GameHashes.EffectAdded, needsshower, (Instance smi) => smi.NeedsShower());
		needsshower_pre.Enter(delegate(Instance smi)
		{
			smi.BecomeDirty();
			smi.GoTo(needsshower);
		});
		needsshower.EventTransition(GameHashes.EffectRemoved, clean, (Instance smi) => !smi.NeedsShower()).ToggleUrge(Db.Get().Urges.Shower).Exit(delegate(Instance smi)
		{
			smi.SetDirtiness(0f);
		});
	}
}
