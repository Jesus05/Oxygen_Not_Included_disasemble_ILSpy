using KSerialization;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SpaceHeater : StateMachineComponent<SpaceHeater.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, SpaceHeater, object>.GameInstance
	{
		public StatesInstance(SpaceHeater master)
			: base(master)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, SpaceHeater>
	{
		public class OnlineStates : State
		{
			public State heating;

			public State overtemp;

			public State undermassliquid;

			public State undermassgas;
		}

		public State offline;

		public OnlineStates online;

		private StatusItem statusItemUnderMassLiquid;

		private StatusItem statusItemUnderMassGas;

		private StatusItem statusItemOverTemp;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = offline;
			base.serializable = false;
			statusItemUnderMassLiquid = new StatusItem("statusItemUnderMassLiquid", BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_LIQUID.NAME, BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_LIQUID.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, 63486);
			statusItemUnderMassGas = new StatusItem("statusItemUnderMassGas", BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_GAS.NAME, BUILDING.STATUSITEMS.HEATINGSTALLEDLOWMASS_GAS.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, 63486);
			statusItemOverTemp = new StatusItem("statusItemOverTemp", BUILDING.STATUSITEMS.HEATINGSTALLEDHOTENV.NAME, BUILDING.STATUSITEMS.HEATINGSTALLEDHOTENV.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID, 63486);
			statusItemOverTemp.resolveStringCallback = delegate(string str, object obj)
			{
				StatesInstance statesInstance = (StatesInstance)obj;
				return string.Format(str, GameUtil.GetFormattedTemperature(statesInstance.master.TargetTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
			};
			offline.EventTransition(GameHashes.OperationalChanged, online, (StatesInstance smi) => smi.master.operational.IsOperational);
			online.EventTransition(GameHashes.OperationalChanged, offline, (StatesInstance smi) => !smi.master.operational.IsOperational).DefaultState(online.heating).Update("spaceheater_online", delegate(StatesInstance smi, float dt)
			{
				switch (smi.master.MonitorHeating(dt))
				{
				case MonitorState.NotEnoughLiquid:
					smi.GoTo(online.undermassliquid);
					break;
				case MonitorState.NotEnoughGas:
					smi.GoTo(online.undermassgas);
					break;
				case MonitorState.TooHot:
					smi.GoTo(online.overtemp);
					break;
				case MonitorState.ReadyToHeat:
					smi.GoTo(online.heating);
					break;
				}
			}, UpdateRate.SIM_4000ms, false);
			online.heating.Enter(delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).Exit(delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(false, false);
			});
			online.undermassliquid.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Heat, statusItemUnderMassLiquid, null);
			online.undermassgas.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Heat, statusItemUnderMassGas, null);
			online.overtemp.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Heat, statusItemOverTemp, null);
		}
	}

	private enum MonitorState
	{
		ReadyToHeat,
		TooHot,
		NotEnoughLiquid,
		NotEnoughGas
	}

	public float targetTemperature = 308.15f;

	public float minimumCellMass = 0f;

	public int radius = 2;

	[SerializeField]
	private bool heatLiquid = false;

	[MyCmpReq]
	private Operational operational;

	private List<int> monitorCells = new List<int>();

	public float TargetTemperature => targetTemperature;

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.StartSM();
	}

	public void SetLiquidHeater()
	{
		heatLiquid = true;
	}

	private MonitorState MonitorHeating(float dt)
	{
		monitorCells.Clear();
		int cell = Grid.PosToCell(base.transform.GetPosition());
		GameUtil.GetNonSolidCells(cell, radius, monitorCells);
		int num = 0;
		float num2 = 0f;
		for (int i = 0; i < monitorCells.Count; i++)
		{
			if (Grid.Mass[monitorCells[i]] > minimumCellMass && ((Grid.Element[monitorCells[i]].IsGas && !heatLiquid) || (Grid.Element[monitorCells[i]].IsLiquid && heatLiquid)))
			{
				num++;
				num2 += Grid.Temperature[monitorCells[i]];
			}
		}
		if (num != 0)
		{
			if (!(num2 / (float)num >= targetTemperature))
			{
				return MonitorState.ReadyToHeat;
			}
			return MonitorState.TooHot;
		}
		return (!heatLiquid) ? MonitorState.NotEnoughGas : MonitorState.NotEnoughLiquid;
	}
}
