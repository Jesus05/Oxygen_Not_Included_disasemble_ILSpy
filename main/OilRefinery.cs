using KSerialization;
using System;
using System.Runtime.CompilerServices;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class OilRefinery : StateMachineComponent<OilRefinery.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, OilRefinery, object>.GameInstance
	{
		public StatesInstance(OilRefinery smi)
			: base(smi)
		{
		}
	}

	public class States : GameStateMachine<States, StatesInstance, OilRefinery>
	{
		public State disabled;

		public State overpressure;

		public State needResources;

		public State ready;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = disabled;
			root.EventTransition(GameHashes.OperationalChanged, disabled, (StatesInstance smi) => !smi.master.operational.IsOperational);
			disabled.EventTransition(GameHashes.OperationalChanged, needResources, (StatesInstance smi) => smi.master.operational.IsOperational);
			needResources.EventTransition(GameHashes.OnStorageChange, ready, (StatesInstance smi) => smi.master.GetComponent<ElementConverter>().HasEnoughMassToStartConverting());
			ready.Transition(needResources, (StatesInstance smi) => !smi.master.GetComponent<ElementConverter>().HasEnoughMassToStartConverting(), UpdateRate.SIM_200ms).Transition(overpressure, (StatesInstance smi) => smi.master.IsOverPressure(), UpdateRate.SIM_200ms).ToggleChore((StatesInstance smi) => new WorkChore<WorkableTarget>(Db.Get().ChoreTypes.Fabricate, smi.master.workable, null, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 0, false), needResources);
			overpressure.ToggleStatusItem(Db.Get().BuildingStatusItems.PressureOk, (object)null).Transition(ready, (StatesInstance smi) => !smi.master.IsOverPressure(), UpdateRate.SIM_200ms);
		}
	}

	public class WorkableTarget : Workable
	{
		[MyCmpGet]
		public Operational operational;

		protected override void OnPrefabInit()
		{
			base.OnPrefabInit();
			showProgressBar = false;
			workerStatusItem = null;
			overrideAnims = new KAnimFile[1]
			{
				Assets.GetAnim("anim_interacts_oilrefinery_kanim")
			};
		}

		protected override void OnSpawn()
		{
			base.OnSpawn();
			SetWorkTime(float.PositiveInfinity);
		}

		public override void AwardExperience(float work_dt, MinionResume resume)
		{
			resume.AddExperienceIfRole(MachineTechnician.ID, work_dt * ROLES.ACTIVE_EXPERIENCE_QUICK);
		}

		protected override void OnStartWork(Worker worker)
		{
			operational.SetActive(true, false);
		}

		protected override void OnStopWork(Worker worker)
		{
			operational.SetActive(false, false);
		}

		protected override void OnCompleteWork(Worker worker)
		{
			operational.SetActive(false, false);
		}
	}

	[SerializeField]
	public float overpressureMass = 2.5f;

	private float maxSrcMass;

	[MyCmpGet]
	private Storage storage;

	[MyCmpReq]
	private Operational operational;

	[MyCmpAdd]
	private WorkableTarget workable;

	private const bool hasMeter = true;

	private MeterController meter;

	private static readonly EventSystem.IntraObjectHandler<OilRefinery> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<OilRefinery>(delegate(OilRefinery component, object data)
	{
		component.OnStorageChanged(data);
	});

	[CompilerGenerated]
	private static Func<int, OilRefinery, bool> _003C_003Ef__mg_0024cache0;

	protected override void OnSpawn()
	{
		Subscribe(-1697596308, OnStorageChangedDelegate);
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		meter = new MeterController((KAnimControllerBase)component, "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Vector3.zero, (string[])null);
		base.smi.StartSM();
		maxSrcMass = GetComponent<ConduitConsumer>().capacityKG;
	}

	private void OnStorageChanged(object data)
	{
		float massAvailable = storage.GetMassAvailable(SimHashes.CrudeOil);
		float positionPercent = Mathf.Clamp01(massAvailable / maxSrcMass);
		meter.SetPositionPercent(positionPercent);
	}

	private bool IsOverPressure()
	{
		int cell = Grid.PosToCell(base.transform.GetPosition());
		cell = Grid.CellAbove(cell);
		return GameUtil.FloodFillCheck(IsCellOverPressure, this, cell, 2, true, true);
	}

	private static bool IsCellOverPressure(int cell, OilRefinery oil_refinery)
	{
		return Grid.Mass[cell] > oil_refinery.overpressureMass;
	}
}
