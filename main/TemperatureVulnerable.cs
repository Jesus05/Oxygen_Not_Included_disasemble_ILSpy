using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[SkipSaveFileSerialization]
public class TemperatureVulnerable : StateMachineComponent<TemperatureVulnerable.StatesInstance>, IGameObjectEffectDescriptor, IWiltCause, ISim1000ms
{
	public class StatesInstance : GameStateMachine<States, StatesInstance, TemperatureVulnerable, object>.GameInstance
	{
		public bool hasMaturity = false;

		public StatesInstance(TemperatureVulnerable master)
			: base(master)
		{
			AmountInstance amountInstance = Db.Get().Amounts.Maturity.Lookup(base.gameObject);
			if (amountInstance != null)
			{
				hasMaturity = true;
			}
		}
	}

	public class States : GameStateMachine<States, StatesInstance, TemperatureVulnerable>
	{
		public FloatParameter internalTemp;

		public State lethalCold;

		public State lethalHot;

		public State warningCold;

		public State warningHot;

		public State normal;

		[CompilerGenerated]
		private static StateMachine<States, StatesInstance, TemperatureVulnerable, object>.State.Callback _003C_003Ef__mg_0024cache0;

		[CompilerGenerated]
		private static StateMachine<States, StatesInstance, TemperatureVulnerable, object>.State.Callback _003C_003Ef__mg_0024cache1;

		public override void InitializeStates(out BaseState default_state)
		{
			default_state = normal;
			lethalCold.Enter(delegate(StatesInstance smi)
			{
				smi.master.internalTemperatureState = TemperatureState.LethalCold;
			}).TriggerOnEnter(GameHashes.TooColdFatal, null).ParamTransition(internalTemp, warningCold, (StatesInstance smi, float p) => p > smi.master.internalTemperatureLethal_Low)
				.Enter(Kill);
			lethalHot.Enter(delegate(StatesInstance smi)
			{
				smi.master.internalTemperatureState = TemperatureState.LethalHot;
			}).TriggerOnEnter(GameHashes.TooHotFatal, null).ParamTransition(internalTemp, warningHot, (StatesInstance smi, float p) => p < smi.master.internalTemperatureLethal_High)
				.Enter(Kill);
			warningCold.Enter(delegate(StatesInstance smi)
			{
				smi.master.internalTemperatureState = TemperatureState.WarningCold;
			}).TriggerOnEnter(GameHashes.TooColdWarning, null).ParamTransition(internalTemp, lethalCold, (StatesInstance smi, float p) => p < smi.master.internalTemperatureLethal_Low)
				.ParamTransition(internalTemp, normal, (StatesInstance smi, float p) => p > smi.master.internalTemperatureWarning_Low);
			warningHot.Enter(delegate(StatesInstance smi)
			{
				smi.master.internalTemperatureState = TemperatureState.WarningHot;
			}).TriggerOnEnter(GameHashes.TooHotWarning, null).ParamTransition(internalTemp, lethalHot, (StatesInstance smi, float p) => p > smi.master.internalTemperatureLethal_High)
				.ParamTransition(internalTemp, normal, (StatesInstance smi, float p) => p < smi.master.internalTemperatureWarning_High);
			normal.Enter(delegate(StatesInstance smi)
			{
				smi.master.internalTemperatureState = TemperatureState.Normal;
			}).TriggerOnEnter(GameHashes.OptimalTemperatureAchieved, null).ParamTransition(internalTemp, warningHot, (StatesInstance smi, float p) => p > smi.master.internalTemperatureWarning_High)
				.ParamTransition(internalTemp, warningCold, (StatesInstance smi, float p) => p < smi.master.internalTemperatureWarning_Low);
		}

		private static void Kill(Instance smi)
		{
			smi.GetSMI<DeathMonitor.Instance>()?.Kill(Db.Get().Deaths.Generic);
		}
	}

	public enum TemperatureState
	{
		LethalCold,
		WarningCold,
		Normal,
		WarningHot,
		LethalHot
	}

	private OccupyArea _occupyArea;

	public float internalTemperatureLethal_Low;

	public float internalTemperatureWarning_Low;

	public float internalTemperaturePerfect_Low;

	public float internalTemperaturePerfect_High;

	public float internalTemperatureWarning_High;

	public float internalTemperatureLethal_High;

	private const float minimumMassForReading = 0.1f;

	[MyCmpReq]
	private PrimaryElement primaryElement;

	[MyCmpReq]
	private SimTemperatureTransfer temperatureTransfer;

	private AmountInstance displayTemperatureAmount;

	private TemperatureState internalTemperatureState = TemperatureState.Normal;

	private float averageTemp;

	private int cellCount;

	[CompilerGenerated]
	private static Func<int, object, bool> _003C_003Ef__mg_0024cache0;

	WiltCondition.Condition[] IWiltCause.Conditions
	{
		get
		{
			return new WiltCondition.Condition[1];
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

	public float InternalTemperature => primaryElement.Temperature;

	public TemperatureState GetInternalTemperatureState => internalTemperatureState;

	public bool IsLethal => GetInternalTemperatureState == TemperatureState.LethalHot || GetInternalTemperatureState == TemperatureState.LethalCold;

	public bool IsNormal => GetInternalTemperatureState == TemperatureState.Normal;

	public string WiltStateString
	{
		get
		{
			if (!base.smi.IsInsideState(base.smi.sm.warningCold))
			{
				if (!base.smi.IsInsideState(base.smi.sm.warningHot))
				{
					return "";
				}
				return Db.Get().CreatureStatusItems.Hot_Crop.resolveStringCallback(CREATURES.STATUSITEMS.HOT_CROP.NAME, this);
			}
			return Db.Get().CreatureStatusItems.Cold_Crop.resolveStringCallback(CREATURES.STATUSITEMS.COLD_CROP.NAME, this);
		}
	}

	public event Action<float, float> OnTemperature;

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Amounts amounts = base.gameObject.GetAmounts();
		displayTemperatureAmount = amounts.Add(new AmountInstance(Db.Get().Amounts.Temperature, base.gameObject));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.smi.sm.internalTemp.Set(primaryElement.Temperature, base.smi);
		base.smi.StartSM();
	}

	public void Configure(float tempWarningLow, float tempLethalLow, float tempWarningHigh, float tempLethalHigh)
	{
		internalTemperatureWarning_Low = tempWarningLow;
		internalTemperatureLethal_Low = tempLethalLow;
		internalTemperatureLethal_High = tempLethalHigh;
		internalTemperatureWarning_High = tempWarningHigh;
	}

	public bool IsCellSafe(int cell)
	{
		float averageTemperature = GetAverageTemperature(cell);
		return averageTemperature > -1f && averageTemperature > internalTemperatureLethal_Low && averageTemperature < internalTemperatureLethal_High;
	}

	public void Sim1000ms(float dt)
	{
		int cell = Grid.PosToCell(base.gameObject);
		if (Grid.IsValidCell(cell))
		{
			base.smi.sm.internalTemp.Set(InternalTemperature, base.smi);
			displayTemperatureAmount.value = InternalTemperature;
			if (this.OnTemperature != null)
			{
				this.OnTemperature(dt, InternalTemperature);
			}
		}
	}

	private static bool GetAverageTemperatureCb(int cell, object data)
	{
		TemperatureVulnerable temperatureVulnerable = data as TemperatureVulnerable;
		if (Grid.Mass[cell] > 0.1f)
		{
			temperatureVulnerable.averageTemp += Grid.Temperature[cell];
			temperatureVulnerable.cellCount++;
		}
		return true;
	}

	private float GetAverageTemperature(int cell)
	{
		averageTemp = 0f;
		cellCount = 0;
		occupyArea.TestArea(cell, this, GetAverageTemperatureCb);
		if (cellCount <= 0)
		{
			return -1f;
		}
		return averageTemp / (float)cellCount;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		list.Add(new Descriptor(string.Format(UI.GAMEOBJECTEFFECTS.REQUIRES_TEMPERATURE, GameUtil.GetFormattedTemperature(internalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, false, false), GameUtil.GetFormattedTemperature(internalTemperatureWarning_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), string.Format(UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_TEMPERATURE, GameUtil.GetFormattedTemperature(internalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, false, false), GameUtil.GetFormattedTemperature(internalTemperatureWarning_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), Descriptor.DescriptorType.Requirement, false));
		return list;
	}
}
