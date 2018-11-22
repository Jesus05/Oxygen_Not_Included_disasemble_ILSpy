using UnityEngine;

public class ElementDropperMonitor : GameStateMachine<ElementDropperMonitor, ElementDropperMonitor.Instance, IStateMachineTarget, ElementDropperMonitor.Def>
{
	public class Def : BaseDef
	{
		public SimHashes dirtyEmitElement;

		public float dirtyProbabilityPercent;

		public float dirtyCellToTargetMass;

		public float dirtyMassPerDirty;

		public float dirtyMassReleaseOnDeath;

		public byte emitDiseaseIdx = byte.MaxValue;

		public float emitDiseasePerKg;
	}

	public new class Instance : GameInstance
	{
		public Instance(IStateMachineTarget master, Def def)
			: base(master, def)
		{
			Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, OnCellChange, "ElementDropperMonitor.Instance");
		}

		public override void StopSM(string reason)
		{
			base.StopSM(reason);
			Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, OnCellChange);
		}

		private void OnCellChange()
		{
			base.sm.cellChangedSignal.Trigger(this);
		}

		public bool ShouldDropElement()
		{
			return IsValidDropCell() && Random.Range(0f, 100f) < base.def.dirtyProbabilityPercent;
		}

		public void DropDeathElement()
		{
			DropElement(base.def.dirtyMassReleaseOnDeath, base.def.dirtyEmitElement, base.def.emitDiseaseIdx, Mathf.RoundToInt(base.def.dirtyMassReleaseOnDeath * base.def.dirtyMassPerDirty));
		}

		public void DropPeriodicElement()
		{
			DropElement(base.def.dirtyMassPerDirty, base.def.dirtyEmitElement, base.def.emitDiseaseIdx, Mathf.RoundToInt(base.def.emitDiseasePerKg * base.def.dirtyMassPerDirty));
		}

		public void DropElement(float mass, SimHashes element_id, byte disease_idx, int disease_count)
		{
			if (!(mass <= 0f))
			{
				Element element = ElementLoader.FindElementByHash(element_id);
				float temperature = GetComponent<PrimaryElement>().Temperature;
				if (element.IsGas || element.IsLiquid)
				{
					int gameCell = Grid.PosToCell(base.transform.GetPosition());
					SimMessages.AddRemoveSubstance(gameCell, element_id, CellEventLogger.Instance.ElementConsumerSimUpdate, mass, temperature, disease_idx, disease_count, true, -1);
				}
				else if (element.IsSolid)
				{
					element.substance.SpawnResource(base.transform.GetPosition() + new Vector3(0f, 0.5f, 0f), mass, temperature, disease_idx, disease_count, false, true);
				}
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, element.name, base.gameObject.transform, 1.5f, false);
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

	public State satisfied;

	public State readytodrop;

	public Signal cellChangedSignal;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		root.EventHandler(GameHashes.DeathAnimComplete, delegate(Instance smi)
		{
			smi.DropDeathElement();
		});
		satisfied.OnSignal(cellChangedSignal, readytodrop, (Instance smi) => smi.ShouldDropElement());
		readytodrop.ToggleBehaviour(GameTags.Creatures.WantsToDropElements, (Instance smi) => true, delegate(Instance smi)
		{
			smi.GoTo(satisfied);
		}).EventHandler(GameHashes.ObjectMovementStateChanged, delegate(Instance smi, object d)
		{
			if ((GameHashes)d == GameHashes.ObjectMovementWakeUp)
			{
				smi.GoTo(satisfied);
			}
		});
	}
}
