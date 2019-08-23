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

		public void TestAreaPressure()
		{
			base.smi.master.TestAreaPressure();
			bool flag = base.smi.master.IsOverPressure();
			bool flag2 = base.smi.master.IsOverWarningPressure();
			if (flag)
			{
				base.smi.master.wasOverPressure = true;
				base.sm.isOverPressure.Set(true, this);
			}
			else if (base.smi.master.wasOverPressure && !flag2)
			{
				base.sm.isOverPressure.Set(false, this);
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, OilRefinery>
	{
		public BoolParameter isOverPressure;

		public BoolParameter isOverPressureWarning;

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
			ready.Update("Test Pressure Update", delegate(StatesInstance smi, float dt)
			{
				smi.TestAreaPressure();
			}, UpdateRate.SIM_1000ms, false).ParamTransition(isOverPressure, overpressure, GameStateMachine<States, StatesInstance, OilRefinery, object>.IsTrue).Transition(needResources, (StatesInstance smi) => !smi.master.GetComponent<ElementConverter>().HasEnoughMassToStartConverting(), UpdateRate.SIM_200ms)
				.ToggleChore((StatesInstance smi) => new WorkChore<WorkableTarget>(Db.Get().ChoreTypes.Fabricate, smi.master.workable, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true), needResources);
			overpressure.Update("Test Pressure Update", delegate(StatesInstance smi, float dt)
			{
				smi.TestAreaPressure();
			}, UpdateRate.SIM_1000ms, false).ParamTransition(isOverPressure, ready, GameStateMachine<States, StatesInstance, OilRefinery, object>.IsFalse).ToggleStatusItem(Db.Get().BuildingStatusItems.PressureOk, (object)null);
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
			skillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
			skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
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

	private bool wasOverPressure;

	[SerializeField]
	public float overpressureWarningMass = 4.5f;

	[SerializeField]
	public float overpressureMass = 5f;

	private float maxSrcMass;

	private float envPressure;

	private float cellCount;

	[MyCmpGet]
	private Storage storage;

	[MyCmpReq]
	private Operational operational;

	[MyCmpAdd]
	private WorkableTarget workable;

	[MyCmpReq]
	private OccupyArea occupyArea;

	private const bool hasMeter = true;

	private MeterController meter;

	private static readonly EventSystem.IntraObjectHandler<OilRefinery> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<OilRefinery>(delegate(OilRefinery component, object data)
	{
		component.OnStorageChanged(data);
	});

	[CompilerGenerated]
	private static Func<int, object, bool> _003C_003Ef__mg_0024cache0;

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

	private static bool UpdateStateCb(int cell, object data)
	{
		OilRefinery oilRefinery = data as OilRefinery;
		if (Grid.Element[cell].IsGas)
		{
			oilRefinery.cellCount += 1f;
			oilRefinery.envPressure += Grid.Mass[cell];
		}
		return true;
	}

	private void TestAreaPressure()
	{
		envPressure = 0f;
		cellCount = 0f;
		if ((UnityEngine.Object)occupyArea != (UnityEngine.Object)null && (UnityEngine.Object)base.gameObject != (UnityEngine.Object)null)
		{
			occupyArea.TestArea(Grid.PosToCell(base.gameObject), this, UpdateStateCb);
			envPressure /= cellCount;
		}
	}

	private bool IsOverPressure()
	{
		return envPressure >= overpressureMass;
	}

	private bool IsOverWarningPressure()
	{
		return envPressure >= overpressureWarningMass;
	}
}
