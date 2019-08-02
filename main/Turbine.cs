using Klei;
using KSerialization;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Turbine : KMonoBehaviour
{
	public class States : GameStateMachine<States, Instance, Turbine>
	{
		public class OperationalStates : State
		{
			public State idle;

			public State spinningUp;

			public State active;
		}

		public State inoperational;

		public OperationalStates operational;

		private static readonly HashedString[] ACTIVE_ANIMS = new HashedString[2]
		{
			"working_pre",
			"working_loop"
		};

		public override void InitializeStates(out BaseState default_state)
		{
			InitializeStatusItems();
			default_state = operational;
			base.serializable = true;
			inoperational.EventTransition(GameHashes.OperationalChanged, operational.spinningUp, (Turbine.Instance smi) => smi.master.GetComponent<Operational>().IsOperational).QueueAnim("off", false, null).Enter(delegate(Turbine.Instance smi)
			{
				smi.master.currentRPM = 0f;
				smi.UpdateMeter();
			});
			operational.DefaultState(operational.spinningUp).EventTransition(GameHashes.OperationalChanged, inoperational, (Turbine.Instance smi) => !smi.master.GetComponent<Operational>().IsOperational).Update("UpdateOperational", delegate(Turbine.Instance smi, float dt)
			{
				smi.UpdateState(dt);
			}, UpdateRate.SIM_200ms, false)
				.Exit(delegate(Turbine.Instance smi)
				{
					smi.DisableStatusItems();
				});
			operational.idle.QueueAnim("on", false, null);
			operational.spinningUp.ToggleStatusItem((Turbine.Instance smi) => spinningUpStatusItem, (Turbine.Instance smi) => smi.master).QueueAnim("buildup", true, null);
			operational.active.Update("UpdateActive", delegate(Turbine.Instance smi, float dt)
			{
				smi.master.Pump(dt);
			}, UpdateRate.SIM_200ms, false).ToggleStatusItem((Turbine.Instance smi) => activeStatusItem, (Turbine.Instance smi) => smi.master).Enter(delegate(Turbine.Instance smi)
			{
				smi.GetComponent<KAnimControllerBase>().Play(ACTIVE_ANIMS, KAnim.PlayMode.Loop);
				smi.GetComponent<Operational>().SetActive(true, false);
			})
				.Exit(delegate(Turbine.Instance smi)
				{
					smi.master.GetComponent<Generator>().ResetJoules();
					smi.GetComponent<Operational>().SetActive(false, false);
				});
		}
	}

	public class Instance : GameStateMachine<States, Instance, Turbine, object>.GameInstance
	{
		public bool isInputBlocked;

		public bool isOutputBlocked;

		public bool insufficientMass;

		public bool insufficientTemperature;

		private Guid inputBlockedHandle = Guid.Empty;

		private Guid outputBlockedHandle = Guid.Empty;

		private Guid insufficientMassHandle = Guid.Empty;

		private Guid insufficientTemperatureHandle = Guid.Empty;

		public Instance(Turbine master)
			: base(master)
		{
		}

		public void UpdateState(float dt)
		{
			float num = (!CanSteamFlow(ref insufficientMass, ref insufficientTemperature)) ? (0f - base.master.rpmDeceleration) : base.master.rpmAcceleration;
			base.master.currentRPM = Mathf.Clamp(base.master.currentRPM + dt * num, 0f, base.master.maxRPM);
			UpdateMeter();
			UpdateStatusItems();
			StateMachine.BaseState currentState = base.smi.GetCurrentState();
			if (base.master.currentRPM >= base.master.minGenerationRPM)
			{
				if (currentState != base.sm.operational.active)
				{
					base.smi.GoTo(base.sm.operational.active);
				}
				base.smi.master.generator.GenerateJoules(base.smi.master.generator.WattageRating * dt, false);
			}
			else if (base.master.currentRPM > 0f)
			{
				if (currentState != base.sm.operational.spinningUp)
				{
					base.smi.GoTo(base.sm.operational.spinningUp);
				}
			}
			else if (currentState != base.sm.operational.idle)
			{
				base.smi.GoTo(base.sm.operational.idle);
			}
		}

		public void UpdateMeter()
		{
			if (base.master.meter != null)
			{
				float num = Mathf.Clamp01(base.master.currentRPM / base.master.maxRPM);
				base.master.meter.SetPositionPercent(num);
				base.master.meter.SetSymbolTint(TINT_SYMBOL, (!(num >= base.master.activePercent)) ? Color.red : Color.green);
			}
		}

		private bool CanSteamFlow(ref bool insufficient_mass, ref bool insufficient_temperature)
		{
			float num = 0f;
			float num2 = 0f;
			float num3 = float.PositiveInfinity;
			isInputBlocked = false;
			for (int i = 0; i < base.master.srcCells.Length; i++)
			{
				int num4 = base.master.srcCells[i];
				float b = Grid.Mass[num4];
				if (Grid.Element[num4].id == base.master.srcElem)
				{
					num = Mathf.Max(num, b);
				}
				float b2 = Grid.Temperature[num4];
				num2 = Mathf.Max(num2, b2);
				byte index = Grid.ElementIdx[num4];
				Element element = ElementLoader.elements[index];
				if (element.IsLiquid || element.IsSolid)
				{
					isInputBlocked = true;
				}
			}
			isOutputBlocked = false;
			for (int j = 0; j < base.master.destCells.Length; j++)
			{
				int i2 = base.master.destCells[j];
				float b3 = Grid.Mass[i2];
				num3 = Mathf.Min(num3, b3);
				byte index2 = Grid.ElementIdx[i2];
				Element element2 = ElementLoader.elements[index2];
				if (element2.IsLiquid || element2.IsSolid)
				{
					isOutputBlocked = true;
				}
			}
			insufficient_mass = (num - num3 < base.master.requiredMassFlowDifferential);
			insufficient_temperature = (num2 < base.master.minActiveTemperature);
			return !insufficient_mass && !insufficient_temperature;
		}

		public void UpdateStatusItems()
		{
			KSelectable component = GetComponent<KSelectable>();
			inputBlockedHandle = UpdateStatusItem(inputBlockedStatusItem, isInputBlocked, inputBlockedHandle, component);
			outputBlockedHandle = UpdateStatusItem(outputBlockedStatusItem, isOutputBlocked, outputBlockedHandle, component);
			insufficientMassHandle = UpdateStatusItem(insufficientMassStatusItem, insufficientMass, insufficientMassHandle, component);
			insufficientTemperatureHandle = UpdateStatusItem(insufficientTemperatureStatusItem, insufficientTemperature, insufficientTemperatureHandle, component);
		}

		private Guid UpdateStatusItem(StatusItem item, bool show, Guid current_handle, KSelectable ksel)
		{
			Guid result = current_handle;
			if (show != (current_handle != Guid.Empty))
			{
				result = ((!show) ? ksel.RemoveStatusItem(current_handle, false) : ksel.AddStatusItem(item, base.master));
			}
			return result;
		}

		public void DisableStatusItems()
		{
			KSelectable component = GetComponent<KSelectable>();
			component.RemoveStatusItem(inputBlockedHandle, false);
			component.RemoveStatusItem(outputBlockedHandle, false);
			component.RemoveStatusItem(insufficientMassHandle, false);
			component.RemoveStatusItem(insufficientTemperatureHandle, false);
		}
	}

	public SimHashes srcElem;

	public float requiredMassFlowDifferential = 3f;

	public float activePercent = 0.75f;

	public float minEmitMass;

	public float minActiveTemperature = 400f;

	public float emitTemperature = 300f;

	public float maxRPM;

	public float rpmAcceleration;

	public float rpmDeceleration;

	public float minGenerationRPM;

	public float pumpKGRate;

	private static readonly HashedString TINT_SYMBOL = new HashedString("meter_fill");

	[Serialize]
	private float storedMass;

	[Serialize]
	private float storedTemperature;

	[Serialize]
	private byte diseaseIdx = byte.MaxValue;

	[Serialize]
	private int diseaseCount;

	[MyCmpGet]
	private Generator generator;

	[Serialize]
	private float currentRPM;

	private int[] srcCells;

	private int[] destCells;

	private Instance smi;

	private static StatusItem inputBlockedStatusItem;

	private static StatusItem outputBlockedStatusItem;

	private static StatusItem insufficientMassStatusItem;

	private static StatusItem insufficientTemperatureStatusItem;

	private static StatusItem activeStatusItem;

	private static StatusItem spinningUpStatusItem;

	private const Sim.Cell.Properties floorCellProperties = (Sim.Cell.Properties)39;

	private MeterController meter;

	private HandleVector<Game.ComplexCallbackInfo<Sim.MassEmittedCallback>>.Handle simEmitCBHandle = HandleVector<Game.ComplexCallbackInfo<Sim.MassEmittedCallback>>.InvalidHandle;

	[CompilerGenerated]
	private static Action<Sim.MassEmittedCallback, object> _003C_003Ef__mg_0024cache0;

	[CompilerGenerated]
	private static Action<Sim.MassConsumedCallback, object> _003C_003Ef__mg_0024cache1;

	[CompilerGenerated]
	private static Func<string, object, string> _003C_003Ef__mg_0024cache2;

	[CompilerGenerated]
	private static Func<string, object, string> _003C_003Ef__mg_0024cache3;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		simEmitCBHandle = Game.Instance.massEmitCallbackManager.Add(OnSimEmittedCallback, this, "TurbineEmit");
		BuildingDef def = GetComponent<BuildingComplete>().Def;
		srcCells = new int[def.WidthInCells];
		destCells = new int[def.WidthInCells];
		int cell = Grid.PosToCell(this);
		for (int i = 0; i < def.WidthInCells; i++)
		{
			int x = i - (def.WidthInCells - 1) / 2;
			srcCells[i] = Grid.OffsetCell(cell, new CellOffset(x, -1));
			destCells[i] = Grid.OffsetCell(cell, new CellOffset(x, def.HeightInCells - 1));
			int num = Grid.OffsetCell(cell, new CellOffset(x, 0));
			SimMessages.SetCellProperties(num, 39);
			Grid.Foundation[num] = true;
			Grid.SetSolid(num, true, CellEventLogger.Instance.SimCellOccupierForceSolid);
			Grid.RenderedByWorld[num] = false;
			World.Instance.OnSolidChanged(num);
			GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.solidChangedLayer, null);
		}
		smi = new Instance(this);
		smi.StartSM();
		CreateMeter();
	}

	private void CreateMeter()
	{
		meter = new MeterController(base.gameObject.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_OL", "meter_frame", "meter_fill");
		smi.UpdateMeter();
	}

	protected override void OnCleanUp()
	{
		if (smi != null)
		{
			smi.StopSM("cleanup");
		}
		BuildingDef def = GetComponent<BuildingComplete>().Def;
		int cell = Grid.PosToCell(this);
		for (int i = 0; i < def.WidthInCells; i++)
		{
			int x = i - (def.WidthInCells - 1) / 2;
			int num = Grid.OffsetCell(cell, new CellOffset(x, 0));
			SimMessages.ClearCellProperties(num, 39);
			Grid.Foundation[num] = false;
			Grid.SetSolid(num, false, CellEventLogger.Instance.SimCellOccupierForceSolid);
			Grid.RenderedByWorld[num] = true;
			World.Instance.OnSolidChanged(num);
			GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.solidChangedLayer, null);
		}
		Game.Instance.massEmitCallbackManager.Release(simEmitCBHandle, "Turbine");
		simEmitCBHandle.Clear();
		base.OnCleanUp();
	}

	private void Pump(float dt)
	{
		float mass = pumpKGRate * dt / (float)srcCells.Length;
		int[] array = srcCells;
		foreach (int gameCell in array)
		{
			HandleVector<Game.ComplexCallbackInfo<Sim.MassConsumedCallback>>.Handle handle = Game.Instance.massConsumedCallbackManager.Add(OnSimConsumeCallback, this, "TurbineConsume");
			SimMessages.ConsumeMass(gameCell, srcElem, mass, 1, handle.index);
		}
	}

	private static void OnSimConsumeCallback(Sim.MassConsumedCallback mass_cb_info, object data)
	{
		((Turbine)data).OnSimConsume(mass_cb_info);
	}

	private void OnSimConsume(Sim.MassConsumedCallback mass_cb_info)
	{
		if (mass_cb_info.mass > 0f)
		{
			storedTemperature = SimUtil.CalculateFinalTemperature(storedMass, storedTemperature, mass_cb_info.mass, mass_cb_info.temperature);
			storedMass += mass_cb_info.mass;
			SimUtil.DiseaseInfo diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(diseaseIdx, diseaseCount, mass_cb_info.diseaseIdx, mass_cb_info.diseaseCount);
			diseaseIdx = diseaseInfo.idx;
			diseaseCount = diseaseInfo.count;
			if (storedMass > minEmitMass && simEmitCBHandle.IsValid())
			{
				float mass = storedMass / (float)destCells.Length;
				int disease_count = diseaseCount / destCells.Length;
				Game.Instance.massEmitCallbackManager.GetItem(simEmitCBHandle);
				int[] array = destCells;
				foreach (int gameCell in array)
				{
					SimMessages.EmitMass(gameCell, mass_cb_info.elemIdx, mass, emitTemperature, diseaseIdx, disease_count, simEmitCBHandle.index);
				}
				storedMass = 0f;
				storedTemperature = 0f;
				diseaseIdx = byte.MaxValue;
				diseaseCount = 0;
			}
		}
	}

	private static void OnSimEmittedCallback(Sim.MassEmittedCallback info, object data)
	{
		((Turbine)data).OnSimEmitted(info);
	}

	private void OnSimEmitted(Sim.MassEmittedCallback info)
	{
		if (info.suceeded != 1)
		{
			storedTemperature = SimUtil.CalculateFinalTemperature(storedMass, storedTemperature, info.mass, info.temperature);
			storedMass += info.mass;
			if (info.diseaseIdx != 255)
			{
				SimUtil.DiseaseInfo diseaseInfo = default(SimUtil.DiseaseInfo);
				diseaseInfo.idx = diseaseIdx;
				diseaseInfo.count = diseaseCount;
				SimUtil.DiseaseInfo a = diseaseInfo;
				diseaseInfo = default(SimUtil.DiseaseInfo);
				diseaseInfo.idx = info.diseaseIdx;
				diseaseInfo.count = info.diseaseCount;
				SimUtil.DiseaseInfo b = diseaseInfo;
				SimUtil.DiseaseInfo diseaseInfo2 = SimUtil.CalculateFinalDiseaseInfo(a, b);
				diseaseIdx = diseaseInfo2.idx;
				diseaseCount = diseaseInfo2.count;
			}
		}
	}

	public static void InitializeStatusItems()
	{
		inputBlockedStatusItem = new StatusItem("TURBINE_BLOCKED_INPUT", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
		outputBlockedStatusItem = new StatusItem("TURBINE_BLOCKED_OUTPUT", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022);
		spinningUpStatusItem = new StatusItem("TURBINE_SPINNING_UP", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID, true, 129022);
		activeStatusItem = new StatusItem("TURBINE_ACTIVE", "BUILDING", string.Empty, StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID, true, 129022);
		activeStatusItem.resolveStringCallback = delegate(string str, object data)
		{
			Turbine turbine2 = (Turbine)data;
			str = string.Format(str, (int)turbine2.currentRPM);
			return str;
		};
		insufficientMassStatusItem = new StatusItem("TURBINE_INSUFFICIENT_MASS", "BUILDING", "status_item_resource_unavailable", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.Power.ID, true, 129022);
		insufficientMassStatusItem.resolveTooltipCallback = delegate(string str, object data)
		{
			Turbine turbine = (Turbine)data;
			str = str.Replace("{MASS}", GameUtil.GetFormattedMass(turbine.requiredMassFlowDifferential, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
			str = str.Replace("{SRC_ELEMENT}", ElementLoader.FindElementByHash(turbine.srcElem).name);
			return str;
		};
		insufficientTemperatureStatusItem = new StatusItem("TURBINE_INSUFFICIENT_TEMPERATURE", "BUILDING", "status_item_plant_temperature", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.Power.ID, true, 129022);
		insufficientTemperatureStatusItem.resolveStringCallback = ResolveStrings;
		insufficientTemperatureStatusItem.resolveTooltipCallback = ResolveStrings;
	}

	private static string ResolveStrings(string str, object data)
	{
		Turbine turbine = (Turbine)data;
		str = str.Replace("{SRC_ELEMENT}", ElementLoader.FindElementByHash(turbine.srcElem).name);
		str = str.Replace("{ACTIVE_TEMPERATURE}", GameUtil.GetFormattedTemperature(turbine.minActiveTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
		return str;
	}
}
