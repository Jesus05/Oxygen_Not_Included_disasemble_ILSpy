using Klei.AI;
using STRINGS;
using UnityEngine;

public class ExternalTemperatureMonitor : GameStateMachine<ExternalTemperatureMonitor, ExternalTemperatureMonitor.Instance>
{
	public new class Instance : GameInstance
	{
		public float AverageExternalTemperature;

		public float ColdThreshold = 283.15f;

		public float HotThreshold = 306.15f;

		private AttributeModifier baseScalindingThreshold = new AttributeModifier("ScaldingThreshold", 345f, DUPLICANTS.STATS.SKIN_DURABILITY.NAME, false, false, true);

		public Attributes attributes;

		public OccupyArea occupyArea;

		public AmountInstance internalTemperature;

		private TemperatureMonitor.Instance internalTemperatureMonitor;

		public CreatureSimTemperatureTransfer temperatureTransferer;

		public Health health;

		public PrimaryElement primaryElement;

		private const float MIN_SCALD_INTERVAL = 5f;

		private float lastScaldTime;

		public float GetCurrentExternalTemperature
		{
			get
			{
				int num = Grid.PosToCell(base.gameObject);
				if ((Object)occupyArea != (Object)null)
				{
					float num2 = 0f;
					for (int i = 0; i < occupyArea.OccupiedCellsOffsets.Length; i++)
					{
						num2 += Grid.Temperature[Grid.OffsetCell(num, occupyArea.OccupiedCellsOffsets[i])];
					}
					return num2 / (float)occupyArea.OccupiedCellsOffsets.Length;
				}
				return Grid.Temperature[num];
			}
		}

		public float GetCurrentColdThreshold
		{
			get
			{
				if (internalTemperatureMonitor.IdealTemperatureDelta() > 0.5f)
				{
					return 0f;
				}
				return CreatureSimTemperatureTransfer.PotentialEnergyFlowToCreature(Grid.PosToCell(base.gameObject), primaryElement, temperatureTransferer, 1f);
			}
		}

		public float GetCurrentHotThreshold => HotThreshold;

		public Instance(IStateMachineTarget master)
			: base(master)
		{
			health = GetComponent<Health>();
			occupyArea = GetComponent<OccupyArea>();
			internalTemperatureMonitor = base.gameObject.GetSMI<TemperatureMonitor.Instance>();
			internalTemperature = Db.Get().Amounts.Temperature.Lookup(base.gameObject);
			temperatureTransferer = base.gameObject.GetComponent<CreatureSimTemperatureTransfer>();
			primaryElement = base.gameObject.GetComponent<PrimaryElement>();
			attributes = base.gameObject.GetAttributes();
		}

		public override void StartSM()
		{
			base.StartSM();
			base.smi.attributes.Get(Db.Get().Attributes.ScaldingThreshold).Add(baseScalindingThreshold);
		}

		public float GetScaldingThreshold()
		{
			return base.smi.attributes.GetValue("ScaldingThreshold");
		}

		public bool IsTooHot()
		{
			if (internalTemperatureMonitor.IdealTemperatureDelta() < -0.5f)
			{
				return false;
			}
			if (base.smi.temperatureTransferer.average_kilowatts_exchanged.GetWeightedAverage > GetExternalWarmThreshold(base.smi.attributes))
			{
				return true;
			}
			return false;
		}

		public bool IsTooCold()
		{
			if (internalTemperatureMonitor.IdealTemperatureDelta() > 0.5f)
			{
				return false;
			}
			if (base.smi.temperatureTransferer.average_kilowatts_exchanged.GetWeightedAverage < GetExternalColdThreshold(base.smi.attributes))
			{
				return true;
			}
			return false;
		}

		public bool IsScalding()
		{
			return AverageExternalTemperature > base.smi.attributes.GetValue("ScaldingThreshold");
		}

		public void ScaldDamage(float dt)
		{
			if ((Object)health != (Object)null && Time.time - lastScaldTime > 5f)
			{
				lastScaldTime = Time.time;
				health.Damage(dt * 10f);
			}
		}

		public float CurrentWorldTransferWattage()
		{
			return temperatureTransferer.currentExchangeWattage;
		}
	}

	public State comfortable;

	public State transitionToTooWarm;

	public State tooWarm;

	public State transitionToTooCool;

	public State tooCool;

	public State transitionToScalding;

	public State scalding;

	private const float SCALDING_DAMAGE_AMOUNT = 10f;

	private const float BODY_TEMPERATURE_AFFECT_EXTERNAL_FEEL_THRESHOLD = 0.5f;

	public const float BASE_STRESS_TOLERANCE_COLD = 0.278933346f;

	public const float BASE_STRESS_TOLERANCE_WARM = 0.278933346f;

	private const float START_GAME_AVERAGING_DELAY = 6f;

	private const float TRANSITION_TO_DELAY = 1f;

	private const float TRANSITION_OUT_DELAY = 6f;

	private const float TEMPERATURE_AVERAGING_RANGE = 6f;

	public static float GetExternalColdThreshold(Attributes affected_attributes)
	{
		if (affected_attributes == null)
		{
			return -0.36261335f;
		}
		return 0f - (0.36261335f - affected_attributes.GetValue(Db.Get().Attributes.RoomTemperaturePreference.Id));
	}

	public static float GetExternalWarmThreshold(Attributes affected_attributes)
	{
		if (affected_attributes == null)
		{
			return 0.195253342f;
		}
		return 0f - (-0.195253342f - affected_attributes.GetValue(Db.Get().Attributes.RoomTemperaturePreference.Id));
	}

	public override void InitializeStates(out BaseState default_state)
	{
		default_state = comfortable;
		root.Enter(delegate(Instance smi)
		{
			smi.AverageExternalTemperature = smi.GetCurrentExternalTemperature;
		}).Update(delegate(Instance smi, float dt)
		{
			smi.AverageExternalTemperature *= Mathf.Max(0f, 1f - dt / 6f);
			smi.AverageExternalTemperature += smi.GetCurrentExternalTemperature * (dt / 6f);
		}, UpdateRate.SIM_200ms, false);
		comfortable.Transition(transitionToTooWarm, (Instance smi) => smi.IsTooHot() && smi.timeinstate > 6f, UpdateRate.SIM_200ms).Transition(transitionToTooCool, (Instance smi) => smi.IsTooCold() && smi.timeinstate > 6f, UpdateRate.SIM_200ms);
		transitionToTooWarm.Transition(comfortable, (Instance smi) => !smi.IsTooHot(), UpdateRate.SIM_200ms).Transition(tooWarm, (Instance smi) => smi.IsTooHot() && smi.timeinstate > 1f, UpdateRate.SIM_200ms);
		transitionToTooCool.Transition(comfortable, (Instance smi) => !smi.IsTooCold(), UpdateRate.SIM_200ms).Transition(tooCool, (Instance smi) => smi.IsTooCold() && smi.timeinstate > 1f, UpdateRate.SIM_200ms);
		transitionToScalding.Transition(tooWarm, (Instance smi) => !smi.IsScalding(), UpdateRate.SIM_200ms).Transition(scalding, (Instance smi) => smi.IsScalding() && smi.timeinstate > 1f, UpdateRate.SIM_200ms);
		tooWarm.Transition(comfortable, (Instance smi) => !smi.IsTooHot() && smi.timeinstate > 6f, UpdateRate.SIM_200ms).Transition(transitionToScalding, (Instance smi) => smi.IsScalding(), UpdateRate.SIM_200ms).ToggleExpression(Db.Get().Expressions.Hot, null)
			.ToggleThought(Db.Get().Thoughts.Hot, null)
			.ToggleStatusItem(Db.Get().DuplicantStatusItems.Hot, (Instance smi) => smi)
			.ToggleEffect("WarmAir")
			.Enter(delegate
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_ThermalComfort);
			});
		scalding.Transition(tooWarm, (Instance smi) => !smi.IsScalding() && smi.timeinstate > 6f, UpdateRate.SIM_200ms).ToggleExpression(Db.Get().Expressions.Hot, null).ToggleThought(Db.Get().Thoughts.Hot, null)
			.ToggleStatusItem(Db.Get().CreatureStatusItems.Scalding, (Instance smi) => smi)
			.Update("ScaldDamage", delegate(Instance smi, float dt)
			{
				smi.ScaldDamage(dt);
			}, UpdateRate.SIM_1000ms, false);
		tooCool.Transition(comfortable, (Instance smi) => !smi.IsTooCold() && smi.timeinstate > 6f, UpdateRate.SIM_200ms).ToggleExpression(Db.Get().Expressions.Cold, null).ToggleThought(Db.Get().Thoughts.Cold, null)
			.ToggleStatusItem(Db.Get().DuplicantStatusItems.Cold, (Instance smi) => smi)
			.ToggleEffect("ColdAir")
			.Enter(delegate
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_ThermalComfort);
			});
	}
}
