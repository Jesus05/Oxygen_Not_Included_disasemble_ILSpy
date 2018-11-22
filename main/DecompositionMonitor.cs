using Klei.AI;
using STRINGS;
using UnityEngine;

public class DecompositionMonitor : GameStateMachine<DecompositionMonitor, DecompositionMonitor.Instance>
{
	public class SubmergedState : State
	{
		public State idle;

		public State dirtywater;
	}

	public class ExposedState : State
	{
		public SubmergedState submerged;

		public State openair;
	}

	public class RottenState : State
	{
		public ExposedState exposed;

		public State stored;

		public State spawningmonster;
	}

	public new class Instance : GameInstance
	{
		public float decompositionRate;

		public Disease disease;

		public int dirtyWaterMaxRange = 3;

		public bool spawnsRotMonsters = true;

		public AttributeModifier satisfiedDecorModifier = new AttributeModifier(Db.Get().BuildingAttributes.Decor.Id, -65f, DUPLICANTS.MODIFIERS.DEAD.NAME, false, false, true);

		public AttributeModifier satisfiedDecorRadiusModifier = new AttributeModifier(Db.Get().BuildingAttributes.DecorRadius.Id, 4f, DUPLICANTS.MODIFIERS.DEAD.NAME, false, false, true);

		public AttributeModifier rottenDecorModifier = new AttributeModifier(Db.Get().BuildingAttributes.Decor.Id, -100f, DUPLICANTS.MODIFIERS.ROTTING.NAME, false, false, true);

		public AttributeModifier rottenDecorRadiusModifier = new AttributeModifier(Db.Get().BuildingAttributes.DecorRadius.Id, 4f, DUPLICANTS.MODIFIERS.ROTTING.NAME, false, false, true);

		public Instance(IStateMachineTarget master, Disease disease, float decompositionRate = 0.000833333354f, bool spawnRotMonsters = true)
			: base(master)
		{
			base.gameObject.AddComponent<DecorProvider>();
			this.decompositionRate = decompositionRate;
			this.disease = disease;
			spawnsRotMonsters = spawnRotMonsters;
		}

		public void UpdateDecomposition(float dt)
		{
			float delta_value = dt * decompositionRate;
			base.sm.decomposition.Delta(delta_value, base.smi);
		}

		public bool IsExposed()
		{
			KPrefabID component = base.smi.GetComponent<KPrefabID>();
			return (Object)component == (Object)null || !component.HasTag(GameTags.Preserved);
		}

		public bool IsRotten()
		{
			return IsInsideState(base.sm.rotten);
		}

		public bool IsSubmerged()
		{
			return PathFinder.IsSubmerged(Grid.PosToCell(base.master.transform.GetPosition()));
		}

		public void DirtyWater(int maxCellRange = 3)
		{
			int num = Grid.PosToCell(base.master.transform.GetPosition());
			if (Grid.Element[num].id == SimHashes.Water)
			{
				SimMessages.ReplaceElement(num, SimHashes.DirtyWater, CellEventLogger.Instance.DecompositionDirtyWater, Grid.Mass[num], Grid.Temperature[num], Grid.DiseaseIdx[num], Grid.DiseaseCount[num], -1);
			}
			else if (Grid.Element[num].id == SimHashes.DirtyWater)
			{
				int[] array = new int[4];
				for (int i = 0; i < maxCellRange; i++)
				{
					for (int j = 0; j < maxCellRange; j++)
					{
						array[0] = Grid.OffsetCell(num, new CellOffset(-i, j));
						array[1] = Grid.OffsetCell(num, new CellOffset(i, j));
						array[2] = Grid.OffsetCell(num, new CellOffset(-i, -j));
						array[3] = Grid.OffsetCell(num, new CellOffset(i, -j));
						array.Shuffle();
						int[] array2 = array;
						foreach (int num2 in array2)
						{
							if (Grid.GetCellDistance(num, num2) < maxCellRange - 1 && Grid.IsValidCell(num2) && Grid.Element[num2].id == SimHashes.Water)
							{
								SimMessages.ReplaceElement(num2, SimHashes.DirtyWater, CellEventLogger.Instance.DecompositionDirtyWater, Grid.Mass[num2], Grid.Temperature[num2], Grid.DiseaseIdx[num2], Grid.DiseaseCount[num2], -1);
								return;
							}
						}
					}
				}
			}
		}
	}

	public FloatParameter decomposition;

	[SerializeField]
	public int remainingRotMonsters = 3;

	public State satisfied;

	public RottenState rotten;

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = satisfied;
		base.serializable = true;
		satisfied.Update("UpdateDecomposition", delegate(Instance smi, float dt)
		{
			smi.UpdateDecomposition(dt);
		}, UpdateRate.SIM_200ms, false).ParamTransition(decomposition, rotten, GameStateMachine<DecompositionMonitor, Instance, IStateMachineTarget, object>.IsGTEOne).ToggleAttributeModifier("Dead", (Instance smi) => smi.satisfiedDecorModifier, null)
			.ToggleAttributeModifier("Dead", (Instance smi) => smi.satisfiedDecorRadiusModifier, null);
		rotten.DefaultState(rotten.exposed).ToggleStatusItem(Db.Get().DuplicantStatusItems.Rotten, (object)null).ToggleAttributeModifier("Rotten", (Instance smi) => smi.rottenDecorModifier, null)
			.ToggleAttributeModifier("Rotten", (Instance smi) => smi.rottenDecorRadiusModifier, null);
		rotten.exposed.DefaultState(rotten.exposed.openair).EventTransition(GameHashes.OnStore, rotten.stored, (Instance smi) => !smi.IsExposed());
		rotten.exposed.openair.Enter(delegate(Instance smi)
		{
			if (smi.spawnsRotMonsters)
			{
				smi.ScheduleGoTo(Random.Range(150f, 300f), rotten.spawningmonster);
			}
		}).Transition(rotten.exposed.submerged, (Instance smi) => smi.IsSubmerged(), UpdateRate.SIM_200ms).ToggleFX((Instance smi) => CreateFX(smi));
		rotten.exposed.submerged.DefaultState(rotten.exposed.submerged.idle).Transition(rotten.exposed.openair, (Instance smi) => !smi.IsSubmerged(), UpdateRate.SIM_200ms);
		rotten.exposed.submerged.idle.ScheduleGoTo(0.25f, rotten.exposed.submerged.dirtywater);
		rotten.exposed.submerged.dirtywater.Enter("DirtyWater", delegate(Instance smi)
		{
			smi.DirtyWater(smi.dirtyWaterMaxRange);
		}).GoTo(rotten.exposed.submerged.idle);
		rotten.spawningmonster.Enter(delegate(Instance smi)
		{
			if (remainingRotMonsters > 0)
			{
				remainingRotMonsters--;
				GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(new Tag("Glom")), smi.transform.GetPosition(), Grid.SceneLayer.Creatures, null, 0);
				gameObject.SetActive(true);
			}
			smi.GoTo(rotten.exposed);
		});
		rotten.stored.EventTransition(GameHashes.OnStore, rotten.exposed, (Instance smi) => smi.IsExposed());
	}

	private FliesFX.Instance CreateFX(Instance smi)
	{
		if (smi.isMasterNull)
		{
			return null;
		}
		return new FliesFX.Instance(smi.master, new Vector3(0f, 0f, -0.1f));
	}
}
