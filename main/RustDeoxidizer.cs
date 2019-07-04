using KSerialization;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class RustDeoxidizer : StateMachineComponent<RustDeoxidizer.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, RustDeoxidizer, object>.GameInstance
	{
		public StatesInstance(RustDeoxidizer smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, RustDeoxidizer>
	{
		public State disabled;

		public State waiting;

		public State converting;

		public State overpressure;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = disabled;
			root.EventTransition(GameHashes.OperationalChanged, disabled, (StatesInstance smi) => !smi.master.operational.IsOperational);
			disabled.EventTransition(GameHashes.OperationalChanged, waiting, (StatesInstance smi) => smi.master.operational.IsOperational);
			waiting.Enter("Waiting", delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(false, false);
			}).EventTransition(GameHashes.OnStorageChange, converting, (StatesInstance smi) => smi.master.GetComponent<ElementConverter>().HasEnoughMassToStartConverting());
			converting.Enter("Ready", delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(true, false);
			}).Transition(waiting, (StatesInstance smi) => !smi.master.GetComponent<ElementConverter>().CanConvertAtAll(), UpdateRate.SIM_200ms).Transition(overpressure, (StatesInstance smi) => !smi.master.RoomForPressure, UpdateRate.SIM_200ms);
			overpressure.Enter("OverPressure", delegate(StatesInstance smi)
			{
				smi.master.operational.SetActive(false, false);
			}).ToggleStatusItem(Db.Get().BuildingStatusItems.PressureOk, (object)null).Transition(converting, (StatesInstance smi) => smi.master.RoomForPressure, UpdateRate.SIM_200ms);
		}
	}

	[SerializeField]
	public float maxMass = 2.5f;

	[MyCmpAdd]
	private Storage storage;

	[MyCmpGet]
	private ElementConverter emitter;

	[MyCmpReq]
	private Operational operational;

	private MeterController meter;

	[CompilerGenerated]
	private static Func<int, RustDeoxidizer, bool> _003C_003Ef__mg_0024cache0;

	private bool RoomForPressure
	{
		get
		{
			int cell = Grid.PosToCell(base.transform.GetPosition());
			cell = Grid.CellAbove(cell);
			return !GameUtil.FloodFillCheck(OverPressure, this, cell, 3, true, true);
		}
	}

	protected override void OnSpawn()
	{
		base.smi.StartSM();
		Tutorial.Instance.oxygenGenerators.Add(base.gameObject);
	}

	protected override void OnCleanUp()
	{
		Tutorial.Instance.oxygenGenerators.Remove(base.gameObject);
		base.OnCleanUp();
	}

	private static bool OverPressure(int cell, RustDeoxidizer rustDeoxidizer)
	{
		return Grid.Mass[cell] > rustDeoxidizer.maxMass;
	}
}
