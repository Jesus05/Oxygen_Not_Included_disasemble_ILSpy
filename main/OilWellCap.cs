using Klei;
using KSerialization;
using STRINGS;
using TUNING;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class OilWellCap : Workable, ISingleSliderControl, IElementEmitter, ISliderControl
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, OilWellCap, object>.GameInstance
	{
		public StatesInstance(OilWellCap master)
			: base(master)
		{
		}

		public float GetPressurePercent()
		{
			return base.sm.pressurePercent.Get(base.smi);
		}
	}

	public class States : GameStateMachine<States, StatesInstance, OilWellCap>
	{
		public FloatParameter pressurePercent;

		public BoolParameter working;

		public State idle;

		public PreLoopPostState active;

		public State overpressure;

		public PreLoopPostState releasing_pressure;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = idle;
			root.ToggleRecurringChore((StatesInstance smi) => smi.master.CreateWorkChore(), null);
			idle.PlayAnim("off").ToggleStatusItem(Db.Get().BuildingStatusItems.WellPressurizing, (object)null).ParamTransition(pressurePercent, overpressure, GameStateMachine<States, StatesInstance, OilWellCap, object>.IsGTEOne)
				.ParamTransition(working, releasing_pressure, GameStateMachine<States, StatesInstance, OilWellCap, object>.IsTrue)
				.EventTransition(GameHashes.OperationalChanged, active, (StatesInstance smi) => smi.master.operational.IsOperational);
			active.DefaultState(active.pre).ToggleStatusItem(Db.Get().BuildingStatusItems.WellPressurizing, (object)null).EventTransition(GameHashes.OperationalChanged, idle, (StatesInstance smi) => !smi.master.operational.IsOperational)
				.Enter(delegate(StatesInstance smi)
				{
					smi.master.operational.SetActive(true, false);
				})
				.Exit(delegate(StatesInstance smi)
				{
					smi.master.operational.SetActive(false, false);
				})
				.Update(delegate(StatesInstance smi, float dt)
				{
					smi.master.AddGasPressure(dt);
				}, UpdateRate.SIM_200ms, false);
			active.pre.PlayAnim("working_pre").ParamTransition(pressurePercent, overpressure, GameStateMachine<States, StatesInstance, OilWellCap, object>.IsGTEOne).ParamTransition(working, releasing_pressure, GameStateMachine<States, StatesInstance, OilWellCap, object>.IsTrue)
				.OnAnimQueueComplete(active.loop);
			active.loop.PlayAnim("working_loop", KAnim.PlayMode.Loop).ParamTransition(pressurePercent, active.pst, GameStateMachine<States, StatesInstance, OilWellCap, object>.IsGTEOne).ParamTransition(working, active.pst, GameStateMachine<States, StatesInstance, OilWellCap, object>.IsTrue)
				.EventTransition(GameHashes.OperationalChanged, active.pst, (StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational);
			active.pst.PlayAnim("working_pst").OnAnimQueueComplete(idle);
			overpressure.PlayAnim("over_pressured_pre", KAnim.PlayMode.Once).QueueAnim("over_pressured_loop", true, null).ToggleStatusItem(Db.Get().BuildingStatusItems.WellOverpressure, (object)null)
				.ParamTransition(pressurePercent, idle, (StatesInstance smi, float p) => p <= 0f)
				.ParamTransition(working, releasing_pressure, GameStateMachine<States, StatesInstance, OilWellCap, object>.IsTrue);
			releasing_pressure.DefaultState(releasing_pressure.pre).ToggleStatusItem(Db.Get().BuildingStatusItems.EmittingElement, (StatesInstance smi) => smi.master).ParamTransition(working, idle, GameStateMachine<States, StatesInstance, OilWellCap, object>.IsFalse)
				.Update(delegate(StatesInstance smi, float dt)
				{
					smi.master.ReleaseGasPressure(dt);
				}, UpdateRate.SIM_200ms, false);
			releasing_pressure.pre.PlayAnim("steam_out_pre").OnAnimQueueComplete(releasing_pressure.loop);
			releasing_pressure.loop.PlayAnim("steam_out_loop", KAnim.PlayMode.Loop).EventTransition(GameHashes.OperationalChanged, releasing_pressure.pst, (StatesInstance smi) => !smi.GetComponent<Operational>().IsOperational);
			releasing_pressure.pst.PlayAnim("steam_out_pst").OnAnimQueueComplete(active);
		}
	}

	private StatesInstance smi;

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private Storage storage;

	public SimHashes gasElement;

	public float gasTemperature;

	public float addGasRate = 1f;

	public float maxGasPressure = 10f;

	public float releaseGasRate = 10f;

	[Serialize]
	private float depressurizePercent = 0.75f;

	private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;

	private MeterController pressureMeter;

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	private static readonly EventSystem.IntraObjectHandler<OilWellCap> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<OilWellCap>(delegate(OilWellCap component, object data)
	{
		component.OnCopySettings(data);
	});

	private static readonly Chore.Precondition AllowedToDepressurize = new Chore.Precondition
	{
		id = "AllowedToDepressurize",
		description = (string)DUPLICANTS.CHORES.PRECONDITIONS.ALLOWED_TO_DEPRESSURIZE,
		fn = (Chore.PreconditionFn)delegate(ref Chore.Precondition.Context context, object data)
		{
			OilWellCap oilWellCap = (OilWellCap)data;
			return oilWellCap.NeedsDepressurizing();
		}
	};

	public SimHashes Element => gasElement;

	public float AverageEmitRate => Game.Instance.accumulators.GetAverageRate(accumulator);

	public string SliderTitleKey => "STRINGS.UI.UISIDESCREENS.OIL_WELL_CAP_SIDE_SCREEN.TITLE";

	public string SliderUnits => UI.UNITSUFFIXES.PERCENT;

	public int SliderDecimalPlaces(int index)
	{
		return 0;
	}

	public float GetSliderMin(int index)
	{
		return 0f;
	}

	public float GetSliderMax(int index)
	{
		return 100f;
	}

	public float GetSliderValue(int index)
	{
		return depressurizePercent * 100f;
	}

	public void SetSliderValue(float value, int index)
	{
		depressurizePercent = value / 100f;
	}

	public string GetSliderTooltipKey(int index)
	{
		return "STRINGS.UI.UISIDESCREENS.OIL_WELL_CAP_SIDE_SCREEN.TOOLTIP";
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Subscribe(-905833192, OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		OilWellCap component = gameObject.GetComponent<OilWellCap>();
		if ((Object)component != (Object)null)
		{
			depressurizePercent = component.depressurizePercent;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Prioritizable.AddRef(base.gameObject);
		accumulator = Game.Instance.accumulators.Add("pressuregas", this);
		showProgressBar = false;
		SetWorkTime(float.PositiveInfinity);
		overrideAnims = new KAnimFile[1]
		{
			Assets.GetAnim("anim_interacts_oil_cap_kanim")
		};
		workingStatusItem = Db.Get().BuildingStatusItems.ReleasingPressure;
		attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
		attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
		skillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
		skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
		KBatchedAnimController component = GetComponent<KBatchedAnimController>();
		pressureMeter = new MeterController((KAnimControllerBase)component, "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new Vector3(0f, 0f, 0f), (string[])null);
		smi = new StatesInstance(this);
		smi.StartSM();
		UpdatePressurePercent();
	}

	protected override void OnCleanUp()
	{
		Game.Instance.accumulators.Remove(accumulator);
		Prioritizable.RemoveRef(base.gameObject);
		base.OnCleanUp();
	}

	public void AddGasPressure(float dt)
	{
		storage.AddGasChunk(gasElement, addGasRate * dt, gasTemperature, 0, 0, true, true);
		UpdatePressurePercent();
	}

	public void ReleaseGasPressure(float dt)
	{
		PrimaryElement primaryElement = storage.FindPrimaryElement(gasElement);
		if ((Object)primaryElement != (Object)null && primaryElement.Mass > 0f)
		{
			float num = releaseGasRate * dt;
			if ((Object)base.worker != (Object)null)
			{
				num *= GetEfficiencyMultiplier(base.worker);
			}
			num = Mathf.Min(num, primaryElement.Mass);
			SimUtil.DiseaseInfo percentOfDisease = SimUtil.GetPercentOfDisease(primaryElement, num / primaryElement.Mass);
			primaryElement.Mass -= num;
			Game.Instance.accumulators.Accumulate(accumulator, num);
			SimMessages.AddRemoveSubstance(Grid.PosToCell(this), ElementLoader.GetElementIndex(gasElement), null, num, primaryElement.Temperature, percentOfDisease.idx, percentOfDisease.count, true, -1);
		}
		UpdatePressurePercent();
	}

	private void UpdatePressurePercent()
	{
		float massAvailable = storage.GetMassAvailable(gasElement);
		float value = massAvailable / maxGasPressure;
		value = Mathf.Clamp01(value);
		smi.sm.pressurePercent.Set(value, smi);
		pressureMeter.SetPositionPercent(value);
	}

	public bool NeedsDepressurizing()
	{
		return smi.GetPressurePercent() >= depressurizePercent;
	}

	private WorkChore<OilWellCap> CreateWorkChore()
	{
		WorkChore<OilWellCap> workChore = new WorkChore<OilWellCap>(Db.Get().ChoreTypes.Depressurize, this, null, true, null, null, null, true, null, false, false, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
		workChore.AddPrecondition(AllowedToDepressurize, this);
		return workChore;
	}

	protected override void OnStartWork(Worker worker)
	{
		base.OnStartWork(worker);
		smi.sm.working.Set(true, smi);
	}

	protected override void OnStopWork(Worker worker)
	{
		base.OnStopWork(worker);
		smi.sm.working.Set(false, smi);
	}

	protected override bool OnWorkTick(Worker worker, float dt)
	{
		return smi.GetPressurePercent() <= 0f;
	}
}
