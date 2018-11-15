using Klei;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class WarmBlooded : StateMachineComponent<WarmBlooded.StatesInstance>
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, WarmBlooded, object>.GameInstance
	{
		public AttributeModifier baseTemperatureModification;

		public AttributeModifier bodyRegulator;

		public AttributeModifier averageBodyRegulation;

		public AttributeModifier burningCalories;

		public float averageInternalTemperature;

		public float TemperatureDelta => bodyRegulator.Value;

		public float BodyTemperature => base.master.primaryElement.Temperature;

		public StatesInstance(WarmBlooded smi)
			: base(smi)
		{
			baseTemperatureModification = new AttributeModifier("TemperatureDelta", 0f, DUPLICANTS.MODIFIERS.BASEDUPLICANT.NAME, false, true, false);
			bodyRegulator = new AttributeModifier("TemperatureDelta", 0f, DUPLICANTS.MODIFIERS.HOMEOSTASIS.NAME, false, true, false);
			burningCalories = new AttributeModifier("CaloriesDelta", 0f, DUPLICANTS.MODIFIERS.BURNINGCALORIES.NAME, false, false, false);
			base.master.GetAttributes().Add(bodyRegulator);
			base.master.GetAttributes().Add(burningCalories);
			base.master.GetAttributes().Add(baseTemperatureModification);
			base.master.SetTemperatureImmediate(310.15f);
		}

		public bool IsHot()
		{
			return BodyTemperature > 310.15f;
		}

		public bool IsCold()
		{
			return BodyTemperature < 310.15f;
		}
	}

	public class States : GameStateMachine<States, StatesInstance, WarmBlooded>
	{
		public class RegulatingState : State
		{
			public State transition;

			public State regulating;
		}

		public class AliveState : State
		{
			public State normal;

			public RegulatingState cold;

			public RegulatingState hot;
		}

		public AliveState alive;

		public State dead;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = alive.normal;
			root.EventTransition(GameHashes.Died, dead, null).Enter(delegate(StatesInstance smi)
			{
				PrimaryElement component3 = smi.master.GetComponent<PrimaryElement>();
				float value = SimUtil.EnergyFlowToTemperatureDelta(0.0836800039f, component3.Element.specificHeatCapacity, component3.Mass);
				smi.baseTemperatureModification.SetValue(value);
				CreatureSimTemperatureTransfer component4 = smi.master.GetComponent<CreatureSimTemperatureTransfer>();
				component4.NonSimTemperatureModifiers.Add(smi.baseTemperatureModification);
				component4.NonSimTemperatureModifiers.Add(smi.bodyRegulator);
			});
			alive.normal.Transition(alive.cold.transition, (StatesInstance smi) => smi.IsCold(), UpdateRate.SIM_200ms).Transition(alive.hot.transition, (StatesInstance smi) => smi.IsHot(), UpdateRate.SIM_200ms);
			alive.cold.transition.ScheduleGoTo(3f, alive.cold.regulating).Transition(alive.normal, (StatesInstance smi) => !smi.IsCold(), UpdateRate.SIM_200ms);
			alive.cold.regulating.Transition(alive.normal, (StatesInstance smi) => !smi.IsCold(), UpdateRate.SIM_200ms).Update("ColdRegulating", delegate(StatesInstance smi, float dt)
			{
				PrimaryElement component2 = smi.master.GetComponent<PrimaryElement>();
				float num4 = SimUtil.EnergyFlowToTemperatureDelta(0.0836800039f, component2.Element.specificHeatCapacity, component2.Mass);
				float num5 = SimUtil.EnergyFlowToTemperatureDelta(0.5578667f, component2.Element.specificHeatCapacity, component2.Mass);
				float num6 = 310.15f - smi.BodyTemperature;
				float num7 = 1f;
				if (num5 + num4 > num6)
				{
					num7 = Mathf.Max(0f, num6 - num4) / num5;
				}
				smi.bodyRegulator.SetValue(num5 * num7);
				smi.burningCalories.SetValue(-0.5578667f * num7 * 1000f / 4184f);
			}, UpdateRate.SIM_200ms, false).Exit(delegate(StatesInstance smi)
			{
				smi.bodyRegulator.SetValue(0f);
				smi.burningCalories.SetValue(0f);
			});
			alive.hot.transition.ScheduleGoTo(3f, alive.hot.regulating).Transition(alive.normal, (StatesInstance smi) => !smi.IsHot(), UpdateRate.SIM_200ms);
			alive.hot.regulating.Transition(alive.normal, (StatesInstance smi) => !smi.IsHot(), UpdateRate.SIM_200ms).Update("WarmRegulating", delegate(StatesInstance smi, float dt)
			{
				PrimaryElement component = smi.master.GetComponent<PrimaryElement>();
				float num = SimUtil.EnergyFlowToTemperatureDelta(0.5578667f, component.Element.specificHeatCapacity, component.Mass);
				float num2 = 310.15f - smi.BodyTemperature;
				float num3 = 1f;
				if ((num - smi.baseTemperatureModification.Value) * dt < num2)
				{
					num3 = Mathf.Clamp(num2 / ((num - smi.baseTemperatureModification.Value) * dt), 0f, 1f);
				}
				smi.bodyRegulator.SetValue((0f - num) * num3);
				smi.burningCalories.SetValue(-0.5578667f * num3 / 4184f);
			}, UpdateRate.SIM_200ms, false).Exit(delegate(StatesInstance smi)
			{
				smi.bodyRegulator.SetValue(0f);
			});
			dead.Enter(delegate(StatesInstance smi)
			{
				smi.master.enabled = false;
			});
		}
	}

	[MyCmpAdd]
	private Notifier notifier;

	private AmountInstance externalTemperature;

	public AmountInstance temperature;

	private PrimaryElement primaryElement;

	public const float TRANSITION_DELAY_HOT = 3f;

	public const float TRANSITION_DELAY_COLD = 3f;

	protected override void OnPrefabInit()
	{
		externalTemperature = Db.Get().Amounts.ExternalTemperature.Lookup(base.gameObject);
		externalTemperature.value = Grid.Temperature[Grid.PosToCell(this)];
		temperature = Db.Get().Amounts.Temperature.Lookup(base.gameObject);
		primaryElement = GetComponent<PrimaryElement>();
	}

	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
	}

	public bool IsAtReasonableTemperature()
	{
		return !base.smi.IsHot() && !base.smi.IsCold();
	}

	public void SetTemperatureImmediate(float t)
	{
		temperature.value = t;
	}
}
