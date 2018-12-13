using Klei.AI;
using KSerialization;
using System;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class OxygenBreather : KMonoBehaviour, ISim200ms
{
	public interface IGasProvider
	{
		void OnSetOxygenBreather(OxygenBreather oxygen_breather);

		void OnClearOxygenBreather(OxygenBreather oxygen_breather);

		bool ConsumeGas(OxygenBreather oxygen_breather, float amount);

		bool ShouldEmitCO2();
	}

	public float O2toCO2conversion = 0.5f;

	public float lowOxygenThreshold;

	public float noOxygenThreshold;

	public Vector2 mouthOffset;

	[Serialize]
	public float accumulatedCO2;

	[SerializeField]
	public float minCO2ToEmit = 0.3f;

	private bool hasAir = true;

	private Timer hasAirTimer = new Timer();

	[MyCmpAdd]
	private Notifier notifier;

	[MyCmpGet]
	private Facing facing;

	private HandleVector<int>.Handle o2Accumulator = HandleVector<int>.InvalidHandle;

	private HandleVector<int>.Handle co2Accumulator = HandleVector<int>.InvalidHandle;

	private AmountInstance temperature;

	private AttributeInstance airConsumptionRate;

	public CellOffset[] breathableCells;

	public Action<Sim.MassConsumedCallback> onSimConsume;

	private IGasProvider gasProvider;

	private static readonly EventSystem.IntraObjectHandler<OxygenBreather> OnDeathDelegate = new EventSystem.IntraObjectHandler<OxygenBreather>(delegate(OxygenBreather component, object data)
	{
		component.OnDeath(data);
	});

	private static readonly EventSystem.IntraObjectHandler<OxygenBreather> OnRevivedDelegate = new EventSystem.IntraObjectHandler<OxygenBreather>(delegate(OxygenBreather component, object data)
	{
		component.OnRevived(data);
	});

	public float CO2EmitRate => Game.Instance.accumulators.GetAverageRate(co2Accumulator);

	public HandleVector<int>.Handle O2Accumulator => o2Accumulator;

	public int mouthCell
	{
		get
		{
			int cell = Grid.PosToCell(this);
			return GetMouthCellAtCell(cell, breathableCells);
		}
	}

	public bool IsUnderLiquid => Grid.Element[mouthCell].IsLiquid;

	public bool IsSuffocating => !hasAir;

	public SimHashes GetBreathableElement => GetBreathableElementAtCell(Grid.PosToCell(this), null);

	public bool IsBreathableElement => IsBreathableElementAtCell(Grid.PosToCell(this), null);

	protected override void OnPrefabInit()
	{
		Subscribe(1623392196, OnDeathDelegate);
		Subscribe(-1117766961, OnRevivedDelegate);
	}

	public bool IsLowOxygen()
	{
		return GetOxygenPressure(mouthCell) < lowOxygenThreshold;
	}

	protected override void OnSpawn()
	{
		airConsumptionRate = Db.Get().Attributes.AirConsumptionRate.Lookup(this);
		o2Accumulator = Game.Instance.accumulators.Add("O2", this);
		co2Accumulator = Game.Instance.accumulators.Add("CO2", this);
		KSelectable component = GetComponent<KSelectable>();
		component.AddStatusItem(Db.Get().DuplicantStatusItems.BreathingO2, this);
		component.AddStatusItem(Db.Get().DuplicantStatusItems.EmittingCO2, this);
		temperature = Db.Get().Amounts.Temperature.Lookup(this);
		NameDisplayScreen.Instance.RegisterComponent(base.gameObject, this, false);
	}

	protected override void OnCleanUp()
	{
		Game.Instance.accumulators.Remove(o2Accumulator);
		Game.Instance.accumulators.Remove(co2Accumulator);
		SetGasProvider(null);
		base.OnCleanUp();
	}

	public void Consume(Sim.MassConsumedCallback mass_consumed)
	{
		if (onSimConsume != null)
		{
			onSimConsume(mass_consumed);
		}
	}

	public void Sim200ms(float dt)
	{
		if (!base.gameObject.HasTag(GameTags.Dead))
		{
			float num = airConsumptionRate.GetTotalValue() * dt;
			bool flag = gasProvider.ConsumeGas(this, num);
			if (flag && gasProvider.ShouldEmitCO2())
			{
				float num2 = num * O2toCO2conversion;
				Game.Instance.accumulators.Accumulate(co2Accumulator, num2);
				accumulatedCO2 += num2;
				if (accumulatedCO2 >= minCO2ToEmit)
				{
					accumulatedCO2 -= minCO2ToEmit;
					Vector3 position = base.transform.GetPosition();
					position.x += ((!facing.GetFacing()) ? mouthOffset.x : (0f - mouthOffset.x));
					position.y += mouthOffset.y;
					position.z -= 0.5f;
					CO2Manager.instance.SpawnBreath(position, minCO2ToEmit, temperature.value);
				}
			}
			if (flag != hasAir)
			{
				hasAirTimer.Start();
				if (hasAirTimer.TryStop(2f))
				{
					hasAir = flag;
				}
			}
			else
			{
				hasAirTimer.Stop();
			}
		}
	}

	private void OnDeath(object data)
	{
		base.enabled = false;
		KSelectable component = GetComponent<KSelectable>();
		component.RemoveStatusItem(Db.Get().DuplicantStatusItems.BreathingO2, false);
		component.RemoveStatusItem(Db.Get().DuplicantStatusItems.EmittingCO2, false);
	}

	private void OnRevived(object data)
	{
		base.enabled = true;
	}

	private int GetMouthCellAtCell(int cell, CellOffset[] offsets)
	{
		float num = 0f;
		int result = cell;
		foreach (CellOffset offset in offsets)
		{
			int num2 = Grid.OffsetCell(cell, offset);
			float oxygenPressure = GetOxygenPressure(num2);
			if (oxygenPressure > num && oxygenPressure > noOxygenThreshold)
			{
				num = oxygenPressure;
				result = num2;
			}
		}
		return result;
	}

	public bool IsBreathableElementAtCell(int cell, CellOffset[] offsets = null)
	{
		return GetBreathableElementAtCell(cell, offsets) != SimHashes.Vacuum;
	}

	public SimHashes GetBreathableElementAtCell(int cell, CellOffset[] offsets = null)
	{
		if (offsets == null)
		{
			offsets = breathableCells;
		}
		int mouthCellAtCell = GetMouthCellAtCell(cell, offsets);
		if (!Grid.IsValidCell(mouthCellAtCell))
		{
			return SimHashes.Vacuum;
		}
		Element element = Grid.Element[mouthCellAtCell];
		return (!element.IsGas || !element.HasTag(GameTags.Breathable) || !(Grid.Mass[mouthCellAtCell] > noOxygenThreshold)) ? SimHashes.Vacuum : element.id;
	}

	private float GetOxygenPressure(int cell)
	{
		if (Grid.IsValidCell(cell))
		{
			Element element = Grid.Element[cell];
			if (element.HasTag(GameTags.Breathable))
			{
				return Grid.Mass[cell];
			}
		}
		return 0f;
	}

	public IGasProvider GetGasProvider()
	{
		return gasProvider;
	}

	public void SetGasProvider(IGasProvider gas_provider)
	{
		if (gasProvider != null)
		{
			gasProvider.OnClearOxygenBreather(this);
		}
		gasProvider = gas_provider;
		if (gasProvider != null)
		{
			gasProvider.OnSetOxygenBreather(this);
		}
	}
}
