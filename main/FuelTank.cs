using KSerialization;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class FuelTank : Storage, IUserControlledCapacity
{
	private bool isSuspended = false;

	private MeterController meter;

	[Serialize]
	public float targetFillMass = BUILDINGS.ROCKETRY_MASS_KG.FUEL_TANK_WET_MASS[0];

	[SerializeField]
	private Tag fuelType;

	public float minimumLaunchMass = BUILDINGS.ROCKETRY_MASS_KG.FUEL_TANK_WET_MASS[0];

	public bool IsSuspended => isSuspended;

	public float UserMaxCapacity
	{
		get
		{
			return targetFillMass;
		}
		set
		{
			targetFillMass = value;
			capacityKg = targetFillMass;
			ConduitConsumer component = GetComponent<ConduitConsumer>();
			if ((Object)component != (Object)null)
			{
				component.capacityKG = targetFillMass;
			}
			ManualDeliveryKG component2 = GetComponent<ManualDeliveryKG>();
			if ((Object)component2 != (Object)null)
			{
				component2.capacity = (component2.refillMass = targetFillMass);
			}
			Trigger(-945020481, this);
		}
	}

	public float MinCapacity => 0f;

	public float MaxCapacity => 900f;

	public float AmountStored => MassStored();

	public bool WholeValues => false;

	public LocString CapacityUnits => GameUtil.GetCurrentMassUnit(false);

	public Tag FuelType
	{
		get
		{
			return fuelType;
		}
		set
		{
			fuelType = value;
			if (storageFilters == null)
			{
				storageFilters = new List<Tag>();
			}
			storageFilters.Add(fuelType);
			ManualDeliveryKG component = GetComponent<ManualDeliveryKG>();
			if ((Object)component != (Object)null)
			{
				component.requestedItemTag = fuelType;
			}
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		GetComponent<KBatchedAnimController>().Play("grounded", KAnim.PlayMode.Loop, 1f, 0f);
		base.gameObject.Subscribe(1366341636, OnReturn);
		UserMaxCapacity = UserMaxCapacity;
		meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, "meter_target", "meter_fill", "meter_frame", "meter_OL");
		Subscribe(-1697596308, delegate
		{
			meter.SetPositionPercent(MassStored() / capacityKg);
		});
	}

	public void FillTank()
	{
		RocketEngine rocketEngine = null;
		foreach (GameObject item in AttachableBuilding.GetAttachedNetwork(GetComponent<AttachableBuilding>()))
		{
			rocketEngine = item.GetComponent<RocketEngine>();
			if ((Object)rocketEngine != (Object)null && rocketEngine.mainEngine)
			{
				break;
			}
		}
		if ((Object)rocketEngine != (Object)null)
		{
			AddLiquid(ElementLoader.GetElementID(rocketEngine.fuelTag), targetFillMass - MassStored(), ElementLoader.GetElement(rocketEngine.fuelTag).defaultValues.temperature, 0, 0, false, true);
		}
		else
		{
			Debug.LogWarning("Fuel tank couldn't find rocket engine");
		}
	}

	private void OnReturn(object data)
	{
		for (int num = items.Count - 1; num >= 0; num--)
		{
			Util.KDestroyGameObject(items[num]);
		}
		items.Clear();
	}
}
