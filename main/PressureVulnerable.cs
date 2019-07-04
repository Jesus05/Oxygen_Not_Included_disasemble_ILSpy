using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class PressureVulnerable : StateMachineComponent<PressureVulnerable.StatesInstance>, IGameObjectEffectDescriptor, IWiltCause, ISim1000ms
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, PressureVulnerable, object>.GameInstance
	{
		public bool hasMaturity = false;

		public StatesInstance(PressureVulnerable master)
			: base(master)
		{
			AmountInstance amountInstance = Db.Get().Amounts.Maturity.Lookup(base.gameObject);
			if (amountInstance != null)
			{
				hasMaturity = true;
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, PressureVulnerable>
	{
		public FloatParameter pressure;

		public BoolParameter safe_element;

		public State unsafeElement;

		public State lethalLow;

		public State lethalHigh;

		public State warningLow;

		public State warningHigh;

		public State normal;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = normal;
			lethalLow.Enter(delegate(StatesInstance smi)
			{
				smi.master.pressureState = PressureState.LethalLow;
			}).TriggerOnEnter(GameHashes.LowPressureFatal, null).ParamTransition(pressure, warningLow, (StatesInstance smi, float p) => p > smi.master.pressureLethal_Low)
				.ParamTransition(safe_element, unsafeElement, GameStateMachine<States, StatesInstance, PressureVulnerable, object>.IsFalse);
			lethalHigh.Enter(delegate(StatesInstance smi)
			{
				smi.master.pressureState = PressureState.LethalHigh;
			}).TriggerOnEnter(GameHashes.HighPressureFatal, null).ParamTransition(pressure, warningHigh, (StatesInstance smi, float p) => p < smi.master.pressureLethal_High)
				.ParamTransition(safe_element, unsafeElement, GameStateMachine<States, StatesInstance, PressureVulnerable, object>.IsFalse);
			warningLow.Enter(delegate(StatesInstance smi)
			{
				smi.master.pressureState = PressureState.WarningLow;
			}).TriggerOnEnter(GameHashes.LowPressureWarning, null).ParamTransition(pressure, lethalLow, (StatesInstance smi, float p) => p < smi.master.pressureLethal_Low)
				.ParamTransition(pressure, normal, (StatesInstance smi, float p) => p > smi.master.pressureWarning_Low)
				.ParamTransition(safe_element, unsafeElement, GameStateMachine<States, StatesInstance, PressureVulnerable, object>.IsFalse);
			unsafeElement.ParamTransition(safe_element, normal, GameStateMachine<States, StatesInstance, PressureVulnerable, object>.IsTrue).TriggerOnExit(GameHashes.CorrectAtmosphere).TriggerOnEnter(GameHashes.WrongAtmosphere, null);
			warningHigh.Enter(delegate(StatesInstance smi)
			{
				smi.master.pressureState = PressureState.WarningHigh;
			}).TriggerOnEnter(GameHashes.HighPressureWarning, null).ParamTransition(pressure, lethalHigh, (StatesInstance smi, float p) => p > smi.master.pressureLethal_High)
				.ParamTransition(pressure, normal, (StatesInstance smi, float p) => p < smi.master.pressureWarning_High)
				.ParamTransition(safe_element, unsafeElement, GameStateMachine<States, StatesInstance, PressureVulnerable, object>.IsFalse);
			normal.Enter(delegate(StatesInstance smi)
			{
				smi.master.pressureState = PressureState.Normal;
			}).TriggerOnEnter(GameHashes.OptimalPressureAchieved, null).ParamTransition(pressure, warningHigh, (StatesInstance smi, float p) => p > smi.master.pressureWarning_High)
				.ParamTransition(pressure, warningLow, (StatesInstance smi, float p) => p < smi.master.pressureWarning_Low)
				.ParamTransition(safe_element, unsafeElement, GameStateMachine<States, StatesInstance, PressureVulnerable, object>.IsFalse);
		}
	}

	public enum PressureState
	{
		LethalLow,
		WarningLow,
		Normal,
		WarningHigh,
		LethalHigh
	}

	private HandleVector<int>.Handle pressureAccumulator = HandleVector<int>.InvalidHandle;

	private HandleVector<int>.Handle elementAccumulator = HandleVector<int>.InvalidHandle;

	private OccupyArea _occupyArea;

	public float pressureLethal_Low;

	public float pressureWarning_Low;

	public float pressureWarning_High;

	public float pressureLethal_High;

	private static float testAreaPressure;

	private static int testAreaCount;

	private static Func<int, object, bool> testAreaCB = delegate(int test_cell, object data)
	{
		if (Grid.IsGas(test_cell))
		{
			testAreaPressure += Grid.Mass[test_cell];
			testAreaCount++;
		}
		return true;
	};

	private AmountInstance displayPressureAmount;

	public bool pressure_sensitive = true;

	public HashSet<Element> safe_atmospheres = new HashSet<Element>();

	private int cell;

	private PressureState pressureState = PressureState.Normal;

	WiltCondition.Condition[] IWiltCause.Conditions
	{
		get
		{
			return new WiltCondition.Condition[2]
			{
				WiltCondition.Condition.Pressure,
				WiltCondition.Condition.AtmosphereElement
			};
		}
	}

	private OccupyArea occupyArea
	{
		get
		{
			if ((UnityEngine.Object)_occupyArea == (UnityEngine.Object)null)
			{
				_occupyArea = GetComponent<OccupyArea>();
			}
			return _occupyArea;
		}
	}

	public PressureState ExternalPressureState => pressureState;

	public Element ExternalElement => Grid.Element[cell];

	public bool IsLethal => pressureState == PressureState.LethalHigh || pressureState == PressureState.LethalLow || !IsSafeElement(ExternalElement);

	public bool IsNormal => IsSafeElement(ExternalElement) && pressureState == PressureState.Normal;

	public string WiltStateString
	{
		get
		{
			string text = "";
			if (base.smi.IsInsideState(base.smi.sm.warningLow) || base.smi.IsInsideState(base.smi.sm.lethalLow))
			{
				text += Db.Get().CreatureStatusItems.AtmosphericPressureTooLow.resolveStringCallback(CREATURES.STATUSITEMS.ATMOSPHERICPRESSURETOOLOW.NAME, this);
			}
			else if (base.smi.IsInsideState(base.smi.sm.warningHigh) || base.smi.IsInsideState(base.smi.sm.lethalHigh))
			{
				text += Db.Get().CreatureStatusItems.AtmosphericPressureTooHigh.resolveStringCallback(CREATURES.STATUSITEMS.ATMOSPHERICPRESSURETOOHIGH.NAME, this);
			}
			else if (base.smi.IsInsideState(base.smi.sm.unsafeElement))
			{
				text += Db.Get().CreatureStatusItems.WrongAtmosphere.resolveStringCallback(CREATURES.STATUSITEMS.WRONGATMOSPHERE.NAME, this);
			}
			return text;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Amounts amounts = base.gameObject.GetAmounts();
		displayPressureAmount = amounts.Add(new AmountInstance(Db.Get().Amounts.AirPressure, base.gameObject));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		cell = Grid.PosToCell(this);
		base.smi.sm.pressure.Set(1f, base.smi);
		base.smi.sm.safe_element.Set(IsSafeElement(ExternalElement), base.smi);
		base.smi.master.pressureAccumulator = Game.Instance.accumulators.Add("pressureAccumulator", this);
		base.smi.master.elementAccumulator = Game.Instance.accumulators.Add("elementAccumulator", this);
		base.smi.StartSM();
	}

	public void Configure(SimHashes[] safeAtmospheres = null)
	{
		pressure_sensitive = false;
		pressureWarning_Low = -3.40282347E+38f;
		pressureLethal_Low = -3.40282347E+38f;
		pressureLethal_High = 3.40282347E+38f;
		pressureWarning_High = 3.40282347E+38f;
		safe_atmospheres = new HashSet<Element>();
		if (safeAtmospheres != null)
		{
			foreach (SimHashes hash in safeAtmospheres)
			{
				safe_atmospheres.Add(ElementLoader.FindElementByHash(hash));
			}
		}
	}

	public void Configure(float pressureWarningLow = 0.25f, float pressureLethalLow = 0.01f, float pressureWarningHigh = 10f, float pressureLethalHigh = 30f, SimHashes[] safeAtmospheres = null)
	{
		pressure_sensitive = true;
		pressureWarning_Low = pressureWarningLow;
		pressureLethal_Low = pressureLethalLow;
		pressureLethal_High = pressureLethalHigh;
		pressureWarning_High = pressureWarningHigh;
		safe_atmospheres = new HashSet<Element>();
		if (safeAtmospheres != null)
		{
			foreach (SimHashes hash in safeAtmospheres)
			{
				safe_atmospheres.Add(ElementLoader.FindElementByHash(hash));
			}
		}
	}

	public bool IsCellSafe(int cell)
	{
		return IsSafeElement(Grid.Element[cell]) && IsSafePressure(GetPressureOverArea(cell));
	}

	public bool IsSafeElement(Element element)
	{
		if (safe_atmospheres != null && safe_atmospheres.Count != 0 && !safe_atmospheres.Contains(element))
		{
			return false;
		}
		return true;
	}

	public bool IsSafePressure(float pressure)
	{
		if (!pressure_sensitive)
		{
			return true;
		}
		return pressure > pressureLethal_Low && pressure < pressureLethal_High;
	}

	public void Sim1000ms(float dt)
	{
		float pressureOverArea = GetPressureOverArea(cell);
		Game.Instance.accumulators.Accumulate(base.smi.master.pressureAccumulator, pressureOverArea);
		float averageRate = Game.Instance.accumulators.GetAverageRate(base.smi.master.pressureAccumulator);
		displayPressureAmount.value = averageRate;
		bool flag = IsSafeElement(ExternalElement);
		Game.Instance.accumulators.Accumulate(base.smi.master.elementAccumulator, (!flag) ? 0f : 1f);
		float averageRate2 = Game.Instance.accumulators.GetAverageRate(base.smi.master.elementAccumulator);
		bool value = (averageRate2 > 0f) ? true : false;
		base.smi.sm.safe_element.Set(value, base.smi);
		base.smi.sm.pressure.Set(averageRate, base.smi);
	}

	public float GetExternalPressure()
	{
		return GetPressureOverArea(cell);
	}

	private float GetPressureOverArea(int cell)
	{
		testAreaPressure = 0f;
		testAreaCount = 0;
		occupyArea.TestArea(cell, null, testAreaCB);
		occupyArea.TestAreaAbove(cell, null, testAreaCB);
		testAreaPressure = ((testAreaCount <= 0) ? 0f : (testAreaPressure / (float)testAreaCount));
		return testAreaPressure;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (pressure_sensitive)
		{
			list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.REQUIRES_PRESSURE, GameUtil.GetFormattedMass(pressureWarning_Low, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_PRESSURE, GameUtil.GetFormattedMass(pressureWarning_Low, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), Descriptor.DescriptorType.Requirement, false));
		}
		if (safe_atmospheres != null && safe_atmospheres.Count > 0)
		{
			string text = "";
			foreach (Element safe_atmosphere in safe_atmospheres)
			{
				text = text + "\n        • " + safe_atmosphere.name;
			}
			list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.REQUIRES_ATMOSPHERE, text), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_ATMOSPHERE, text), Descriptor.DescriptorType.Requirement, false));
		}
		return list;
	}
}
