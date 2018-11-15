using Klei.AI;
using KSerialization;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class WaterCooler : StateMachineComponent<WaterCooler.StatesInstance>, IApproachable, IEffectDescriptor
{
	public class States : GameStateMachine<States, StatesInstance, WaterCooler>
	{
		public State unoperational;

		public State waitingfordelivery;

		public State dispensing;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = unoperational;
			unoperational.TagTransition(GameTags.Operational, waitingfordelivery, false).PlayAnim("off");
			waitingfordelivery.TagTransition(GameTags.Operational, unoperational, true).Transition(dispensing, (StatesInstance smi) => smi.HasMinimumMass(), UpdateRate.SIM_200ms).EventTransition(GameHashes.OnStorageChange, dispensing, (StatesInstance smi) => smi.HasMinimumMass())
				.PlayAnim("off");
			dispensing.Enter("StartMeter", delegate(StatesInstance smi)
			{
				smi.StartMeter();
			}).Enter("UpdateDrinkChores.force", delegate(StatesInstance smi)
			{
				smi.master.UpdateDrinkChores(true);
			}).Update("UpdateDrinkChores", delegate(StatesInstance smi, float dt)
			{
				smi.master.UpdateDrinkChores(true);
			}, UpdateRate.SIM_200ms, false)
				.Exit("CancelDrinkChores", delegate(StatesInstance smi)
				{
					smi.master.CancelDrinkChores();
				})
				.TagTransition(GameTags.Operational, unoperational, true)
				.EventTransition(GameHashes.OnStorageChange, waitingfordelivery, (StatesInstance smi) => !smi.HasMinimumMass())
				.PlayAnim("working");
		}
	}

	public class StatesInstance : GameStateMachine<States, StatesInstance, WaterCooler, object>.GameInstance
	{
		private Storage storage;

		private MeterController meter;

		public StatesInstance(WaterCooler smi)
			: base(smi)
		{
			meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_bottle", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, "meter_bottle");
			storage = base.master.GetComponent<Storage>();
			Subscribe(-1697596308, OnStorageChange);
		}

		private void OnStorageChange(object data)
		{
			float positionPercent = Mathf.Clamp01(storage.MassStored() / storage.capacityKg);
			meter.SetPositionPercent(positionPercent);
		}

		public void StartMeter()
		{
			PrimaryElement primaryElement = storage.FindFirstWithMass(GameTags.Water);
			if (!((Object)primaryElement == (Object)null))
			{
				meter.SetSymbolTint(new KAnimHashedString("meter_water"), primaryElement.Element.substance.colour);
				OnStorageChange(null);
			}
		}

		public bool HasMinimumMass()
		{
			return storage.GetMassAvailable(GameTags.Water) >= 1f;
		}
	}

	public const float DRINK_MASS = 1f;

	public const string SPECIFIC_EFFECT = "Socialized";

	public CellOffset[] socializeOffsets = new CellOffset[4]
	{
		new CellOffset(-1, 0),
		new CellOffset(2, 0),
		new CellOffset(0, 0),
		new CellOffset(1, 0)
	};

	public int choreCount = 2;

	public float workTime = 5f;

	private CellOffset[] drinkOffsets = new CellOffset[2]
	{
		new CellOffset(0, 0),
		new CellOffset(1, 0)
	};

	private Chore[] chores;

	private HandleVector<int>.Handle validNavCellChangedPartitionerEntry;

	private SocialGatheringPointWorkable[] workables;

	[MyCmpGet]
	private Storage storage;

	public bool choresDirty;

	private static readonly EventSystem.IntraObjectHandler<WaterCooler> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<WaterCooler>(delegate(WaterCooler component, object data)
	{
		component.OnStorageChange(data);
	});

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GameScheduler.Instance.Schedule("Scheduling Tutorial", 2f, delegate
		{
			Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Schedule);
		}, null, null);
		workables = new SocialGatheringPointWorkable[socializeOffsets.Length];
		for (int i = 0; i < workables.Length; i++)
		{
			int cell = Grid.OffsetCell(Grid.PosToCell(this), socializeOffsets[i]);
			Vector3 pos = Grid.CellToPosCBC(cell, Grid.SceneLayer.Move);
			GameObject go = ChoreHelpers.CreateLocator("WaterCoolerWorkable", pos);
			SocialGatheringPointWorkable socialGatheringPointWorkable = go.AddOrGet<SocialGatheringPointWorkable>();
			socialGatheringPointWorkable.specificEffect = "Socialized";
			socialGatheringPointWorkable.SetWorkTime(workTime);
			workables[i] = socialGatheringPointWorkable;
		}
		chores = new Chore[socializeOffsets.Length];
		Extents extents = new Extents(Grid.PosToCell(this), socializeOffsets);
		validNavCellChangedPartitionerEntry = GameScenePartitioner.Instance.Add("WaterCooler", this, extents, GameScenePartitioner.Instance.validNavCellChangedLayer, OnCellChanged);
		Subscribe(-1697596308, OnStorageChangeDelegate);
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref validNavCellChangedPartitionerEntry);
		CancelDrinkChores();
		for (int i = 0; i < workables.Length; i++)
		{
			if ((bool)workables[i])
			{
				Util.KDestroyGameObject(workables[i]);
				workables[i] = null;
			}
		}
		base.OnCleanUp();
	}

	public void UpdateDrinkChores(bool force = true)
	{
		if (force || choresDirty)
		{
			float num = storage.GetMassAvailable(GameTags.Water);
			int num2 = 0;
			for (int i = 0; i < socializeOffsets.Length; i++)
			{
				CellOffset offset = socializeOffsets[i];
				Chore chore = chores[i];
				if (num2 < choreCount && IsOffsetValid(offset) && num >= 1f)
				{
					num2++;
					num -= 1f;
					if (chore == null || chore.isComplete)
					{
						chores[i] = new WaterCoolerChore(this, workables[i], null, null, OnChoreEnd);
					}
				}
				else if (chore != null)
				{
					chore.Cancel("invalid");
					chores[i] = null;
				}
			}
			choresDirty = false;
		}
	}

	public void CancelDrinkChores()
	{
		for (int i = 0; i < socializeOffsets.Length; i++)
		{
			Chore chore = chores[i];
			if (chore != null)
			{
				chore.Cancel("cancelled");
				chores[i] = null;
			}
		}
	}

	private bool IsOffsetValid(CellOffset offset)
	{
		int cell = Grid.PosToCell(this);
		int cell2 = Grid.OffsetCell(cell, offset);
		int anchor_cell = Grid.CellBelow(cell2);
		return GameNavGrids.FloorValidator.IsWalkableCell(cell2, anchor_cell, Grid.BitFields, false, false);
	}

	private void OnChoreEnd(Chore chore)
	{
		choresDirty = true;
	}

	private void OnCellChanged(object data)
	{
		choresDirty = true;
	}

	private void OnStorageChange(object data)
	{
		choresDirty = true;
	}

	public CellOffset[] GetOffsets()
	{
		return drinkOffsets;
	}

	public int GetCell()
	{
		return Grid.PosToCell(this);
	}

	private void AddRequirementDesc(List<Descriptor> descs, Tag tag, float mass)
	{
		string arg = tag.ProperName();
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, arg, GameUtil.GetFormattedMass(mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, arg, GameUtil.GetFormattedMass(mass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.##}")), Descriptor.DescriptorType.Requirement);
		descs.Add(item);
	}

	List<Descriptor> IEffectDescriptor.GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor item = default(Descriptor);
		item.SetupDescriptor(UI.BUILDINGEFFECTS.RECREATION, UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION, Descriptor.DescriptorType.Effect);
		list.Add(item);
		Effect.AddModifierDescriptions(base.gameObject, list, "Socialized", true);
		AddRequirementDesc(list, GameTags.Water, 1f);
		return list;
	}

	Transform get_transform()
	{
		return base.transform;
	}

	Transform IApproachable.get_transform()
	{
		//ILSpy generated this explicit interface implementation from .override directive in get_transform
		return this.get_transform();
	}
}
