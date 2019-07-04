public class DiseaseDropper : GameStateMachine<DiseaseDropper, DiseaseDropper.Instance, IStateMachineTarget, DiseaseDropper.Def>
{
	public class Def : BaseDef
	{
		public byte diseaseIdx = byte.MaxValue;

		public int singleEmitQuantity = 0;

		public int averageEmitPerSecond = 0;

		public float emitFrequency = 1f;
	}

	public new class Instance : GameInstance
	{
		private float timeSinceLastDrop = 0f;

		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
		}

		public bool ShouldDropDisease()
		{
			return true;
		}

		public void DropSingleEmit()
		{
			DropDisease(base.def.diseaseIdx, base.def.singleEmitQuantity);
		}

		public void DropPeriodic(float dt)
		{
			timeSinceLastDrop += dt;
			if (base.def.averageEmitPerSecond > 0 && base.def.emitFrequency > 0f)
			{
				while (timeSinceLastDrop > base.def.emitFrequency)
				{
					DropDisease(base.def.diseaseIdx, (int)((float)base.def.averageEmitPerSecond * base.def.emitFrequency));
					timeSinceLastDrop -= base.def.emitFrequency;
				}
			}
		}

		public void DropDisease(byte disease_idx, int disease_count)
		{
			if (disease_count > 0 && disease_idx != 255)
			{
				int num = Grid.PosToCell(base.transform.GetPosition());
				if (Grid.IsValidCell(num))
				{
					SimMessages.ModifyDiseaseOnCell(num, disease_idx, disease_count);
				}
			}
		}

		public bool IsValidDropCell()
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			if (Grid.IsValidCell(num))
			{
				if (Grid.IsGas(num))
				{
					if (!(Grid.Mass[num] > 1f))
					{
						return true;
					}
					return false;
				}
				return false;
			}
			return false;
		}
	}

	public State working;

	public State stopped;

	public Signal cellChangedSignal;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = stopped;
		root.EventHandler(GameHashes.BurstEmitDisease, delegate(Instance smi)
		{
			smi.DropSingleEmit();
		});
		working.TagTransition(GameTags.PreventEmittingDisease, stopped, false).Update(delegate(Instance smi, float dt)
		{
			smi.DropPeriodic(dt);
		}, UpdateRate.SIM_200ms, false);
		stopped.TagTransition(GameTags.PreventEmittingDisease, working, true);
	}
}
