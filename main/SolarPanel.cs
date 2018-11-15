using KSerialization;
using System;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SolarPanel : Generator
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, SolarPanel, object>.GameInstance
	{
		public StatesInstance(SolarPanel master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, SolarPanel>
	{
		public State idle;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			idle.DoNothing();
		}
	}

	private MeterController meter;

	private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;

	private StatesInstance smi;

	private Guid statusHandle;

	private const Sim.Cell.Properties floorCellProperties = (Sim.Cell.Properties)39;

	private CellOffset[] solarCellOffsets = new CellOffset[14]
	{
		new CellOffset(-3, 2),
		new CellOffset(-2, 2),
		new CellOffset(-1, 2),
		new CellOffset(0, 2),
		new CellOffset(1, 2),
		new CellOffset(2, 2),
		new CellOffset(3, 2),
		new CellOffset(-3, 1),
		new CellOffset(-2, 1),
		new CellOffset(-1, 1),
		new CellOffset(0, 1),
		new CellOffset(1, 1),
		new CellOffset(2, 1),
		new CellOffset(3, 1)
	};

	private static readonly EventSystem.IntraObjectHandler<SolarPanel> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<SolarPanel>(delegate(SolarPanel component, object data)
	{
		component.OnActiveChanged(data);
	});

	public float CurrentWattage => Game.Instance.accumulators.GetAverageRate(accumulator);

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Subscribe(824508782, OnActiveChangedDelegate);
		smi = new StatesInstance(this);
		smi.StartSM();
		accumulator = Game.Instance.accumulators.Add("Element", this);
		BuildingDef def = GetComponent<BuildingComplete>().Def;
		int cell = Grid.PosToCell(this);
		for (int i = 0; i < def.WidthInCells; i++)
		{
			int x = i - (def.WidthInCells - 1) / 2;
			int num = Grid.OffsetCell(cell, new CellOffset(x, 0));
			SimMessages.SetCellProperties(num, 39);
			Grid.Foundation[num] = true;
			Grid.PreviousSolid[num] = Grid.Solid[num];
			Grid.SetSolid(num, true, CellEventLogger.Instance.SimCellOccupierForceSolid);
			World.Instance.OnSolidChanged(num);
			GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.solidChangedLayer, null);
			Grid.RenderedByWorld[num] = false;
		}
		meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_target", "meter_fill", "meter_frame", "meter_OL");
	}

	protected override void OnCleanUp()
	{
		smi.StopSM("cleanup");
		BuildingDef def = GetComponent<BuildingComplete>().Def;
		int cell = Grid.PosToCell(this);
		for (int i = 0; i < def.WidthInCells; i++)
		{
			int x = i - (def.WidthInCells - 1) / 2;
			int num = Grid.OffsetCell(cell, new CellOffset(x, 0));
			SimMessages.ClearCellProperties(num, 39);
			Grid.Foundation[num] = false;
			Grid.PreviousSolid[num] = Grid.Solid[num];
			Grid.SetSolid(num, false, CellEventLogger.Instance.SimCellOccupierForceSolid);
			World.Instance.OnSolidChanged(num);
			GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.solidChangedLayer, null);
			Grid.RenderedByWorld[num] = true;
		}
		Game.Instance.accumulators.Remove(accumulator);
		base.OnCleanUp();
	}

	protected void OnActiveChanged(object data)
	{
		StatusItem status_item = (!((Operational)data).IsActive) ? Db.Get().BuildingStatusItems.GeneratorOffline : Db.Get().BuildingStatusItems.Wattage;
		GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item, this);
	}

	private void UpdateStatusItem()
	{
		selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.Wattage, false);
		if (statusHandle == Guid.Empty)
		{
			statusHandle = selectable.AddStatusItem(Db.Get().BuildingStatusItems.SolarPanelWattage, this);
		}
		else if (statusHandle != Guid.Empty)
		{
			GetComponent<KSelectable>().ReplaceStatusItem(statusHandle, Db.Get().BuildingStatusItems.SolarPanelWattage, this);
		}
	}

	public override void EnergySim200ms(float dt)
	{
		base.EnergySim200ms(dt);
		ushort circuitID = base.CircuitID;
		operational.SetFlag(Generator.wireConnectedFlag, circuitID != 65535);
		if (operational.IsOperational)
		{
			float num = 0f;
			CellOffset[] array = solarCellOffsets;
			foreach (CellOffset offset in array)
			{
				int num2 = Grid.LightIntensity[Grid.OffsetCell(Grid.PosToCell(this), offset)];
				num += (float)num2 * 0.00053f;
			}
			operational.SetActive(num > 0f, false);
			num = Mathf.Clamp(num, 0f, 380f);
			Game.Instance.accumulators.Accumulate(accumulator, num * dt);
			if (num > 0f)
			{
				num *= dt;
				num = Mathf.Max(num, 1f * dt);
				GenerateJoules(num, false);
			}
			meter.SetPositionPercent(Game.Instance.accumulators.GetAverageRate(accumulator) / 380f);
			UpdateStatusItem();
		}
	}
}
